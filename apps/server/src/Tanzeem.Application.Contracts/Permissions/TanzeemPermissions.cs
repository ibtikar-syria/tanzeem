using Volo.Abp.Reflection;

namespace Tanzeem.Permissions;

public static class TanzeemPermissions
{
    public const string GroupName = "Tanzeem";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(TanzeemPermissions));
    }

    public class Assignments
    {
        public const string Default = GroupName + ".Assignments";
        public const string Create = Default + ".Create";
        public const string Get = Default + ".Get";
        public const string GetList = Default + ".GetList";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string AssignUsers = Default + ".AssignUsers";
    }
}
