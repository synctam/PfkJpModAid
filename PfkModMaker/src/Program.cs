namespace PfkModMaker
{
    using System;
    using LibPfk.Glossary;
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

            MakeMod(opt.Arges);

            TDebugUtils.Pause();
            return 0;
        }

        private static void MakeMod(TOptions.TArgs opt)
        {
            //// 用語集の読み込み
            PfkGlossaryInfo glossaryInfo = null;
            if (string.IsNullOrWhiteSpace(opt.FileNameGlossary))
            {
                glossaryInfo = new PfkGlossaryInfo();
            }
            else
            {
                glossaryInfo = PfkGlossaryDao.LoadFromCsv(opt.FileNameGlossary);
            }

            //// 翻訳シートを読み込む
            var sheetInfo = new PfkTransSheetInfo();
            var fileID = "enGB";
            switch (opt.SheetType)
            {
                case TOptions.NSheetType.Unknown:
                    break;
                case TOptions.NSheetType.SingleFile:
                    //// タグの制御文字への変換は副作用がないため、必ず実行する。
                    PfkTransSheetDao.LoadFromFile(sheetInfo, opt.FileNameSheet, fileID, true);
                    break;
                case TOptions.NSheetType.MultiFile:
                    PfkTransSheetDao.LoadFromFolder(sheetInfo, opt.FileNameSheet, fileID, true);
                    break;
                default:
                    throw new Exception($"Unknown error. Sheet type({opt.SheetType})");
            }

            if (opt.UseUnityModMan)
            {
                //// UMM形式のCSVファイルを出力する（タグは制御文字に変換する）。
                PfkUmmDataDao.SaveToUmmFile(
                    opt.FileNameInput,
                    sheetInfo,
                    glossaryInfo,
                    opt.FileNameOutput,
                    opt.UseReferenceId,
                    opt.UseMachineTrans,
                    opt.MtransMark,
                    opt.UseForceMt);
            }
            else
            {
                //// 翻訳済みJSONファイルを出力する（タグは制御文字に変換する）。
                PfkLanguageDao.SaveToFile(
                    opt.FileNameInput,
                    sheetInfo,
                    glossaryInfo,
                    opt.FileNameOutput,
                    opt.UseReferenceId,
                    opt.UseMachineTrans,
                    opt.MtransMark,
                    opt.UseForceMt);
            }
        }
    }
}
