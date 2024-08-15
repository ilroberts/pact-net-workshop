using provider.Repositories;

namespace Provider
{
    public class Startup
    {

        public static WebApplication WebApp(params string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<IProductRepository, ProductRepository>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapGet("/api/products", (IProductRepository productRepository) =>
            {
                return Results.Ok(productRepository.List());
            });

            app.MapGet("/api/products/{id}", (int id, IProductRepository productRepository) =>
            {
                var product = productRepository.Get(id);
                if (product == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(product);
            });



            return app;
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        
    }
}
