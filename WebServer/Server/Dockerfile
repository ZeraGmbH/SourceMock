#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY BarcodeApi BarcodeApi
COPY BurdenApi BurdenApi
COPY DutApi DutApi
COPY ErrorCalculatorApi ErrorCalculatorApi 
COPY MeterTestSystemApi MeterTestSystemApi
COPY RefMeterApi RefMeterApi
COPY SerialPortProxy SerialPortProxy
COPY Shared Shared
COPY SourceApi SourceApi
COPY WatchDogApi WatchDogApi
COPY ZIFApi ZIFApi

COPY WebServer/Server/WebSamDeviceApis.csproj Server/WebSamDeviceApis/WebSamDeviceApis.csproj

RUN dotnet restore "Server/WebSamDeviceApis/WebSamDeviceApis.csproj"

WORKDIR "/src/Server/WebSamDeviceApis"
COPY WebServer/Server .
RUN dotnet build "WebSamDeviceApis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebSamDeviceApis.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebSamDeviceApis.dll"]