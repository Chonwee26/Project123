#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Project123/Project123.csproj", "Project123/"]
COPY ["Project123.Dto/Project123.Dto.csproj", "Project123.Dto/"]
COPY ["Project123Api/Project123Api.csproj", "Project123Api/"]
RUN dotnet restore "./Project123/Project123.csproj"
COPY . .
WORKDIR "/src/Project123"
RUN dotnet build "./Project123.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Project123.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Project123.dll"]