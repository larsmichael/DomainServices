namespace DomainServices.Abstractions
{
    using System;
    using System.IO;
    using Ardalis.GuardClauses;

    /// <summary>
    /// Abstract base class for an entity ID in a file-based grouped repository.
    /// </summary>
    public abstract class BaseGroupedFileEntityId
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGroupedFileEntityId"/> class.
        /// </summary>
        /// <param name="relativeFilePath">The relative file path.</param>
        /// <exception cref="ArgumentNullException">relativeFilePath</exception>
        protected BaseGroupedFileEntityId(string relativeFilePath)
        {
            Guard.Against.NullOrEmpty(relativeFilePath, nameof(relativeFilePath));
            RelativeFilePath = relativeFilePath;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGroupedFileEntityId"/> class.
        /// </summary>
        /// <param name="relativeFilePath">The relative file path.</param>
        /// <param name="objId">The object identifier.</param>
        /// <exception cref="ArgumentNullException">relativeFilePath</exception>
        protected BaseGroupedFileEntityId(string relativeFilePath, string objId)
        {
            Guard.Against.NullOrEmpty(relativeFilePath, nameof(relativeFilePath));
            RelativeFilePath = relativeFilePath;
            ObjId = objId;
        }

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName => Path.GetFileName(RelativeFilePath);

        /// <summary>
        /// Gets the relative file path.
        /// </summary>
        /// <value>The relative file path.</value>
        public string RelativeFilePath { get; }

        /// <summary>
        /// Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        public string FullName => new FullName(Group, Name).ToString();

        /// <summary>
        /// Gets the group.
        /// </summary>
        /// <value>The group.</value>
        public string Group => Path.GetDirectoryName(RelativeFilePath).Replace('\\', '/');

        /// <summary>
        /// Gets the object identifier.
        /// </summary>
        /// <value>The object identifier.</value>
        public string ObjId { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name => string.IsNullOrEmpty(ObjId) ? FileName : $"{FileName};{ObjId}";

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is BaseGroupedFileEntityId other))
            {
                return base.Equals(obj);
            }

            return Equals(FullName, other.FullName);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return FullName;
        }
    }
}