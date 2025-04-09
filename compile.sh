#!/bin/bash

version=$(cat version.txt)

cd IpAddr

# Win:
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishTrimmed=true /p:TrimMode=link /p:PublishSingleFile=true /p:InvariantGlobalization=true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName="IpAddr_win-x64-$version"

# Lin:
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishTrimmed=true /p:TrimMode=link /p:PublishSingleFile=true /p:InvariantGlobalization=true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName="IpAddr_linux-x64-$version"

# masOS:
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishTrimmed=true /p:TrimMode=link /p:PublishSingleFile=true /p:InvariantGlobalization=true /p:DebugType=None /p:DebugSymbols=false /p:AssemblyName="IpAddr_osx-x64-$version"

# Returning to the main directory
cd ..
cd ..

# Copy executables to the main directory
cp IpAddr/bin/Release/net8.0/win-x64/publish/IpAddr_win-x64-$version.exe ./
cp IpAddr/bin/Release/net8.0/linux-x64/publish/IpAddr_linux-x64-$version ./
cp IpAddr/bin/Release/net8.0/osx-x64/publish/IpAddr_osx-x64-$version ./

# Cleaning up the install
rm -rf IpAddr/bin/*
rm -rf IpAddr/obj/*