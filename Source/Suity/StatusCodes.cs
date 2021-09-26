// Copyright (c) Suity by HuangWei(4477289@qq.com)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
namespace Suity
{
    /// <summary>
    /// 状态码
    /// </summary>
    public enum StatusCodes
    {
        None = 0,

        /// <summary>
        /// 服务器内部错误
        /// </summary>
        ServerInternalError = 1,
        /// <summary>
        /// 服务器没有响应请求
        /// </summary>
        ServerNoResponse = 2,
        /// <summary>
        /// 服务器请求未实现
        /// </summary>
        ServerNotImplemented = 3,
        /// <summary>
        /// 未知的请求
        /// </summary>
        UnknownRequest = 4,
        /// <summary>
        /// 不合法的请求
        /// </summary>
        BadRequest = 5,
        /// <summary>
        /// 服务暂时不可用
        /// </summary>
        ServiceUnavailable = 6,

        /// <summary>
        /// 连接已关闭
        /// </summary>
        ConnectionClosed = 11,
        /// <summary>
        /// 客户端错误
        /// </summary>
        ClientError = 12,
        /// <summary>
        /// 频道非空闲
        /// </summary>
        ChannelNotIdle = 13,

        /// <summary>
        /// 账号或密码错误
        /// </summary>
        InvalidUserNameOrPassword = 21,
        /// <summary>
        /// 未登录
        /// </summary>
        NotLoggedIn = 22,
        /// <summary>
        /// 权限不足
        /// </summary>
        NoPermission = 23,
        /// <summary>
        /// 只允许网络组用户
        /// </summary>
        NetworkGroupUserOnly = 24,

        /// <summary>
        /// 协议包类型不匹配
        /// </summary>
        InvalidCast = 31,
        /// <summary>
        /// 签名不匹配
        /// </summary>
        InvalidSignature = 32,

        /// <summary>
        /// 满人
        /// </summary>
        Full = 41,
        /// <summary>
        /// 超时
        /// </summary>
        TimeOut = 42,
        /// <summary>
        /// 资源没有找到
        /// </summary>
        ResourceNotFound = 43,

        /// <summary>
        /// 数据没有找到
        /// </summary>
        DataNotFound = 51,

        /// <summary>
        /// 数据已存在
        /// </summary>
        DataExists = 52,

        /// <summary>
        /// 数据操作失败
        /// </summary>
        DataOperationFailed = 53,
    }
}
