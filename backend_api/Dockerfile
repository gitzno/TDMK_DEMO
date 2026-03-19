# Giai đoạn 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Chỉ cần trỏ trực tiếp vào folder TDMK_API_DEMO
COPY ["TDMK_API_DEMO/TDMK_API_DEMO.csproj", "TDMK_API_DEMO/"]
RUN dotnet restore "TDMK_API_DEMO/TDMK_API_DEMO.csproj"

COPY . .
WORKDIR "/src/TDMK_API_DEMO"
RUN dotnet publish "TDMK_API_DEMO.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai đoạn 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

USER root
RUN mkdir -p /app/wwwroot/uploads && chmod 777 /app/wwwroot/uploads

ENV ASPNETCORE_HTTP_PORTS=8081
EXPOSE 8081

ENTRYPOINT ["dotnet", "TDMK_API_DEMO.dll"]