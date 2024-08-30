
# .Net Pact Demonstrator

![example workflow](https://github.com/ilroberts/pact-net-workshop/actions/workflows/dotnet.yml/badge.svg?branch=main)

This project is intended to be a gentle introduction to [Pact testing](https://pact.io/) for .Net projects. It is based on the [pact-workshop-dotnet-core-v3](https://github.com/DiUS/pact-workshop-dotnet-core-v3) with a few updates to bring it up to date. The readme file in that repository contains a detailed description of the consumer and provider and the associated pact tests.

## Running locally

### Running the consumer

1. ```cd Consumer```
2. ```dotnet restore```
3. ```dotnet run``` 

Note: the consumer will display errors if the producer is not also running.

### Running the provider

1. ```cd Provider```
2. ```dotnet restore```
3. ```dotnet run```

## Running the pact tests

### Generating the pact file

In the ```Consumer.Tests``` directory, run ```dotnet test```. This will run the tests and, if successful, generate a pact file in the **Consumer.Tests\pacts** folder.

### Testing the contract
In the ```Provider.Tests``` directory, run ```dotnet test```. This will use the pact file from the ```Consumer.Tests\pacts``` and verify that the producer satisfies the contract.

## Lessons Learned

Documentation for Pact testing in .Net is either in short supply or outdated. It was particularly difficult to figure out how the provider state was implemented. The instantiation of the web app in ```Producer.Tests``` directory is as follows:

```csharp
var args = Array.Empty<string>();
            using var app = Startup.WebApp(args);
            app.UseMiddleware<ProviderStateMiddleware>();
            app.Urls.Add(_pactServiceUri);
            app.Start();
```            

Adding the ```ProviderStateMiddleware``` class, an additional endpoint (```/provider-states```) is exposed by the service. This endpoint is used by the middleware to change the state of the internal product repository. A POST request is sent to the endpoint with the following format:

```json
{"action":"setup","params":{},"state":"products exist"}
```

The state "no products exist" matches the Given clause in the Consumer test:

```csharp
      pact.UponReceiving("A valid request for all products")
              .Given("products exist")
              .WithRequest(HttpMethod.Get, "/api/products")
          .WillRespond()
              .WithStatus(HttpStatusCode.OK)
              .WithHeader("Content-Type", "application/json; charset=utf-8")
              .WithJsonBody(new TypeMatcher(products));
```

The middleware then uses that state to invoke the required method to set the repository to the required state before the test is executed.
## License

[MIT](https://choosealicense.com/licenses/mit/)

