namespace Consumer
{
    public class ApiClient(Uri baseUri)
    {
        private readonly Uri BaseUri = baseUri;

        public async Task<HttpResponseMessage> GetAllProducts()
        {
            using var client = new HttpClient { BaseAddress = BaseUri };
            try
            {
                var response = await client.GetAsync($"/api/products");
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem connecting to Products API.", ex);
            }
        }

        public async Task<HttpResponseMessage> GetProduct(int id)
        {
            using var client = new HttpClient { BaseAddress = BaseUri };
            try
            {
                var response = await client.GetAsync($"/api/products/{id}");
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("There was a problem connecting to Products API.", ex);
            }
        }
    }
}
