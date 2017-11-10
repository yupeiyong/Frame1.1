using System;
using Peiyong.Models.Base;


namespace Peiyong.Models.Entities
{

    /// <summary>
    ///     在线用户
    /// </summary>
    public class OnlineUser : IEntity<long>
    {
        public long Id { get; set; }

        public string UserHashKey { get; set; }

        public long UserId { get; set; }

        public string AccountName { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public DateTime LoginTime { get; set; }

        public string LoginIp { get; set; }

        public string UserKey { get; set; }

        public string Md5 { get; set; }

        public DateTime UpdateTime { get; set; }

        public string Sex { get; set; }

        public string SessionId { get; set; }

        public string UserAgent { get; set; }

        public string OpeartingSystem { get; set; }

        public int TerminalType { get; set; }

        public string BrowserName { get; set; }

        public string BrowserVersion { get; set; }


        /*
	[CurrentPage] [nvarchar](100) NOT NULL,
	[CurrentPageTitle] [nvarchar](250) NOT NULL,
	[SessionId] [nvarchar](100) NOT NULL,
	[UserAgent] [nvarchar](1000) NOT NULL,
	[OperatingSystem] [nvarchar](50) NOT NULL,
	[TerminalType] [int] NOT NULL,
	[BrowserName] [nvarchar](50) NOT NULL,
	[BrowserVersion] [nvarchar](10) NOT NULL,
         
         */

    }

}