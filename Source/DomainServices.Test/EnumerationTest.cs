namespace DomainServices.Test
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Xunit;

    public class EnumerationTest
    {
        [Fact]
        public void FromInvalidValueThrows()
        {
            Assert.Throws<ApplicationException>(() => Enumeration.FromValue<SomeType>(99));
        }

        [Fact]
        public void FromInvalidDisplayNameThrows()
        {
            Assert.Throws<ApplicationException>(() => Enumeration.FromDisplayName<SomeType>("InvalidDisplayName"));
        }

        [Fact]
        public void FromValueIsOk()
        {
            Assert.Equal(SomeType.Type0, Enumeration.FromValue<SomeType>(0));
            Assert.Equal(SomeType.Type1, Enumeration.FromValue<SomeType>(1));
            Assert.Equal(SomeType.Type2, Enumeration.FromValue<SomeType>(2));
        }

        [Fact]
        public void FromDisplayNameIsOk()
        {
            Assert.Equal(SomeType.Type0, Enumeration.FromDisplayName<SomeType>("Type 0"));
            Assert.Equal(SomeType.Type1, Enumeration.FromDisplayName<SomeType>("Type 1"));
            Assert.Equal(SomeType.Type2, Enumeration.FromDisplayName<SomeType>("Type 2"));
        }

        [Fact]
        public void GetAllIsOk()
        {
            var types = Enumeration.GetAll<SomeType>();
            foreach (var type in types)
            {
                Assert.Equal(typeof(SomeType), type.GetType());
            }
        }

        [Fact]
        public void EqualsIsOk()
        {
            Assert.Equal(SomeType.Type0, SomeType.Type0);
        }

        [Fact]
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        public void NotEqualsIsOk()
        {
            Assert.NotEqual(SomeType.Type1, SomeType.Type0);
            Assert.NotEqual<object>(1, SomeType.Type0);
            Assert.False(SomeType.Type0.Equals(1));
        }

        [Fact]
        public void ToStringIsOk()
        {
            Assert.Equal("Type 0", SomeType.Type0.ToString());
            Assert.Equal("Type 1", SomeType.Type1.ToString());
        }

        private class SomeType : Enumeration
        {
            public static readonly SomeType Type0 = new SomeType(0, "Type 0");

            public static readonly SomeType Type1 = new SomeType(1, "Type 1");

            public static readonly SomeType Type2 = new SomeType(2, "Type 2");

            private SomeType(int value, string displayName)
                : base(value, displayName)
            {
            }
        }
    }
}