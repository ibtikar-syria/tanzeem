using Tanzeem.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Tanzeem.Permissions;

public class TanzeemPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(TanzeemPermissions.GroupName);

        var assignments = myGroup.AddPermission(TanzeemPermissions.Assignments.Default, L("Permissions:Assignments"));
        assignments.AddChild(TanzeemPermissions.Assignments.Create, L("Permissions:Assignments.Create"));
        assignments.AddChild(TanzeemPermissions.Assignments.Get, L("Permissions:Assignments.Get"));
        assignments.AddChild(TanzeemPermissions.Assignments.GetList, L("Permissions:Assignments.GetList"));
        assignments.AddChild(TanzeemPermissions.Assignments.Update, L("Permissions:Assignments.Update"));
        assignments.AddChild(TanzeemPermissions.Assignments.Delete, L("Permissions:Assignments.Delete"));
        assignments.AddChild(TanzeemPermissions.Assignments.AssignUsers, L("Permissions:Assignments.AssignUsers"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TanzeemResource>(name);
    }
}
