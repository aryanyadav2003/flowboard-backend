FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/FlowBoard.BoardService/FlowBoard.BoardService.csproj", "src/Services/FlowBoard.BoardService/"]
COPY ["src/Shared/FlowBoard.Shared/FlowBoard.Shared.csproj", "src/Shared/FlowBoard.Shared/"]
RUN dotnet restore "src/Services/FlowBoard.BoardService/FlowBoard.BoardService.csproj"
COPY . .
RUN dotnet publish "src/Services/FlowBoard.BoardService/FlowBoard.BoardService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlowBoard.BoardService.dll"]