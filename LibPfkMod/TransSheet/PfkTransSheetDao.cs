namespace LibPfkMod.TransSheet
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using ClosedXML.Excel;
    using CsvHelper;
    using LibPfkMod.Language;
    using LibPfkMod.Umm;
    using PfkHashUtils;
    using S5mCommon_1F1F6148_9E9B_4F66_AEB6_EB749A40E94E;

    /// <summary>
    /// 翻訳シートの入出力
    /// </summary>
    public class PfkTransSheetDao
    {
        /// <summary>
        /// 翻訳シート情報を作成する。
        /// </summary>
        /// <param name="sheetInfo">翻訳シート情報</param>
        /// <param name="sheetFolderPath">CSV形式の翻訳シートが格納されているフォルダーのパス</param>
        /// <param name="fileID">FileID</param>
        /// <param name="removeTag">タグ化の制御文字化の有無</param>
        /// <param name="enc">文字コード</param>
        public static void LoadFromFolder(
            PfkTransSheetInfo sheetInfo,
            string sheetFolderPath,
            string fileID,
            bool removeTag,
            Encoding enc = null)
        {
            IEnumerable<string> files =
                Directory.EnumerateFiles(sheetFolderPath, "*.csv", SearchOption.AllDirectories);

            foreach (string sheetFilePath in files)
            {
                if (Path.GetExtension(sheetFilePath).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    LoadFromFile(sheetInfo, sheetFilePath, fileID, removeTag);
                }
            }
        }

        /// <summary>
        /// 翻訳シート情報を作成する。
        /// </summary>
        /// <param name="sheetInfo">翻訳シート情報</param>
        /// <param name="path">CSV形式の翻訳シートファイルのパス</param>
        /// <param name="fileID">FileID</param>
        /// <param name="removeTag">タグ化の制御文字化の有無</param>
        /// <param name="enc">文字コード</param>
        public static void LoadFromFile(
            PfkTransSheetInfo sheetInfo,
            string path,
            string fileID,
            bool removeTag,
            Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    //// 区切り文字
                    csv.Configuration.Delimiter = ",";
                    //// ヘッダーの有無
                    csv.Configuration.HasHeaderRecord = true;
                    //// CSVファイルに合ったマッピングルールを登録
                    csv.Configuration.RegisterClassMap<CsvMapperTransSheetForMod>();
                    //// データを読み出し
                    var records = csv.GetRecords<PfkTransSheetEntry>();

                    var sheetFile = new PfkTransSheetFile(fileID);
                    foreach (var record in records)
                    {
                        record.FileID = fileID;
                        if (removeTag)
                        {
                            //// タグを制御文字に置換する。
                            record.Japanese = PfkTransSheetEntry.GetUnEscapedText(record.Japanese);
                            record.MachineTranslation = PfkTransSheetEntry.GetUnEscapedText(record.MachineTranslation);
                        }

                        //// 翻訳シートのTABはタグ化されているので、無条件に制御文字化する。
                        record.Japanese = PfkTransSheetEntry.GetUnEscapedTab(record.Japanese);
                        record.MachineTranslation = PfkTransSheetEntry.GetUnEscapedTab(record.MachineTranslation);

                        sheetFile.AddEntry(record);
                    }

                    sheetInfo.AddFile(sheetFile);
                }
            }
        }

        public static void SaveToExcel(
            PfkLanguageInfo langInfo,
            PfkLanguageInfo funInfo,
            PfkUmmDataInfo ummDataInfo,
            string path,
            int maxRowCount,
            bool useTag)
        {
            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = null;

                bool useFunTrans = false;
                if (funInfo.Items.Count > 0)
                {
                    useFunTrans = true;
                }

                if (maxRowCount == 0)
                {
                    maxRowCount = 9999999;
                }

                int sheetNo = 1;
                foreach (var langFile in langInfo.Items.Values)
                {
                    int sequenceNo = 1;
                    int rowNo = maxRowCount + 2;
                    foreach (var langEntry in langFile.Items.Values)
                    {
                        if (string.IsNullOrWhiteSpace(langEntry.Text))
                        {
                            continue;
                        }

                        if (rowNo > maxRowCount + 1)
                        {
                            worksheet = workbook.Worksheets.Add($"Sheet{sheetNo}");
                            sheetNo++;

                            //// ヘッダー出力
                            WriteExcelHeader(worksheet);

                            rowNo = 2;
                        }

                        // 出力
                        var data = new PfkTransSheetEntry();

                        data.Key = langEntry.Key;
                        data.English = langEntry.Text;

                        //// 有志翻訳版が指定された場合は、有志翻訳のデータを反映する。
                        if (useFunTrans)
                        {
                            var funEntry = funInfo.GetEntry(langEntry.Key);
                            if (JapaneseStringUtils.TJapaneseStringUtils.ContainsJapanese(funEntry.Text))
                            {
                                data.Japanese = funEntry.Text;
                            }
                        }

                        if (useTag)
                        {
                            //// 制御文字をタグ化する。
                            data.English = PfkTransSheetEntry.GetEscapedText(data.English);
                            data.Japanese = PfkTransSheetEntry.GetEscapedText(data.Japanese);
                        }

                        //// TAB文字は無条件にタグ化する。
                        //// 理由：CSVファイルを表計算ソフトで読み込んだ場合、TAB文字は無視されてしまう場合があるため。
                        data.English = PfkTransSheetEntry.GetEscapedTab(data.English);
                        data.Japanese = PfkTransSheetEntry.GetEscapedTab(data.Japanese);

                        //// UMMデータの取得
                        var ummDataEntry = ummDataInfo.GetEntry(data.Key);
                        if (ummDataEntry == null)
                        {
                            data.MachineTranslation = string.Empty;
                        }
                        else
                        {
                            //// UMMデータがある場合は機械翻訳として取り込む。
                            data.MachineTranslation = PfkTransSheetEntry.GetEscapedTab(ummDataEntry.Value);
                        }

                        //// リファレンスIDを算出する。
                        data.ReferenceID = PfkHashTools.ComputeHashX(langEntry.Key.ToString());
                        data.Sequence = sequenceNo;

                        //// 各カラムの属性を設定する。
                        int dummyInt = 0;
                        if (int.TryParse(data.English, out dummyInt))
                        {
                            //// 符号付き数値を適切の扱うための対処。
                            //// シングルコーテーションを付けた文字列形式で格納する。
                            worksheet.Cell(rowNo, 2).Style.NumberFormat.Format = "@";
                            worksheet.Cell(rowNo, 2).Style.IncludeQuotePrefix = true;

                            worksheet.Cell(rowNo, 3).Style.NumberFormat.Format = "@";
                            worksheet.Cell(rowNo, 3).Style.IncludeQuotePrefix = true;
                        }
                        else if (
                            data.English.StartsWith("-") ||
                            data.English.StartsWith("+") ||
                            data.English.StartsWith("="))
                        {
                            //// 計算式と誤認されないようにするための対処。
                            //// シングルコーテーションを付けた標準形式で格納する。
                            //// （注：文字列形式では255文字以上のデータがエラーとなるため不可）
                            worksheet.Cell(rowNo, 2).Style.IncludeQuotePrefix = true;
                            worksheet.Cell(rowNo, 3).Style.IncludeQuotePrefix = true;
                        }
                        else
                        {
                            //// 上記以外のデータは標準形式とする。
                        }

                        //// データを転機する。
                        worksheet.Cell(rowNo, 1).Value = data.Key;
                        worksheet.Cell(rowNo, 2).Value = data.English;
                        worksheet.Cell(rowNo, 3).Value = data.Japanese;
                        worksheet.Cell(rowNo, 4).Value = data.MachineTranslation;
                        worksheet.Cell(rowNo, 5).Style.NumberFormat.Format = "@";
                        worksheet.Cell(rowNo, 5).Style.IncludeQuotePrefix = true;
                        worksheet.Cell(rowNo, 5).Value = data.ReferenceID;
                        worksheet.Cell(rowNo, 6).Value = data.Sequence;

                        rowNo++;

                        sequenceNo++;
                    }
                }

                workbook.SaveAs(path);
            }
        }

        /// <summary>
        /// 比較用翻訳シートを出力する。
        /// （比較用のため、出力項目は、キーと原文のみ）
        /// </summary>
        /// <param name="langInfo">言語情報</param>
        /// <param name="path">CSV形式の翻訳シートのパス</param>
        /// <param name="from">シート分割の始点</param>
        /// <param name="to">シート分割の終点</param>
        /// <param name="useTag">タグ化の有無</param>
        public static void SaveToCsv(
            PfkLanguageInfo langInfo, string path, int from, int to, bool useTag)
        {
            using (var sw = new StreamWriter(path, false, Encoding.UTF8))
            {
                using (var writer = new CsvWriter(sw, CultureInfo.InvariantCulture))
                {
                    writer.Configuration.RegisterClassMap<CsvMapperTransSheetForCompare>();
                    writer.WriteHeader<PfkTransSheetEntry>();
                    writer.NextRecord();
                    int total = 0;
                    foreach (var langFile in langInfo.Items.Values)
                    {
                        foreach (var langEntry in langFile.Items.Values)
                        {
                            if (string.IsNullOrWhiteSpace(langEntry.Text))
                            {
                                continue;
                            }

                            if (total >= from && total <= to)
                            {
                                // 出力
                                var data = new PfkTransSheetEntry();

                                data.English = langEntry.Text;

                                if (useTag)
                                {
                                    //// 制御文字をタグ化する。
                                    data.English = PfkTransSheetEntry.GetEscapedText(data.English);
                                }

                                //// TAB文字は無条件にタグ化する。
                                //// 理由：CSVファイルを表計算ソフトで読み込んだ場合、TAB文字は無視されてしまう場合があるため。
                                data.English = PfkTransSheetEntry.GetEscapedTab(data.English);

                                data.Key = langEntry.Key;
                                writer.WriteRecord(data);
                                writer.NextRecord();
                            }

                            total++;
                        }
                    }
                }
            }
        }

        private static void WriteExcelHeader(IXLWorksheet worksheet)
        {
            int colimnNo = 0;
            //// ヘッダー出力
            colimnNo++;
            worksheet.Cell(1, colimnNo).Value = "[[Key]]";
            colimnNo++;
            worksheet.Cell(1, colimnNo).Value = "[[English]]";
            colimnNo++;
            worksheet.Cell(1, colimnNo).Value = "[[Japanese]]";
            colimnNo++;
            worksheet.Cell(1, colimnNo).Value = "[[MachineTranslation]]";
            colimnNo++;
            worksheet.Cell(1, colimnNo).Value = "[[ReferenceID]]";
            colimnNo++;
            worksheet.Cell(1, colimnNo).Value = "[[Sequence]]";
        }

        /// <summary>
        /// 格納ルール：翻訳シート作成用
        /// </summary>
        public class CsvMapperTransSheet : CsvHelper.Configuration.ClassMap<PfkTransSheetEntry>
        {
            public CsvMapperTransSheet()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.Key).Name("[[Key]]");
                this.Map(x => x.English).Name("[[English]]");
                this.Map(x => x.Japanese).Name("[[Japanese]]");
                this.Map(x => x.MachineTranslation).Name("[[MachineTranslation]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
                this.Map(x => x.Sequence).Name("[[Sequence]]");
            }
        }

        /// <summary>
        /// 格納ルール：MOD作成用。必要最低限の項目のみ使用する。
        /// </summary>
        public class CsvMapperTransSheetForMod : CsvHelper.Configuration.ClassMap<PfkTransSheetEntry>
        {
            public CsvMapperTransSheetForMod()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.Key).Name("[[Key]]");
                this.Map(x => x.Japanese).Name("[[Japanese]]");
                this.Map(x => x.ReferenceID).Name("[[ReferenceID]]");
                this.Map(x => x.MachineTranslation).Name("[[MachineTranslation]]");
            }
        }

        /// <summary>
        /// 格納ルール：比較用用。必要最低限の項目のみ使用する。
        /// </summary>
        public class CsvMapperTransSheetForCompare : CsvHelper.Configuration.ClassMap<PfkTransSheetEntry>
        {
            public CsvMapperTransSheetForCompare()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.Key).Name("[[Key]]");
                this.Map(x => x.English).Name("[[English]]");
            }
        }
    }
}
