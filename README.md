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

## Inportant notes about different roles
### User
1. User can get their own data via GET /api/accounts (it makes more sense to me, than using by id, because how would a User know their own database id?!). Here the API uses User's token to identify them and return adata about them
2. User can get their assigned devices using GET /api/accounts/devices (same logic as previously - use the token to identify the User and return the data)
3. User can update their data using PUT /api/accounts (still same logic - use the token to identify the User for who to update the data)
4. User can use PUT /api/devices/{id} to update only their devices (it is validated in the Service layer)
### Admin
Admin has access to almost all endpoints, except these:
1. GET /api/accounts/devices - to view devices assigned to a User (don't really see the point, since Admin can view all the devices in the database anyway, could be easily corrected though)
2. PUT /api/accounts - to update the personal data of the User. Also, no point in that, since the Admin has more direct access to the database via other endpoints

That's it, I hope my points are valid and I will not lose my points over some stupid shit I overlooked. Surely..

"Scientists discover the world that exists. Engineers create the world that never was." - Theodore Von Karman

(Okay-okay, I saw the 12th task, i will change it to IDs 😒)
