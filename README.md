# Stock Check Sheet

Stock Check Sheet is a system to update product entries and outputs in its simplest way to be represented in an application. This program focuses more on the correct way to use .NET Framework along with ASP.NET Core Identity, both combined can create applications totally out of this world.

## Sample Nº 1:
![image](https://github.com/CrisOporta/StockCheckSheet_MVC/assets/47622028/acbfcb21-dd65-454a-8b5f-a5feaad93621)

## Sample Nº 2:
![image](https://github.com/CrisOporta/StockCheckSheet_MVC/assets/47622028/8b7f09f8-c2fe-41a2-b1c0-bb5e638087fa)


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
