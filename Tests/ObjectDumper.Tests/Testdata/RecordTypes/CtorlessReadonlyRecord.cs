#if NET7_0_OR_GREATER

namespace ObjectDumping.Tests.Testdata.RecordTypes
{
    public readonly record struct CtorlessReadonlyRecord
    {
        public required string Name { get; init; }
    }
}
#endif