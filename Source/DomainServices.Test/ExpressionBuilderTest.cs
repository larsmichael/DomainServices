namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    public class ExpressionBuilderTest
    {
        [Fact]
        public void FilterNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => ExpressionBuilder.Build<Account>(null!));
        }

        [Fact]
        public void FilterEmptyThrows()
        {
            Assert.Throws<ArgumentException>(() => ExpressionBuilder.Build<Account>(new List<QueryCondition>()));
        }

        [Fact]
        public void NonExistingPropertyThrows()
        {
            var filter = new List<QueryCondition> { new QueryCondition("NonExistingProperty", QueryOperator.Equal, "value") };

            Assert.Throws<ArgumentException>(() => ExpressionBuilder.Build<Account>(filter));
        }

        [Fact]
        public void IllegalValueTypeThrows()
        {
            var filter = new List<QueryCondition> { new QueryCondition("Email", QueryOperator.Equal, 99) };

            Assert.Throws<InvalidOperationException>(() => ExpressionBuilder.Build<Account>(filter));
        }

        [Theory]
        [InlineData(QueryOperator.Contains)]
        [InlineData(QueryOperator.Intersects)]
        [InlineData(QueryOperator.Like)]
        [InlineData(QueryOperator.NotLike)]
        [InlineData(QueryOperator.SpatiallyContains)]
        [InlineData(QueryOperator.SpatiallyIntersects)]
        [InlineData(QueryOperator.SpatiallyWithin)]
        [InlineData(QueryOperator.SpatiallyWithinDistance)]
        public void NotSupportedOperatorThrows(QueryOperator queryOperator)
        {
            var filter = new List<QueryCondition> { new QueryCondition("Name", queryOperator, "John Doe") };

            Assert.Throws<NotImplementedException>(() => ExpressionBuilder.Build<Account>(filter));
        }

        [Fact]
        public void BuildIsOk()
        {
            var filter = new List<QueryCondition>
            {
                new QueryCondition("Id", QueryOperator.Equal, "john.doe"),
                new QueryCondition("Activated", QueryOperator.Equal, true)
            };

            var expression = ExpressionBuilder.Build<Account>(filter);

            Assert.Equal(typeof(Func<Account, bool>), expression.Type);
        }
    }
}