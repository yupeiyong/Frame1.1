using System.Data.Entity.ModelConfiguration;
using Models.Base;


namespace DataAccess.Mapping
{

    public class BaseEntityMapping : EntityTypeConfiguration<BaseEntity>
    {

        public BaseEntityMapping()
        {
            Property(p => p.RowVersion).IsRowVersion();
        }

    }

}