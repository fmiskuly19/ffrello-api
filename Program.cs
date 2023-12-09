using System.Text.Json.Serialization;

namespace FFrelloApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            //this does not work with .net 6.0, looks for Startup.cs which doesnt exist
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(c => { c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()); });

            builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.IgnoreCycles);

            //var MyAllowSpecificOrigins = "_arbitraryPolicyName";

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy(name: MyAllowSpecificOrigins,
            //                      policy =>
            //                      {
            //                          policy.WithOrigins("https://localhost",
            //                                              "https://localhost:8080");
            //                      });
            //});

            var app = builder.Build();

            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:5173"));

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.UseSwagger();
                app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}