using System;


namespace Peiyong.Models.Base
{

    /// <summary>
    ///     基础实体模型
    /// </summary>
    public class BaseEntity
    {

        /// <summary>
        ///     行版本
        /// </summary>
        public byte[] RowVersion { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? LastUpdateTime { get; set; }

    }

}