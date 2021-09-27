namespace DomainServices.Test
{
    using System;
    using Newtonsoft.Json.Linq;
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

        [Fact]
        public void ObjectArrayToDataTableIsOk()
        {
            var array = new object[,]
            {
                {"DateTimes", "Values"},
                {DateTime.Now, 99.9},
                {DateTime.Now.AddDays(-1), 888}
            };

            var dataTable = array.ToDataTable();

            Assert.Equal(2, dataTable.Columns.Count);
            Assert.Equal(3, dataTable.Rows.Count);
        }

        [Theory]
        [InlineData("{foo:1,bar:2,baz:3}", "foo", "{foo:1}")]
        [InlineData("{foo:1,bar:2,baz:3}", "foo,bar", "{foo:1,bar:2}")]
        [InlineData("{foo:1,bar:[\"baz\",\"cux\"]}", "foo", "{foo:1}")]
        [InlineData("{foo:1,bar:[\"baz\",\"cux\"]}", "bar", "{bar:[\"baz\",\"cux\"]}")]
        [InlineData("{foo:{bar:1},baz:2}", "foo", "{foo:{bar:1}}")]
        [InlineData("{foo:{bar:[1,2]},baz:3}", "foo.bar", "{foo:{bar:[1,2]}}")]
        public void JObjectFilterIsOk(
            string input,
            string filter,
            string expected)
        {
            var data = JObject.Parse(input);
            var actual = data.Filter(filter.Split(','));

            Assert.Equal(JObject.Parse(expected), actual);
        }
    }
}