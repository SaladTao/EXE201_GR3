# Giai đoạn build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy đúng tên file .csproj
COPY exe201.csproj ./
RUN dotnet restore

# Copy toàn bộ mã nguồn và build
COPY . ./
RUN dotnet publish -c Release -o out

# Giai đoạn runtime (chạy)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

# Chạy đúng file .dll (cùng tên với csproj)
ENTRYPOINT ["dotnet", "exe201.dll"]
