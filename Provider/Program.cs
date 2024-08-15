using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Provider
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var app = Startup.WebApp(args);
            app.Urls.Add("http://localhost:9001");
            app.Run();
        }
    }
}
