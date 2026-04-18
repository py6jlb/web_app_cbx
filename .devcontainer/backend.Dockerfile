FROM mcr.microsoft.com/devcontainers/dotnet:dev-9.0
RUN su vscode -c "source /usr/local/share/nvm/nvm.sh && nvm install 22 && nvm use 22 && npm install -g typescript" 2>&1

WORKDIR /workspace

