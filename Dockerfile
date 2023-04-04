FROM mcr.microsoft.com/dotnet/runtime:7.0

WORKDIR /app 

COPY /bin/Debug . 

ENTRYPOINT ["AvitoMusicInstrumentsBot.exe"] 