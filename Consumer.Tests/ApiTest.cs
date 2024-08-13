
using PactNet;
using Xunit.Abstractions;
using System.Net;
using Consumer;
using PactNet.Matchers;
using PactNet.Infrastructure.Outputters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace tests
{
    public class ApiTest
    {
        private IPactBuilderV3 pact;
        private readonly ApiClient ApiClient;
        private readonly int port = 9000;
        private readonly List<object> products;

        public ApiTest(ITestOutputHelper output)
        {
            products = new List<object>()
            {
                new { id = 9, type = "CREDIT_CARD", name = "GEM Visa", version = "v2" },
                new { id = 10, type = "CREDIT_CARD", name = "28 Degrees", version = "v1" }
            };

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
            };            //pact = Pact.V3("ApiClient", "ProductService", Config).UsingNativeBackend(port);
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

            await pact.VerifyAsync(async ctx => {
                var response = await ApiClient.GetAllProducts();
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Fact]
        public async void GetProduct()
        {
            // Arange
            pact.UponReceiving("A valid request for a product")
                    .Given("There is data")
                    .WithRequest(HttpMethod.Get, "/api/product/10")
                .WillRespond()
                    .WithStatus(HttpStatusCode.OK)
                    .WithHeader("Content-Type", "application/json; charset=utf-8")
                    .WithJsonBody(new TypeMatcher(products[1]));

            await pact.VerifyAsync(async ctx => {
                var response = await ApiClient.GetProduct(10);
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            });
        }
    }
}
