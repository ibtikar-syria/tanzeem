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

        var teams = myGroup.AddPermission(TanzeemPermissions.Teams.Default, L("Permissions:Teams"));
        teams.AddChild(TanzeemPermissions.Teams.Create, L("Permissions:Teams.Create"));
        teams.AddChild(TanzeemPermissions.Teams.Get, L("Permissions:Teams.Get"));
        teams.AddChild(TanzeemPermissions.Teams.GetList, L("Permissions:Teams.GetList"));
        teams.AddChild(TanzeemPermissions.Teams.Update, L("Permissions:Teams.Update"));
        teams.AddChild(TanzeemPermissions.Teams.Delete, L("Permissions:Teams.Delete"));
        teams.AddChild(TanzeemPermissions.Teams.AssignUsers, L("Permissions:Teams.AssignUsers"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TanzeemResource>(name);
    }
}
