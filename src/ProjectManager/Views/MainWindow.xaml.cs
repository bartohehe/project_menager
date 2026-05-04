using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ProjectManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SourceInitialized += (_, _) =>
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(WndProc);
    }

    // ── Win32 plumbing ───────────────────────────────────────────────────

    private const int WM_GETMINMAXINFO = 0x0024;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int x, y; }

    [StructLayout(LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MONITORINFO
    {
        public int    cbSize;
        public RECT   rcMonitor;
        public RECT   rcWork;
        public uint   dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT { public int left, top, right, bottom; }

    [DllImport("user32.dll")]
    private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    // ── Window procedure hook ────────────────────────────────────────────

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam,
                           ref bool handled)
    {
        if (msg == WM_GETMINMAXINFO)
        {
            FixMaximizedWindowSize(hwnd, lParam);
            handled = true;
        }
        return IntPtr.Zero;
    }

    /// <summary>
    /// Constrains the maximized window to the working area of whichever
    /// monitor it is on, so it never goes under the taskbar.
    /// </summary>
    private static void FixMaximizedWindowSize(IntPtr hwnd, IntPtr lParam)
    {
        const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        var monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        if (monitor == IntPtr.Zero) return;

        var info = new MONITORINFO { cbSize = Marshal.SizeOf(typeof(MONITORINFO)) };
        if (!GetMonitorInfo(monitor, ref info)) return;

        var work = info.rcWork;

        var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
        mmi.ptMaxPosition = new POINT { x = work.left, y = work.top };
        mmi.ptMaxSize     = new POINT
        {
            x = work.right  - work.left,
            y = work.bottom - work.top
        };
        Marshal.StructureToPtr(mmi, lParam, true);
    }
}
