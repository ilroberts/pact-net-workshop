using provider.Model;
using Optional;
using System.Reflection.Metadata.Ecma335;

namespace provider.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> State { get; set; }

        public ProductRepository()
        {
            State =
            [
                new Product(9, "GEM Visa", "CREDIT_CARD", "v2"),
                new Product(10, "28 Degrees", "CREDIT_CARD", "v1")
            ];
        }

        public void SetState(List<Product> state)
        {
            this.State = state;
        }

        List<Product> IProductRepository.List()
        {
            return State;
        }

        public Option<Product> Get(int id)
        {
            var result = State.Find(p => p.Id == id);
            return (result == null) ? Option.None<Product>() : Option.Some(result);
        }
    }
}
