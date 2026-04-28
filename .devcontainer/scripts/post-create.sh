#!/bin/bash

sudo chown -R $USER:$USER /workspace

dotnet tool install dotnet-ef
dotnet tool install csharpier
dotnet dev-certs https
dotnet restore ./src/cookbook

