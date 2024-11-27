@echo off
echo Batch to pack a specific project
del /q packs\*

set /p projectName= Type the project name (without .csproj): 
set /p input= Type the new version: 

rem Construir el proyecto
dotnet build %projectName%.csproj --configuration Release /p:Version=%input%

rem Empaquetar el proyecto
dotnet pack %projectName%.csproj --configuration Release /p:Version=%input% --no-build --output ./packs/.

rem Copiar el paquete a un directorio específico
set targetDir=C:\Users\Daniel Acevedo\Documents\GitHub\CodeQuality\LocalNuget
copy ./packs/*.nupkg "%targetDir%"

pause
