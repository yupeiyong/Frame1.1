using System;
using System.Linq;
using DataAccess;
using DataTransferObjects;
using DotNet.Utilities;
using Models.Entities;


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
            //用户名验证
            if (string.IsNullOrEmpty(dto.AccountName))
            {
                throw new Exception("登录失败，用户帐号为空！");
            }

            //密码验证
            if (string.IsNullOrEmpty(dto.Password))
            {
                throw new Exception("登录失败，用户密码为空！");
            }

            //验证码验证
            if (string.IsNullOrEmpty(dto.VerificationCode))
            {
                throw new Exception("登录失败，验证码为空！");
            }

            User user;
            using (var dao = new DataBaseContext())
            {
                user=dao.Set<User>().Single(u => u.AccountName == dto.AccountName);
                if(user==null)
                    throw new Exception($"登录失败，用户不存在！（帐号：{dto.AccountName}）");

                ////判断用户是否存在
                //if (userinfo == null)
                //{
                //    LoginLogBll.GetInstence().Save(0, "账号【" + username + "】不存在，登录失败！");
                //    txtUserName.Focus();
                //    FineUI.Alert.ShowInParent("用户名不存在，请仔细检查您输入的用户名！", FineUI.MessageBoxIcon.Error);
                //    return;
                //}

                //密码不匹配
                if (!user.Password.Equals(Encrypt.Md5(Encrypt.Md5(dto.Password))))
                {
                    //LoginLogBll.GetInstence().Save(userinfo.Id, "账号【" + userinfo.LoginName + "】的用户【" + userinfo.CName + "】登录失败！登录密码错误。");
                    //txtPassword.Focus();
                    //FineUI.Alert.ShowInParent("您输入的用户密码错误！", FineUI.MessageBoxIcon.Error);
                    //return;
                }
            }
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