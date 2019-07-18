Install latest AutoRest globally via 'npm install -g autorest@2.0.4387' command
 
1. Open Tools > NuGet Package Manager > Package Manager Console
2. Run the following commands to generate API clients:

$modules = @('Cart','Catalog','Customer','Orders','Pricing','Store')
$modules.ForEach( { autoRest -Input http://localhost/admin/docs/VirtoCommerce.$_/v1  -OutputFileName $_`ModuleApi.cs -Namespace VirtoCommerce.OrderBot.AutoRestClients.$_`ModuleApi -ClientName $_`ModuleApiClient -OutputDirectory VirtoCommerce.OrderBot\AutoRestClients -AddCredentials true -UseDateTimeOffset false })

Troubleshooting

See AutoRest guide here:
https://github.com/Azure/autorest/blob/master/docs/developer/guide/building-code.md#strong-name-validation-errors
