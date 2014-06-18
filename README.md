# ViaggiaTrentino

***

## Setting up Visual Studio

Visual Studio 2012 and the Windows Phone 8.0 SDK are required.

This solution requires the following plugins/extensions to be installed and configured
* Microsoft Multilingual App tool
* SQLite for Windows Phone
* Nuget

The solution is also dependant on the libraries found in the following repositories: 
* scwp.support
and it expects to find it at its same level. That is
```
+---GitHub
|   +---scwp.apps
|   |   \---ViaggiaTrentino
|   \---scwp.support
|       +---AuthenticationLibrary
|       +---CommonHelpers
|       +---CompiledDLL
|       +---MobilityServiceLibrary
|       +---Models

```

## Preparation

This application requires a pre-loaded database in order to run properly
in order to obtain such DB, issue the following commands onto a Powershell®
terminal

first navigate to the correct directory
Program Files (x86)\Microsoft SDKs\Windows Phone\v8.0\Tools\IsolatedStorageExplorerTool
by giving this command

```
cd "C:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.0\Tools\IsolatedStorageExplorerTool"
```
then issue the following

```
ISETool.exe ts xd <product-id> <destination-directory>
```

where product-id is the product id found in WMAppManifest.xml, 
and destination-directory is an empty directory in which files will be stored.

After obtaining the .sqlite file, copy it in the ViaggiaTrentino project and perform 
a clean solution=> build cycle


