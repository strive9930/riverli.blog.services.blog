using System;
using System.Collections.Generic;
using MediatR;
using RiverLi.DDD.Core.Application.Common.Models; // �������� Result λ�ڴ˴�

namespace RiverLi.Blog.Services.Blog.Application.Features.Articles.Commands;

/// <summary>
/// �������µ��������� (����������Ϣ��������Ϣ���� Handler ��ͨ����ǰ��¼�� JWT Token �Զ�����)
/// </summary>
/// <param name="Title">���±���</param>
/// <param name="Content">Markdown ��������</param>
/// <param name="Summary">����ժҪ����</param>
/// <param name="CoverUrl">����ͼ URL������Ϊ�գ�</param>
/// <param name="CategoryId">������� ID</param>
/// <param name="TagIds">�����ı�ǩ ID ���ϣ�����Ϊ�գ�</param>
public record CreateArticleCommand(
    string Title,
    string Content,
    string Summary,
    string? CoverUrl,
    Guid CategoryId,
    List<Guid>? TagIds
) : IRequest<Result<Guid>>; // �涨��д����ִ����Ϻ��� API �㷵�������µ� Guid