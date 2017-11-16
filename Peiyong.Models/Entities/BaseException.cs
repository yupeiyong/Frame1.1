using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Peiyong.Models.Entities
{

    public class BaseException
    {
        #region 变量

        private readonly Exception outermostException;

        #endregion

        #region 属性
        public string ErrorPageUrl => this.GetExceptionUrl();

        public Exception Exception
        {
            get
            {
                return (HttpContext.Current.Session["Exception"] as Exception);
            }
            private set
            {
                HttpContext.Current.Session["Exception"] = value;
            }
        }
        public string ExceptionMessage { get; private set; }

        public string ExceptionName { get; private set; }

        public string InnerExceptionMessage { get; private set; }

        public string InnerExceptionName { get; private set; }

        public bool IsShowStackInfo { get; private set; }

        public string SourceErrorFile { get; private set; }

        public string SourceErrorRowId { get; private set; }

        public string StackInfo { get; private set; }

        public string TargetSite { get; private set; }

        #endregion


        public BaseException()
        {
            this.outermostException = null;
            this.ExceptionName = null;
            this.ExceptionMessage = null;
            this.InnerExceptionName = null;
            this.InnerExceptionMessage = null;
            this.TargetSite = null;
            this.StackInfo = null;
            this.SourceErrorFile = null;
            this.SourceErrorRowId = null;
            this.IsShowStackInfo = false;
            try
            {
                this.Exception = HttpContext.Current.Application["LastError"] as Exception;
                if (this.Exception != null)
                {
                    this.outermostException = this.Exception;
                    if ((this.Exception is HttpUnhandledException) && (this.Exception.InnerException != null))
                    {
                        this.Exception = this.Exception.InnerException;
                    }
                    this.ExceptionName = this.GetExceptionName(this.Exception);
                    this.ExceptionMessage = this.GetExceptionMessage(this.Exception);
                    if (this.Exception.InnerException != null)
                    {
                        this.InnerExceptionName = this.GetExceptionName(this.Exception.InnerException);
                        this.InnerExceptionMessage = this.GetExceptionMessage(this.Exception.InnerException);
                    }
                    this.TargetSite = this.GetTargetSite(this.Exception);
                    this.StackInfo = this.GetStackInfo(this.Exception);
                    if ((this.outermostException is HttpUnhandledException) && (this.outermostException.InnerException != null))
                    {
                        this.StackInfo = this.StackInfo + "\r\n<a href='#' onclick=\"if(document.getElementById('phidden').style.display=='none') document.getElementById('phidden').style.display='block'; else document.getElementById('phidden').style.display='none'; return false;\"><b>[" + this.outermostException.GetType().ToString() + "]</b></a>\r\n";
                        this.StackInfo = this.StackInfo + "<pre id='phidden' style='display:none;'>" + this.outermostException.StackTrace + "</pre>";
                    }
                    this.SourceErrorFile = this.GetSourceErrorFile();
                    this.SourceErrorRowId = this.GetSourceErrorRowId();
                    this.IsShowStackInfo = true;
                }
                HttpContext.Current.Session["LastError"] = null;
            }
            catch (Exception exception)
            {
                this.ExceptionMessage = "异常基页出错" + exception.Message;
            }
        }

        #region 方法
        private string GetExceptionMessage(Exception ex)
        {
            return ex.Message;
        }

        private string GetExceptionMessageForLog()
        {
            StringBuilder builder = new StringBuilder(50);
            builder.AppendFormat("<ExceptionName>{0}</ExceptionName>", this.ExceptionName);
            builder.AppendFormat("<ExceptionMessage>{0}</ExceptionMessage>", this.ExceptionMessage);
            builder.AppendFormat("<InnerExceptionName>{0}</InnerExceptionName>", this.InnerExceptionName);
            builder.AppendFormat("<InnerExceptionMessage>{0}</InnerExceptionMessage>", this.InnerExceptionMessage);
            builder.AppendFormat("<TargetSite>{0}</TargetSite>", this.TargetSite);
            builder.AppendFormat("<ErrorPageUrl>{0}</ErrorPageUrl>", this.ErrorPageUrl);
            builder.AppendFormat("<SourceErrorFile>{0}</SourceErrorFile>", this.SourceErrorFile);
            builder.AppendFormat("<SourceErrorRowID>{0}</SourceErrorRowID>", this.SourceErrorRowId);
            return builder.ToString();
        }

        private string GetExceptionMessageForMail()
        {
            StringBuilder builder = new StringBuilder(50);
            builder.Append("<ExceptionInfo>");
            builder.Append(this.GetExceptionMessageForLog());
            builder.AppendFormat("<StackInfo><![CDATA[{0}]]></StackInfo>", this.StackInfo);
            builder.Append("</ExceptionInfo>");
            return builder.ToString();
        }

        private string GetExceptionName(Exception ex)
        {
            string str = null;
            if (ex != null)
            {
                str = ex.GetType().FullName;
            }

            return str;
        }

        private string GetExceptionUrl()
        {
            string str = null;
            if (HttpContext.Current.Request["ErrorUrl"] != null)
            {
                str = HttpContext.Current.Request["ErrorUrl"].ToString();
            }
            return str;
        }

        private string GetSourceErrorFile()
        {
            string stackInfo = this.StackInfo;
            string[] strArray = new string[0];
            if (stackInfo == null)
            {
                return stackInfo;
            }
            strArray = stackInfo.Split(new string[] { "位置", "行号" }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length >= 3)
            {
                stackInfo = strArray[1];
                if (stackInfo.LastIndexOf(":") == (stackInfo.Length - 1))
                {
                    stackInfo = stackInfo.Substring(0, stackInfo.Length - 1);
                }
                return stackInfo;
            }
            return "";
        }
        private string GetSourceErrorRowId()
        {
            string stackInfo = this.StackInfo;
            string[] strArray = new string[0];
            if (stackInfo == null)
            {
                return stackInfo;
            }
            strArray = stackInfo.Split(new string[] { "行号" }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length >= 2)
            {
                stackInfo = strArray[1].Trim();
                string[] strArray2 = stackInfo.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                if (strArray2.Length >= 2)
                {
                    stackInfo = strArray2[0];
                }
                return stackInfo;
            }
            return "";
        }
        private string GetStackInfo(Exception ex)
        {
            string str = null;
            if (ex == null) return null;
            str = "<b>[" + ex.GetType().ToString() + "]</b>\r\n" + ex.StackTrace;
            if (ex.InnerException != null)
            {
                str = this.GetStackInfo(ex.InnerException) + "\r\n" + str;
            }
            return str;
        }
        private string GetTargetSite(Exception ex)
        {
            string str = null;
            if (ex == null) return null;
            ex = this.GetBenmostException(ex);
            var site = ex.TargetSite;
            if (site != null)
            {
                str = $"{site.DeclaringType}.{site.Name}";
            }
            return str;
        }
        protected Exception GetBenmostException(Exception ex)
        {
            while (true)
            {
                if (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                else
                {
                    return ex;
                }
            }
        }
        #endregion
    }

}
