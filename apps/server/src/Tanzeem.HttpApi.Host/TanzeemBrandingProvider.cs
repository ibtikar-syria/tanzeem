using Microsoft.Extensions.Localization;
using Tanzeem.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Tanzeem;

[Dependency(ReplaceServices = true)]
public class TanzeemBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<TanzeemResource> _localizer;

    public TanzeemBrandingProvider(IStringLocalizer<TanzeemResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
