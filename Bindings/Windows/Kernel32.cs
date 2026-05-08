using System.Runtime.InteropServices;

namespace Ignis.Bindings.Windows;

internal static partial class Kernel32
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [LibraryImport(
        "kernel32.dll",
        StringMarshalling = StringMarshalling.Utf16)]
    internal static partial nint GetModuleHandleW(
        string? lpModuleName);
}