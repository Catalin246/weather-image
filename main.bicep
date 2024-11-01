@description('Location for all resources')
param location string = resourceGroup().location

// Define the storage account
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-05-01' = {
    name: 'weatherimagestorage123'
    location: location
    sku: {
      name: 'Standard_LRS'
    }
    kind: 'StorageV2'
    properties: {
        allowBlobPublicAccess: true
    }
  }
  
  // Create a blob container for storing weather images
  resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2022-05-01' = {
    name: '${storageAccount.name}/default/weather-image-public'
    properties: {
      publicAccess: 'Blob'
    }
  }
  
  // Define a queue for background job processing
  resource weatherQueue 'Microsoft.Storage/storageAccounts/queueServices/queues@2022-05-01' = {
    name: '${storageAccount.name}/default/weather-image-queue'
  }
  
  // Define a table for storing job statuses
  resource jobStatusTable 'Microsoft.Storage/storageAccounts/tableServices/tables@2022-05-01' = {
    name: '${storageAccount.name}/default/JobStatusTable'
  }
  
  // Define the Function App to run Azure Functions
  resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
    name: 'weatherImageFunctionApp${uniqueString(resourceGroup().id)}'
    location: location
    kind: 'functionapp'
    properties: {
      serverFarmId: functionPlan.id
      siteConfig: {
        appSettings: [
          {
            name: 'AzureWebJobsStorage'
            value: storageAccount.properties.primaryEndpoints.blob
          }
        ]
      }
    }
  }
  
  // Define the App Service Plan for the Function App (can be Consumption or Premium plan)
  resource functionPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
    name: 'weatherImagePlan${uniqueString(resourceGroup().id)}'
    location: location
    sku: {
      name: 'Y1' // Y1 is Consumption plan, adjust if needed
      tier: 'Dynamic'
    }
  }
  