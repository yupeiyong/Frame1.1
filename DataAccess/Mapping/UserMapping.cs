using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;


namespace DataAccess.Mapping
{
    public class UserMapping:EntityTypeConfiguration<User>
    {

        public UserMapping()
        {
            this.Property(p => p.Name).HasMaxLength(20).IsRequired();
        }
    }
}
