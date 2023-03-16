#if NET6_0_OR_GREATER
using System;

namespace ObjectDumping.Tests.Testdata.RecordTypes
{
    public record Sprint(int SprintId, DateTimeOffset StartDate, DateTimeOffset EndDate);
}
#endif