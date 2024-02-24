set output=./publish
if exist "%output%" rd /S /Q "%output%"
dotnet publish -c Release -o "%output%/JudasClient/Judas" ./Client/FileDownloader/FileDownloader.csproj
dotnet publish -c Release -o "%output%/JudasClient/Judas" ./Client/FileUploader/FileUploader.csproj
dotnet publish -c Release -o "%output%/JudasClient/Judas" ./Client/JudasClient/JudasClient.csproj
dotnet publish -c Release -o "%output%/JudasClient/Judas" ./Client/JudasShell/JudasShell.csproj
dotnet publish -c Release -o "%output%/JudasClient/Judas" ./Client/Screenshot/Screenshot.csproj