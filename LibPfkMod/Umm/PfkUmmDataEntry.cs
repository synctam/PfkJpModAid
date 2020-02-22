namespace LibPfkMod.Umm
{
    using System;

    /// <summary>
    /// UMMデータエントリー
    /// </summary>
    public class PfkUmmDataEntry
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PfkUmmDataEntry() { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">テキスト</param>
        /// <param name="no">番号</param>
        public PfkUmmDataEntry(Guid key, string value, string no)
        {
            this.Key = key;
            this.Value = value;
            this.No = no;
        }

        /// <summary>
        /// キー
        /// </summary>
        public Guid Key { get; set; } = default;

        /// <summary>
        /// テキスト
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 番号
        /// </summary>
        public string No { get; set; } = string.Empty;
    }
}
