namespace DomainServices.Test
{
    using System;
    using System.Collections;
    using Xunit;

    public class QueryConditionTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("foo")]
        [InlineData("foo,bar")]
        [InlineData(1.234)]
        public void CreateWithIllegalValueThrows(object value)
        {
            var e = Assert.Throws<ArgumentException>(() => new QueryCondition("foo", QueryOperator.Any, value));
            Assert.Contains("is not a collection", e.Message);
        }

        [Fact]
        public void CreateIsOk()
        {
            var condition = new QueryCondition("foo", QueryOperator.Any, new [] {"foo", "bar"});
            Assert.IsAssignableFrom<ICollection>(condition.Value);

            condition = new QueryCondition("foo", QueryOperator.Any, new[] { 1.234, 5.678 });
            Assert.IsAssignableFrom<ICollection>(condition.Value);
        }

        [Fact]
        public void ToStringIsOk()
        {
            var condition = new QueryCondition("foo", QueryOperator.Any, new[] { "foo", "bar" });
            Assert.Contains("Any", condition.ToString());
        }

        [Fact]
        public void Int64IsTreatedAsInt32()
        {
            var condition = new QueryCondition("foo", 99L);
            Assert.IsType<int>(condition.Value);
        }
    }
}
