FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY ./API/API.csproj ./API/
RUN dotnet restore ./API/API.csproj

COPY ./API/. ./API
WORKDIR /app/API
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]