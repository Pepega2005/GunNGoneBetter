FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /app
COPY ./GunNGoneBetter/ ./GunNGoneBetter/
COPY ./GunNGoneBetter_DataMigrations/ ./GunNGoneBetter_DataMigrations/
COPY ./GunNGoneBetter_Utility/ ./GunNGoneBetter_Utility/
COPY ./GunNGoneBetter_Models/ ./GunNGoneBetter_Models/
COPY ./GunNGoneBetter.sln .
RUN dotnet publish -c Release -o release

WORKDIR /app/release

ENV Urls http://0.0.0.0:5000

CMD ["./GunNGoneBetter"]