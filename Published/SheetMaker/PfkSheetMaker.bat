@rem
@rem Pathfinder: Kingmaker 翻訳シート作成
@rem

@SET PATH="..\tools";%PATH%

@set yyyy=%date:~0,4%
@set mm=%date:~5,2%
@set dd=%date:~8,2%
 
@set time2=%time: =0%
 
@set hh=%time2:~0,2%
@set mn=%time2:~3,2%
@set ss=%time2:~6,2%
 
@set DATE_TIME=%yyyy%.%mm%.%dd%_%hh%.%mn%.%ss%

@rem CSV形式の比較用翻訳シートを出力。
PfkSheetMaker.exe -i "..\original\enGB.json"                               -s "backup\PathfinderKingmaker_Compare_%DATE_TIME%.csv" -r

@rem Excel形式の翻訳シートを出力。
PfkSheetMaker.exe -i "..\original\enGB.json" -f "fanTranslation\enGB.json" -s "sheet\PathfinderKingmaker_%DATE_TIME%.xlsx" -n 10000 -r

@pause
@exit /b
