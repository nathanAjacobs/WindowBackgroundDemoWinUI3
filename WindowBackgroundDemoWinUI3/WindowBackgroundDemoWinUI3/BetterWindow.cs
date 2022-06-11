using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;

namespace WindowBackgroundDemoWinUI3
{
    public class BetterWindow : Window
    {
        private SolidColorBrush _backgroundBrush = new SolidColorBrush(); 
        public SolidColorBrush Background
        {
            get
            {
                return _backgroundBrush;
            }
            set
            {
                _backgroundBrush = value;
            }
        }

        public int MinWidth { get; set; }
        public int MinHeight { get; set; }

        private readonly SUBCLASSPROC SubClassDelegate;
        private readonly IntPtr hWnd = IntPtr.Zero;

        public BetterWindow()
        {
            hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            SubClassDelegate = new SUBCLASSPROC(WindowSubClass);
            bool bRet = SetWindowSubclass(hWnd, SubClassDelegate, 0, 0);
        }

        private int WindowSubClass(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData)
        {
            switch (uMsg)
            {
                case WM_ERASEBKGND:
                    {
                        RECT rect;
                        GetClientRect(hWnd, out rect);
                        IntPtr hBrush = CreateSolidBrush(System.Drawing.ColorTranslator.ToWin32(System.Drawing.Color.FromArgb(Background.Color.A,
                                                                                                                              Background.Color.R,
                                                                                                                              Background.Color.G,
                                                                                                                              Background.Color.B)));
                        FillRect(wParam, ref rect, hBrush);
                        DeleteObject(hBrush);
                        return 1;
                    }
                case WM_GETMINMAXINFO:
                    {
                        var dpi = GetDpiForWindow(hWnd);
                        float scalingFactor = (float)dpi / 96;

                        MINMAXINFO minMaxInfo = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                        minMaxInfo.ptMinTrackSize.x = (int)(MinWidth * scalingFactor);
                        minMaxInfo.ptMinTrackSize.y = (int)(MinHeight * scalingFactor);
                        Marshal.StructureToPtr(minMaxInfo, lParam, true);
                        break;
                    }
            }
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        #region Interop Imports
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
            public RECT(int Left, int Top, int Right, int Bottom)
            {
                left = Left;
                top = Top;
                right = Right;
                bottom = Bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;

            public int y;

#if !UAP10_0
            public static implicit operator System.Drawing.Point(POINT point) => new System.Drawing.Point(point.x, point.y);

            public static implicit operator POINT(System.Drawing.Point point) => new POINT { x = point.X, y = point.Y };
#endif
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        private delegate int SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        private static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, uint uIdSubclass, uint dwRefData);

        [DllImport("Comctl32.dll", SetLastError = true)]
        private static extern int DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool FillRect(IntPtr hdc, [In] ref RECT rect, IntPtr hbrush);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateSolidBrush(int crColor);

        [DllImport("Gdi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool DeleteObject([In] IntPtr hObject);

        [DllImport("User32.dll")]
        private static extern int GetDpiForWindow(IntPtr hwnd);

        private const int WM_ERASEBKGND = 0x0014;
        private const uint LWA_COLORKEY = 0x00000001;
        private const uint LWA_ALPHA = 0x00000002;
        private const uint WM_GETMINMAXINFO = 0x0024;
        #endregion
    }
}
