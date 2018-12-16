using System;
using System.Globalization;
#if NET452
using System.Threading;
#endif

namespace ObjectDumperLib_Tests.Utils
{
    public static class CurrentCultureHelper
    {
        public static IDisposable ChangeCulture(CultureInfo temporaryCultureInfo)
        {
            if (temporaryCultureInfo == null)
            {
                throw new ArgumentNullException(nameof(temporaryCultureInfo));
            }

#if NET452
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var currentUiCulture = Thread.CurrentThread.CurrentUICulture;
#else
            var currentCulture = CultureInfo.CurrentCulture;
            var currentUiCulture = CultureInfo.CurrentUICulture;
#endif

            var revertCurrentCultureHandler = new CurrentCultureHandler(currentCulture, currentUiCulture);

#if NET452
            Thread.CurrentThread.CurrentCulture = temporaryCultureInfo;
            Thread.CurrentThread.CurrentUICulture = temporaryCultureInfo;
#else
            CultureInfo.CurrentCulture = temporaryCultureInfo;
            CultureInfo.CurrentUICulture = temporaryCultureInfo;
#endif

            CultureInfo.DefaultThreadCurrentCulture = temporaryCultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = temporaryCultureInfo;

            return revertCurrentCultureHandler;
        }
    }

    public class CurrentCultureHandler : IDisposable
    {
        private readonly CultureInfo currentCulture;
        private readonly CultureInfo currentUiCulture;

        internal CurrentCultureHandler(CultureInfo currentCulture, CultureInfo currentUiCulture)
        {
            this.currentCulture = currentCulture;
            this.currentUiCulture = currentUiCulture;
        }

        public void Dispose()
        {
            try
            {
#if NET452
                Thread.CurrentThread.CurrentCulture = this.currentCulture;
                Thread.CurrentThread.CurrentUICulture = this.currentCulture;
#else
                CultureInfo.CurrentCulture = this.currentCulture;
                CultureInfo.CurrentUICulture = this.currentCulture;
#endif

                CultureInfo.DefaultThreadCurrentCulture = this.currentCulture;
                CultureInfo.DefaultThreadCurrentUICulture = this.currentUiCulture;
            }
            catch
            {
            }
        }
    }
}