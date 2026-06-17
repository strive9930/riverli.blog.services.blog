using FluentValidation;

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

public class UpdateArticleCommandValidator : AbstractValidator<UpdateArticleCommand>
{
    public UpdateArticleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().WithMessage("文章标题不能为空").MaximumLength(200).WithMessage("标题不能超过200字");
        RuleFor(x => x.Content).NotEmpty().WithMessage("文章正文不能为空");
        RuleFor(x => x.Summary).NotEmpty().WithMessage("文章摘要不能为空").MaximumLength(500).WithMessage("摘要不能超过500字");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("请选择分类");
    }
}
