namespace LibPfkMod.Language
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// 言語ファイル
    /// </summary>
    public class PfkLanguageFile
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="path">言語データファイルのパス</param>
        public PfkLanguageFile(string path)
        {
            this.FileID = GetFileID(path);
        }

        /// <summary>
        /// 言語エントリーの辞書。
        /// キーは、Key。
        /// </summary>
        public Dictionary<Guid, PfkLanguageEntry> Items { get; } =
            new Dictionary<Guid, PfkLanguageEntry>();

        /// <summary>
        /// File ID
        /// </summary>
        public string FileID { get; } = string.Empty;

        /// <summary>
        /// 言語ファイルのパスからFileIDを返す。
        /// </summary>
        /// <param name="path">言語ファイルのパス</param>
        /// <returns>FileID</returns>
        public static string GetFileID(string path)
        {
            var fullPath = Path.GetFullPath(path);
            var fileID = Path.GetFileNameWithoutExtension(fullPath);

            return fileID;
        }

        /// <summary>
        /// 言語エントリーを追加する。
        /// </summary>
        /// <param name="langEntry">言語エントリー</param>
        public void AddEntry(PfkLanguageEntry langEntry)
        {
            if (this.Items.ContainsKey(langEntry.Key))
            {
                throw new Exception($"Duplicate key({langEntry.Key}). FileID({this.FileID})");
            }
            else
            {
                this.Items.Add(langEntry.Key, langEntry);
            }
        }

        /// <summary>
        /// 指定されたキーの言語エントリーを返す。
        /// 存在しない場合は null を返す。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>言語エントリー</returns>
        public PfkLanguageEntry GetEntry(Guid key)
        {
            if (this.Items.ContainsKey(key))
            {
                return this.Items[key];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 空白以外のエントリー数を返す。
        /// </summary>
        /// <returns>空白以外のエントリー数</returns>
        public int GetEntryCountWithoutEmpty()
        {
            int totalEntryCount = 0;
            foreach (var langEntry in this.Items.Values)
            {
                if (!string.IsNullOrWhiteSpace(langEntry.Text))
                {
                    totalEntryCount++;
                }
            }

            return totalEntryCount;
        }

        /// <summary>
        /// Debug用
        /// </summary>
        /// <returns>Debug用テキストを返す</returns>
        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"FileID({this.FileID})");
            foreach (var entry in this.Items)
            {
                buff.Append($"\t{entry.ToString()}");
            }

            return buff.ToString();
        }
    }
}
