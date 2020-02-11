namespace LibPfk.Glossary
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CsvHelper;
    using CsvHelper.Configuration;

    public class PfkGlossaryDao
    {
        public static PfkGlossaryInfo LoadFromCsv(string path, Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.UTF8;
            }

            var fileID = Path.GetFileNameWithoutExtension(path);
            using (var reader = new StreamReader(path, enc))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    //// 区切り文字
                    csv.Configuration.Delimiter = ",";
                    //// ヘッダーの有無
                    csv.Configuration.HasHeaderRecord = true;
                    //// CSVファイルに合ったマッピングルールを登録
                    csv.Configuration.RegisterClassMap<CsvMapper>();
                    //// データを読み出し
                    var records = csv.GetRecords<FlashCardSheet>();

                    var errorCount = 0;
                    PfkGlossaryInfo sheet = new PfkGlossaryInfo();
                    foreach (var record in records)
                    {
                        PfkGlossaryEntry.NConversionType conversionType = PfkGlossaryEntry.NConversionType.None;
                        switch (record.ConversionType)
                        {
                            case "":
                                conversionType = PfkGlossaryEntry.NConversionType.None;
                                break;
                            case "#":
                                conversionType = PfkGlossaryEntry.NConversionType.AlwaysTranslate;
                                break;
                            case "$":
                                conversionType = PfkGlossaryEntry.NConversionType.NounTranslate;
                                break;
                            default:
                                throw new Exception($"Unknown ConversionType({record.ConversionType}).");
                        }

                        PfkGlossaryEntry entry = new PfkGlossaryEntry(conversionType, record.Prefix, record.OriginalText, record.TranslatedText);
                        if (!sheet.AddEntry(entry))
                        {
                            errorCount++;
                        }
                    }

                    if (errorCount > 0)
                    {
                        var msg = $"Duplicated key. errors({errorCount})";
                        Console.WriteLine("*****************************");
                        Console.WriteLine(msg);
                        Console.WriteLine("*****************************");
                        throw new Exception(msg);
                    }

                    return sheet;
                }
            }
        }

        public class FlashCardSheet
        {
            public string ConversionType { get; set; } = string.Empty;

            public string Prefix { get; set; } = string.Empty;

            public string OriginalText { get; set; } = string.Empty;

            public string TranslatedText { get; set; } = string.Empty;
        }

        public class CsvMapper : ClassMap<FlashCardSheet>
        {
            public CsvMapper()
            {
                //// 出力時の列の順番は指定した順となる。
                this.Map(x => x.ConversionType).Name("[[変換区分]]");
                this.Map(x => x.Prefix).Name("[[接頭辞]]");
                this.Map(x => x.OriginalText).Name("[[英語]]");
                this.Map(x => x.TranslatedText).Name("[[日本語]]");
            }
        }
    }
}
