## Web API and Web Scraper/Crawler

### How to run and use it:
- Install Docker daemon
- Within Covid19Watcher run:

```bash
    docker build -t covid19watcher:latest . && docker run -p 5000:5000 -d covid19watcher:latest
```
- Now run crawler/scraper with:

```bash
    cd Covid19Watcher.Scraper &&
    dotnet run -- Brazil UnitedStates ChinaMainLand {all intended countries here}
```
- You must make your own appsettings.json file into each project's folder!
- API template:

```json
{
  "MongoDBSettings": {
    "DataBase": "covid19",
    "ConnString": "mongodb://localhost:27017/",
    "Collections": ["Notifications"]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "Credentials": {
    "key": "{your_key_here}",
    "Login": "user",
    "Password": "12345"
  }
}
```
- Scraper template:

```json
{
    "SeleniumConfigurations": {
        "GoogleDriver": "{your_path_here}/Covid19Watcher/Covid19Watcher.Scraper/",
        "URI": "https://www.bing.com/covid/",
        "Timeout": 5000
    },
    "API_URI": "http://localhost:5000/api/"
}
```