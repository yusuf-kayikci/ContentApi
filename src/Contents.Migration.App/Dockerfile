#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Contents.Migration.App/Contents.Migration.App.csproj", "Contents.Migration.App/"]
COPY ["Contents.Migration/Contents.Migration.csproj", "Contents.Migration/"]
COPY ["Contents.Common/Contents.Common.csproj", "Contents.Common/"]
COPY ["Contents.Data/Contents.Data.csproj", "Contents.Data/"]
RUN dotnet restore "Contents.Migration.App/Contents.Migration.App.csproj"
COPY . .
WORKDIR "/src/Contents.Migration.App"
RUN dotnet build "Contents.Migration.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contents.Migration.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contents.Migration.App.dll"]
