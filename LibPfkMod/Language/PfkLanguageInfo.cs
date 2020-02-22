namespace LibPfkMod.Language
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 言語情報
    /// </summary>
    public class PfkLanguageInfo
    {
        /// <summary>
        /// 言語ファイルの辞書。
        /// キーは File ID。
        /// </summary>
        public Dictionary<string, PfkLanguageFile> Items { get; } =
            new Dictionary<string, PfkLanguageFile>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 言語ファイルを追加する。
        /// </summary>
        /// <param name="newSheetFile">言語ファイル</param>
        public void AddFile(PfkLanguageFile newSheetFile)
        {
            if (this.Items.ContainsKey(newSheetFile.FileID))
            {
                //// すでに登録済みの場合は、言語エントリーを追加する。
                var currentSheetFile = this.Items[newSheetFile.FileID];
                foreach (var newEntry in newSheetFile.Items.Values)
                {
                    var langEntry = new PfkLanguageEntry(newEntry.Key, newEntry.Text);
                    currentSheetFile.AddEntry(langEntry);
                }
            }
            else
            {
                this.Items.Add(newSheetFile.FileID, newSheetFile);
            }
        }

        /// <summary>
        /// 指定されたキーの言語エントリーを返す。
        /// 存在しない場合は null object を返す。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>言語エントリー</returns>
        public PfkLanguageEntry GetEntry(Guid key)
        {
            foreach (var langFile in this.Items.Values)
            {
                var langEntry = langFile.GetEntry(key);
                if (langEntry != null)
                {
                    return langEntry;
                }
            }

            return new PfkLanguageEntry(key, string.Empty);
        }

        /// <summary>
        /// 空白以外のエントリー数を返す。
        /// </summary>
        /// <returns>空白以外のエントリー数</returns>
        public int GetEntryCountWithoutEmpty()
        {
            int totalEntryCount = 0;
            foreach (var langFile in this.Items.Values)
            {
                totalEntryCount += langFile.GetEntryCountWithoutEmpty();
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

            foreach (var langFile in this.Items)
            {
                buff.Append(langFile.ToString());
            }

            return buff.ToString();
        }
    }
}
