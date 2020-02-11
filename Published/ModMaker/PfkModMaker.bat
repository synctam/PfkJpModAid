@rem
@rem Pathfinder: Kingmaker 日本語化MOD作成
@rem

@SET PATH="..\tools";%PATH%

@rem 通常版、機械翻訳あり、機械翻訳にマークを付ける
PfkModMaker.exe -i ..\original\enGB.json -s sheet -o jpMod\normal\enGB.json   -k # -m -r

@rem ReferenceID付き、機械翻訳あり、機械翻訳にマークを付ける
PfkModMaker.exe -i ..\original\enGB.json -s sheet -o jpMod\refid\enGB.json -e -k # -m -r

@rem Unity mod manager用CSVファイル作成
PfkModMaker.exe -i ..\original\enGB.json -s sheet -o jpMod\umm\jaJP.csv    -u -k # -m -r

@pause
@exit /b
