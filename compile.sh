#!/bin/bash

version=$(cat version.txt)

cd src/IpAddr

# Win:
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:AssemblyName="IpAddr_win-x64-$version"

# Lin:
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=true /p:AssemblyName="IpAddr_linux-x64-$version"

# masOS:
dotnet publish -c Release -r osx-x64 --self-contained true /p:PublishSingleFile=true /p:AssemblyName="IpAddr_osx-x64-$version"

# Returning to the main directory
cd ..
cd ..

# Copy executables to the main directory
cp src/IpAddr/bin/Release/net8.0/win-x64/publish/IpAddr_win-x64-$version.exe ./
cp src/IpAddr/bin/Release/net8.0/linux-x64/publish/IpAddr_linux-x64-$version ./
cp src/IpAddr/bin/Release/net8.0/osx-x64/publish/IpAddr_osx-x64-$version ./

# Cleaning up the install
rm -rf src/IpAddr/bin/*
rm -rf src/IpAddr/obj/*