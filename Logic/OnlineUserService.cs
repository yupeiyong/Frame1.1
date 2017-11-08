using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using DotNet.Utilities;
using Peiyong.DataAccess;
using Peiyong.DataTransferObjects;
using Peiyong.Logic.Application;
using Peiyong.Models.Entities;
using Peiyong.Models.Enums;
using Peiyong.Models.Table;


namespace Peiyong.Logic
{

    public class OnlineUserService
    {
        private const string const_CacheKey = "Cache_OnlineUsers";
        private const string const_CacheKey_Date = "Cache_OnlineUsers_Date";

        public OnlineUser Login(UserLoginDto dto)
        {
            //当前时间
            var localTime = DateTime.Now.ToLocalTime();
            var ip = IpHelper.GetUserIp();

            var request = HttpContext.Current.Request;
            //创建客户端信息获取实体
            var clientHelper = new ClientHelper(request);

            User user;
            using (var dao = new DataBaseContext())
            {
                user = dao.Set<User>().Single(u => u.AccountName == dto.AccountName);
                if (user == null)
                    throw new Exception($"登录失败，用户不存在！（帐号：{dto.AccountName}）");

                //密码不匹配
                if (!user.Password.Equals(Encrypt.Md5(Encrypt.Md5(dto.Password))))
                    throw new Exception("登录失败，密码错误！");

                //已注销用户不能登录
                if (user.UserState == UserState.Cancelled)
                    throw new Exception("登录失败，您已经没有权限登录本系统！");

                //判断当前账号是否被启用
                if (user.UserState == UserState.Disable)
                    throw new Exception("登录失败，当前账号未被启用，请联系管理人员激活！");

                //检查用户权限

                //检查用户是否允许多点登录

                //创建在线用户
                var onlineUser = new OnlineUser();
                //当前用户的Id编号
                onlineUser.UserId = user.Id;
                onlineUser.AccountName = user.AccountName;
                onlineUser.Password = user.Password;
                onlineUser.Name = user.Name;
                onlineUser.LoginTime = localTime;
                onlineUser.LoginIp = ip;
                //生成密钥
                onlineUser.UserKey = RandomHelper.GetRndNum(32, true);
                //Md5(密钥+登陆帐号+密码+IP+密钥.Substring(6,8))
                onlineUser.Md5 = GenerateMd5(onlineUser);
                HttpContext.Current.Session[OnlineUsersTable.Md5] = onlineUser.Md5;
                onlineUser.UpdateTime = localTime;
                onlineUser.Sex = user.Sex;
                //onlineUser.Branch_Id = user.Branch_Id;
                //onlineUser.Branch_Code = user.Branch_Code;
                //onlineUser.Branch_Name = user.Branch_Name;
                //onlineUser.Position_Id = user.Position_Id;
                //onlineUser.Position_Name = user.Position_Name;
                //onlineUser.CurrentPage = "";
                //onlineUser.CurrentPageTitle = "";
                //SessionId
                onlineUser.SessionId = HttpContext.Current.Session.SessionID;
                onlineUser.UserAgent = StringHelper.FilterSql(HttpContext.Current.Request.Headers["User-Agent"] + "");
                onlineUser.OpeartingSystem = clientHelper.GetSystem();
                onlineUser.TerminalType = clientHelper.IsMobileDevice(onlineUser.UserAgent) ? 1 : 0;
                onlineUser.BrowserName = clientHelper.GetBrowserName();
                onlineUser.BrowserVersion = clientHelper.GetBrowserVersion();


                #region 记录当前用户UserId
                //定义HashTable表里Key的名称UserId
                string userHashKey = "";
                //判断当前用户帐户是否支持同一帐号在不同地方登陆功能，取得用户在HashTable表里Key的名称
                //不支持则
                if (user.IsMultiUser)
                {
                    userHashKey = user.Id + "";
                }
                //支持则
                else
                {
                    userHashKey = user.Id + "_" + onlineUser.SessionId;
                }
                //记录用户的HashTable Key
                onlineUser.UserHashKey = userHashKey;
                HttpContext.Current.Session[OnlineUsersTable.UserHashKey] = userHashKey;
                #endregion

                #region 将在线用户信息存入全局变量中
                //运行在线数据加载函数，如果缓存不存在，则尝试加载数据库中的在线表记录到缓存中
                //——主要用于IIS缓存被应用程序池或其他原因回收后，对在线数据进行重新加载，而不会使所有用户都被迫退出系统
                var onlineUsersList = GetList();

                //判断缓存中["OnlineUsers"]是否存在，不存在则直接将在线实体添加到缓存中
                if (onlineUsersList == null || onlineUsersList.Count == 0)
                {
                    //清除在线表里与当前用户同名的记录
                    Delete(x => x.AccountName == onlineUser.AccountName);

                    //将在线实体保存到数据库的在线表中
                    Save(onlineUser, null, true, false);
                }
                //存在则将它取出HashTable并进行处理
                else
                {
                    //将HashTable里存储的前一登陆帐户移除
                    //获取在线缓存实体
                    var onlineModel = GetOnlineUser(userHashKey);
                    if (onlineModel != null)
                    {
                        ////添加用户下线记录
                        //LoginLogBll.GetInstence().Save(userHashKey, "用户【{0}】的账号已经在另一处登录，本次登陆下线！在线时间【{1}】");

                        //清除在线表里与当前用户同名的记录
                        Delete(x => x.UserId == onlineUser.UserId);
                    }

                    //将在线实体保存到数据库的在线表中
                    Save(onlineUser, null, true, false);
                }
                #endregion
                //检查在线列表数据，将不在线人员删除
                CheckOnline();

                //不在线用户全部下线

                //保存当前在线用户

                user.LoginIp = ip;
                user.LoginCount = ++user.LoginCount;
                user.LoginTime = localTime;
                var entity = dao.Entry(user);
                entity.State = EntityState.Modified;
                dao.SaveChanges();
            }

            return null;
        }

