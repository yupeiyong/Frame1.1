using System.ComponentModel;
using Models.Base;


namespace Models.Entities
{

    public class User : IBaseModel<long>
    {
        /// <summary>
        ///     序号
        /// </summary>
        [Description("序号")]
        public long Id { get; set; }

        [Description("姓名")]
        public string Name { get; set; }

        //[Description("年龄")]
        //public byte Age { get; set; }

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


        /*
         	[LoginName] [nvarchar](20) NOT NULL,
	[LoginPass] [nvarchar](32) NOT NULL,
	[LoginTime] [datetime] NOT NULL,
	[LoginIp] [nvarchar](30) NOT NULL,
	[LoginCount] [int] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NOT NULL,
	[IsMultiUser] [tinyint] NOT NULL,
	[Branch_Id] [int] NOT NULL,
	[Branch_Code] [nvarchar](20) NOT NULL,
	[Branch_Name] [nvarchar](25) NOT NULL,
	[Position_Id] [nvarchar](50) NOT NULL,
	[Position_Name] [nvarchar](100) NOT NULL,
	[IsWork] [tinyint] NOT NULL,
	[IsEnable] [tinyint] NOT NULL,
	[CName] [nvarchar](20) NOT NULL,
	[EName] [nvarchar](50) NOT NULL,
	[PhotoImg] [nvarchar](250) NOT NULL,
	[Sex] [nvarchar](4) NOT NULL,
	[Birthday] [nvarchar](20) NOT NULL,
	[NativePlace] [nvarchar](100) NOT NULL,
	[NationalName] [nvarchar](50) NOT NULL,
	[Record] [nvarchar](25) NOT NULL,
	[GraduateCollege] [nvarchar](30) NOT NULL,
	[GraduateSpecialty] [nvarchar](50) NOT NULL,
	[Tel] [nvarchar](30) NOT NULL,
	[Mobile] [nvarchar](30) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Qq] [nvarchar](30) NOT NULL,
	[Msn] [nvarchar](30) NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
	[Content] [ntext] NOT NULL,
	[Manager_Id] [int] NOT NULL,
	[Manager_CName] [nvarchar](20) NOT NULL,
         */

    }

}