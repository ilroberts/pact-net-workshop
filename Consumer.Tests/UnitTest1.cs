namespace Consumer.Tests;

using PactNet;
using Xunit;

public class UnitTest1
{
    private readonly IMessagePactBuilderV4 pact;

    public UnitTest1()
    {
        var config = new PactConfig
        {
            PactDir = @".\pacts"
        }; 

        this.pact = Pact.V4("Fullfilment API", "Orders API", config).WithMessageInteractions();
    }

    [Fact]
    public void Test1()
    {

    }
}
