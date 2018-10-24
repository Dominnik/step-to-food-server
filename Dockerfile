FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY StepToFoodServer/*.csproj ./StepToFoodServer/
RUN dotnet restore

# copy everything else and build app
COPY . .
WORKDIR /app/StepToFoodServer
RUN dotnet build
 
FROM build AS publish
WORKDIR /app/StepToFoodServer
RUN dotnet publish -c Release -o out  

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS runtime
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
WORKDIR /app
COPY --from=publish /app/StepToFoodServer/out ./
ENTRYPOINT ["dotnet", "StepToFoodServer.dll"]