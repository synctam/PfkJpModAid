# PfkJpModAid
Pathfinder: Kingmaker 日本語化支援ツール


使い方については、こちらのブログ記事を御覧ください。  
[「synctam: Pathfinder: Kingmaker 日本語化支援ツールの使い方」](https://synctam.blogspot.com/2020/02/pathfinder-kingmaker.html)


## PfkSheetMaker ##

```
JSON形式の言語ファイルから翻訳シートを作成する。
  usage: PfkSheetMaker.exe -i <lang file path> [-f <fun lang file path>] -s <trans sheet path> [-u <umm csv path>] [-n <number>] [-t] [-r] [-h]
OPTIONS:
  -i, --in=VALUE             オリジナル版の言語ファイルのパスを指定する。
  -f, --fun=VALUE            有志翻訳版の言語ファイルのパス名を指定する。
                               有志翻訳版がある場合のみ指定する。
  -s, --sheet=VALUE          CSV形式またはExcel形式(.xlsx)の翻訳シートのパス名。
  -u, --umm=VALUE            UMM対応版CSVファイルのパスを指定する。
                               UMM対応版CSVファイルの訳を機械翻訳として取り込む場合に指定する。
  -n, --number=VALUE         シートを分割する場合はシートあたりの行数を指定する。
                               指定可能な行数は5,000以上。
                               省略時はシート分割は行わない。
  -t, --tag                  制御文字をタグに置換する。
                               省略時はタグへの置換は行わない。
                               なお、TABについては無条件にタグに置換する。
  -r                         翻訳シートが既に存在する場合はを上書きする。
  -h, --help                 ヘルプ
Example:
  言語ファイル(-i)から翻訳シート(-s)を作成する。
    PfkSheetMaker.exe -i enGB.json -s pfkTransSheet.csv
終了コード:
 0  正常終了
 1  異常終了
```


## PfkModMaker ##

```
日本語化MODを作成する。
  usage: PfkModMaker.exe -i <original lang file path> -o <japanized lang folder path> -s <Trans Sheet or folder path> [-g <glossary path>] [-k <value>] [-e] [-u] [-m] [-f] [-r] [-h]
OPTIONS:
  -i, --in=VALUE             オリジナル版の言語ファイルのパスを指定する。
  -o, --out=VALUE            日本語化された言語ファイルのパスを指定する。
  -s, --sheet=VALUE          CSV形式の翻訳シートのパス、
                               または翻訳シートが格納されているフォルダーのパスを指定する。
  -g, --glossary=VALUE       CSV形式の用語集のパスを指定する。
                               省略時は用語集を使用しない。
  -k, --mark=VALUE           機械翻訳のテキストの先頭に付ける記号
                               省略時は記号を付けない。
  -e, --refid                ReferenceIDを付加する。
                               省略時はReferenceIDを付加しない。
  -u, --umm                  Unity mod manager版CSVファイルを出力する。
  -m                         有志翻訳がない場合は機械翻訳を使用する。
  -f, --forcemt              機械翻訳使用時、置換文字などの特殊文字列を含む場合でも機械翻訳を使用する。
  -r                         日本語化された言語ファイルが既に存在する場合はを上書きする。
  -h, --help                 ヘルプ
Example:
  翻訳シート(-s)とオリジナルの言語ファイル(-i)を元に日本語化MOD(-o)を作成する。
    PfkModMaker.exe -i EN\enGB.json -o JP\enGB.json -s PfkTransSheet.csv
終了コード:
 0  正常終了
 1  異常終了
```
