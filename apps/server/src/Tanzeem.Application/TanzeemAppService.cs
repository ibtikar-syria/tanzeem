using Tanzeem.Localization;
using Volo.Abp.Application.Services;

namespace Tanzeem;

/* Inherit your application services from this class.
 */
public abstract class TanzeemAppService : ApplicationService
{
    protected TanzeemAppService()
    {
        LocalizationResource = typeof(TanzeemResource);
    }
}
