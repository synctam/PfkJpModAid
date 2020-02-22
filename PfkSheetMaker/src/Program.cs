namespace PfkSheetMaker
{
    using System;
    using System.IO;
    using LibPfkMod.Language;
    using LibPfkMod.TransSheet;
    using LibPfkMod.Umm;
    using MonoOptions;
    using S5mDebugTools;

    internal class Program
    {
        private static int Main(string[] args)
        {
            //// コマンドラインオプションの処理
            TOptions opt = new TOptions(args);
            if (opt.IsError)
            {
                TDebugUtils.Pause();
                return 1;
            }

            if (opt.Arges.Help)
            {
                opt.ShowUsage();

                TDebugUtils.Pause();
                return 1;
            }

            try
            {
                switch (opt.Arges.SheetType)
                {
                    case TOptions.NSheetType.Csv:
                        MakeCsv(opt.Arges);
                        break;
                    case TOptions.NSheetType.Excel:
                        MakeExcel(opt.Arges);
                        break;
                    case TOptions.NSheetType.Unknown:
                    default:
                        var msg = $"Unknown sheet type error. SheetType({opt.Arges.SheetType})";
                        throw new Exception(msg);
                }

                TDebugUtils.Pause();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TDebugUtils.Pause();
                return 1;
            }
        }

        private static void MakeExcel(TOptions.TArgs opt)
        {
            var langInfo = new PfkLanguageInfo();
            PfkLanguageDao.LoadFromFile(langInfo, opt.FileNameLangInput);

            var fanInfo = new PfkLanguageInfo();
            if (!string.IsNullOrWhiteSpace(opt.FileNameFanInput))
            {
                //// 有志翻訳版の言語情報を作成する。
                PfkLanguageDao.LoadFromFile(fanInfo, opt.FileNameFanInput);
            }

            //// UMM対応版データを読み込む。
            var ummDataInfo = new PfkUmmDataInfo();
            if (!string.IsNullOrEmpty(opt.FileNameUmm))
            {
                PfkUmmDataDao.LoadFromCsv(ummDataInfo, opt.FileNameUmm);
            }

            PfkTransSheetDao.SaveToExcel(
                langInfo, fanInfo, ummDataInfo, opt.FileNameSheet, opt.RowsPerSheet, opt.UseTag);
        }

        private static void MakeCsv(TOptions.TArgs opt)
        {
            var langInfo = new PfkLanguageInfo();
            //// 原文を読み込み言語情報を作成する。
            PfkLanguageDao.LoadFromFile(langInfo, opt.FileNameLangInput);

            //// 言語情報から翻訳シートを出力する。制御文字はタグ化する。
            if (opt.RowsPerSheet > 0)
            {
                //// シートを分割する。
                var dir = Path.GetDirectoryName(Path.GetFullPath(opt.FileNameSheet));
                var fileName = Path.GetFileNameWithoutExtension(opt.FileNameSheet);

                int total = langInfo.GetEntryCountWithoutEmpty();
                var count = total / opt.RowsPerSheet;
                for (var i = 0; i < count + 1; i++)
                {
                    var path = Path.Combine(dir, $"{fileName}_{i + 1}.csv");
                    var from = i * opt.RowsPerSheet;
                    var to = (i * opt.RowsPerSheet) + opt.RowsPerSheet - 1;
                    PfkTransSheetDao.SaveToCsv(
                        langInfo, path, from, to, opt.UseTag);
                }
            }
            else
            {
                //// シートを分割しない。
                PfkTransSheetDao.SaveToCsv(
                    langInfo, opt.FileNameSheet, 0, 9999999, opt.UseTag);
            }
        }
    }
}
