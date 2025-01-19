using System.Security.Principal;

namespace SecretSwitcher.Common;

internal static class AdminUtility
{
    public static bool IsUserAdmin()
    {
        try
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }
}