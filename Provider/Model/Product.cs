namespace provider.Model
{
    public class Product(int id, string name, string type, string version)
    {
        public int Id { get; set; } = id;
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
        public string Version { get; set; } = version;
    }
}
