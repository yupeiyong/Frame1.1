using System;
using System.Linq;
using DataAccess;
using DataTransferObjects;
using DotNet.Utilities;
using Models.Entities;
using Models.Enums;


namespace Logic
{

    /// <summary>
    ///     用户服务类
    /// </summary>
    public class UserService
    {

        /// <summary>
        ///     登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public OnlineUser Login(UserLoginDto dto)
        {
            #region 用户验证
            //if (!SecurityCodeService.IsValid(dto.Token, dto.SecurityCode))
            //    throw new Exception("错误：图形验证码错误！");

            if (string.IsNullOrEmpty(dto.AccountName))
                throw new Exception("登录失败：请输入您的账号！");

            if (string.IsNullOrEmpty(dto.Password))
                throw new Exception("登录失败：请输入您的密码！");

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
            }

            #endregion

            #region 更新用户登陆信息


            #endregion

            return null;
        }


        /// <summary>
        ///     注销登录
        /// </summary>
        /// <param name="onlineUser"></param>
        public void Logout(OnlineUser onlineUser)
        {
        }

    }

}