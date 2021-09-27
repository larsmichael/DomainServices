namespace DomainServices.Abstractions
{
    using System.IO;
    using System.Security.Claims;

    /// <summary>
    ///     Interface for a streamable entity
    /// </summary>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public interface IStreamableService<in TEntityId>
    {
        (Stream, string fileType, string fileName) GetStream(TEntityId id, ClaimsPrincipal user = null);
    }
}