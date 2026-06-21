using System;
using System.Threading.Tasks;

namespace RiverLi.Blog.Services.Blog.Application.Interfaces;

/// <summary>
/// 邮件发送服务抽象
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
}
