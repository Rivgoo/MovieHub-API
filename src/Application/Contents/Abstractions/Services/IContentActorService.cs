using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Contents.Abstractions.Services;

public interface IContentActorService : IEntityService<ContentActor, int>
{
}