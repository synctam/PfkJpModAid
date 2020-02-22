namespace LibPfkMod.Umm
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using CsvHelper;
    using LibPfk.Glossary;
    using LibPfkMod.Language;
    using LibPfkMod.TransSheet;
    using QuickType;

    /// <summary>
    /// UMMデータの入出力
    /// </summary>
    public class PfkUmmDataDao
    {
        /// <summary>
        /// UMM形式のCSVファイルの読み込み。
        /// </summary>
        /// <param name="ummDataInfo">Ummデータ情報</param>
        /// <param name="path">UMM形式のCSVファイルのパス</param>
        /// <param name="enc">文字コード</param>
        public static void LoadFromCsv(PfkUmmDataInfo ummDataInfo, string path, Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.RegisterClassMap<CsvMapperUmm>();
                    var records = csv.GetRecords<PfkUmmDataEntry>();

                    foreach (var record in records)
                    {
                        ummDataInfo.AddEntry(record);
                    }
                }
            }
        }

        /// <summary>
        /// UMM形式のCSVファイルの保存。
        /// </summary>
        /// <param name="originalPath">原文のパス</param>
        /// <param name="sheetInfo">翻訳シートのパス</param>
        /// <param name="glossaryInfo">用語集のパス</param>
        /// <param name="fileNameOutput">日本語化ファイルのパス</param>
        /// <param name="useReferenceID">ReferenceIDの出力有無</param>
        /// <param name="useMT">機械翻訳の使用有無</param>
        /// <param name="mtMark">機械翻訳テキストの先頭に表示する文字</param>
        /// <param name="forceMT">機械翻訳使用時、置換文字などの特殊文字列を含む場合でも機械翻訳を使用する</param>
        public static void SaveToUmmFile(
            string originalPath,
            PfkTransSheetInfo sheetInfo,
            PfkGlossaryInfo glossaryInfo,
            string fileNameOutput,
            bool useReferenceID,
            bool useMT,
            string mtMark,
            bool forceMT)
        {
            bool useGlossary = false;
            if (glossaryInfo.Count > 0)
            {
                //// 用語集が未指定(空)の場合は、用語集を使用しない。
                useGlossary = true;
            }

            //// 原文の言語ファイルのパスからFileIDを取得する。
            var fileID = PfkLanguageFile.GetFileID(originalPath);
            //// FileIDに該当する翻訳ファイルを取得する。
            var sheetFile = sheetInfo.GetFile(fileID);
            if (sheetFile == null)
            {
                throw new Exception($"Trans sheet file not found. File({originalPath})");
            }

            //// 原文を読み込む。
            var text = string.Empty;
            using (var sr = new StreamReader(originalPath, Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }

            //// JSONテキストから、言語情報を取得する。
            var jsonData = PfkGameDesignBase.FromJson(text);

            using (var sw = new StreamWriter(fileNameOutput, false, Encoding.UTF8))
            {
                using (var writer = new CsvWriter(sw, CultureInfo.InvariantCulture))
                {
                    writer.Configuration.RegisterClassMap<CsvMapperUmm>();
                    writer.WriteHeader<PfkUmmDataEntry>();
                    writer.NextRecord();

                    int no = 1;
                    foreach (var stringPair in jsonData.Strings)
                    {
                        var ummEntry = new PfkUmmDataEntry();

                        var sheetEntry = sheetFile.GetEntry(stringPair.Key);
                        //// 翻訳済みテキストを取得する。
                        var translatedText = sheetEntry.Translate(stringPair.Value, useMT, mtMark, forceMT);
                        //// ReferenceIDを付加する。
                        if (useReferenceID && !string.IsNullOrWhiteSpace(translatedText))
                        {
                            translatedText = $"#{sheetEntry.ReferenceID}:{translatedText}";
                        }

                        if (useGlossary)
                        {
                            //// 用語を置換する。
                            translatedText = glossaryInfo.ReplaceVariable(
                                translatedText, PfkGlossaryEntry.NConversionType.NounTranslate, string.Empty);
                        }

                        ummEntry.Key = stringPair.Key;
                        ummEntry.Value = translatedText;
                        ummEntry.No = sheetEntry.ReferenceID;
                        writer.WriteRecord(ummEntry);
                        writer.NextRecord();
                        no++;
                    }
                }
            }
        }

        /// <summary>
        /// 格納ルール：MOD作成用。必要最低限の項目のみ使用する。
        /// </summary>
        public class CsvMapperUmm : CsvHelper.Configuration.ClassMap<PfkUmmDataEntry>
        {
            public CsvMapperUmm()
            {
                // 出力時の列の順番は指定した順となる。
                this.Map(x => x.Key).Name("Key");
                this.Map(x => x.Value).Name("Value");
                this.Map(x => x.No).Name("No");
            }
        }
    }
}
