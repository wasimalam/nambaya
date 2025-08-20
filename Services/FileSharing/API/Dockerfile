#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY . .
RUN dotnet restore "FileSharing/Wrapper/IDgard/FileSharing.Wrapper.IDgard.csproj"
RUN dotnet restore "FileSharing/API/FileSharing.API.csproj"
WORKDIR "/src/FileSharing/Wrapper/IDgard/"
RUN dotnet build "FileSharing.Wrapper.IDgard.csproj" -c Release -o /app/build
WORKDIR "/src/FileSharing/Wrapper/S3/"
RUN dotnet build "FileSharing.Wrapper.S3.csproj" -c Release -o /app/build
WORKDIR "/src/FileSharing/API"
RUN dotnet build "FileSharing.API.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/FileSharing/Wrapper/IDgard/"
RUN dotnet publish "FileSharing.Wrapper.IDgard.csproj" -c Release -o /app/publish
WORKDIR "/src/FileSharing/Wrapper/S3/"
RUN dotnet publish "FileSharing.Wrapper.S3.csproj" -c Release -o /app/publish
WORKDIR "/src/FileSharing/API"
RUN dotnet publish "FileSharing.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileSharing.API.dll"]