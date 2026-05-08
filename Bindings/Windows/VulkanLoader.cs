using System.Runtime.InteropServices;

namespace Ignis.Bindings.Windows;

internal static partial class VulkanLoader
{
    [DefaultDllImportSearchPaths(
        DllImportSearchPath.System32)]
    [LibraryImport(
        "vulkan-1.dll",
        StringMarshalling = StringMarshalling.Utf8)]
    internal static partial nint vkGetInstanceProcAddr(
        nint instance,
        string pName);
}