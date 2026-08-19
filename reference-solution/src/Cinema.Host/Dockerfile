FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Build.props Directory.Packages.props Cinema.slnx ./
COPY src/Cinema.Host/Cinema.Host.csproj src/Cinema.Host/
COPY src/Cinema.Api/Cinema.Api.csproj src/Cinema.Api/
COPY src/Cinema.Domain/Cinema.Domain.csproj src/Cinema.Domain/
COPY src/Cinema.Infrastructure/Cinema.Infrastructure.csproj src/Cinema.Infrastructure/
RUN dotnet restore src/Cinema.Host/Cinema.Host.csproj
COPY src ./src
RUN dotnet publish src/Cinema.Host/Cinema.Host.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "Cinema.Host.dll"]
