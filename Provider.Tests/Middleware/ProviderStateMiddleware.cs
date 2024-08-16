using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using provider.Model;
using provider.Repositories;

namespace Provider.Tests.Middleware
{
    public class ProviderStateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IProductRepository _repository;
        private readonly IDictionary<string, Action> _providerStates;

        public ProviderStateMiddleware(RequestDelegate next, IProductRepository repository)
        {
            _next = next;
            _repository = repository;
            _providerStates = new Dictionary<string, Action>
            {
                { "products exist", ProductsExist},
                { "no products exist", NoProductsExist },
                { "product with ID 10 exists", Product10Exists },
                { "product with ID 11 does not exist", Product11DoesNotExist }
            };
        }

        private void ProductsExist()
        {
            List<Product> products =
            [
                new Product(9, "GEM Visa", "CREDIT_CARD", "v2"),
                new Product(10, "28 Degrees", "CREDIT_CARD", "v1")
            ];

            _repository.SetState(products);
        }

        private void NoProductsExist()
        {
            _repository.SetState([]);
        }

        private void Product10Exists()
        {
            List<Product> products =
            [
                new Product(10, "28 Degrees", "CREDIT_CARD", "v1")
            ];

            _repository.SetState(products);
        }

        private void Product11DoesNotExist()
        {
            ProductsExist();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/provider-states"))
            {
                await HandleProviderStatesRequest(context);
                await context.Response.WriteAsync(string.Empty);
            }
            else
            {
                await _next(context);
            }
        }

        private async Task HandleProviderStatesRequest(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            if (context.Request.Method.Equals(HttpMethod.Post.ToString().ToUpperInvariant(), StringComparison.OrdinalIgnoreCase) &&
                context.Request.Body != null)
            {
                string jsonRequestBody = string.Empty;
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8))
                {
                    jsonRequestBody = await reader.ReadToEndAsync();
                }

                var providerState = JsonConvert.DeserializeObject<ProviderState>(jsonRequestBody);

                if (providerState != null && !string.IsNullOrEmpty(providerState.State))
                {
                    _providerStates[providerState.State].Invoke();
                }
            }
        }
    }
}
