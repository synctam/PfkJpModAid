@rem
@rem Pathfinder: Kingmaker �|��V�[�g�쐬
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

@rem CSV�`���̔�r�p�|��V�[�g���o�́B
PfkSheetMaker.exe -i "..\original\enGB.json"                               -s "backup\PathfinderKingmaker_Compare_%DATE_TIME%.csv" -r

@rem Excel�`���̖|��V�[�g���o�́B
PfkSheetMaker.exe -i "..\original\enGB.json" -f "fanTranslation\enGB.json" -s "sheet\PathfinderKingmaker_%DATE_TIME%.xlsx" -n 10000 -r

@pause
@exit /b
