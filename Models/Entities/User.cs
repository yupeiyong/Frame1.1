using System;
using System.ComponentModel;
using Models.Base;
using Models.Enums;


namespace Models.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : BaseEntity, IEntity<long>
    {
        /// <summary>
        ///     序号
        /// </summary>
        [Description("序号")]
        public long Id { get; set; }

        [Description("姓名")]
        public string Name { get; set; }

        /// <summary>
        ///     帐号
        /// </summary>
        [Description("帐号")]
        public string AccountName { get; set; }


        /// <summary>
        ///     登录密码
        /// </summary>
        [Description("登录密码")]
        public string Password { get; set; }

        public DateTime LoginTime { get; set; }

        public string LoginIp { get; set; }

        public int LoginCount { get; set; }

        public string Tel { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Qq { get; set; }

        public string Sex { get; set; }

        /// <summary>
        ///     用户状态
        /// </summary>
        public UserState UserState { get; set; }


        /// <summary>
        ///     是否支持多地登录
        /// </summary>
        public bool IsMultiUser { get; set; }


        public DateTime? Birthday { get; set; }


        /// <summary>
        ///     用户头像
        /// </summary>
        public string PhotoImg { get; set; }

        /*
	[Branch_Id] [int] NOT NULL,
	[Branch_Code] [nvarchar](20) NOT NULL,
	[Branch_Name] [nvarchar](25) NOT NULL,
	[Position_Id] [nvarchar](50) NOT NULL,
	[Position_Name] [nvarchar](100) NOT NULL,

	[CName] [nvarchar](20) NOT NULL,
	[EName] [nvarchar](50) NOT NULL,
	[PhotoImg] [nvarchar](250) NOT NULL,
	[NativePlace] [nvarchar](100) NOT NULL,
	[NationalName] [nvarchar](50) NOT NULL,
	[Record] [nvarchar](25) NOT NULL,
	[GraduateCollege] [nvarchar](30) NOT NULL,
	[GraduateSpecialty] [nvarchar](50) NOT NULL,

	[Address] [nvarchar](100) NOT NULL,
	[Content] [ntext] NOT NULL,
	[Manager_Id] [int] NOT NULL,
	[Manager_CName] [nvarchar](20) NOT NULL,
         */

    }

}