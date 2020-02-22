namespace LibPfkMod.TransSheet
{
    using System;
    using System.Text;

    /// <summary>
    /// 翻訳シートエントリー
    /// </summary>
    public class PfkTransSheetEntry
    {
        /// <summary>
        /// File ID
        /// </summary>
        public string FileID { get; set; } = string.Empty;

        /// <summary>
        /// キー
        /// </summary>
        public Guid Key { get; set; } = default;

        /// <summary>
        /// 原文
        /// </summary>
        public string English { get; set; } = string.Empty;

        /// <summary>
        /// 翻訳文
        /// </summary>
        public string Japanese { get; set; } = string.Empty;

        /// <summary>
        /// 機械翻訳文
        /// </summary>
        public string MachineTranslation { get; set; } = string.Empty;

        /// <summary>
        /// リファレンスID
        /// </summary>
        public string ReferenceID { get; set; } = string.Empty;

        /// <summary>
        /// シーケンス番号
        /// </summary>
        public int Sequence { get; set; } = 0;

        /// <summary>
        /// テキスト中のTAB文字をタグ化したテキストを返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>タグ化したテキスト</returns>
        public static string GetEscapedTab(string text)
        {
            var buff = new StringBuilder(text);

            buff.Replace("\t", "<TAB>");

            return buff.ToString();
        }

        /// <summary>
        /// テキスト中のTABタグをTABに置換したテキストを返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>TABに置換したテキスト</returns>
        public static string GetUnEscapedTab(string text)
        {
            var buff = new StringBuilder(text);

            buff.Replace("<TAB>", "\t");

            return buff.ToString();
        }

        /// <summary>
        /// タグ化されたテキストを制御文字化したテキストを返す。
        /// </summary>
        /// <param name="text">タグ化されたテキスト</param>
        /// <returns>制御文字化したテキスト</returns>
        public static string GetUnEscapedText(string text)
        {
            var buff = new StringBuilder(text);

            buff.Replace("<CRLF>", "\r\n");
            buff.Replace("<CR>", "\r");
            buff.Replace("<LF>", "\n");

            return buff.ToString();
        }

        /// <summary>
        /// テキスト中の制御文字をタグ化したテキストを返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>タグ化したテキスト</returns>
        public static string GetEscapedText(string text)
        {
            var buff = new StringBuilder(text);

            buff.Replace("\r\n", "<CRLF>");
            buff.Replace("\r", "<CR>");
            buff.Replace("\n", "<LF>");

            return buff.ToString();
        }

        /// <summary>
        /// 翻訳済みのテキストを返す。
        /// </summary>
        /// <param name="originalText">原文</param>
        /// <param name="useMT">機械翻訳の有無</param>
        /// <param name="mtMark">機械翻訳の印</param>
        /// <param name="forceMT">置換文字などの特殊文字列を含む場合でも機械翻訳を適用する</param>
        /// <returns>翻訳済みのテキスト</returns>
        public string Translate(string originalText, bool useMT, string mtMark, bool forceMT)
        {
            //// en | jp | mt | result
            ////  o |  o |  o | jp
            ////  o |  o |  x | jp
            ////  o |  x |  o | mt <-> en
            ////  o |  x |  x | en
            ////  x |  o |  o | en
            ////  x |  o |  x | en
            ////  x |  x |  o | en
            ////  x |  x |  x | en
            bool en = !string.IsNullOrWhiteSpace(originalText);
            bool jp = !string.IsNullOrWhiteSpace(this.Japanese);
            bool mt = !string.IsNullOrWhiteSpace(this.MachineTranslation);
            if (en && jp && mt)
            {
                return this.Japanese;
            }
            else if (en && jp && !mt)
            {
                return this.Japanese;
            }
            else if (en && !jp && mt)
            {
                if (useMT)
                {
                    if (this.HasSpecialString(this.MachineTranslation))
                    {
                        //// 置換文字などの特殊文字列を含む場合は、機械翻訳せず原文を返す。
                        return originalText;
                    }
                    else
                    {
                        return $"{mtMark}{this.MachineTranslation}";
                    }
                }
                else
                {
                    return originalText;
                }
            }
            else if (en && !jp && !mt)
            {
                return originalText;
            }
            else if (!en && jp && mt)
            {
                return originalText;
            }
            else if (!en && jp && !mt)
            {
                return originalText;
            }
            else if (!en && !jp && mt)
            {
                return originalText;
            }
            else if (!en && !jp && !mt)
            {
                return originalText;
            }
            else
            {
                throw new Exception($"logic error.");
            }
        }

        /// <summary>
        /// Debug用
        /// </summary>
        /// <returns>Debug用テキスト</returns>
        public override string ToString()
        {
            var buff = new StringBuilder();

            buff.AppendLine($"Key({this.Key}), ReferenceID({this.ReferenceID}), English({this.English}), Japanese({this.Japanese}), MT({this.MachineTranslation}), No({this.Sequence})");

            return buff.ToString();
        }

        /// <summary>
        /// テキスト中に特殊文字列の存在有無を返す。
        /// </summary>
        /// <param name="text">テキスト</param>
        /// <returns>特殊文字列の有無</returns>
        private bool HasSpecialString(string text)
        {
            if (text.Contains("{") || text.Contains("<") || text.Contains("["))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
