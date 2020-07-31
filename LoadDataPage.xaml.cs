using DeadpoolSearch.Helpers;
using DeadpoolSearch.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI;
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
    public sealed partial class LoadDataPage : Page, INotifyPropertyChanged
    {
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                NotifyPropertyChanged("IsActive");
            }
        }

        private string _percentage;
        public string Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                NotifyPropertyChanged("Percentage");
            }
        }

        private string _currentSource;
        public string CurrentSource
        {
            get => _currentSource;
            set
            {
                _currentSource = value;
                NotifyPropertyChanged("CurrentSource");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public LoadDataPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //this.TopAppBar.Foreground = new SolidColorBrush("#D12F2D");
            
            this.Background = MathHelper.GetSolidColorBrush("#FFD12F2D");

            ContentDialog dialog = new ContentDialog
            {
                Title = "提示",
                Content = "是否重新加载数据？(选是会重新爬取数据)",
                CloseButtonText = "否",
                PrimaryButtonText = "是"
            };

            ContentDialogResult result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                LoadingDataWithCrawler();
            }
            else
            {
                LoadingDataWithOutCrawler();
            }
        }

        private async void LoadingDataWithCrawler()
        {
            IsActive = true;
            CrawlerHelper.OnProgressChanged += CrawlerOnProgressChanged;
            //留一手
            //await CrawlerHelper.CrawlComics(CrwalerOptions.Fandom);
            //await CrawlerHelper.CrawlComics(CrwalerOptions.MarvelHQ);
            await CrawlerHelper.CrawlComics(CrwalerOptions.Comixology);
            List<MarvelComic> comics = await CrawlerHelper.GetComics();
            SearchCoreHelper.CreateDirectory(comics);
            IsActive = false;
            Frame.Navigate(typeof(MainPage));
        }

        

        private async void LoadingDataWithOutCrawler()
        {
            IsActive = true;
            List<MarvelComic> comics = await CrawlerHelper.GetComics();
            SearchCoreHelper.CreateDirectory(comics);
            IsActive = false;
            Frame.Navigate(typeof(MainPage));
        }

        private void CrawlerOnProgressChanged(int progress, CrwalerOptions options)
        {
            string percentage = "0%";
            switch (options)
            {
                case CrwalerOptions.Fandom:
                    {
                        percentage = ((progress - 1939) * 100 / (2020 - 1939)).ToString(); 
                    }
                    break;
                case CrwalerOptions.MarvelHQ:
                    {
                        percentage = (progress * 100 / 183).ToString();
                    }
                    break;
                case CrwalerOptions.Comixology:
                    {
                        percentage = (progress * 100 / 53).ToString();
                    }
                    break;
                default:
                    break;
            }
            if (percentage.Length < 2)
            {
                Percentage = percentage + "%";
            }
            else
            {
                if (percentage.StartsWith("100"))
                    Percentage = "100%";
                else
                    Percentage = percentage.Substring(0, 2) + "%";
            }
            CurrentSource = options.ToString();
        }
    }
}
