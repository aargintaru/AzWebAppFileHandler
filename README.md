# AzWebAppFileHandler

## Description

This app started as a need to download and upload files from an Azure Web App in a programatic way.

The intent for the app is to be integrated into a Powershell script and provide a way to Download or Upload a file (hardcoded in current version) to and from a deployed Web App.

The following environment variables (machine-level) must be set:
- AWAFH_HOSTNAME (e.g. test.ftp.azurewebsites.windows.net)
- AWAFH_USERNAME (e.g. taken from FTPS credentials in Web App settings)
- AWAFH_PASSWORD (e.g. taken from FTPS credentials in Web App settings)

## How to publish a new version to GitHub

1. Go to solution level directory
2. Run command: `dotnet publish --output .\bin\publish`
3. Pack into zip archive: `Compress-Archive .\bin\publish\* .\bin\app.zip`
4. Publish a new tag for the new version.
4. Go to GitHub/Releases/Draft a new release and select previously created tag

## How to use it

`.\bin\AzWebAppFileHandler.Console.exe DOWNLOAD`

or

`.\bin\AzWebAppFileHandler.Console.exe UPLOAD`