namespace DomainServices
{
    using System;
    using Ardalis.GuardClauses;

    /// <summary>
    ///     Struct representing the full name of an entity. A full name is a combination of a group and an entity name.
    /// </summary>
    public readonly struct FullName
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FullName" /> struct.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentException">Cannot be null or empty.;name</exception>
        public FullName(string group, string name) : this()
        {
            Guard.Against.NullOrEmpty(name, nameof(name));
            Group = group;
            Name = name;
        }

        /// <summary>
        ///     Gets the group-part of the FullName.
        /// </summary>
        /// <value>The group.</value>
        public string Group { get; }

        /// <summary>
        ///     Gets the name-part of the FullName.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Group) ? Name : $"{Group}/{Name}";
        }

        /// <summary>
        ///     Parses the specified full name string. A full name string has the format {group}/{name}.
        /// </summary>
        /// <param name="s">The full name string.</param>
        /// <returns>A FullName object.</returns>
        /// <exception cref="ArgumentException">Cannot be null or empty.;s</exception>
        public static FullName Parse(string s)
        {
            Guard.Against.NullOrEmpty(s, nameof(s));
            string group = null;
            var name = s;
            var i = s.LastIndexOf('/');
            if (i > 0)
            {
                group = s.Substring(0, i);
                name = s.Substring(i + 1);
            }

            return new FullName(group, name);
        }
    }
}