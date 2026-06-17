using FluentValidation;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public class ChangeArticleStatusCommandValidator : AbstractValidator<ChangeArticleStatusCommand>
{
    public ChangeArticleStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum().WithMessage("无效的文章状态");
    }
}
