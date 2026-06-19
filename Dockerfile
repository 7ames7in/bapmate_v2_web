# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "BapMate.WebApi/BapMate.WebApi.csproj"
RUN dotnet publish "BapMate.WebApi/BapMate.WebApi.csproj" -c Release -o /app

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
# Render가 PORT 환경변수를 줍니다. 반드시 그 포트로 리슨
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
# (선택) 한국시간 등 필요시 추가 ENV
ENTRYPOINT ["dotnet", "BapMate.WebApi.dll"]