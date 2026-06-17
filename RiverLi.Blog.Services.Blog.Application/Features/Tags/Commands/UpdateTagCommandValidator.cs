using FluentValidation;

namespace RiverLi.Blog.Services.Blog.Application.Features.Tags.Commands;

public class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().WithMessage("标签名称不能为空").MaximumLength(50).WithMessage("名称不能超过50字");
        RuleFor(x => x.Slug).NotEmpty().WithMessage("标签标识不能为空").MaximumLength(100).WithMessage("标识不能超过100字");
    }
}
