#!/bin/sh

#
# Author: Filipe GOMES PEIXOTO <gomespeixoto.filipe@gmail.com>
# Title: Ether.Network test script
# Description :
# This script tests the Ether.Network solution.
#

dotnet restore
dotnet test ./test/Ether.Network.Tests/Ether.Network.Tests.csproj --framework $TEST_FRAMEWORK