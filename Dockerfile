# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["AIProject.csproj", "."]
RUN dotnet restore "AIProject.csproj"

COPY . .

RUN dotnet publish "AIProject.csproj" -c Release -o /app/publish /p:UseAppHost=false


# Run stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "AIProject.dll"]