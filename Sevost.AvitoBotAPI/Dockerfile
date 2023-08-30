#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Sevost.AvitoBotAPI/Sevost.AvitoBotAPI.csproj", "Sevost.AvitoBotAPI/"]
RUN dotnet restore "Sevost.AvitoBotAPI/Sevost.AvitoBotAPI.csproj"
COPY . .
WORKDIR "/src/Sevost.AvitoBotAPI"
RUN dotnet build "Sevost.AvitoBotAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sevost.AvitoBotAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sevost.AvitoBotAPI.dll"]