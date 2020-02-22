namespace LibPfkMod.Language
{
    using System;
    using System.Text;

    /// <summary>
    /// 言語エントリー
    /// </summary>
    public class PfkLanguageEntry
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="text">テキスト</param>
        public PfkLanguageEntry(Guid key, string text)
        {
            this.Key = key;
            this.Text = text;
        }

        /// <summary>
        /// キー
        /// </summary>
        public Guid Key { get; }

        /// <summary>
        /// テキスト
        /// </summary>
        public string Text { get; } = string.Empty;

        /// <summary>
        /// Debug用
        /// </summary>
        /// <returns>Debug用テキストを返す</returns>
        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();

            buff.AppendLine($"({this.Key}), Text({this.Text})");

            return buff.ToString();
        }
    }
}
