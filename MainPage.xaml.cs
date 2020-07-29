using DeadpoolSearch.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace DeadpoolSearch
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged  
    {

        private string _query;
        public string Query
        {
            get => _query;
            set
            {
                _query = value;
                OnPropertyChanged("Query");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            DeadpoolSEBackground.Blur(duration: 3, delay: 0, value: 0).StartAsync();
            
        }


        private void StartSearch(string query)
        {
            System.Diagnostics.Debug.WriteLine(query);
            Frame.Navigate(typeof(SearchDetailPage), query);
        }

        
        

        private void GoSearch(object sender, RoutedEventArgs e)
        {
            StartSearch(Query);
        }

        private void SearchBarKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                SearchBar.IsEnabled = false;
                SearchBar.IsEnabled = true;
                this.Focus(FocusState.Programmatic);
                StartSearch(SearchBar.Text);
            }
        }


        private async void RecoverBackground(UIElement sender, LosingFocusEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Recover:"+args.FocusState);
            await DeadpoolSEBackground.Blur(duration: 3, delay: 0, value: 0).StartAsync();
        }

        private async void BlurBackground(UIElement sender, GettingFocusEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Blur:"+args.FocusState);
            await DeadpoolSEBackground.Blur(duration: 1000, delay: 0, value: 10).StartAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Background = MathHelper.GetSolidColorBrush("#FFD12F2D");
        }
    }
}
