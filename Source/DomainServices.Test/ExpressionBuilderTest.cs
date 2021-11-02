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
            Assert.Throws<ArgumentNullException>(() => ExpressionBuilder.Build<FakeEntity>(null!));
        }

        [Fact]
        public void FilterEmptyThrows()
        {
            Assert.Throws<ArgumentException>(() => ExpressionBuilder.Build<FakeEntity>(new List<QueryCondition>()));
        }

        [Fact]
        public void NonExistingPropertyThrows()
        {
            var filter = new List<QueryCondition> { new("NonExistingProperty", QueryOperator.Equal, "value") };

            Assert.Throws<ArgumentException>(() => ExpressionBuilder.Build<FakeEntity>(filter));
        }

        [Fact]
        public void IllegalValueTypeThrows()
        {
            var filter = new List<QueryCondition> { new("Bar", QueryOperator.Equal, 99) };

            Assert.Throws<InvalidOperationException>(() => ExpressionBuilder.Build<FakeEntity>(filter));
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
            var filter = new List<QueryCondition> { new("Name", queryOperator, "John Doe") };

            Assert.Throws<NotImplementedException>(() => ExpressionBuilder.Build<FakeEntity>(filter));
        }

        [Fact]
        public void BuildIsOk()
        {
            var filter = new List<QueryCondition>
            {
                new("Id", QueryOperator.Equal, "john.doe"),
                new("Foo", QueryOperator.Equal, true),
                new("Bar", QueryOperator.GreaterThan, DateTime.Now)
            };

            var expression = ExpressionBuilder.Build<FakeEntity>(filter);

            Assert.Equal(typeof(Func<FakeEntity, bool>), expression.Type);
        }
    }
}