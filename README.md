# Stock Check Sheet

Stock Check Sheet is a system to update product entries and outputs in its simplest way to be represented in an application. This program focuses more on the correct way to use xxx along with ASP.NET Core Identity, both combined can create applications totally out of this world.

## Sample Nº 1:
![image](https://github.com/CrisOporta/StockCheckSheet_MVC/assets/47622028/30f9b4d3-cff8-4782-8498-de96a4184139)

## ConnectionDafault:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server={yourServer};Database=StockCheckSheet;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

## Commands:

For package manager console.

```sh
Add-Migration InitialMigration
Update-Database
```

If you prefer dotnet commands.

```sh
dotnet ef migrations add InitialMigration
dotnet ef database update
```
