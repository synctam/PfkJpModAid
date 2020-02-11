namespace LibPfkMod.TransSheet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 翻訳シートファイル
    /// </summary>
    public class PfkTransSheetFile
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">FileID</param>
        public PfkTransSheetFile(string path)
        {
            this.FileID = this.GetFileID(path);
        }

        /// <summary>
        /// 翻訳シートエントリーの辞書。
        /// キーは File ID。
        /// </summary>
        public Dictionary<Guid, PfkTransSheetEntry> Items { get; } =
            new Dictionary<Guid, PfkTransSheetEntry>();

        /// <summary>
        /// File ID
        /// </summary>
        public string FileID { get; } = string.Empty;

        /// <summary>
        /// 翻訳エントリーを追加する。
        /// </summary>
        /// <param name="entry">翻訳エントリー</param>
        public void AddEntry(PfkTransSheetEntry entry)
        {
            if (this.Items.ContainsKey(entry.Key))
            {
                throw new Exception($"Duplicate entry. Key({entry.Key})");
            }
            else
            {
                this.Items.Add(entry.Key, entry);
            }
        }

        /// <summary>
        /// 翻訳結果を返す。
        /// </summary>
        /// <param name="originalText">原文</param>
        /// <param name="key">キー</param>
        /// <param name="useMT">機械翻訳の有無</param>
        /// <param name="mtMark">機械翻訳の印</param>
        /// <returns>翻訳結果</returns>
        public string Translate(string originalText, Guid key, bool useMT, string mtMark)
        {
            if (this.Items.ContainsKey(key))
            {
                var entry = this.Items[key];
                var translatedText = entry.Translate(originalText, useMT, mtMark);
                return translatedText;
            }
            else
            {
                return originalText;
            }
        }

        /// <summary>
        /// キーに該当する翻訳シートエントリーを返す。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>翻訳シートエントリー</returns>
        public PfkTransSheetEntry GetEntry(Guid key)
        {
            if (this.Items.ContainsKey(key))
            {
                return this.Items[key];
            }
            else
            {
                return new PfkTransSheetEntry();
            }
        }

        /// <summary>
        /// 言語データファイルのパスからFileIDを返す。
        /// </summary>
        /// <param name="path">言語データファイルのパス</param>
        /// <returns>FileID</returns>
        private string GetFileID(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var fileID = Path.GetFileNameWithoutExtension(fullPath);

            return fileID;
        }
    }
}
