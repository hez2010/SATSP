@dotnet publish /p:PublishTrimmed=true /p:TrimMode=Link /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -c Release -r win-x64
@copy /y bin\Release\net5.0\win-x64\publish\SATSP.exe SATSP.exe
