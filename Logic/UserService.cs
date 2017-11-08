using System;
using Peiyong.DataTransferObjects;
using Peiyong.Models.Entities;


namespace Peiyong.Logic
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

            #endregion

            #region 登录为在线用户
            var onlineUserService=new OnlineUserService();
            return onlineUserService.Login(dto);

            #endregion

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