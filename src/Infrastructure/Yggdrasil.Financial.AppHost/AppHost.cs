var builder = DistributedApplication.CreateBuilder(args);

//var apiService = builder.AddProject<Projects.Yggdrasil_Financial_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");
var apiCredit = builder.AddProject<Projects.Yggdrasil_Credit_ApiService>("yggdrasil-credit-apiservice");

var apiOrigination = builder.AddProject<Projects.Yggdrasil_Origination_ApiService>("yggdrasil-origination-apiservice");

var apiQuotation = builder.AddProject<Projects.Yggdrasil_Quotation_ApiService>("yggdrasil-quotation-apiservice");

var authService = builder.AddProject<Projects.Yggdrasil_AuthServer>("yggdrasil-authserver");

var apiGateway = builder.AddProject<Projects.Yggdrasil_ApiGateway>("yggdrasil-apigateway");

builder.AddProject<Projects.Yggdrasil_Financial_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiCredit)
    .WaitFor(apiCredit);


builder.AddProject<Projects.Yggdrasil_Catalog_ApiService>("yggdrasil-catalog-apiservice");


builder.Build().Run();
