namespace DomainServices.Test
{
    using System;
    using Xunit;

    public class FullNameTest
    {
        [Fact]
        public void CreateWithNameNullOrEmptyThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new FullName("group", null));
            Assert.Throws<ArgumentException>(() => new FullName("group", ""));
        }

        [Fact]
        public void ParseNullOrEmptyThrows()
        {
            Assert.Throws<ArgumentNullException>(() => FullName.Parse(null));
            Assert.Throws<ArgumentException>(() => FullName.Parse(""));
        }

        [Theory]
        [InlineData("dir/subdir", "entity", "dir/subdir/entity")]
        [InlineData("group", "entity", "group/entity")]
        [InlineData(null, "entity", "entity")]
        [InlineData("", "entity", "entity")]
        public void ToStringIsOk(string group, string name, string fullName)
        {
            Assert.Equal(fullName, new FullName(group, name).ToString());
        }

        [Theory]
        [InlineData("dir/subdir", "entity", "dir/subdir/entity")]
        [InlineData("group", "entity", "group/entity")]
        [InlineData(null, "entity", "entity")]
        public void ParseIsOk(string group, string name, string fullName)
        {
            Assert.Equal(group, FullName.Parse(fullName).Group);
            Assert.Equal(name, FullName.Parse(fullName).Name);
        }
    }
}