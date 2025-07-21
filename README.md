# GraphAttachmentDownloader

A .NET console application that connects to Microsoft Graph API to download email attachments (especially .eml and .msg) from a specified user’s mailbox. It supports downloading both file and item attachments, and exports metadata in JSON and CSV formats.

## Features
- Authenticates with Microsoft Graph using `Azure.Identity` (`ClientSecretCredential`)
- Uses Microsoft Graph SDK v5+ for message and attachment access
- Downloads:
  - File attachments (e.g., PDFs, images)
  - Embedded item attachments (`.eml`) using raw HTTP fallback
- Logs processing details using Serilog
- Exports attachment metadata to:
  - `attachment_metadata.json`
  - `attachment_metadata.csv`


## Prerequisites

- .NET 6.0 SDK or later
- Azure AD App Registration with:
  - `client_id`, `client_secret`, and `tenant_id`
  - API permissions for `Mail.Read` (Application)

## Configuration
Rename the root directory file `appsettings.template.json` to  `appsettings.json`. Then, update the contents with your tenant ID, Client ID, Client Secret, and the user whom you want to search:

```json
{
  "AzureAd": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  },
  "Graph": {
    "Endpoint": "https://graph.microsoft.com/v1.0",
    "SearchUser": "user@domain.com"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" }
    ]
  }
}
```

## Usage

1. Clone the repository

```bash
git clone https://github.com/dylanstetts/GraphAttachmentDownloader.git
cd GraphAttachmentDownloader
```

2. Resitre dependencies and build:

```shell
dotnet restore
dotnet build
```

3. Run the application:

```shell
dotnet run
```

4. Output:
   
- Attachments saved to the working directory
- Metadata exported to:
    - attachment_metadata.json
    - attachment_metadata.csv

## Project Structure

- Program.cs – Entry point
- GraphClient.cs – Handles authentication and Graph API requests
- AttachmentProcessor.cs – Downloads and processes attachments
- MetadataExporter.cs – Exports metadata to JSON and CSV
- AttachmentMetadata.cs – Data model for attachment metadata

## Logging

Uses Serilog for structured logging. Logs are output to the console by default.

