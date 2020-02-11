namespace LibPfk.Glossary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// 用語集エントリー
    /// </summary>
    public class PfkGlossaryEntry
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="conversionType">変換タイプ</param>
        /// <param name="prefix">接頭辞</param>
        /// <param name="originalText">原文</param>
        /// <param name="translatedText">翻訳文</param>
        public PfkGlossaryEntry(
            NConversionType conversionType, string prefix, string originalText, string translatedText)
        {
            this.ConversionType = conversionType;
            this.OriginalText = originalText;
            this.Prefix = prefix;
            this.TranslatedText = translatedText;
        }

        /// <summary>
        /// 会話タイプ
        /// </summary>
        public enum NConversionType
        {
            /// <summary>
            /// コメント：このデータは無効のため使用しない。
            /// </summary>
            None,

            /// <summary>
            /// 常に適用
            /// </summary>
            AlwaysTranslate,

            /// <summary>
            /// 固有名詞日本語版のみ適用
            /// </summary>
            NounTranslate,
        }

        /// <summary>
        /// 変換タイプ
        /// </summary>
        public NConversionType ConversionType { get; } = NConversionType.None;

        /// <summary>
        /// 接頭辞
        /// </summary>
        public string Prefix { get; } = string.Empty;

        /// <summary>
        /// 原文
        /// </summary>
        public string OriginalText { get; } = string.Empty;

        /// <summary>
        /// 翻訳文
        /// </summary>
        public string TranslatedText { get; } = string.Empty;

        /// <summary>
        /// デバッグ用
        /// </summary>
        /// <returns>デバッグ用テキスト</returns>
        public override string ToString()
        {
            return $"ConversionType({this.ConversionType}) Prefix({this.Prefix}) Original({this.OriginalText}) Translated({this.TranslatedText})";
        }
    }
}
