@rem
@rem Pathfinder: Kingmaker ���{�ꉻMOD�쐬
@rem

@SET PATH="..\tools";%PATH%

@rem �ʏ�ŁA�@�B�|�󂠂�A�@�B�|��Ƀ}�[�N��t����
PfkModMaker.exe -i ..\original\enGB.json -s sheet -o jpMod\normal\enGB.json   -k # -m -r

@rem ReferenceID�t���A�@�B�|�󂠂�A�@�B�|��Ƀ}�[�N��t����
PfkModMaker.exe -i ..\original\enGB.json -s sheet -o jpMod\refid\enGB.json -e -k # -m -r

@rem Unity mod manager�pCSV�t�@�C���쐬
PfkModMaker.exe -i ..\original\enGB.json -s sheet -o jpMod\umm\jaJP.csv    -u -k # -m -r

@pause
@exit /b
