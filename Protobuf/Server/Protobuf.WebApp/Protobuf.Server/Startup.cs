namespace Protobuf.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
