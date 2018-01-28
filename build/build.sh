#!/bin/sh

#
# Author: Filipe GOMES PEIXOTO <gomespeixoto.filipe@gmail.com>
# Title: Ether.Network build script
# Description :
# This script builds the Ether.Network solution.
#

dotnet restore
dotnet build ./src/Ether.Network/Ether.Network.csproj --framework $BUILD_FRAMEWORK