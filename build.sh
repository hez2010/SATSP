#!/usr/bin/env bash

if [ "$(uname)" == "Darwin" ]; then
dotnet publish /p:PublishTrimmed=true /p:TrimMode=Link /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -c Release -r osx-x64
cp -f ./bin/Release/net5.0/osx-x64/publish/SATSP ./SATSP
chmod +x ./SATSP
elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then   
dotnet publish /p:PublishTrimmed=true /p:TrimMode=Link /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true -c Release -r linux-x64
cp -f ./bin/Release/net5.0/linux-x64/publish/SATSP ./SATSP
chmod +x ./SATSP
fi
