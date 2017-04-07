using System.Data.Entity.ModelConfiguration;
using Models.Entities;


namespace DataAccess.Mapping
{

    public class UserMapping : BaseDomainMapping<User>
    {


        public override void Init()
        {
            this.Property(l => l.Name).HasMaxLength(200).IsRequired();//设置Name属性长度为200 并且是必填
        }

    }

}