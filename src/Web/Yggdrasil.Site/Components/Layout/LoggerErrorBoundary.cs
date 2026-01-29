using Microsoft.AspNetCore.Components.Web;
//using Serilog;

namespace Yggdrasil.Site.Components.Layout
{
    public class LoggerErrorBoundary : ErrorBoundary
    {

        protected override async Task OnErrorAsync(Exception exception)
        {
            //Log.Error(exception, string.Empty);
            await base.OnErrorAsync(exception);
        }
    }
}
