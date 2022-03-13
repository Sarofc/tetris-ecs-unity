@echo off

set TABLE_EXCEL=.\excel\
set TABLE_DATA=.\data\config\
set TABLE_CS=.\data\table_cs\

set UNITY_PROJECT=..\..\
@REM set UNITY_PROJECT_DATA=%unity_project%\Assets\StreamingAssets\Generate\Config\
set UNITY_PROJECT_CS=%unity_project%\Assets\Scripts\Generate\DataTable\

".\bin\GTable.exe" --out_client %TABLE_DATA% --out_cs %TABLE_CS% --in_excel %TABLE_EXCEL%

@REM md %UNITY_PROJECT_DATA%
md %UNITY_PROJECT_CS%

@REM 不用再拷贝到streammingasset目录
@REM del %UNITY_PROJECT_DATA%*.txt >nul 2>nul
@REM copy %TABLE_DATA%*.txt %UNITY_PROJECT_DATA%

del %UNITY_PROJECT_CS%*.cs >nul 2>nul
copy %TABLE_CS%*.cs %UNITY_PROJECT_CS%

pause