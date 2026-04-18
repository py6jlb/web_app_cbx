#!/bin/bash

sudo chown -R $USER:$USER /workspace/persistence
sudo chown -R $USER:$USER /workspace/src/cookbook/db
sudo chown -R $USER:$USER /workspace/src/cookbook/filestorage

dotnet tool install dotnet-ef
dotnet tool install csharpier
dotnet dev-certs https
dotnet restore ./backend

