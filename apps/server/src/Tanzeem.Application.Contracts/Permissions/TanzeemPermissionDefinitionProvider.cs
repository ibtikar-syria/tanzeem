using Tanzeem.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Tanzeem.Permissions;

public class TanzeemPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(TanzeemPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(TanzeemPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TanzeemResource>(name);
    }
}
