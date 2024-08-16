namespace Provider
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using WebApplication app = Startup.WebApp(args);
            app.Urls.Add("http://localhost:9001");
            app.Run();
        }
    }
}
