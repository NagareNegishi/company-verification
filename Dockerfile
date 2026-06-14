FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files first so restore is cached unless dependencies change
COPY CompanyVerification.Api/CompanyVerification.Api.csproj CompanyVerification.Api/
COPY CompanyVerification.Core/CompanyVerification.Core.csproj CompanyVerification.Core/
RUN dotnet restore CompanyVerification.Api/CompanyVerification.Api.csproj

COPY CompanyVerification.Api/ CompanyVerification.Api/
COPY CompanyVerification.Core/ CompanyVerification.Core/
RUN dotnet publish CompanyVerification.Api/CompanyVerification.Api.csproj \
    -c Release -o /app/publish --no-restore

# Runtime image only
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
# Render routes external traffic to port 10000
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "CompanyVerification.Api.dll"]
