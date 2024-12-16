using System.Runtime.CompilerServices;

namespace HotChocolate.AspNetCore.CommandLine;

internal static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        CookieCrumbleXunit.Initialize();
    }
}