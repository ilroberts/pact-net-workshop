
using PactNet;
using Xunit.Abstractions;
using System.Net;
using PactNet.Matchers;
using PactNet.Infrastructure.Outputters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Consumer.Tests
{
  public class ApiTest
  {
    private readonly IPactBuilderV3 pact;
    private readonly ApiClient ApiClient;
    private readonly int port = 9000;
    private readonly List<object> products;

    public ApiTest(ITestOutputHelper output)
    {
      products =
      [
          new { id = 9, type = "CREDIT_CARD", name = "GEM Visa", version = "v2" },
          new { id = 10, type = "CREDIT_CARD", name = "28 Degrees", version = "v1" }
      ];

      var Config = new PactConfig
      {
        Outputters =
        [
          new ConsoleOutput()
        ],
        DefaultJsonSettings = new JsonSerializerSettings
        {
          ContractResolver = new CamelCasePropertyNamesContractResolver()
        }
      };
      this.pact = Pact.V3("ApiClient", "ProductService", Config).WithHttpInteractions(port);
      ApiClient = new ApiClient(new Uri($"http://localhost:{port}"));
    }

    [Fact]
    public async void GetAllProducts()
    {
      // Arange
      pact.UponReceiving("A valid request for all products")
              .Given("There is data")
              .WithRequest(HttpMethod.Get, "/api/products")
          .WillRespond()
              .WithStatus(HttpStatusCode.OK)
              .WithHeader("Content-Type", "application/json; charset=utf-8")
              .WithJsonBody(new TypeMatcher(products));

      await pact.VerifyAsync(async ctx =>
      {
        var response = await ApiClient.GetAllProducts();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      });
    }

    [Fact]
    public async void GetProduct()
    {
      pact.UponReceiving("A valid request for a product")
              .Given("There is data")
              .WithRequest(HttpMethod.Get, "/api/products/10")
          .WillRespond()
              .WithStatus(HttpStatusCode.OK)
              .WithHeader("Content-Type", "application/json; charset=utf-8")
              .WithJsonBody(new TypeMatcher(products[1]));

      await pact.VerifyAsync(async ctx =>
      {
        var response = await ApiClient.GetProduct(10);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      });
    }

    [Fact]
    public async void NoProductExists()
    {
      pact.UponReceiving("A valid request for all products")
              .Given("no product exists")
              .WithRequest(HttpMethod.Get, "/api/products")
          .WillRespond()
              .WithStatus(HttpStatusCode.OK)
              .WithHeader("Content-Type", "application/json; charset=utf-8")
              .WithJsonBody(new List<object>());

      await pact.VerifyAsync(async ctx =>
      {
        var response = await ApiClient.GetAllProducts();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"received body: {body}");
        Assert.Equal("[]", body);
      });
    }

    [Fact]
    public async void ProductDoesNotExist()
    {
      pact.UponReceiving("A valid request for a product")
              .Given("product with ID 11 does not exist")
              .WithRequest(HttpMethod.Get, "/api/products/11")
          .WillRespond()
              .WithStatus(HttpStatusCode.NotFound);

      await pact.VerifyAsync(async ctx =>
      {
        var response = await ApiClient.GetProduct(11);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
      });
    }
  }
}
