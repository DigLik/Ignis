using System.Runtime.InteropServices;

namespace Ignis.Bindings.Windows;

internal static partial class Winmm
{
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [LibraryImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
    internal static partial uint TimeBeginPeriod(uint uPeriod);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [LibraryImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
    internal static partial uint TimeEndPeriod(uint uPeriod);
}
