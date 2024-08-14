using PactNet.Verifier;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Provider;

namespace Provider.Tests
{
  public class ProductTest
  {
    private string _pactServiceUri = "http://127.0.0.1:9001";

    [Fact]
    public void EnsureProviderApiHonoursPactWithConsumer()
    {
      var config = new PactVerifierConfig
      {
        // NOTE: We default to using a ConsoleOutput, however xUnit 2 does not capture the console output,
        // so a custom outputter is required.
        Outputters = [
            new PactNet.Infrastructure.Outputters.ConsoleOutput(),
                ]
      };
      var args = Array.Empty<string>();
      using var _webHost = WebHost.CreateDefaultBuilder(args)
          .UseStartup<Startup>()
          .UseUrls(_pactServiceUri)
          .Build();
      _webHost.Start();

      using var verifier = new PactVerifier(config);
      var pactFolder = new DirectoryInfo(Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "Consumer.Tests", "pacts"));
      verifier.ServiceProvider("ProductService", new Uri(_pactServiceUri))
      .WithDirectorySource(pactFolder)
      .Verify();
    }
  }
}

