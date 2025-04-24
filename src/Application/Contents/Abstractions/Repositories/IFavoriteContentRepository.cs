using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Contents.Abstractions.Repositories;
public interface IFavoriteContentRepository : IEntityOperations<FavoriteContent, int>
{
}