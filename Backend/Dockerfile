FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Proyekt fayllarını kopyalayırıq və restore edirik
COPY src/EHisse.Domain/EHisse.Domain.csproj src/EHisse.Domain/
COPY src/EHisse.Application/EHisse.Application.csproj src/EHisse.Application/
COPY src/EHisse.Persistence/EHisse.Persistence.csproj src/EHisse.Persistence/
COPY src/EHisse.Infrastructure/EHisse.Infrastructure.csproj src/EHisse.Infrastructure/
COPY src/EHisse.WebApi/EHisse.WebApi.csproj src/EHisse.WebApi/

RUN dotnet restore src/EHisse.WebApi/EHisse.WebApi.csproj

# Bütün mənbə kodunu kopyalayırıq və build edirik
COPY . .
WORKDIR /src/src/EHisse.WebApi
RUN dotnet publish -c Release -o /app/publish

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
ENV PREPROD_MODE=true
EXPOSE 10000

ENTRYPOINT ["dotnet", "EHisse.WebApi.dll"]
