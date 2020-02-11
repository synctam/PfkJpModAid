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

        /// <summary>
        /// 翻訳シートの出力形式
        /// </summary>
        public enum NSheetType
        {
            /// <summary>
            /// CSV形式
            /// </summary>
            Csv,

            /// <summary>
            /// Excel形式
            /// </summary>
            Excel,

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
            StringWriter msg = new StringWriter();

            string exeName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            msg.WriteLine(string.Empty);
            msg.WriteLine($@"使い方：");
            msg.WriteLine($@"JSON形式の言語ファイルから翻訳シートを作成する。");
            msg.WriteLine($@"  usage: {exeName} -i <lang file path> [-f <fun lang file path>] -s <trans sheet path> [-n <number>] [-r]");
            msg.WriteLine($@"OPTIONS:");
            this.optionSet.WriteOptionDescriptions(msg);
            msg.WriteLine($@"Example:");
            msg.WriteLine($@"  言語ファイル(-i)から翻訳シート(-s)を作成する。");
            msg.WriteLine($@"    {exeName} -i enGB.json -s pfkTransSheet.csv");
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
                { "i|in="     , this.args.FileNameLangInputText   ,      v  => this.args.FileNameLangInput = v},
                { "f|fun="    , this.args.FileNameFunInputText    ,      v  => this.args.FileNameFunInput  = v},
                { "s|sheet="  , this.args.FileNameSheetText       ,      v  => this.args.FileNameSheet     = v},
                { "n|number=" , this.args.RowsPerSheetText        , (int v) => this.args.RowsPerSheet      = v},
                { "t|tag"     , this.args.UseTagText              ,      v  => this.args.UseTag            = v != null},
                { "r"         , this.args.UseReplaceText          ,      v  => this.args.UseReplace        = v != null},
                { "h|help"    , "ヘルプ"                          ,      v  => this.args.Help              = v != null},
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

            if (this.IsErrorLangInputFile())
            {
                return;
            }

            if (this.IsErrorFunInputFile())
            {
                return;
            }

            if (this.IsErrorTransSheetFile())
            {
                return;
            }

            if (this.IsErrorRowsPerSheet())
            {
                return;
            }

            this.isError = false;
            return;
        }

        private bool IsErrorLangInputFile()
        {
            if (string.IsNullOrWhiteSpace(this.Arges.FileNameLangInput))
            {
                this.SetErrorMessage($@"{Environment.NewLine}エラー：(-i)オリジナル版の言語ファイルのパスを指定してください。");
                this.isError = true;

                return true;
            }
            else
            {
                if (!File.Exists(this.Arges.FileNameLangInput))
                {
                    this.SetErrorMessage($@"{Environment.NewLine}エラー：(-i)オリジナル版の言語ファイルがみつかりません。{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameLangInput)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        private bool IsErrorFunInputFile()
        {
            if (!string.IsNullOrWhiteSpace(this.Arges.FileNameLangInput))
            {
                //// 有志翻訳の言語ファイルが指定された場合。
                if (!File.Exists(this.Arges.FileNameLangInput))
                {
                    this.SetErrorMessage($@"{Environment.NewLine}エラー：(-f)有志翻訳版の言語ファイルがみつかりません。{Environment.NewLine}({Path.GetFullPath(this.Arges.FileNameFunInput)})");
                    this.isError = true;

                    return true;
                }
            }

            return false;
        }

        private bool IsErrorTransSheetFile()
        {
            if (string.IsNullOrWhiteSpace(this.args.FileNameSheet))
            {
                this.SetErrorMessage($@"{Environment.NewLine}エラー：(-s)翻訳シートファイルのパスを指定してください。");
                this.isError = true;

                return true;
            }

            if (File.Exists(this.args.FileNameSheet) && !this.args.UseReplace)
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}" +
                    $@"エラー：(-s)翻訳シートファイルが既に存在します。{Environment.NewLine}" +
                    $@"({Path.GetFullPath(this.args.FileNameSheet)}){Environment.NewLine}" +
                    $@"上書きする場合は '-r' オプションを指定してください。");
                this.isError = true;

                return true;
            }

            if (Path.GetExtension(this.args.FileNameSheet).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                this.args.SheetType = NSheetType.Csv;
            }
            else if (Path.GetExtension(this.args.FileNameSheet).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                this.args.SheetType = NSheetType.Excel;
            }
            else
            {
                this.SetErrorMessage(
                    $"{Environment.NewLine}" +
                    $@"エラー：(-s)指定された翻訳シートの拡張子はサポート対象外です。{Environment.NewLine}" +
                    $@"({Path.GetFullPath(this.args.FileNameSheet)}){Environment.NewLine}" +
                    $@"'.csv' または '.xlsx' を指定してください。");
                this.args.SheetType = NSheetType.Unknown;
                this.isError = true;

                return true;
            }

            return false;
        }

        private bool IsErrorRowsPerSheet()
        {
            if (this.Arges.RowsPerSheet > 0 && this.Arges.RowsPerSheet < 5000)
            {
                this.SetErrorMessage($@"{Environment.NewLine}エラー：(-n)指定可能なシートあたりの行数は5,000以上です。");
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
            public string FileNameLangInput { get; internal set; }

            public string FileNameLangInputText { get; internal set; } =
                "オリジナル版の言語ファイルのパスを指定する。";

            public string FileNameFunInput { get; internal set; }

            public string FileNameFunInputText { get; internal set; } =
                $"有志翻訳版の言語ファイルのパス名を指定する。{Environment.NewLine}有志翻訳版がある場合のみ指定する。";

            public string FolderNameLangInput { get; internal set; }

            public string FolderNameLangInputText { get; internal set; } =
                "オリジナル版の言語フォルダーのパスを指定する。";

            public string FileNameSheet { get; set; }

            public string FileNameSheetText { get; set; } =
                "CSV形式またはExcel形式(.xlsx)の翻訳シートのパス名。";

            public int RowsPerSheet { get; internal set; }

            public string RowsPerSheetText { get; internal set; } =
                $"シートを分割する場合はシートあたりの行数を指定する。{Environment.NewLine}指定可能な行数は5,000以上。{Environment.NewLine}省略時はシート分割は行わない。";

            public bool UseTag { get; internal set; }

            public string UseTagText { get; internal set; } =
                $"制御文字をタグに置換する。{Environment.NewLine}省略時はタグへの置換は行わない。{Environment.NewLine}なお、TABについては無条件にタグに置換する。";

            public bool UseReplace { get; internal set; }

            public string UseReplaceText { get; internal set; } = $"翻訳シートが既に存在する場合はを上書きする。";

            public bool Help { get; set; }

            public NSheetType SheetType { get; set; } = NSheetType.Unknown;
        }
    }
}
