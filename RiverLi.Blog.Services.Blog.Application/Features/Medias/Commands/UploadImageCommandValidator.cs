using FluentValidation;

namespace RiverLi.Blog.Services.Blog.Application.Features.Medias.Commands;

public class UploadImageCommandValidator : AbstractValidator<UploadImageCommand>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };

    public UploadImageCommandValidator()
    {
        RuleFor(x => x.Extension)
            .NotEmpty().WithMessage("文件扩展名不能为空")
            .Must(ext => System.Array.IndexOf(AllowedExtensions, ext?.ToLowerInvariant()) >= 0)
            .WithMessage($"不支持的图片格式，仅支持 {string.Join(", ", AllowedExtensions)}");
    }
}
