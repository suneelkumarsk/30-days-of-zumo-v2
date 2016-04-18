using Owin;
using System.Data.Entity;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.Mobile.Server.Tables.Config;
using backend.dotnet.Models;

namespace backend.dotnet
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Configure the Azure Mobile Apps section
            new MobileAppConfiguration()
                .AddTables(
                    new MobileAppTableConfiguration()
                        .MapTableControllers()
                        .AddEntityFramework())
                .MapApiControllers()
                .ApplyTo(config);

            // Initialize the database with EF Code First
            Database.SetInitializer(new AzureMobileInitializer());

            // Link the Web API into the configuration
            app.UseWebApi(config);
        }
    }

    public class AzureMobileInitializer : CreateDatabaseIfNotExists<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            // You can seed your database here
            base.Seed(context);
        }
    }
}
