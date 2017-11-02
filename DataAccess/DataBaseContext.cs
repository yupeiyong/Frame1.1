using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Reflection;
using DataAccess.Migrations;
using Models.Base;


namespace DataAccess
{

    public class DataBaseContext : DbContext
    {

        public DataBaseContext(string conn)
        {
            Database.Connection.ConnectionString = conn;
        }


        public DataBaseContext() : base("name=DbConnectionString")
        {
            //自动升级到最新版本
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataBaseContext, Configuration>());
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.Types<BaseEntity>().Configure(entity => entity.Property(p => p.RowVersion).IsRowVersion());

                //获取数据模型
                var modelTypes = AppDomain.CurrentDomain.Load("Models").GetTypes()
                    .Where(t => t.IsClass && t.GetInterfaces()
                        .Any(m => m.GetGenericTypeDefinition() == typeof (IEntity<>))).ToList();

                //modelBuilder.Configurations.Add(new UserMapping());
                //获取映射模型
                var mapTypes = Assembly.GetExecutingAssembly().GetTypes()
                    .Where(type => !string.IsNullOrEmpty(type.Namespace))
                    .Where(type => type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof (EntityTypeConfiguration<>)).ToList();
                mapTypes.ForEach(t =>
                {
                    dynamic instance = Activator.CreateInstance(t);
                    if (instance != null)
                        modelBuilder.Configurations.Add(instance);
                });

                modelTypes.ForEach(modelBuilder.RegisterEntityType);

                //modelBuilder.Configurations.Add(new UserMapping());
                //var configTypes = Assembly.GetExecutingAssembly().GetTypes()
                //    .Where(type => !string.IsNullOrEmpty(type.Namespace))
                //    .Where(type => type.BaseType != null && type.BaseType.BaseType != null && type.BaseType.BaseType.IsGenericType && type.BaseType.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>)).ToList();
                //foreach (var type in configTypes)
                //{
                //    dynamic configurationInstance = Activator.CreateInstance(type);
                //    modelBuilder.Configurations.Add(new UserMapping());
                //    modelBuilder.Configurations.Add(configurationInstance);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            base.OnModelCreating(modelBuilder);
        }

    }

}