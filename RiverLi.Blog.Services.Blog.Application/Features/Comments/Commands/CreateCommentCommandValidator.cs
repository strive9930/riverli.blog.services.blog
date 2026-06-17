using FluentValidation;

namespace RiverLi.Blog.Services.Blog.Application.Features.Comments.Commands;

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.ArticleId).NotEmpty().WithMessage("文章ID不能为空");
        RuleFor(x => x.Content).NotEmpty().WithMessage("评论内容不能为空").MaximumLength(1000).WithMessage("评论不能超过1000字");
    }
}
