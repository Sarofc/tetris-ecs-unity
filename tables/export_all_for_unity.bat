@echo off

set TABLE_EXCEL=.\excel\
set TABLE_DATA=.\data\config\
set TABLE_CS=.\data\table_cs\

set UNITY_PROJECT=..\
set UNITY_PROJECT_DATA=%unity_project%\Assets\StreamingAssets\Generate\Config\
set UNITY_PROJECT_CS=%unity_project%\Assets\Scripts\Generate\DataTable\

".\bin\tabtool.exe" --out_client %TABLE_DATA% --out_cs %TABLE_CS% --in_excel %TABLE_EXCEL%

md %UNITY_PROJECT_DATA%
md %UNITY_PROJECT_CS%

del %UNITY_PROJECT_DATA%*.txt >nul 2>nul
copy %TABLE_DATA%*.txt %UNITY_PROJECT_DATA%

del %UNITY_PROJECT_CS%*.cs >nul 2>nul
copy %TABLE_CS%*.cs %UNITY_PROJECT_CS%

pause