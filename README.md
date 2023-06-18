# RestaurantAPI

RestaurantAPI is a web API application built using ASP.NET Core. It provides functionality for managing restaurants, dishes, user accounts, and file uploads. The API allows users to perform CRUD operations on restaurants, create and delete dishes, register and login user accounts, and upload/download files.
##
##
## Technologies Used

ASP.NET Core  
Entity Framework Core  
AutoMapper  
FluentValidation  
JWT Authentication  
Swagger UI  
NLog

##
##
## Requirements

.NET 6.0 SDK
SQL Server (or SQL Server Express) for database storage

##
##
## Get Started
##
1. Clone the repository:
```bash
git clone https://github.com/grimuri/RestaurantAPI.git
```
##
2. Navigate to the project directory:
```bash
cd RestaurantAPI
```
##
3. Update the connection string in the appsettings.json file to point to your SQL Server instance:
```javascript
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RestaurantDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```
##
4. Run the migrations to create the database:
```bash
dotnet ef database update
```
##
5. Build and run the application:
```bash
dotnet run
```
The API will be accessible at https://localhost:7121  
The SWAGGER will be accessible at https://localhost:7121/swagger/index.html
##
##
## API Endpoints
Please refer to the API documentation available in Swagger UI for detailed information on request/response formats and required parameters.
[SWAGGER](https://restaurantsapi.azurewebsites.net/swagger/index.html)

##
##
## Authorization
The RestaurantAPI uses JWT (JSON Web Tokens) for authentication and authorization. To access protected endpoints, you need to include a valid JWT token in the Authorization header of your requests. The token can be obtained by logging in using the /api/account/login endpoint.
