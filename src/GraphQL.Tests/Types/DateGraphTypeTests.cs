using System;
using System.Globalization;
using GraphQL.Types;
using Shouldly;
using Xunit;

namespace GraphQL.Tests.Types
{
    public class DateGraphTypeTests
    {
        private readonly DateGraphType _type = new DateGraphType();

        [Fact]
        public void serialize_string_to_date()
        {
            CultureTestHelper.UseCultures(() =>
            {
                var actual = _type.Serialize("2018-07-24");
                actual.ShouldBe("2018-07-24");
            });
        }

        [Fact]
        public void serialize_local_date_returns_date_only()
        {
            CultureTestHelper.UseCultures(() =>
            {
                var date = new DateTime(2000, 1, 2, 3, 4, 5, 6, DateTimeKind.Local);

                var actual = _type.Serialize(date);

                actual.ShouldBe("2000-01-02");
            });
        }

        [Fact]
        public void serialize_utc_date_returns_date_only()
        {
            CultureTestHelper.UseCultures(() =>
            {
                var date = new DateTime(2000, 1, 2, 3, 4, 5, 6, DateTimeKind.Utc);

                var actual = _type.Serialize(date);

                actual.ShouldBe("2000-01-02");
            });
        }

        [Fact]
        public void coerces_valid_date()
        {
            CultureTestHelper.UseCultures(() =>
            {
                var expected = DateTime.UtcNow;
                var input = expected.ToLocalTime().ToString("O", DateTimeFormatInfo.InvariantInfo);

                var actual = _type.ParseValue(input);

                actual.ShouldBe(expected);
            });
        }

        [Fact]
        public void coerces_datetimes_to_utc()
        {
            CultureTestHelper.UseCultures(() =>
            {
                ((DateTime) _type.ParseValue("2015-11-21T19:59:32.987+0200")).Kind.ShouldBe(
                    DateTimeKind.Utc);
            });
        }

        [Fact]
        public void coerces_invalid_string_to_exception()
        {
            CultureTestHelper.UseCultures(() =>
            {
                Should.Throw<FormatException>(() => _type.ParseValue("some unknown date"));
            });
        }

        [Fact]
        public void coerces_invalidly_formatted_date_to_exception()
        {
            CultureTestHelper.UseCultures(() =>
            {
                Should.Throw<FormatException>(() => _type.ParseValue("Dec 32 2012"));
            });
        }

        [Fact]
        public void coerces_iso8601_formatted_string_to_date()
        {
            CultureTestHelper.UseCultures(() =>
            {
                _type.ParseValue("2015-12-01T14:15:07.123Z").ShouldBe(
                    new DateTime(2015, 12, 01, 14, 15, 7) + TimeSpan.FromMilliseconds(123));
            });
        }

        [Fact]
        public void coerces_iso8601_string_with_tzone_to_date()
        {
            CultureTestHelper.UseCultures(() =>
            {
                _type.ParseValue("2015-11-21T19:59:32.987+0200").ShouldBe(
                    new DateTime(2015, 11, 21, 17, 59, 32) + TimeSpan.FromMilliseconds(987));
            });
        }
    }
}
