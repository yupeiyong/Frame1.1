using System.ComponentModel;


namespace Models.Enums
{

    public enum UserState
    {

        /// <summary>
        ///     未启用帐户
        /// </summary>
        [Description("未启用帐户")] Disable = 0,

        /// <summary>
        ///     帐户启用
        /// </summary>
        [Description("帐户启用")] Enable = 1,

        /// <summary>
        ///     帐户已注销
        /// </summary>
        [Description("帐户已注销")] Cancelled = 2

    }

}