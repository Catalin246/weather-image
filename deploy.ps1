# Define variables
$resourceGroupName = "WeatherImageResources"
$location = "westeurope"  # Set location to West Europe
$bicepFilePath = "./main.bicep"  # Path to your Bicep template file
$functionAppName = "weatherImageFunctionApp" + (Get-Random) # Ensures unique name
$projectPath = "./"  # Path to your .NET project (e.g., where your .csproj file is located)

# Step 1: Login to Azure
Write-Output "Logging into Azure..."
az login

# Step 2: Create Resource Group (if it doesn’t exist)
Write-Output "Creating resource group $resourceGroupName in $location if it does not already exist..."
az group create --name $resourceGroupName --location $location

# Step 3: Deploy Resources using Bicep
Write-Output "Deploying resources with Bicep template..."
az deployment group create --resource-group $resourceGroupName --template-file $bicepFilePath --parameters location=$location

# Step 4: Publish the Function App using dotnet CLI
Write-Output "Publishing the Azure Function code using dotnet CLI..."
dotnet publish $projectPath --configuration Release -o ./publish

# Step 5: Deploy the Function App Code to Azure
Write-Output "Deploying the function app code to Azure..."
$zipFilePath = "./functionapp.zip"
Compress-Archive -Path "./publish/*" -DestinationPath $zipFilePath -Force

Write-Output "Deploying the function app package to Azure..."
az functionapp deployment source config-zip --resource-group $resourceGroupName --name $functionAppName --src $zipFilePath

# Step 6: Clean Up
Write-Output "Cleaning up..."
Remove-Item -Force -Path $zipFilePath
Write-Output "Deployment completed successfully."
