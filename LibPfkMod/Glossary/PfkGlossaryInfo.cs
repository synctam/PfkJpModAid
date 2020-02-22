namespace LibPfk.Glossary
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 用語集情報
    /// </summary>
    public class PfkGlossaryInfo
    {
        /// <summary>
        /// 用語の登録数
        /// </summary>
        public int Count
        {
            get
            {
                return this.Items.Count;
            }
        }

        /// <summary>
        /// 対訳表。キーは prefix:originalText または originalText
        /// </summary>
        private Dictionary<string, PfkGlossaryEntry> Items { get; } = new Dictionary<string, PfkGlossaryEntry>();

        /// <summary>
        /// 単語登録
        /// </summary>
        /// <param name="glossaryEntry">単語エントリ</param>
        /// <returns>処理結果</returns>
        public bool AddEntry(PfkGlossaryEntry glossaryEntry)
        {
            var key = this.MakeKey(glossaryEntry.Prefix, glossaryEntry.OriginalText);
            if (this.Items.ContainsKey(key))
            {
                var msg = $"Duplicate key({key}).";
                Console.WriteLine(msg);
                return false;
            }
            else
            {
                this.Items.Add(key, glossaryEntry);
                return true;
            }
        }

        /// <summary>
        /// 用語集に従いテキストを翻訳する。
        /// </summary>
        /// <param name="buff">文字列のバッファ</param>
        /// <param name="nConversionType">変換タイプ</param>
        /// <param name="pattern">変換パターン</param>
        /// <returns>変換後のテキスト</returns>
        public bool ReplaceVariable(StringBuilder buff, PfkGlossaryEntry.NConversionType nConversionType, string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                pattern = $@"@.*?@";
            }

            var buffText = buff.ToString();
            HashSet<string> hash = new HashSet<string>();
            //// テキスト中の変数を抽出する。
            var regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var mc = regEx.Matches(buff.ToString());
            foreach (Match mached in mc)
            {
                //// 抽出されたテキストから変数($で囲まれた文字列)部分を抽出する。
                hash.Add(mached.Value);
            }

            if (hash.Count == 0)
            {
                return false;
            }

            foreach (var variable in hash)
            {
                var tranclatexNoun = this.GetTranslatedNoun(variable);
                if (string.IsNullOrWhiteSpace(tranclatexNoun))
                {
                    Console.WriteLine($"Undefined noun({variable}).");
                }
                else
                {
                    switch (nConversionType)
                    {
                        case PfkGlossaryEntry.NConversionType.None:
                            var noun = variable.Replace("@", string.Empty);
                            if (noun.Contains("_"))
                            {
                                var splittedNoun = noun.Split('_');
                                noun = splittedNoun[1];
                            }

                            buff.Replace(variable, noun);
                            break;
                        case PfkGlossaryEntry.NConversionType.AlwaysTranslate:
                            buff.Replace(variable, tranclatexNoun);
                            break;
                        case PfkGlossaryEntry.NConversionType.NounTranslate:
                            buff.Replace(variable, tranclatexNoun);
                            break;
                        default:
                            throw new Exception($"Unknown ConversionType({nConversionType}).");
                    }
                }
            }

            return true;
        }

        public string ReplaceVariable(string text, PfkGlossaryEntry.NConversionType nConversionType, string pattern)
        {
            StringBuilder buff = new StringBuilder(text);

            this.ReplaceVariable(buff, nConversionType, pattern);

            return buff.ToString();
        }

        public PfkGlossaryEntry GetEntry(string prefix, string originalText)
        {
            var key = this.MakeKey(prefix, originalText);
            if (this.Items.ContainsKey(key))
            {
                return this.Items[key];
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            StringBuilder buff = new StringBuilder();
            foreach (var entry in this.Items.Values)
            {
                buff.AppendLine(entry.ToString());
            }

            return buff.ToString();
        }

        /// <summary>
        /// 変数から対訳を得る。存在しない場合はstring.emptyを返す。
        /// </summary>
        /// <param name="variable">変数名</param>
        /// <returns>対訳</returns>
        private string GetTranslatedNoun(string variable)
        {
            var prefix = string.Empty;
            var originalText = string.Empty;

            variable = variable.Replace("_", ":");
            variable = variable.Replace("@", string.Empty);

            if (this.Items.ContainsKey(variable))
            {
                return this.Items[variable].TranslatedText;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// キーの作成。
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="originalText">OriginalText</param>
        /// <returns>キー</returns>
        private string MakeKey(string prefix, string originalText)
        {
            var key = string.Empty;
            if (string.IsNullOrWhiteSpace(prefix))
            {
                key = originalText;
            }
            else
            {
                key = $"{prefix}:{originalText}";
            }

            return key;
        }
    }
}
