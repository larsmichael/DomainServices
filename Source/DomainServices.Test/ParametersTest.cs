namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using AutoFixture.Xunit2;
    using Xunit;

    public class ParametersTest
    {
        [Fact]
        public void ParseNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => Parameters.Parse(null!));
        }

        [Fact]
        public void ParseEmptyStringThrows()
        {
            Assert.Throws<ArgumentException>(() => Parameters.Parse(""));
        }

        [Theory]
        [InlineData("foobar")]
        [InlineData("foo:2;bar:3")]
        public void ParseIllegalStringThrows(string s)
        {
            var e = Assert.Throws<ArgumentException>(() => Parameters.Parse(s));
            Assert.Contains("Could not parse parameters string", e.Message);
        }

        [Theory, AutoData]
        public void GetNonExistingParameterThrows(Parameters parameters)
        {
            var e = Assert.Throws<KeyNotFoundException>(() => parameters.GetParameter("NonExistingParameter"));
            Assert.Contains("not found.", e.Message);
        }

        [Theory, AutoData]
        public void TryGetNonExistingParameterFails(Parameters parameters)
        {
            var success = parameters.TryGetParameter("NonExistingParameter", out string value);
            Assert.False(success);
            Assert.Null(value);
        }

        [Theory, AutoData]
        public void TryGetParameterOfWrongTypeFails(Parameters parameters)
        {
            parameters.Add("SomeInt", "999");
            var success = parameters.TryGetParameter("SomeInt", out DateTime? value);
            Assert.False(success);
            Assert.Null(value);
        }

        [Theory, AutoData]
        public void TryGetParameterOfWrongTypeReturnsDefault(Parameters parameters)
        {
            parameters.Add("SomeInt", "999");
            var success = parameters.TryGetParameter("SomeInt", out DateTime value);
            Assert.False(success);
            Assert.Equal(default, value);
        }

        [Theory, AutoData]
        public void GetIntIsOk(Parameters parameters)
        {
            parameters.Add("SomeInt", "999");
            Assert.Equal(999, parameters.GetParameter("SomeInt", 888));
        }

        [Theory, AutoData]
        public void GetDefaultIntIsOk(Parameters parameters)
        {
            Assert.Equal(888, parameters.GetParameter("SomeNonExistingInt", 888));
        }

        [Theory, AutoData]
        public void GetIntUnsafeIsOk(Parameters parameters)
        {
            parameters.Add("SomeInt", "999");
            Assert.Equal(999, parameters.GetParameter("SomeInt"));
        }

        [Theory, AutoData]
        public void TryGetIntIsOk(Parameters parameters)
        {
            parameters.Add("SomeInt", "999");
            var success = parameters.TryGetParameter("SomeInt", out int? value);
            Assert.True(success);
            Assert.Equal(999, value);
        }

        [Theory, AutoData]
        public void GetDoubleIsOk(Parameters parameters)
        {
            parameters.Add("SomeDouble", "999.99");
            Assert.Equal(999.99, parameters.GetParameter("SomeDouble", 888.88));
        }

        [Theory, AutoData]
        public void GetDefaultDoubleIsOk(Parameters parameters)
        {
            Assert.Equal(888.88, parameters.GetParameter("SomeNonExistingDouble", 888.88));
        }

        [Theory, AutoData]
        public void GetDoubleUnsafeIsOk(Parameters parameters)
        {
            parameters.Add("SomeDouble", "999.99");
            Assert.Equal(999.99, parameters.GetParameter("SomeDouble"));
        }

        [Theory, AutoData]
        public void TryGetDoubleIsOk(Parameters parameters)
        {
            parameters.Add("SomeDouble", "999.99");
            var success = parameters.TryGetParameter("SomeDouble", out double? value);
            Assert.True(success);
            Assert.Equal(999.99, value);
        }

        [Theory, AutoData]
        public void GetDateTimeIsOk(Parameters parameters)
        {
            parameters.Add("SomeDate", "2015-01-01");
            Assert.Equal(new DateTime(2015, 1, 1), parameters.GetParameter("SomeDate", DateTime.MinValue));
        }

        [Theory, AutoData]
        public void GetDefaultDateTimeIsOk(Parameters parameters)
        {
            Assert.Equal(DateTime.MinValue, parameters.GetParameter("SomeDate", DateTime.MinValue));
        }

        [Theory, AutoData]
        public void GetDateTimeUnsafeIsOk(Parameters parameters)
        {
            parameters.Add("SomeDate", "2015-01-01");
            Assert.Equal(new DateTime(2015, 1, 1), parameters.GetParameter("SomeDate"));
        }

        [Theory, AutoData]
        public void TryGetDateTimeIsOk(Parameters parameters)
        {
            parameters.Add("SomeDate", "2015-01-01");
            var success = parameters.TryGetParameter("SomeDate", out DateTime? value);
            Assert.True(success);
            Assert.Equal(new DateTime(2015, 1, 1), value);
        }

        [Theory, AutoData]
        public void GetTimeSpanIsOk(Parameters parameters)
        {
            parameters.Add("SomeTimeSpan", "6:00:00");
            Assert.Equal(TimeSpan.FromHours(6), parameters.GetParameter("SomeTimeSpan", TimeSpan.MinValue));
        }

        [Theory, AutoData]
        public void GetDefaultTimeSpanIsOk(Parameters parameters)
        {
            Assert.Equal(TimeSpan.MinValue, parameters.GetParameter("SomeTimeSpan", TimeSpan.MinValue));
        }

        [Theory, AutoData]
        public void GetTimeSpanUnsafeIsOk(Parameters parameters)
        {
            parameters.Add("SomeTimeSpan", "6:00:00");
            Assert.Equal(TimeSpan.FromHours(6), parameters.GetParameter("SomeTimeSpan"));
        }

        [Theory, AutoData]
        public void TryGetTimeSpanIsOk(Parameters parameters)
        {
            parameters.Add("SomeTimeSpan", "6:00:00");
            var success = parameters.TryGetParameter("SomeTimeSpan", out TimeSpan? value);
            Assert.True(success);
            Assert.Equal(TimeSpan.FromHours(6), value);
        }

        [Theory, AutoData]
        public void GetBoolIsOk(Parameters parameters)
        {
            parameters.Add("SomeBool", "true");
            Assert.True(parameters.GetParameter("SomeBool", false));
        }

        [Theory, AutoData]
        public void GetBoolDefaultIsOk(Parameters parameters)
        {
            Assert.False(parameters.GetParameter("SomeBool", false));
        }

        [Theory, AutoData]
        public void GetBoolUnsafeIsOk(Parameters parameters)
        {
            parameters.Add("SomeBool", "true");
            Assert.True((bool)parameters.GetParameter("SomeBool"));
        }

        [Theory, AutoData]
        public void TryGetBoolIsOk(Parameters parameters)
        {
            parameters.Add("SomeBool", "true");
            var success = parameters.TryGetParameter("SomeBool", out bool? value);
            Assert.True(success);
            Assert.True(value);
        }

        [Theory, AutoData]
        public void GetGuidIsOk(Parameters parameters)
        {
            parameters.Add("SomeGuid", "37fbabf5-ff11-4d42-90ad-ff83c1488a69");
            Assert.Equal(new Guid("37fbabf5-ff11-4d42-90ad-ff83c1488a69"), parameters.GetParameter("SomeGuid", default(Guid)));
        }

        [Theory, AutoData]
        public void GetGuidDefaultIsOk(Parameters parameters)
        {
            Assert.Equal(default, parameters.GetParameter("SomeGuid", default(Guid)));
        }

        [Theory, AutoData]
        public void GetGuidUnsafeIsOk(Parameters parameters)
        {
            parameters.Add("SomeGuid", "37fbabf5-ff11-4d42-90ad-ff83c1488a69");
            Assert.Equal(new Guid("37fbabf5-ff11-4d42-90ad-ff83c1488a69"), parameters.GetParameter("SomeGuid"));
        }

        [Theory, AutoData]
        public void TryGetGuidIsOk(Parameters parameters)
        {
            parameters.Add("SomeGuid", "37fbabf5-ff11-4d42-90ad-ff83c1488a69");
            var success = parameters.TryGetParameter("SomeGuid", out Guid? value);
            Assert.True(success);
            Assert.Equal(new Guid("37fbabf5-ff11-4d42-90ad-ff83c1488a69"), value);
        }

        [Theory, AutoData]
        public void GetStringIsOk(Parameters parameters)
        {
            parameters.Add("SomeString", "MyString");
            Assert.Equal("MyString", parameters.GetParameter("SomeString", "Default"));
        }

        [Theory, AutoData]
        public void GetStringDefaultIsOk(Parameters parameters)
        {
            Assert.Equal("Default", parameters.GetParameter("SomeString", "Default"));
        }

        [Theory, AutoData]
        public void GetStringUnsafeIsOk(Parameters parameters)
        {
            parameters.Add("SomeString", "MyString");
            Assert.Equal("MyString", parameters.GetParameter("SomeString"));
        }

        [Theory, AutoData]
        public void TryGetStringIsOk(Parameters parameters)
        {
            parameters.Add("SomeString", "MyString");
            var success = parameters.TryGetParameter("SomeString", out string value);
            Assert.True(success);
            Assert.Equal("MyString", value);
        }

        [Theory, AutoData]
        public void ToStringIsOk(Parameters parameters)
        {
            parameters.Clear();
            parameters.Add("SomeString", "MyString");
            parameters.Add("SomeBool", "true");
            Assert.Equal("SomeBool=true;SomeString=MyString", parameters.ToString());
        }

        [Fact]
        public void ParseIsOk()
        {
            var parameters = Parameters.Parse("SomeBool=true;SomeString=MyString;SomeDate=2016-02-03T22:30:00;SomeDouble=9.99;SomeInt=99;SomeGuid=37fbabf5-ff11-4d42-90ad-ff83c1488a69");
            Assert.True(parameters.GetParameter("SomeBool", false));
            Assert.Equal("MyString", parameters.GetParameter("SomeString", "Default"));
            Assert.Equal(new DateTime(2016, 2, 3, 22, 30, 00), parameters.GetParameter("SomeDate", DateTime.MinValue));
            Assert.Equal(9.99, parameters.GetParameter("SomeDouble", 1.0));
            Assert.Equal(99, parameters.GetParameter("SomeInt", 1));
            Assert.Equal(new Guid("37fbabf5-ff11-4d42-90ad-ff83c1488a69"), parameters.GetParameter("SomeGuid", default(Guid)));
        }
    }
}