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

# Step 3: Check if the Function App is created
Write-Output "Checking if Function App '$functionAppName' exists..."
$functionAppCheck = az functionapp show --name $functionAppName --resource-group $resourceGroupName --query "name" -o tsv

if ($functionAppCheck -eq $functionAppName) {
    Write-Output "Function App '$functionAppName' exists. Proceeding with deployment..."
    
    # Publish the Function App
    Write-Output "Publishing Function App '$functionAppName'..."
    func azure functionapp publish $functionAppName
    Write-Output "Function App '$functionAppName' published successfully."
} else {
    Write-Output "Function App '$functionAppName' was not found in resource group '$resourceGroupName'. Please check the deployment and try again."
    exit 1
}
