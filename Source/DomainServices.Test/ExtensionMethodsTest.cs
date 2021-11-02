namespace DomainServices.Test
{
    using System;
    using Xunit;

    public class ExtensionMethodsTest
    {
        [Theory]
        [InlineData(QueryOperator.Equal, "=")]
        [InlineData(QueryOperator.GreaterThan, ">")]
        [InlineData(QueryOperator.GreaterThanOrEqual, ">=")]
        [InlineData(QueryOperator.LessThan, "<")]
        [InlineData(QueryOperator.LessThanOrEqual, "<=")]
        [InlineData(QueryOperator.Like, "Like")]
        [InlineData(QueryOperator.NotLike, "Not Like")]
        [InlineData(QueryOperator.Contains, "Contains")]
        public void GetQueryOperatorDescriptionIsOk(QueryOperator queryOperator, string description)
        {
            Assert.Equal(description, queryOperator.GetDescription());
        }

        [Fact]
        public void QueryToCommandStringIsOk()
        {
            var query = new Query<FakeEntity>
            {
                new("Foo", QueryOperator.Equal, true),
                new("Bar", QueryOperator.GreaterThan, DateTime.Now)
            };

            Assert.Equal("(Foo = ?) AND (Bar > ?)", query.ToCommandString());
        }
    }
}