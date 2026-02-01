var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging")
    .WithManagementPlugin()
    .WithHttpEndpoint(port: 15673) 
    .WithEnvironment("RABBITMQ_DEFAULT_USER", "guest")
    .WithEnvironment("RABBITMQ_DEFAULT_PASS", "guest"); ;

var apiCredit = builder.AddProject<Projects.Yggdrasil_Credit_ApiService>("credit-apiservice")
    .WithReference(rabbitmq);

var apiOrigination = builder.AddProject<Projects.Yggdrasil_Origination_ApiService>("origination-apiservice")
    .WithReference(rabbitmq);

var apiQuotation = builder.AddProject<Projects.Yggdrasil_Quotation_ApiService>("quotation-apiservice")
    .WithReference(rabbitmq);

var apiCatalog = builder.AddProject<Projects.Yggdrasil_Catalog_ApiService>("catalog-apiservice")
    .WithReference(rabbitmq);


var apiGateway = builder.AddProject<Projects.Yggdrasil_ApiGateway>("apigateway")
    .WithReference(apiQuotation)
    .WaitFor(apiQuotation);



var authService = builder.AddProject<Projects.Yggdrasil_AuthServer>("authserver");


builder.AddProject<Projects.Yggdrasil_Site>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(rabbitmq)
    .WithReference(apiGateway)
    .WaitFor(authService);



builder.Build().Run();
