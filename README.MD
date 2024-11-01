# Azure / Azure Devops  assignment
## Task Requirements
Using azure functions, create an http endpoint which will start a background job. That job will create multiple images, each containing the weather data from a different weather station. The initial http endpoint will return a unique id for (or link to) a different endpoint. That endpoint will either display that the jobs are still in progress, or a list with links to all the generated images served from the blob storage.
A sample of code for writing text on an image can be found here.
https://github.com/wearetriple/InHolland-CCD/tree/master/2024/samples/ImageEditor

Must
o	Expose publicly accessible API for requesting a set of fresh images with current weather data using a HttpTrigger. (done)
o	Employ QueueTrigger to process the job in the background so the initial call stays fast. (done)
    	Employ Blob Storage to store all generated images and to expose the files. (done)
    	Employ Buienrader api to get weather station data (done)
        	https://data.buienradar.nl/2.0/feed/json
    	Employ any public api for retrieving an image to write the weather data on. (done) 
        	e.g. https://unsplash.com/developers
o	Expose a publicly accessible API for fetching the generated images using HttpTriggers.(done) - images url not accessible - all images retured instead     only the generated images (for a specific jobId)

o	Provide HTTP files as API documentation.
o	Create a fitting Bicep template (include the queues as well).
o	Add all files to Azure DevOps repo and add tripleinhollandccd2024@outlook.com 
to organization and project.
o	Create a deploy.ps1 script that publishes your code using the dotnet cli, creates the resources in azure using the Bicep template and deploys the function using the azure cli.
o	Employ multiple queues one for starting the job and one for writing text on a single image.
o	Deploy the code to azure and have a working endpoint.
 
Could
o	Use SAS token instead of publicly accessible blob storage for fetching finished image directly from Blob.
o	Build and deploy the code automatically from Azure DevOps.
o	Use authentication on request API. (Be sure to provide me with credentials)
o	Provide a status endpoint for fetching progress status and saving status in Table.
*Having all “Must” requirements will result in a minimal passing grade, also having “Could” requirements results in a higher grade.

Deadline: Friday 8 November 09:00 AM CET
Please inform me that you finished the assignment by mailing to tripleinhollandccd2024@outlook.com 
Also include the link to the correct repository.
Extra instruction for giving me access:
1.	Add me to the organization using my email (https://dev.azure.com/{organization} /_settings/users) (You can add me as visual studio subscriber, and send me the invite).
2.	Under policies, enable External Guest Access, it is the third option from the bottom. (https://dev.azure.com/{organization}/_settings/organizationPolicy)
3.	Add me as administrator to your project (https://dev.azure.com/{organization}/{projectName} /_settings/).
4.	If you have created your own azure devops tenant, please invite me to this as well.


