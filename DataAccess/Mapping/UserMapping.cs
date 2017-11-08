using System.Data.Entity.ModelConfiguration;
using Peiyong.Models.Entities;


namespace Peiyong.DataAccess.Mapping
{

    public class UserMapping : EntityTypeConfiguration<User>
    {

        public UserMapping()
        {
            Property(p => p.RowVersion).IsRowVersion();
            Property(p => p.Name).HasMaxLength(20).IsRequired();
        }

    }

}