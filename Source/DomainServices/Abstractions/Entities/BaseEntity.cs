﻿namespace DomainServices.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security.Claims;
    using Ardalis.GuardClauses;
    using Authorization;

    /// <summary>
    ///     Abstract base class for an entity
    /// </summary>
    /// <typeparam name="TId">The type of the entity identifier.</typeparam>
    [Serializable]
    public abstract class BaseEntity<TId> : ISecuredEntity<TId>, ICloneable
    {
        private readonly Dictionary<object, object> _metadata;
        private readonly List<Permission> _permissions;
        private TId _id;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseEntity{TId}" /> class.
        /// </summary>
        protected BaseEntity()
        {
            _metadata = new Dictionary<object, object>();
            _permissions = new List<Permission>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseEntity{TId}" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException">id</exception>
        protected BaseEntity(TId id) : this()
        {
            Guard.Against.Null(id, nameof(id));
            _id = id;
        }

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        public object Clone()
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
            stream.Position = 0;
            return formatter.Deserialize(stream);
        }

        /// <summary>
        ///     Gets the permissions.
        /// </summary>
        public virtual IList<Permission> Permissions => _permissions;

        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public virtual TId Id
        {
            get => _id;

            protected set => _id = value;
        }

        /// <summary>
        ///     Gets the datetime the entity was added to the repository.
        /// </summary>
        public virtual DateTime? Added { get; set; }

        /// <summary>
        ///     Gets the most recent time the entity was updated in the repository.
        /// </summary>
        public virtual DateTime? Updated { get; set; }

        /// <summary>
        ///     Gets the metadata.
        /// </summary>
        public virtual IDictionary<object, object> Metadata => _metadata;

        /// <summary>
        ///     Determines whether the specified principals are allowed to perform the specified operation.
        /// </summary>
        /// <param name="principals">The principals.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        ///     <c>true</c> if the specified principals are allowed to perform the specified operation; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool IsAllowed(HashSet<string> principals, string operation)
        {
            if (_permissions.Any(p => p.Principals.Intersect(principals).Any() &&
                                      p.Operation == operation &&
                                      p.Type == PermissionType.Denied))
            {
                return false;
            }

            return _permissions.Any(p => p.Principals.Intersect(principals).Any() &&
                                         p.Operation == operation &&
                                         p.Type == PermissionType.Allowed);
        }

        /// <summary>
        ///     Determines whether the specified principals are allowed to perform the specified operation.
        /// </summary>
        /// <param name="principals">The principals.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        ///     <c>true</c> if the specified principals are allowed to perform the specified operation; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool IsAllowed(IEnumerable<string> principals, string operation)
        {
            return IsAllowed(new HashSet<string>(principals), operation);
        }

        /// <summary>
        ///     Determines whether the specified user is allowed to perform the specified operation                        .
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="operation">The operation.</param>
        /// <returns><c>true</c> if the specified user is allowed to perform the specified operation; otherwise, <c>false</c>.</returns>
        public bool IsAllowed(ClaimsPrincipal user, string operation)
        {
            return IsAllowed(user.GetPrincipals(), operation);
        }

        /// <summary>
        ///     Adds a permission.
        /// </summary>
        /// <param name="principals">The principals.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="permissionType">Type of the permission.</param>
        public void AddPermission(IEnumerable<string> principals, string operation, PermissionType permissionType = PermissionType.Allowed)
        {
            _permissions.Add(new Permission(principals, operation, permissionType));
        }

        /// <summary>
        ///     Adds multiple permissions.
        /// </summary>
        /// <param name="principals">The principals.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="permissionType">Type of the permission.</param>
        public void AddPermissions(IEnumerable<string> principals, IEnumerable<string> operations, PermissionType permissionType = PermissionType.Allowed)
        {
            principals = principals.ToArray();
            foreach (var operation in operations)
            {
                _permissions.Add(new Permission(principals, operation, permissionType));
            }
        }

        /// <summary>
        ///     Determines whether the Metadata property should be serialized
        /// </summary>
        public bool ShouldSerializeMetadata()
        {
            return _metadata.Count > 0;
        }

        /// <summary>
        ///     Determines whether the Permissions property should be serialized
        /// </summary>
        public bool ShouldSerializePermissions()
        {
            return _permissions.Count > 0;
        }
    }
}