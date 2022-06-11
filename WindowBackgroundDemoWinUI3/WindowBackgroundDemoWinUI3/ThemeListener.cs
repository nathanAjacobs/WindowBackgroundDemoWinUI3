using CommunityToolkit.WinUI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;

namespace WindowBackgroundDemoWinUI3
{
    public class ThemeListener : IDisposable
    {
        private readonly UISettings _settings;

        private ApplicationTheme currentTheme;

        public event Action ThemeChanged;

        private readonly DispatcherQueue _dispatcherQueue;

        public ThemeListener(DispatcherQueue dispatcherQueue = null)
        {
            _dispatcherQueue = dispatcherQueue ?? DispatcherQueue.GetForCurrentThread();
            currentTheme = Application.Current.RequestedTheme;

            _settings = new UISettings();
            _settings.ColorValuesChanged += Settings_ColorValuesChanged;

            if (Window.Current != null)
            {
                Window.Current.Activated += Window_Activated;
            }
        }

        private void Window_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (currentTheme != Application.Current.RequestedTheme)
                UpdateProperties();
        }

        private async void Settings_ColorValuesChanged(UISettings sender, object args)
        {
            await OnThemePropertyChangedAsync();
        }

        private Task OnThemePropertyChangedAsync()
        {
            // Getting called off thread, so we need to dispatch to request value.
            return _dispatcherQueue.EnqueueAsync(
                () =>
                {
                    if (currentTheme != Application.Current.RequestedTheme)
                    {
                        UpdateProperties();
                    }
                }, DispatcherQueuePriority.Normal);
        }

        private void UpdateProperties()
        {
            currentTheme = Application.Current.RequestedTheme;
            ThemeChanged?.Invoke();
        }

        public void Dispose()
        {
            _settings.ColorValuesChanged -= Settings_ColorValuesChanged;

            if (Window.Current != null)
            {
                Window.Current.Activated -= Window_Activated;
            }
        }
    }
}
