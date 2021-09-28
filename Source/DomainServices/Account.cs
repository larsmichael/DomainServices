namespace DomainServices
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using Ardalis.GuardClauses;
    using Abstractions;

    /// <summary>
    ///     Class Account.
    /// </summary>
    [Serializable]
    public class Account : BaseNamedEntity<string>
    {
        private byte[]? _encryptedPassword;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Account" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        public Account(string id, string name)
            : base(id, name)
        {
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="Account" /> is activated.
        /// </summary>
        /// <value><c>true</c> if activated; otherwise, <c>false</c>.</value>
        public virtual bool Activated { get; set; }

        /// <summary>
        ///     Gets or sets a token for account activation or password reset.
        /// </summary>
        /// <value>The activation token.</value>
        public virtual string? Token { get; set; }

        /// <summary>
        ///     Gets or sets a token for account activation or password reset.
        /// </summary>
        /// <value>The expiration datetime of the token.</value>
        public virtual DateTime? TokenExpiration { get; set; }

        /// <summary>
        ///     Gets or sets the company.
        /// </summary>
        /// <value>The company.</value>
        public virtual string? Company { get; set; }

        /// <summary>
        ///     Gets or sets the email.
        /// </summary>
        /// <value>The email.</value>
        public virtual string? Email { get; set; }

        /// <summary>
        ///     Gets or sets the encrypted password.
        /// </summary>
        /// <value>The encrypted password.</value>
        public virtual byte[]? EncryptedPassword
        {
            get => _encryptedPassword;
            set => _encryptedPassword = value;
        }

        /// <summary>
        ///     Gets or sets the phone number.
        /// </summary>
        /// <value>The phone number.</value>
        public virtual string? PhoneNumber { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether a user is allowed to change the password of his/her own account.
        /// </summary>
        /// <value>
        ///     <c>true</c> if a user is allowed to change the password his/her own account; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllowMePasswordChange { get; set; } = true;

        /// <summary>
        ///     Gets or sets the roles as a comma-separated string of roles.
        /// </summary>
        /// <value>The roles.</value>
        [Obsolete("You should use user-groups instead of roles for authorization. This property will be removed in a future version.")]
        public virtual string? Roles { get; set; }

        /// <summary>
        ///     Gets the roles as an array of strings.
        /// </summary>
        /// <returns>System.String[].</returns>
        [Obsolete("You should use user-groups instead of roles for authorization. This method will be removed in a future version.")]
        public virtual string[] GetRoles()
        {
            return Roles is null ? Array.Empty<string>() : Roles.Split(',').Select(r => r.Trim()).ToArray();
        }

        /// <summary>
        ///     Sets the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <exception cref="ArgumentNullException">password</exception>
        /// <exception cref="ArgumentException">Password cannot be empty;password</exception>
        public void SetPassword(string password)
        {
            Guard.Against.NullOrEmpty(password, nameof(password));
            _encryptedPassword = HashPasswordStrong(password);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"{Id} ({Name})";
        }

        ///// <summary>
        /////     Hashes the password.
        ///// </summary>
        ///// <param name="password">The password.</param>
        //[Obsolete("You should hash passwords using the HashPasswordStrong() method.")]
        //public static byte[] HashPassword(string password)
        //{
        //    var provider = new SHA1CryptoServiceProvider();
        //    var encoding = new UnicodeEncoding();
        //    return provider.ComputeHash(encoding.GetBytes(password));
        //}

        /// <summary>
        ///     Hashes the password using a salt.
        /// </summary>
        /// <param name="password">The password.</param>
        public static byte[] HashPasswordStrong(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdfs2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdfs2.GetBytes(20);
            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return hashBytes;
        }
    }
}