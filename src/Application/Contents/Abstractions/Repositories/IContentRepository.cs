using Application.Abstractions.Repositories;
using Application.Contents.Dtos;
using Domain.Entities;

namespace Application.Contents.Abstractions;

public interface IContentRepository : IEntityOperations<Content, int>
{
	Task<ICollection<ContentDto>> GetAllContentDtosAsync(CancellationToken cancellationToken);
	Task<ContentDto?> GetContentDtoAsync(int contentId, CancellationToken cancellationToken);
}