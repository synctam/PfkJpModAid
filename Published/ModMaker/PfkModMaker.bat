@rem
@rem Pathfinder: Kingmaker ���{�ꉻMOD�쐬
@rem

@SET PATH="..\tools";%PATH%

@rem �ʏ�ŁA�@�B�|�󂠂�A�@�B�|��Ƀ}�[�N��t����
PfkModMaker.exe -i ..\original\enGB.json -s sheet -g "glossary\PathfinderKingmaker - �p��W.csv" -o jpMod\normal\enGB.json     -k # -m -f -r

@rem ReferenceID�t���A�@�B�|�󂠂�A�@�B�|��Ƀ}�[�N��t����
PfkModMaker.exe -i ..\original\enGB.json -s sheet -g "glossary\PathfinderKingmaker - �p��W.csv" -o jpMod\refid\enGB.json   -e -k # -m -f -r

@rem Unity mod manager�pCSV�t�@�C���쐬
PfkModMaker.exe -i ..\original\enGB.json -s sheet -g "glossary\PathfinderKingmaker - �p��W.csv" -o jpMod\umm\jaJP.csv      -u -k # -m -f -r

@pause
@exit /b
