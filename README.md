# heimdall
Poor man's service discovery tool in C# with ASP.NET Core and MongoDb. A web client is provided, written with React.

Has a dependency https://github.com/mizrael/LibCore .

## Installation
* launch the MongoDb instance (a batch script is provided here: https://github.com/mizrael/heimdall/blob/master/db_server_start.bat )
* update the appsettings.json files
* launch the API and Web projects (although the Web is useful only if you want to use the web client)
* schedule a cron job (or similar) to execute a POST request to /api/services/refresh/[service name]

## TODO
* api to refresh all services simultaneously
* improve the web client usability
