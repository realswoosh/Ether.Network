#!/usr/bin/env bash

cd ../src/Ether.Network/
dotnet restore
dotnet build --configuration Release
dotnet pack --configuration Release