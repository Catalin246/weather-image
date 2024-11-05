# Define variables
$resourceGroupName = "WeatherImageDemo"
$location = "westeurope"  # Azure region for the resource group
$templateFilePath = "./main.bicep"  # Path to your Bicep template file
$appInsightsLocation = "westeurope"  # Application Insights location
$functionAppName = "fnappoekk4niakfyi4"  # Name of the Function App

# Step 1: Create the resource group
Write-Output "Creating resource group '$resourceGroupName' in location '$location'..."
az group create --name $resourceGroupName --location $location
Write-Output "Resource group '$resourceGroupName' created successfully."

# Step 2: Deploy resources using the Bicep template
Write-Output "Deploying resources to resource group '$resourceGroupName' using template '$templateFilePath'..."
az deployment group create --resource-group $resourceGroupName --template-file $templateFilePath --parameters appInsightsLocation=$appInsightsLocation
Write-Output "Resources deployed successfully in resource group '$resourceGroupName'."

# Step 3: Publish the Function App
Write-Output "Publishing Function App '$functionAppName'..."
func azure functionapp publish $functionAppName
Write-Output "Function App '$functionAppName' published successfully."
