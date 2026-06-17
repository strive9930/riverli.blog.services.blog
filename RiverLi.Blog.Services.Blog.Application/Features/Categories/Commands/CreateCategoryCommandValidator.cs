using FluentValidation;

namespace RiverLi.Blog.Services.Blog.Application.Features.Categories.Commands;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("分类名称不能为空").MaximumLength(50).WithMessage("名称不能超过50字");
        RuleFor(x => x.Slug).NotEmpty().WithMessage("分类标识不能为空").MaximumLength(100).WithMessage("标识不能超过100字");
    }
}
