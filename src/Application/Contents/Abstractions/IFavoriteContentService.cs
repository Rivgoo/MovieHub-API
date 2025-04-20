using Application.Abstractions.Services;
using Domain.Entities;

namespace Application.Contents.Abstractions;

internal interface IFavoriteContentService : IEntityService<FavoriteContent, int>
{
}