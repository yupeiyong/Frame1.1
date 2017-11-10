using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Base;


namespace DataAccess.Mapping
{

    public abstract class BaseDomainMapping<T> : EntityTypeConfiguration<T>
       where T : class, new()
    {

        public BaseDomainMapping()
        {
            Init();
        }


        /// <summary>
        /// 初始化代码
        /// </summary>
        public virtual void Init()
        {
            
        }
    }
}
