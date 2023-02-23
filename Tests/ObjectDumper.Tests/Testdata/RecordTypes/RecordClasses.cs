#if NET6_0_OR_GREATER

namespace ObjectDumping.Tests.Testdata.RecordTypes
{
    public record class EmptyRecordClass();

    public record class RecordClass(double Property1, string Property2);

    public record RecordClassInherited : RecordClass
    {
        protected RecordClassInherited(RecordClass original) : base(original)
        {
        }
    }
}
#endif