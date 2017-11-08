using System;
using System.Net;
using DotNet.Utilities;
using DotNet.Utilities.Log;


/***********************************************************************
 *   作    者：AllEmpty（陈焕）-- 1654937@qq.com
 *   博    客：http://www.cnblogs.com/EmptyFS/
 *   技 术 群：327360708
 *  
 *   创建日期：2014-06-17
 *   文件名称：CommonBll.cs
 *   描    述：公共逻辑类
 *             
 *   修 改 人：
 *   修改日期：
 *   修改原因：
 ***********************************************************************/
namespace Peiyong.Logic.Application
{
    /// <summary>
    /// CommonBll公共逻辑类
    /// </summary>
    public class CommonBll
    {
        #region 写log日志

        /// <summary>
        /// 将内容写入日志里
        /// </summary>
        /// <param name="content">要写入日志文件的内容</param>
        /// <param name="ex">异常</param>
        public static void WriteLog(string content, Exception ex = null)
        {
            try
            {
                if (ConfigHelper.GetConfigBool("IsWriteLog"))
                {
                    if (ex == null)
                    {
                        LogHelper.WriteLog(content);
                    }
                    else
                    {
                        LogHelper.WriteLog(content, ex);
                    }
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region 清除前端缓存
        /// <summary>
        /// 清除前台缓存——通过与前端指定的接口提交约定的字串，来执行缓存清理程序，如果前后端在一个站点里，则直接注释本函数即可
        /// </summary>
        /// <param name="cacheName">将要清除的缓存名称，值为AllCache时，表示清除所有缓存</param>
        public static void RemoveCache(string cacheName)
        {
            try
            {
                //获取参数
                var time = DateTime.Now.Ticks;
                var setKey = ConfigHelper.GetConfigString("SetKey");
                //对加密后的验证码与提交的key进行匹配，如果不正确则直接退出
                var checkKey = Encrypt.Md5(cacheName + time + setKey + setKey.Substring(2, 8));

                var url = ConfigHelper.GetConfigString("Site") + "Handles/RemoveCache.ashx?cacheName=" + cacheName + "&time=" + time + "&key=" + checkKey;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Accept = "*/*";
                request.Timeout = 2000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
            }
            catch
            {

            }
        }
        #endregion

        #region 计算在线时间
        /// <summary>
        /// 计算当前用户在线时长
        /// </summary>
        /// <param name="startTime">用户登陆时间</param>
        /// <param name="endTime">用户离线时间</param>
        /// <returns></returns>
        public static string LoginDuration(object startTime, object endTime)
        {
            try
            {
                double minu = TimeHelper.DateDiff("n", TimeHelper.CDate(startTime), TimeHelper.CDate(endTime));
                if (minu < 1.0)
                {
                    return "小于1分钟";
                }
                return minu.ToString("0") + "分钟";
            }
            catch (Exception)
            {
                return "计算异常";
            }
        }
        #endregion

        #region 检查指定缓存是否过期
        /// <summary>
        /// 检查指定缓存是否过期——缓存当天有效，第二天自动清空
        /// </summary>
        /// <param name="constCacheKeyDate">当前缓存日期名称</param>
        /// <returns></returns>
        public static bool CheckCacheIsExpired(string constCacheKeyDate)
        {
            //判断缓存日期是否存在
            if (CacheHelper.GetCache(constCacheKeyDate) == null)
            {
                return false;
            }

            //判断当前日期是否是第二天（即是否过期），如果是的话，则返回true
            if (TimeHelper.DateDiff("d", TimeHelper.CDate(CacheHelper.GetCache(constCacheKeyDate)), DateTime.Now) != 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 是否启用缓存
        /// <summary>
        /// 是否启用缓存
        /// </summary>
        /// <returns></returns>
        public static bool IsUseCache()
        {
            return ConfigHelper.GetConfigBool("IsUseCache");
        }
        #endregion

    }
}
