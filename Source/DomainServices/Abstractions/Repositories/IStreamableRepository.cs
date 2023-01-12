namespace DomainServices.Abstractions;

using System.IO;
using System.Security.Claims;

/// <summary>
///     Interface for a streamable repository
/// </summary>
/// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
public interface IStreamableRepository<in TEntityId>
{
    (Maybe<Stream>, string fileType, string fileName) GetStream(TEntityId id, ClaimsPrincipal? user = null);
}