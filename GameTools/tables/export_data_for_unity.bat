@echo off

echo export data only!

set TABLE_EXCEL=.\excel\
set TABLE_DATA=.\data\config\
set TABLE_CS=.\data\table_cs\

".\bin\GTable.exe" --out_client %TABLE_DATA% --out_cs %TABLE_CS% --in_excel %TABLE_EXCEL%

set UNITY_PROJECT=..\..\
@REM set UNITY_PROJECT_DATA=%unity_project%\Assets\StreamingAssets\Gen\Config\
@REM set UNITY_PROJECT_CS=%unity_project%\Assets\Scripts\Gen\DataTable\

@REM md %UNITY_PROJECT_DATA%
@REM md %UNITY_PROJECT_CS%

@REM 不用再拷贝到streammingasset目录
@REM del %UNITY_PROJECT_DATA%*.txt >nul 2>nul
@REM copy %TABLE_DATA%*.txt %UNITY_PROJECT_DATA%

@REM del %UNITY_PROJECT_CS%*.cs >nul 2>nul
@REM copy %TABLE_CS%*.cs %UNITY_PROJECT_CS%

pause