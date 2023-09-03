# Qless-WebAPI

### Technologies and Tools Used:
- WebAPI
  - .NET 6
  - EFCore 6
  - Sql Server
- Tools
  - VS 2022
  - SSMS 18
  - Git

### Prerequisites
- .NET 6 Runtime: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
  - Download the version for your OS.
- Database : Sql Server: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
  - Download express for quick-setup local development / testing.

### Migrations
- Requires dotnet ef CLI Tools : https://docs.microsoft.com/en-us/ef/core/cli/dotnet
- Install using terminal (requires .NET runtime)
```
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```
- In the root folder (containing the .sln file), run the following in the terminal: 
```
dotnet ef database update -p './Qless.Repositories' -s './Qless.WebAPI'
```
### How to test
- In the root folder (containing the .sln file), run the following in the terminal: 
```
dotnet test 
```
### How to run
- In the root folder (containing the .sln file), run the following in the terminal:
- API:
```
dotnet run --project './Qless.WebAPI'
```
### View Swagger Doc
- While running, open a browser and navigate to https://localhost:6868/swagger


