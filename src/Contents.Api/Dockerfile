#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Contents.Api/Contents.Api.csproj", "Contents.Api/"]
COPY ["Contents.Business/Contents.Business.csproj", "Contents.Business/"]
COPY ["Contents.Data/Contents.Data.csproj", "Contents.Data/"]
COPY ["Contents.Common/Contents.Common.csproj", "Contents.Common/"]
RUN dotnet restore "Contents.Api/Contents.Api.csproj"
COPY . .
WORKDIR "/src/Contents.Api"
RUN dotnet build "Contents.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Contents.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Contents.Api.dll"]
