#if NET6_0_OR_GREATER

namespace ObjectDumping.Tests.Testdata.RecordTypes
{
    public record struct EmptyRecordStruct();

    public record struct RecordStruct(double Property1, string Property2);

    public readonly record struct ReadonlyRecordStruct(double Property1, string Property2);
}
#endif