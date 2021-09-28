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
            var now = DateTime.Now;
            var query = new Query<Account>
            {
                new QueryCondition("Activated", QueryOperator.Equal, true),
                new QueryCondition("TokenExpiration", QueryOperator.GreaterThan, now)
            };

            Assert.Equal("(Activated = ?) AND (TokenExpiration > ?)", query.ToCommandString());
        }
    }
}