using backend.dotnet.DataObjects;
using Microsoft.Azure.Mobile.Server.Tables;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace backend.dotnet.Models
{
    public class MyDbContext : DbContext
    {
        private const string connectionStringName = "Name=MS_TableConnectionString";

        public MyDbContext() : base(connectionStringName)
        {
            this.Database.Log = this.Write;
        }

        public void Write(object m)
        {
            System.Diagnostics.Debug.Write(m);
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn",
                    (property, attributes) => attributes.Single().ColumnType.ToString()
                )
            );
        }
    }
}
