using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Contents.Abstractions;

public interface IContentService : IEntityService<Content, int>
{
}