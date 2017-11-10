namespace Peiyong.DataTransferObjects
{

    public class UserLoginDto
    {

        /// <summary>
        ///     帐号
        /// </summary>
        public string AccountName { get; set; }


        /// <summary>
        ///     登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     验证码
        /// </summary>
        public string VerificationCode { get; set; }

    }

}