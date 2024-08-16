using provider.Repositories;

namespace Provider
{
    public class Startup(IConfiguration configuration)
    {

        public static WebApplication WebApp(params string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
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

            app.MapGet("/api/products", static (IProductRepository productRepository) =>
            {
                return Results.Ok(productRepository.List());
            });

            app.MapGet("/api/products/{id}", static (int id, IProductRepository productRepository) =>
            {
                var result = productRepository.Get(id)
                    .Map(p => Results.Ok(p))
                    .ValueOr(() => Results.NotFound());

                return result;
            });
            return app;
        }

        public IConfiguration Configuration { get; } = configuration;
        // This method gets called by the runtime. Use this method to add services to the container.
    }
}
