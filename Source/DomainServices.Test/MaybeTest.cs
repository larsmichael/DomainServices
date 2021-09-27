namespace DomainServices.Test
{
    using System;
    using System.Linq;
    using Xunit;

    public class MaybeTest
    {
        [Fact]
        public void CreateWithNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Maybe<int?>(null));
        }

        [Fact]
        public void HasValueIsOk()
        {
            var maybe = new Maybe<int>(5);
            Assert.True(maybe.HasValue);
            Assert.Equal(5, maybe.Value);
        }

        [Fact]
        public void DoesNotHaveValueIsOk()
        {
            var maybe = new Maybe<int>();
            Assert.False(maybe.HasValue);
        }

        [Fact]
        [Obsolete]
        public void SingleIsOk()
        {
            var maybe = new Maybe<int>(5);
            Assert.True(maybe.HasValue);
            Assert.Equal(5, maybe.Single());
        }

        [Fact]
        public void ToMaybeIsOk()
        {
            var maybe = 5.ToMaybe();
            Assert.True(maybe.HasValue);
            Assert.Equal(5, maybe.Value);
            var dMaybe = 5.5.ToMaybe();
            Assert.Equal(5.5, dMaybe.Value);
        }

        [Fact]
        public void EmptyIsOk()
        {
            Assert.False(Maybe.Empty<int>().HasValue);
            Assert.False(Maybe.Empty<double>().HasValue);
        }

        [Fact]
        public void PipeOperatorIsOk()
        {
            var emptyDouble = Maybe.Empty<double>();
            Assert.Equal(5.5, emptyDouble | 5.5);
            Assert.Equal(55.0, emptyDouble | 5.5 * 10);
            Assert.Equal(5.5, emptyDouble | new[] { 3.3, 4.4, 5.5 }.Max());
            Assert.Equal(55, emptyDouble | (() => 5.5 * 10));

            var emptyBoolean = Maybe.Empty<bool>();
            Assert.True(emptyBoolean | 7 > 5);
            Assert.False(emptyBoolean | 5 > 7);

            var maybeDouble = new Maybe<double>(9.9);
            Assert.Equal(9.9, maybeDouble | 5.5);

            var maybeBoolean = new Maybe<bool>(true);
            Assert.True(maybeBoolean | false);
        }
    }
}