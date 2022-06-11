using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WindowBackgroundDemoWinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : BetterWindow, INotifyPropertyChanged
    {
        private readonly ThemeListener _themeListener;

        private bool _useCustomColor = false;

        private SolidColorBrush _pageBackgroundBrush;

        public SolidColorBrush PageBackgroundBrush
        {
            get
            {
                return _pageBackgroundBrush;
            }
            set
            {
                _pageBackgroundBrush = value;
                Background = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindow()
        {
            this.InitializeComponent();

            // resize and center window
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var _apw = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
            _apw.Resize(new Windows.Graphics.SizeInt32(MinWidth, MinHeight));
            Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(myWndId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var CenteredPosition = _apw.Position;
                CenteredPosition.X = ((displayArea.WorkArea.Width - _apw.Size.Width) / 2);
                CenteredPosition.Y = ((displayArea.WorkArea.Height - _apw.Size.Height) / 2);
                _apw.Move(CenteredPosition);
            }

            ColorPicker.Color = Microsoft.UI.Colors.Cyan;

            _themeListener = new ThemeListener();

            LoadThemeBackgroundBrush();
            _themeListener.ThemeChanged += OnThemeChanged;

            Closed += Window_Closed;
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            _themeListener.Dispose();
        }

        private void OnThemeChanged()
        {
            if (_useCustomColor)
                return;
            LoadThemeBackgroundBrush();
        }

        private void LoadThemeBackgroundBrush()
        {
            PageBackgroundBrush = Application.Current.Resources["SolidBackgroundFillColorTertiaryBrush"] as SolidColorBrush;
        }

        private void OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            if (_useCustomColor)
            {
                PageBackgroundBrush.Color = sender.Color;
            }
        }

        private void ColorCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            _useCustomColor = true;
            PageBackgroundBrush = new SolidColorBrush(ColorPicker.Color);
        }

        private void ColorCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            _useCustomColor = false;
            LoadThemeBackgroundBrush();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
