FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Gateway/FlowBoard.Gateway/FlowBoard.Gateway.csproj", "src/Gateway/FlowBoard.Gateway/"]
RUN dotnet restore "src/Gateway/FlowBoard.Gateway/FlowBoard.Gateway.csproj"
COPY . .
RUN dotnet publish "src/Gateway/FlowBoard.Gateway/FlowBoard.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlowBoard.Gateway.dll"] 