        /// <summary>
        /// 根据字段名称，获取当前用户在线表中的内容
        /// </summary>
        /// <returns></returns>
        public OnlineUser GetOnlineUser(string userHashKey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userHashKey))
                {
                    userHashKey = GetUserHashKey();
                }

                //如果不存在在线表则退出
                if (string.IsNullOrEmpty(userHashKey))
                    return null;

                //返回指定字段的内容
                var model = GetModelForCache(x => x.UserHashKey == userHashKey);

                return model;
            }
            catch (Exception e)
            {
                //记录出错日志
                CommonBll.WriteLog("", e);
            }

            return null;
        }
        #region 从IIS缓存中获取指定条件的记录
        /// <summary>
        /// 从IIS缓存中获取指定条件的记录
        /// </summary>
        /// <param name="expression">条件</param>
        /// <returns>DataAccess.Model.OnlineUsers</returns>
        public OnlineUser GetModelForCache(Expression<Func<OnlineUser, bool>> expression)
        {
            //从缓存中读取记录列表
            var list = GetList();
            //如果条件为空，则查询全表所有记录
            if (expression == null)
            {
                //查找并返回记录实体
                if (list == null || list.Count == 0)
                {
                    return null;
                }
                else
                {
                    return list.First();
                }
            }
            else
            {
                //查找并返回记录实体
                if (list == null || list.Count == 0)
                {
                    return null;
                }
                else
                {
                    //先进行条件筛选，得出的数据，再取第一个
                    var tmp = list.AsQueryable().Where(expression);
                    if (tmp.Any())
                    {
                        return tmp.First();
                    }

                    return null;
                }
            }
        }
        #endregion

        #region 获取用户UserHashKey
        /// <summary>
        /// 获取用户UserHashKey
        /// </summary>
        /// <returns></returns>
        public string GetUserHashKey()
        {
            //读取Session中存储的UserHashKey值
            var userHashKey = SessionHelper.GetSession(OnlineUsersTable.UserHashKey);
            //如果为null
            if (userHashKey == null)
            {
                //为null则表示用户Session过期了，所以要检查用户登陆，避免用户权限问题
                //IsTimeOut();
            }
            return SessionHelper.GetSessionString(OnlineUsersTable.UserHashKey);
        }
        #endregion
        #region 检查在线列表
        /// <summary>
        /// 检查在线列表，将不在线人员删除
        /// </summary>
        public void CheckOnline()
        {
            //获取在线列表
            var onlineUsers = GetList();

            //如果不存在在线表则退出
            if (onlineUsers == null || onlineUsers.Count == 0)
                return;

            //循环读取在线信息
            foreach (var model in onlineUsers)
            {
                //判断该用户最后更新时间是否已经有10分钟未更新，是的话则不将其添加到缓存中
                if (TimeHelper.DateDiff("n", model.UpdateTime, DateTime.Now) > 10)
                {
                    ////添加用户下线记录
                    //LoginLogBll.GetInstence().Save(model.UserHashKey, "用户【{0}】退出系统！在线时间【{1}】");
                    //移除在线数据
                    Delete(model.Id);
                }
            }
        }
        #endregion
        #region 添加与编辑OnlineUsers表记录
        /// <summary>
        /// 添加与编辑OnlineUsers记录
        /// </summary>
        /// <param name="model">OnlineUsers表实体</param>
        /// <param name="content">更新说明</param>
        /// <param name="isCache">是否更新缓存</param>
        /// <param name="isAddUseLog">是否添加用户操作日志</param>
        public void Save(OnlineUser model, string content = null, bool isCache = true, bool isAddUseLog = true)
        {
            try
            {
                using (var dao = new DataBaseContext())
                {

                    var onlineUser = dao.Set<OnlineUser>().FirstOrDefault(o => o.Id == model.Id);
                    if (onlineUser != null)
                    {
                        dao.Entry(onlineUser).State = EntityState.Modified;
                    }
                    else
                    {
                        dao.Set<OnlineUser>().Add(model);
                    }
                    dao.SaveChanges();

                    //判断是否启用缓存
                    if (CommonBll.IsUseCache() && isCache)
                    {
                        SetModelForCache(model);
                    }

                    if (isAddUseLog)
                    {
                        if (string.IsNullOrEmpty(content))
                        {
                            content = "{0}" + (model.Id == 0 ? "添加" : "编辑") + "OnlineUsers记录成功，ID为【" + model.Id + "】";
                        }

                        ////添加用户访问记录
                        //UseLogBll.GetInstence().Save(page, content);
                    }

                }

            }
            catch (Exception e)
            {
                var result = "执行OnlineUsersBll.Save()函数出错！";

                //出现异常，保存出错日志信息
                CommonBll.WriteLog(result, e);
            }
        }
        #endregion
        #region 更新IIS缓存中指定Id记录
        /// <summary>
        /// 更新IIS缓存中指定Id记录
        /// </summary>
        /// <param name="model">记录实体</param>
        public void SetModelForCache(OnlineUser model)
        {
            if (model == null) return;

            //从缓存中删除记录
            DelCache(model.Id);

            //从缓存中读取记录列表
            var list = GetList();
            if (list == null)
            {
                list = new List<OnlineUser>();
            }
            //添加记录
            list.Add(model);
        }

        ///// <summary>
        ///// 更新IIS缓存中指定Id记录
        ///// </summary>
        ///// <param name="model">记录实体</param>
        //public void SetModelForCache(OnlineUser model)
        //{
        //    SetModelForCache(Transform(model));
        //}
        #endregion
        #region 清空缓存
        /// <summary>清空缓存</summary>
        private void DelAllCache()
        {
            //清除模板缓存
            CacheHelper.RemoveOneCache(const_CacheKey);
            CacheHelper.RemoveOneCache(const_CacheKey_Date);

            //清除前台缓存
            CommonBll.RemoveCache(const_CacheKey);
            //运行自定义缓存清理程序
            //DelCache();
        }
        #endregion


        /// <summary>
        /// 删除OnlineUsers表记录
        /// </summary>
        /// <param name="id">记录的主键值</param>
        /// <param name="isAddUseLog">是否添加用户操作日志</param>
        public void Delete(long id, bool isAddUseLog = true)
        {
            using (var dao = new DataBaseContext())
            {

                var onlineUser = dao.Set<OnlineUser>().FirstOrDefault(o => o.Id == id);
                if (onlineUser != null)
                {
                    dao.Entry(onlineUser).State = EntityState.Deleted;
                    dao.SaveChanges();
                }


                //判断是否启用缓存
                if (CommonBll.IsUseCache())
                {
                    //删除缓存
                    DelCache(id);
                }

                if (isAddUseLog)
                {
                    //添加用户操作记录
                    //UseLogBll.GetInstence().Save(page, "{0}删除了OnlineUsers表id为【" + id + "】的记录！");
                }

            }
        }

        /// <summary>
        /// 删除OnlineUsers表记录——如果使用了缓存，删除成功后会清空本表的所有缓存记录，然后重新加载进缓存
        /// </summary>
        /// <param name="expression">条件语句</param>
        /// <param name="isAddUseLog">是否添加用户操作日志</param>
        public void Delete(Expression<Func<OnlineUser, bool>> expression, bool isAddUseLog = true)
        {

            //执行删除
            using (var dao = new DataBaseContext())
            {

                var onlineUser = dao.Set<OnlineUser>().FirstOrDefault(expression);
                if (onlineUser != null)
                {
                    dao.Entry(onlineUser).State = EntityState.Deleted;
                    dao.SaveChanges();
                }


                //判断是否启用缓存
                if (CommonBll.IsUseCache())
                {
                    //删除缓存
                    if (onlineUser != null) DelCache(onlineUser.Id);
                }

                if (isAddUseLog)
                {
                    //添加用户操作记录
                    //UseLogBll.GetInstence().Save(page, "{0}删除了OnlineUsers表id为【" + id + "】的记录！");
                }

            }
            //判断是否启用缓存
            if (CommonBll.IsUseCache())
            {
                //清空当前表所有缓存记录
                DelAllCache();
                //重新载入缓存
                GetList();
            }

            if (isAddUseLog)
            {
                //添加用户操作记录
                //UseLogBll.GetInstence().Save(page, "{0}删除了OnlineUsers表记录！");
            }
        }
        #region 删除IIS缓存中指定Id记录
        /// <summary>
        /// 删除IIS缓存中指定Id记录
        /// </summary>
        /// <param name="id">主键Id</param>
        public bool DelCache(long id)
        {
            //从缓存中获取List
            var list = GetList(true);
            if (list == null || list.Count == 0)
            {
                return false;
            }
            else
            {
                //找到指定主键Id的实体
                var model = list.SingleOrDefault(x => x.Id == id);
                //删除指定Id的记录
                return model != null && list.Remove(model);
            }
        }

        /// <summary>
        /// 批量删除IIS缓存中指定Id记录
        /// </summary>
        /// <param name="ids">主键Id</param>
        public void DelCache(IEnumerable ids)
        {
            //循环删除指定Id队列
            foreach (var id in ids)
            {
                DelCache((int)id);
            }
        }

        /// <summary>
        /// 按条件删除IIS缓存中OnlineUsers表的指定记录
        /// </summary>
        /// <param name="expression">条件，值为null时删除全有记录</param>
        public void DelCache(Expression<Func<OnlineUser, bool>> expression)
        {
            //从缓存中获取List
            var list = GetList();
            //如果缓存为null，则不做任何处理
            if (list == null || list.Count == 0)
            {
                return;
            }

            //如果条件为空，则删除全部记录
            if (expression == null)
            {
                //删除所有记录
                DelAllCache();
            }
            else
            {
                var tem = list.AsQueryable().Where(expression);
                foreach (var model in tem)
                {
                    list.Remove(model);
                }
            }
        }
        #endregion

        #region 从IIS缓存中获取OnlineUsers表记录
        /// <summary>
        /// 从IIS缓存中获取OnlineUsers表记录
        /// </summary>
        /// <param name="isCache">是否从缓存中读取</param>
        public IList<OnlineUser> GetList(bool isCache = true)
        {
            try
            {
                //判断是否使用缓存
                if (CommonBll.IsUseCache() && isCache)
                {
                    //检查指定缓存是否过期——缓存当天有效，第二天自动清空
                    if (CommonBll.CheckCacheIsExpired(const_CacheKey_Date))
                    {
                        //删除缓存
                        DelAllCache();
                    }

                    //从缓存中获取DataTable
                    var obj = CacheHelper.GetCache(const_CacheKey);
                    //如果缓存为null，则查询数据库
                    if (obj == null)
                    {
                        var list = GetList(false);

                        //将查询出来的数据存储到缓存中
                        CacheHelper.SetCache(const_CacheKey, list);
                        //存储当前时间
                        CacheHelper.SetCache(const_CacheKey_Date, DateTime.Now);

                        return list;
                    }
                    //缓存中存在数据，则直接返回
                    else
                    {
                        return (IList<OnlineUser>)obj;
                    }
                }
                else
                {
                    using (var dao = new DataBaseContext())
                    {
                        var onlineUsers = dao.Set<OnlineUser>().ToList();
                        return onlineUsers;
                    }
                }
            }
            catch (Exception e)
            {
                //记录日志
                CommonBll.WriteLog("从IIS缓存中获取OnlineUsers表记录时出现异常", e);
            }

            return null;
        }
        #endregion

        #region 生成加密串——用户加密密钥计算
        /// <summary>
        /// 生成加密串——用户加密密钥计算
        /// </summary>
        /// <param name="model">在线实体</param>
        /// <returns></returns>
        public string GenerateMd5(OnlineUser model)
        {
            if (model == null)
            {
                return RandomHelper.GetRndKey();
            }
            else
            {
                //Md5(密钥+登陆帐号+密码+IP+密钥.Substring(6,8))
                //return Encrypt.Md5(model.UserKey + model.Manager_LoginName + model.Manager_LoginPass +
                //            IpHelper.GetUserIp() + model.UserKey.Substring(6, 8));
                return Encrypt.Md5(model.UserKey + model.AccountName + model.Password + model.UserKey.Substring(6, 8));
            }
        }

        ///// <summary>
        ///// 生成加密串——用户加密密钥计算
        ///// </summary>
        ///// <param name="model">在线实体</param>
        ///// <returns></returns>
        //public string GenerateMd5(OnlineUser model)
        //{
        //    if (model == null)
        //    {
        //        return RandomHelper.GetRndKey();
        //    }
        //    else
        //    {
        //        return Encrypt.Md5(model.UserKey + model.Manager_LoginName + model.Manager_LoginPass + model.UserKey.Substring(6, 8));
        //    }
        //}

        ///// <summary>
        ///// 生成加密串——用户加密密钥计算，直接读取当前用户实体
        ///// </summary>
        ///// <returns></returns>
        //public string GenerateMd5()
        //{
        //    //读取当前用户实体
        //    var model = GetOnlineUsersModel();
        //    return GenerateMd5(model);
        //}
        #endregion
    }

}