namespace LibPfkMod.Language
{
    using System;
    using System.IO;
    using System.Text;
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
        /// <param name="forceMT">置換文字などの特殊文字列を含む場合でも機械翻訳を適用する</param>
        public static void SaveToFile(
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

            foreach (var stringPair in jsonData.Strings)
            {
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
    }
}
