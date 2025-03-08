# NZWalks

NZWalks is an ASP.NET Core application combining a Web API (NZWalks.API) and an MVC client (NZWalks.UI). The API provides walking route data, while the MVC app consumes the API to manage walks, regions, and difficulties with a user-friendly interface.

## Features
- **Walk Management:**
  - Add, edit, and view walking routes.
- **Region Management:**
  - Add, edit, and list regions.
- **Difficulty Management:**
  - Define and manage difficulty levels.
- **Image Management:**
  - Upload and manage images for walks.
- **Authentication & Authorization:**
  - Secure login/register with ASP.NET Identity.
  - JWT Token for API authorization.
- **API Consumption:**
  - MVC UI consumes RESTful API endpoints.
- **Responsive Design:** Mobile and desktop compatible.

## Technologies
- **Framework:** ASP.NET Core MVC & Web API (.NET 6/8)
- **Language:** C#
- **Database:** SQL Server 2022, Entity Framework Core
- **Authentication:** ASP.NET Identity
- **Authorization:** JWT Token
- **Design Patterns:** Repository Pattern, AutoMapper
- **Frontend:** HTML, CSS, JavaScript, Bootstrap
- **Tools:** Visual Studio 2022, SSMS

## Prerequisites
- .NET SDK (e.g., .NET 6/8)
- Visual Studio 2022 (ASP.NET workload)
- SQL Server 2022
- SSMS

## Installation
1. Clone: `git clone https://github.com/celikmehmetfatihh/NZWalks.git`
2. Navigate to API: `cd NZWalks/NZWalks.API`
3. Navigate to UI: `cd NZWalks/NZWalks.UI`
4. Open solutions in Visual Studio.
5. Restore NuGet packages.
6. Set up `NZWalksAuthDb` and `NZWalksDb` in SQL Server (run EF migrations).
7. Update `appsettings.json` with connection strings:
   "ConnectionStrings": {
     "NZWalksAuthDb": "Server=FATIh;Database=NZWalksAuthDb;Trusted_Connection=True;",
     "NZWalksDb": "Server=FATIh;Database=NZWalksDb;Trusted_Connection=True;"
   }
