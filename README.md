# Drug Information Application

This application retrieves and displays drug information from the OpenFDA API. It allows users to search for drugs, view details, and analyze drug indications.

## Features
- Search for drugs by name or purpose.
- View detailed information about a drug, including purpose, active ingredients, warnings, and dosage.
- Pagination support for browsing large datasets.
- Responsive design for seamless use on all devices.

## Technologies Used
- **Backend**: ASP.NET Core
- **Frontend**: HTML, CSS, JavaScript, jQuery
- **Database**: MySQL (via Entity Framework Core)
- **API**: OpenFDA

### Prerequisites
- .NET 6 SDK
- MySQL Server
- OpenFDA API Key (register at [OpenFDA](https://open.fda.gov/apis/))

### Installation and usage
1. Clone the repository:
   ```bash
   git clone https://github.com/nalosara/openfda.git
2. Open project and navigate to appsettings.json. Add ConnectionString with connection to the db, and the OpenFDA API Key and base url.
3. Open terminal and run 'dotnet run'.
