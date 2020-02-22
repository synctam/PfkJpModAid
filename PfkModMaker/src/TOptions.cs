// ******************************************************************************
// Copyright (c) 2015-2019 synctam
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace MonoOptions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Mono.Options;

    /// <summary>
    /// コマンドライン オプション
    /// </summary>
    public class TOptions
    {
        //// ******************************************************************************
        //// Property fields
        //// ******************************************************************************
        private TArgs args;
        private bool isError = false;
        private StringWriter errorMessage = new StringWriter();
        private OptionSet optionSet;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arges">コマンドライン引数</param>
        public TOptions(string[] arges)
        {
            this.args = new TArgs();
            this.Settings(arges);
            if (this.IsError)
            {
                this.ShowErrorMessage();
                this.ShowUsage();
            }
            else
            {
                this.CheckOption();
                if (this.IsError)
                {
                    this.ShowErrorMessage();
                    this.ShowUsage();
                }
                else
                {
                    // skip
                }
            }
        }

        public enum NSheetType
        {
            /// <summary>
            /// 単一ファイル
            /// </summary>
            SingleFile,

            /// <summary>
            /// 分割ファイル
            /// </summary>
            MultiFile,

            /// <summary>
            /// 不明
            /// </summary>
            Unknown,
        }

        //// ******************************************************************************
        //// Property
        //// ******************************************************************************

        /// <summary>
        /// コマンドライン オプション
        /// </summary>
        public TArgs Arges { get { return this.args; } }

        /// <summary>
        /// コマンドライン オプションのエラー有無
        /// </summary>
        public bool IsError { get { return this.isError; } }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage { get { return this.errorMessage.ToString(); } }

        /// <summary>
        /// Uasgeを表示する
        /// </summary>
        public void ShowUsage()
        {
            TextWriter writer = Console.Error;
            this.ShowUsage(writer);
        }

        /// <summary>
        /// Uasgeを表示する
        /// </summary>
        /// <param name="textWriter">出力先</param>
        public void ShowUsage(TextWriter textWriter)
        {
            var msg = new StringWriter();

            string exeName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            msg.WriteLine(string.Empty);
            msg.WriteLine($@"使い方：");
            msg.WriteLine($@"日本語化MODを作成する。");
            msg.WriteLine(
                $@"  usage: {exeName} -i <original lang file path> -o <japanized lang folder path>" +
                $@" -s <Trans Sheet or folder path> [-g <glossary path>] [-k <value>] [-e] [-u] [-m] [-f] [-r] [-h]");
            msg.WriteLine($@"OPTIONS:");
            this.optionSet.WriteOptionDescriptions(msg);
            msg.WriteLine($@"Example:");
            msg.WriteLine($@"  翻訳シート(-s)とオリジナルの言語ファイル(-i)を元に日本語化MOD(-o)を作成する。");
            msg.WriteLine(
                $@"    {exeName} -i EN\enGB.json -o JP\enGB.json" +
                $@" -s PfkTransSheet.csv");
            msg.WriteLine($@"終了コード:");
            msg.WriteLine($@" 0  正常終了");
            msg.WriteLine($@" 1  異常終了");
            msg.WriteLine();

            if (textWriter == null)
            {
                textWriter = Console.Error;
            }

            textWriter.Write(msg.ToString());
        }

        /// <summary>
        /// エラーメッセージ表示
        /// </summary>
        public void ShowErrorMessage()
        {
            TextWriter writer = Console.Error;
            this.ShowErrorMessage(writer);
        }

        /// <summary>
        /// エラーメッセージ表示
        /// </summary>
        /// <param name="textWriter">出力先</param>
        public void ShowErrorMessage(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                textWriter = Console.Error;
            }

            textWriter.Write(this.ErrorMessage);
        }

        /// <summary>
        /// オプション文字の設定
        /// </summary>
        /// <param name="args">args</param>
        private void Settings(string[] args)
        {
            this.optionSet = new OptionSet()
            {
                { "i|in="      , this.args.FileNameInputText   , v => this.args.FileNameInput    = v},
                { "o|out="     , this.args.FileNameOutputText  , v => this.args.FileNameOutput   = v},
                { "s|sheet="   , this.args.FileNameSheetText   , v => this.args.FileNameSheet    = v},
                { "g|glossary=", this.args.FileNameGlossaryText, v => this.args.FileNameGlossary = v},
                { "k|mark="    , this.args.MtransMarkText      , v => this.args.MtransMark       = v},
                { "e|refid"    , this.args.UseReferenceIdText  , v => this.args.UseReferenceId   = v != null},
                { "u|umm"      , this.args.UseUnityModManText  , v => this.args.UseUnityModMan   = v != null},
                { "m"          , this.args.UseMachineTransText , v => this.args.UseMachineTrans  = v != null},
                { "f|forcemt"  , this.args.UseForceMtText      , v => this.args.UseForceMt       = v != null},
                { "r"          , this.args.UseReplaceText      , v => this.args.UseReplace       = v != null},
                { "h|help"     , "ヘルプ"                      , v => this.args.Help             = v != null},
            };

            List<string> extra;
            try
            {
                extra = this.optionSet.Parse(args);
                if (extra.Count > 0)
                {
                    // 指定されたオプション以外のオプションが指定されていた場合、
                    // extra に格納される。
                    // 不明なオプションが指定された。
                    this.SetErrorMessage($"{Environment.NewLine}エラー：不明なオプションが指定されました。");
                    extra.ForEach(t => this.SetErrorMessage(t));
                    this.isError = true;
                }
            }
            catch (OptionException e)
            {
                ////パースに失敗した場合OptionExceptionを発生させる
                this.SetErrorMessage(e.Message);
                this.isError = true;
            }
        }

        /// <summary>
        /// オプションのチェック
        /// </summary>
        private void CheckOption()
        {
            //// -h
            if (this.Arges.Help)
            {
                this.SetErrorMessage();
                this.isError = false;
                return;
            }

            if (this.IsErrorSheetSystemFile())
            {
                return;
            }

            if (this.IsErrorSheetGlossaryFile())
            {
                return;
            }

            if (this.IsErrorInputFile())
            {
                return;
            }

            if (this.IsErrorOutputFile())
            {
                return;
            }

            this.isError = false;
            return;
        }

        /// <summary>
        /// 翻訳シートの有無を確認
        /// </summary>
        /// <returns>翻訳シートの存在有無</returns>
        private bool IsErrorSheetSystemFile()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameSheet))
            {
                this.SetErrorMessage(
                    $@"{Environment.NewLine}エラー：(-s)翻訳シートのパスを指定してください。");
                this.Arges.SheetType = NSheetType.Unknown;
                this.isError = true;

                return true;
            }
            else
            {
                if (File.Exists(this.Arges.FileNameSheet))
                {
                    //// file
                    this.Arges.SheetType = NSheetType.SingleFile;
                }
                else if (Directory.Exists(this.Arges.FileNameSheet))
                {
                    //// folder
                    this.Arges.SheetType = NSheetType.MultiFile;
                }
                else
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：(-s)翻訳シートまたはフォルダーが見つかりません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameSheet)})");
                    this.Arges.SheetType = NSheetType.Unknown;
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 用語集の有無を確認する。
        /// </summary>
        /// <returns>用語集の有無</returns>
        private bool IsErrorSheetGlossaryFile()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameGlossary))
            {
                //// 未指定の場合は用語集を使用しない。
            }
            else
            {
                if (!File.Exists(this.Arges.FileNameGlossary))
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：(-g)用語集が見つかりません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameGlossary)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        private bool IsErrorInputFile()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameInput))
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}エラー：" +
                    $"(-i)オリジナル版の言語ファイルのパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!File.Exists(this.Arges.FileNameInput))
                {
                    this.SetErrorMessage(
                        $"{Environment.NewLine}エラー：" +
                        $"(-i)オリジナル版の言語ファイルがありません。" +
                        $"{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameInput)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        private bool IsErrorOutputFile()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameOutput))
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}エラー：" +
                    $"(-o)日本語化された言語ファイルのパスを指定してください。");
                this.isError = true;

                return true;
            }

            return false;
        }

        private void SetErrorMessage(string errorMessage = null)
        {
            if (errorMessage != null)
            {
                this.errorMessage.WriteLine(errorMessage);
            }
        }

        /// <summary>
        /// オプション項目
        /// </summary>
        public class TArgs
        {
            public string FileNameInput { get; internal set; }

            public string FileNameInputText { get; internal set; } =
                "オリジナル版の言語ファイルのパスを指定する。";

            public string FileNameOutput { get; internal set; }

            public string FileNameOutputText { get; internal set; } =
                "日本語化された言語ファイルのパスを指定する。";

            public NSheetType SheetType { get; set; } = NSheetType.Unknown;

            public string FileNameSheet { get; set; }

            public string FileNameSheetText { get; set; } =
                $"CSV形式の翻訳シートのパス、{Environment.NewLine}" +
                $"または翻訳シートが格納されているフォルダーのパスを指定する。{Environment.NewLine}";

            public bool UseMachineTrans { get; internal set; }

            public string FileNameGlossary { get; internal set; }

            public string FileNameGlossaryText { get; internal set; } =
                $"CSV形式の用語集のパスを指定する。{Environment.NewLine}" +
                $"省略時は用語集を使用しない。";

            public string UseMachineTransText { get; internal set; } =
                $"有志翻訳がない場合は機械翻訳を使用する。";

            public bool UseReferenceId { get; internal set; }

            public string UseReferenceIdText { get; internal set; } =
                $"ReferenceIDを付加する。{Environment.NewLine}" +
                $"省略時はReferenceIDを付加しない。";

            public string MtransMark { get; internal set; }

            public string MtransMarkText { get; internal set; } =
                $"機械翻訳のテキストの先頭に付ける記号{Environment.NewLine}" +
                $"省略時は記号を付けない。";

            public bool UseUnityModMan { get; internal set; }

            public string UseUnityModManText { get; internal set; } =
                $"Unity mod manager版CSVファイルを出力する。";

            public bool UseForceMt { get; internal set; }

            public string UseForceMtText { get; internal set; } =
                $"機械翻訳使用時、置換文字などの特殊文字列を含む場合でも機械翻訳を使用する。";

            public bool UseReplace { get; internal set; }

            public string UseReplaceText { get; internal set; } =
                $"日本語化された言語ファイルが既に存在する場合はを上書きする。";

            public bool Help { get; set; }
        }
    }
}
