using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiverLi.Blog.Services.Blog.Application.Interfaces;

namespace RiverLi.Blog.Services.Blog.Infrastructure.Storage;

/// <summary>
/// SMTP 邮件服务实现
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string htmlBody)
    {
        var host = _configuration["Email:Host"] ?? "smtp.qq.com";
        var port = int.Parse(_configuration["Email:Port"] ?? "465");
        var userName = _configuration["Email:UserName"];
        var password = _configuration["Email:Password"];

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
        {
            _logger.LogWarning("邮件服务未配置，跳过发送：{Subject}", subject);
            return;
        }

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(userName, password)
        };

        var message = new MailMessage
        {
            From = new MailAddress(userName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        message.To.Add(to);

        await client.SendMailAsync(message);
        _logger.LogInformation("邮件已发送：{Subject} -> {To}", subject, to);
    }
}
