using System;
using System.Globalization;
using Xunit;

namespace ObjectDumper_Tests.Utils
{
    [CollectionDefinition(TestCollections.CultureSpecific)]
    public sealed class CultureSpecificFixture : ICollectionFixture<CultureSpecificFixture>
    {
        private readonly IDisposable changeCultureHelper;

        public CultureSpecificFixture()
        {
            var testCulture = new CultureInfo("de-CH");
            this.changeCultureHelper = CurrentCultureHelper.ChangeCulture(testCulture);
        }

        ~CultureSpecificFixture()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.changeCultureHelper.Dispose();
        }
    }
}