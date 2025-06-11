APBD Task 10

To launch your project, you need to add appsettings.json file with these contents:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "EmployeeDatabase": "<your connection string>"
  },
  "Jwt":{
    "Issuer": <who provides tokens>,
    "Audience": <who is provided with tokens>,
    "Key": <hashing key>,
    "ValidInMinutes": <token's lifespan>
  }
}
```

## Splitting the projects
I decided to split the project as usual into 4 parts: Models, Repositories, Services and API (endpoints). The reasons for that are the same as before: Models for model definition, Repositories for repositories (connection to the database), Services for validation and API for endpoints and Dependency Injections.

Correction: removed repository pattern because there's no need for it
