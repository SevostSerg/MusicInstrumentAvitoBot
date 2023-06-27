#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Sevost.AvitoMusicInstrumentsBot/Sevost.AvitoMusicInstrumentsBot.csproj", "Sevost.AvitoMusicInstrumentsBot/"]
RUN dotnet restore "Sevost.AvitoMusicInstrumentsBot/Sevost.AvitoMusicInstrumentsBot.csproj"
COPY . .
WORKDIR "/src/Sevost.AvitoMusicInstrumentsBot"
RUN dotnet build "Sevost.AvitoMusicInstrumentsBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sevost.AvitoMusicInstrumentsBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sevost.AvitoMusicInstrumentsBot.dll"]