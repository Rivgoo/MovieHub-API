using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Application.Contents.Abstractions;
public interface IFavoriteContentRepository : IEntityOperations<FavoriteContent, int>
{
}