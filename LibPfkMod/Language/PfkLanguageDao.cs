namespace LibPfkMod.Language
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CsvHelper;
    using LibPfk.Glossary;
    using LibPfkMod.TransSheet;
    using QuickType;

    /// <summary>
    /// 言語情報の入出力
    /// </summary>
    public class PfkLanguageDao
    {
        /// <summary>
        /// 言語ファイルから言語情報を作成する。
        /// </summary>
        /// <param name="sheetInfo">言語情報</param>
        /// <param name="path">言語ファイルのパス</param>
        public static void LoadFromFile(PfkLanguageInfo sheetInfo, string path)
        {
            using (var sr = new StreamReader(path, Encoding.UTF8))
            {
                var sheetFile = new PfkLanguageFile(path);
                sheetInfo.AddFile(sheetFile);
                //// JSONファイルをテキスト形式で読み込む。
                var text = sr.ReadToEnd();
                //// テキスト形式のJSONファイルを解析する。
                var jsonData = PfkGameDesignBase.FromJson(text);
                foreach (var stringPair in jsonData.Strings)
                {
                    //// 解析済みデータから、言語エントリーを作成する。
                    var langEntry = new PfkLanguageEntry(stringPair.Key, stringPair.Value);
                    sheetFile.AddEntry(langEntry);
                }
            }
        }

        /// <summary>
        /// 翻訳済みのJSONファイルを出力する。
        /// </summary>
        /// <param name="originalPath">原文の言語ファイルのパス</param>
        /// <param name="sheetInfo">翻訳情報</param>
        /// <param name="glossaryInfo">用語集情報</param>
        /// <param name="fileNameOutput">翻訳されたJSONファイルのパス</param>
        /// <param name="useReferenceID">ReferenveIDの有無</param>
        /// <param name="useMT">機械翻訳の有無</param>
        /// <param name="mtMark">機械翻訳の印</param>
        public static void SaveToFile(
            string originalPath,
            PfkTransSheetInfo sheetInfo,
            PfkGlossaryInfo glossaryInfo,
            string fileNameOutput,
            bool useReferenceID,
            bool useMT,
            string mtMark)
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

            foreach (var stringPair in jsonData.Strings)
            {
                var sheetEntry = sheetFile.GetEntry(stringPair.Key);
                //// 翻訳済みテキストを取得する。
                var translatedText = sheetEntry.Translate(stringPair.Value, useMT, mtMark);
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

                //// JSONに翻訳済みテキストを反映する。
                stringPair.Value = translatedText;
            }

            //// フォーマット済みの形式でJSONファイル書き出す。
            QuickType.Converter.Settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //// JSON形式のテキストを取得する。
            var translatedJson = Serialize.ToJson(jsonData);
            var utf8WithoutBom = new UTF8Encoding(false);
            //// UTF-8(BOMなし)で出力する。
            using (var sw = new StreamWriter(fileNameOutput, false, utf8WithoutBom))
            {
                sw.Write(translatedJson);
            }
        }

        public static void SaveToUmmFile(
            string originalPath,
            PfkTransSheetInfo sheetInfo,
            PfkGlossaryInfo glossaryInfo,
            string fileNameOutput,
            bool useReferenceID,
            bool useMT,
            string mtMark)
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
                    writer.WriteHeader<UnityModManagerEntry>();
                    writer.NextRecord();

                    int no = 1;
                    foreach (var stringPair in jsonData.Strings)
                    {
                        var ummEntry = new UnityModManagerEntry();

                        var sheetEntry = sheetFile.GetEntry(stringPair.Key);
                        //// 翻訳済みテキストを取得する。
                        var translatedText = sheetEntry.Translate(stringPair.Value, useMT, mtMark);
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

                        ummEntry.Key = stringPair.Key.ToString();
                        ummEntry.Value = translatedText;
                        ummEntry.No = sheetEntry.ReferenceID;
                        writer.WriteRecord(ummEntry);
                        writer.NextRecord();
                        no++;
                    }
                }
            }
        }

        public class UnityModManagerEntry
        {
            public string Key { get; set; } = string.Empty;

            public string Value { get; set; } = string.Empty;

            public string No { get; set; } = string.Empty;
        }

        /// <summary>
        /// 格納ルール：MOD作成用。必要最低限の項目のみ使用する。
        /// </summary>
        public class CsvMapperUmm : CsvHelper.Configuration.ClassMap<UnityModManagerEntry>
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
