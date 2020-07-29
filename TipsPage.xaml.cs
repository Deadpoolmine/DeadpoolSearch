using DeadpoolSearch.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace DeadpoolSearch
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TipsPage : Page
    {
        public TipsPage()
        {
            this.InitializeComponent();
            
            
        }
        private bool isNavigated = false;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = MathHelper.GetSolidColorBrush("#FFD12F2D").Color;
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.BackgroundColor = MathHelper.GetSolidColorBrush("#FFD12F2D").Color;
                    titleBar.ForegroundColor = Colors.White;
                }

                
            }
            this.Background = MathHelper.GetSolidColorBrush("#FFD12F2D");
            await Task.Delay(1000);
            await Tip.Fade(0, 5000).StartAsync();
            System.Diagnostics.Debug.WriteLine("1");
            if (!isNavigated)
            {
                this.Frame.Navigate(typeof(LoadDataPage));
            }
        }

        private void SkipThis(object sender, RoutedEventArgs e)
        {
            Tip.Fade().Stop();
            this.Frame.Navigate(typeof(LoadDataPage));
            isNavigated = true;
            System.Diagnostics.Debug.WriteLine("2");
        }
    }
}
