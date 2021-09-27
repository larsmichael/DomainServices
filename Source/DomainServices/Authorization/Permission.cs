﻿namespace DomainServices.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    [Serializable]
    public struct Permission
    {
        [JsonConstructor]
        public Permission(HashSet<string> principals, string operation, PermissionType permissionType = PermissionType.Allowed)
        {
            if (principals is null || !principals.Any())
            {
                throw new ArgumentException("Principals cannot be null or empty.", nameof(principals));
            }

            Principals = principals;
            Operation = operation?.Trim().ToLower() ?? throw new ArgumentNullException(nameof(operation));
            Type = permissionType;
        }

        public Permission(IEnumerable<string> principals, string operation, PermissionType permissionType = PermissionType.Allowed)
            : this(new HashSet<string>(principals), operation, permissionType)
        {
        }

        public HashSet<string> Principals { get; }

        public string Operation { get; }

        public PermissionType Type { get; }

        public override string ToString()
        {
            return $"[{string.Join(", ", Principals)}] are {Type.ToString().ToLower()} to {Operation}.";
        }
    }
}