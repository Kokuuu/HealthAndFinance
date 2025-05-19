using HealthAndFinance.BL;

namespace HealthAndFinance.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<DapperContext>(provider =>
                new DapperContext(builder.Configuration));

            // Register services
            builder.Services.AddScoped<PostService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<FriendShipService>();
            builder.Services.AddScoped<LikeService>();
            builder.Services.AddScoped<UserRankService>();
            builder.Services.AddScoped<QuestionService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}