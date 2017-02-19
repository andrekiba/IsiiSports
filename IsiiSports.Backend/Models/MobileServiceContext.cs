using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using IsiiSports.DataObjects;
using Microsoft.Azure.Mobile.Server.Tables;

namespace IsiiSports.Backend.Models
{
    public class MobileServiceContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
        //
        // To enable Entity Framework migrations in the cloud, please ensure that the 
        // service name, set by the 'MS_MobileServiceName' AppSettings in the local 
        // Web.config, is the same as the service name when hosted in Azure.

        private const string ConnectionStringName = "Name=MS_TableConnectionString";

        public MobileServiceContext() : base(ConnectionStringName)
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>().ToTable("Players");
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<Game>().ToTable("Games");
            modelBuilder.Entity<GameField>().ToTable("GameFields");
            modelBuilder.Entity<GameResult>().ToTable("GameResults");
            modelBuilder.Entity<Notification>().ToTable("Notifications");

            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }

        #region DbSet

        public DbSet<Player> Players { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<GameField> GameFields { get; set; }

        public DbSet<GameResult> GameResults { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        #endregion
    }
}
