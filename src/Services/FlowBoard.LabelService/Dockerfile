FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5007

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/FlowBoard.LabelService/FlowBoard.LabelService.csproj", "src/Services/FlowBoard.LabelService/"]
COPY ["src/Shared/FlowBoard.Shared/FlowBoard.Shared.csproj", "src/Shared/FlowBoard.Shared/"]
RUN dotnet restore "src/Services/FlowBoard.LabelService/FlowBoard.LabelService.csproj"
COPY . .
RUN dotnet publish "src/Services/FlowBoard.LabelService/FlowBoard.LabelService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlowBoard.LabelService.dll"]