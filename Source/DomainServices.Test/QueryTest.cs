namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    public class QueryTest
    {
        [Fact]
        public void CreateWithNullOrEmptyQueryThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Query<Account>(default(QueryCondition)!));
            Assert.Throws<ArgumentNullException>(() => new Query<Account>(default(IEnumerable<QueryCondition>)!));
            Assert.Throws<ArgumentException>(() => new Query<Account>(new List<QueryCondition>()));
        }

        [Fact]
        public void CreateWithIllegalValueTypeThrows()
        {
            var e = Assert.Throws<Exception>(() => new Query<Account>
            {
                new("Name", QueryOperator.Like, "John"),
                new("Activated", "true")
            });

            Assert.Contains("The value must be assignable to the type", e.Message);
        }

        [Fact]
        public void CreateWithIllegalValueTypeForAnyThrows()
        {
            var e = Assert.Throws<Exception>(() => new Query<Account>
            {
                new("Name", QueryOperator.Any, new [] {1, 2, 3}),
                new("Activated", true)
            });

            Assert.Contains("The value must be assignable to the type", e.Message);
        }

        [Fact]
        public void AddWithIllegalValueTypeThrows()
        {
            var query = new Query<Account>();
            var e = Assert.Throws<Exception>(() => query.Add(new QueryCondition("Activated", "true")));
            Assert.Contains("The value must be assignable to the type", e.Message);
        }

        [Fact]
        public void ToExpressionForEmptyConditionsThrows()
        {
            var query = new Query<FakeEntity>();
            Assert.Throws<ArgumentException>(() => query.ToExpression());
        }

        [Fact]
        public void ToExpressionForNonExistingPropertyThrows()
        {
            var conditions = new List<QueryCondition> { new("NonExistingProperty", QueryOperator.Equal, "value") };
            var query = new Query<FakeEntity>(conditions);

            Assert.Throws<ArgumentException>(() => query.ToExpression());
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
        [InlineData(QueryOperator.SpatiallyTransform)]
        public void ToExpressionForNotSupportedOperatorThrows(QueryOperator queryOperator)
        {
            var conditions = new List<QueryCondition> { new("Name", queryOperator, "John Doe") };
            var query = new Query<Account>(conditions);

            Assert.Throws<NotImplementedException>(() => query.ToExpression());
        }

        [Fact]
        public void ToExpressionIsOk()
        {
            var conditions = new List<QueryCondition>
            {
                new("Id", QueryOperator.Equal, "john.doe"),
                new("Activated", QueryOperator.Equal, true),
                new("Company", QueryOperator.Equal, null!)
            };

            var query = new Query<Account>(conditions);

            Assert.Equal(typeof(Func<Account, bool>), query.ToExpression().Type);
        }

        [Fact]
        public void GetEnumeratorIsOk()
        {
            var query = new Query<Account>
            {
                new("Id", QueryOperator.Equal, "john.doe"),
                new("TokenExpiration", QueryOperator.GreaterThan, DateTime.Now.AddDays(-1))
            };

            Assert.Equal(2, query.Count());
        }

        [Fact]
        public void ToStringIsOk()
        {
            var conditions = new List<QueryCondition>
            {
                new("Id", QueryOperator.Any, new object[] {"john.doe", "donald.duck"}),
                new("TokenExpiration", QueryOperator.GreaterThan, DateTime.Now.AddDays(-1)),
                new("Company", null!)
            };

            var query = new Query<Account>(conditions);

            Assert.IsType<string>(query.ToString());
        }
    }
}