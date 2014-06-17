# ViaggiaTrentino

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