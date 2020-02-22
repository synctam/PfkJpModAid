namespace LibPfkMod.Umm
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Ummデータ情報
    /// </summary>
    public class PfkUmmDataInfo
    {
        /// <summary>
        /// UMM形式のデータエントリー。
        /// キーは、GUID
        /// </summary>
        public Dictionary<Guid, PfkUmmDataEntry> Items { get; } =
            new Dictionary<Guid, PfkUmmDataEntry>();

        /// <summary>
        /// UmmDataEntryを追加する。
        /// </summary>
        /// <param name="entry">UmmDataEntry</param>
        public void AddEntry(PfkUmmDataEntry entry)
        {
            if (this.Items.ContainsKey(entry.Key))
            {
                throw new Exception($"Duplicate key({entry.Key})");
            }
            else
            {
                this.Items.Add(entry.Key, entry);
            }
        }

        /// <summary>
        /// 指定したキーのUmmDataEntryを返す。
        /// </summary>
        /// <param name="key">キー</param>
        /// <returns>UmmDataEntry</returns>
        public PfkUmmDataEntry GetEntry(Guid key)
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
    }
}
