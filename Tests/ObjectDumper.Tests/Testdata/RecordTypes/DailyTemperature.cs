#if NET6_0_OR_GREATER

namespace ObjectDumping.Tests.Testdata.RecordTypes
{
    public record class PersonRecord(int Age, Person Person);

    public record class DailyTemperature(double HighTemp, double LowTemp)
    {
        public DailyTemperature(double highTemp) : this(highTemp, 0d)
        {
        }

        public DailyTemperature() : this(0d, 0d)
        {
        }

        public string InitOnlyProperty { get; init; }

        public bool ReadWriteProperty { get; set; }
    }
}
#endif