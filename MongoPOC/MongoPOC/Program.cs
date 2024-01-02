
using AutoMapper;
using MongoPOC.Mapper;
using MongoPOC.Models;
using MongoPOC.Services;
using MongoPOC.Services.Impl;

namespace MongoPOC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
            builder.Services.AddSingleton<IProductService, ProductService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}
