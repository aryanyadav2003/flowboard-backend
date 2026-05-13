FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5006

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/FlowBoard.CommentService/FlowBoard.CommentService.csproj", "src/Services/FlowBoard.CommentService/"]
COPY ["src/Shared/FlowBoard.Shared/FlowBoard.Shared.csproj", "src/Shared/FlowBoard.Shared/"]
RUN dotnet restore "src/Services/FlowBoard.CommentService/FlowBoard.CommentService.csproj"
COPY . .
RUN dotnet publish "src/Services/FlowBoard.CommentService/FlowBoard.CommentService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlowBoard.CommentService.dll"]