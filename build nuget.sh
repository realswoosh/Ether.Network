#!/usr/bin/env bash

cd src/Ether.Network/
dotnet restore
dotnet build
dotnet pack -c Release