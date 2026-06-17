using FluentValidation;

namespace RiverLi.Blog.Services.Blog.Application.Features.Comments.Commands;

public class AuditCommentCommandValidator : AbstractValidator<AuditCommentCommand>
{
    public AuditCommentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum().WithMessage("无效的审核状态");
    }
}
