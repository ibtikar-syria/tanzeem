using Tanzeem.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Tanzeem.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class TanzeemController : AbpControllerBase
{
    protected TanzeemController()
    {
        LocalizationResource = typeof(TanzeemResource);
    }
}
