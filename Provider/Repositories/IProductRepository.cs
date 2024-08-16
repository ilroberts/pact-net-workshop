using provider.Model;
using Optional;

namespace provider.Repositories
{
    public interface IProductRepository
    {
        public List<Product> List();
        public Option<Product> Get(int id);

        public void SetState(List<Product> product);
    }
}
