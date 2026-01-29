var builder = DistributedApplication.CreateBuilder(args);

var apiCredit = builder.AddProject<Projects.Yggdrasil_Credit_ApiService>("credit-apiservice");
var apiOrigination = builder.AddProject<Projects.Yggdrasil_Origination_ApiService>("origination-apiservice");
var apiQuotation = builder.AddProject<Projects.Yggdrasil_Quotation_ApiService>("quotation-apiservice");
var apiCatalog = builder.AddProject<Projects.Yggdrasil_Catalog_ApiService>("catalog-apiservice");


var apiGateway = builder.AddProject<Projects.Yggdrasil_ApiGateway>("apigateway")
    .WithReference(apiQuotation)
    .WaitFor(apiQuotation);



var authService = builder.AddProject<Projects.Yggdrasil_AuthServer>("authserver");


builder.AddProject<Projects.Yggdrasil_Site>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiGateway)
    .WaitFor(authService);



builder.Build().Run();
