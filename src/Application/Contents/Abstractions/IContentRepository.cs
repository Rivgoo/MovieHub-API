using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Contents.Abstractions;

public interface IContentRepository : IEntityOperations<Content, int>
{
}