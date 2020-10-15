# AFI - Technical test

## Packages
### MVC Versioning
Used to version API controllers - hot topic
### Swagger + Swashbuckle
Internal only! - API specification tool
### Serilog
Used for easier formatting of console read out but also allows many different sinks to be used easily
### Application Insights
Not fully implemented but shows how logging could be pushed to App Insights

## Documentation


### Structure
https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice
#### AFI.API
API project - consumes all lower level projects to become the endpoint (point of contact)

#### AFI.Domain
This project is concerned with aggregating data from the persistence layer to output from the API.

#### AFI.Persistence
This project is concerned with Database storage and retrieval. It is a Code First migration .NET Core application. The repository pattern can be used to aggregate Domain models in to single units of work.

## DB Migrations
Open a command prompt and navigate to the AFI.Persistance folder.

To create new migrations:
dotnet ef --startup-project ../AFI.API/ migrations add Initial --context AFIContext

To run migrations:
dotnet ef --startup-project ../AFI.API/ database update --context AFIContext
*The connection string assumes SQL Authentication with a username and password of "api".*
