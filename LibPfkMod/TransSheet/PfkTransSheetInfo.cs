namespace LibPfkMod.TransSheet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LibPfkMod.Language;

    /// <summary>
    /// 翻訳シート情報
    /// </summary>
    public class PfkTransSheetInfo
    {
        /// <summary>
        /// 翻訳シートファイルの辞書。
        /// キーは、FileID。
        /// </summary>
        public Dictionary<string, PfkTransSheetFile> Items { get; } =
            new Dictionary<string, PfkTransSheetFile>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 翻訳シートファイルを追加する。
        /// </summary>
        /// <param name="newSheetFile">翻訳シートファイル</param>
        public void AddFile(PfkTransSheetFile newSheetFile)
        {
            if (this.Items.ContainsKey(newSheetFile.FileID))
            {
                var currentSheetFile = this.Items[newSheetFile.FileID];
                foreach (var newSheetEntry in newSheetFile.Items.Values)
                {
                    if (currentSheetFile.Items.ContainsKey(newSheetEntry.Key))
                    {
                        throw new Exception($"Duplicate key({newSheetEntry.Key}). FileID({newSheetFile.FileID})");
                    }
                    else
                    {
                        currentSheetFile.AddEntry(newSheetEntry);
                    }
                }
            }
            else
            {
                this.Items.Add(newSheetFile.FileID, newSheetFile);
            }
        }

        /// <summary>
        /// FileIDに該当する翻訳シートファイルを返す。
        /// </summary>
        /// <param name="fileID">FileID</param>
        /// <returns>翻訳シートファイル</returns>
        public PfkTransSheetFile GetFile(string fileID)
        {
            if (this.Items.ContainsKey(fileID))
            {
                var langFile = this.Items[fileID];
                return langFile;
            }
            else
            {
                return null;
            }
        }
    }
}
