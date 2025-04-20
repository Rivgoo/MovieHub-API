using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Contents.Abstractions.Services;

internal interface IFavoriteContentService : IEntityService<FavoriteContent, int>
{
}