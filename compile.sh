#!/bin/bash

proj_name="IpAddr"
version=$(cat version.txt)

cd $proj_name

# Win:
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishTrimmed=true /p:TrimMode=link /p:PublishSingleFile=true /p:InvariantGlobalization=true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName="$proj_name-win-x64-$version"

# Lin:
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishTrimmed=true /p:TrimMode=link /p:PublishSingleFile=true /p:InvariantGlobalization=true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName="$proj_name-linux-x64-$version"

# masOS:
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishTrimmed=true /p:TrimMode=link /p:PublishSingleFile=true /p:InvariantGlobalization=true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName="$proj_name-osx-x64-$version"

# Returning to the main directory
cd ..

# Copy executables to the main directory
cp $proj_name/bin/Release/net8.0/win-x64/publish/$proj_name-win-x64-$version.exe ./
cp $proj_name/bin/Release/net8.0/linux-x64/publish/$proj_name-linux-x64-$version ./
cp $proj_name/bin/Release/net8.0/osx-x64/publish/$proj_name-osx-x64-$version ./

# Cleaning up the install
rm -rf $proj_name/bin/*
rm -rf $proj_name/obj/*