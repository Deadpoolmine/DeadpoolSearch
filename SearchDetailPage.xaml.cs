using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using System.Data;
using Lucene.Net.Analysis;
using Lucene.Net.Index;
using DeadpoolSearch.Models;
using Microsoft.Toolkit.Collections;
using System.Collections.ObjectModel;
using Lucene.Net.Documents;
using DeadpoolSearch.Helpers;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.System;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Threading;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace DeadpoolSearch
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchDetailPage : Page, INotifyPropertyChanged
    {
        //INotifyPropertyChanged,一定要给上，不然没用  
        public ObservableCollection<MarvelComic> MarvelComics { get; set;}
        private string _query;
        public string Query
        {
            get => _query;
            set
            {
                _query = value;
                NotifyPropertyChanged(nameof(Query));
            }
        }

        private int _recordNumber;
        public int RecordNumber
        {
            get => _recordNumber;
            set
            {
                _recordNumber = value;
                NotifyPropertyChanged(nameof(RecordNumber));
            }
        }


       
        

        private MarvelComic _currentComic;
        public MarvelComic CurrentComic
        {
            get => _currentComic;
            set
            {
                _currentComic = value;
                NotifyPropertyChanged(nameof(CurrentComic));
            }
        }

        private string _randomAvatar = "./Pictures/Deadpool1.jpg";
        public string RandomAvatar
        {
            get => _randomAvatar;
            set
            {
                _randomAvatar = value;
                NotifyPropertyChanged(nameof(RandomAvatar));
            }
        }

        public SearchDetailPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Start();

            MarvelComics = new ObservableCollection<MarvelComic>();
        }

        private void DispatcherTimerTick(object sender, object e)
        {
            int randomAvatarIndex = new Random().Next(1, 4);

            RandomAvatar = "./Pictures/Deadpool" + randomAvatarIndex + ".jpg";
            
        }

        #region UI Related
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
           // System.Diagnostics.Debug.WriteLine(e.Parameter.ToString());
            var query = e.Parameter.ToString();
            
            MarvelComics = SearchCoreHelper.Search(query);
            RecordNumber = MarvelComics.Count;

            Query = query;
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }

        private void SearchBarKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                CopyCollection(SearchCoreHelper.Search(SearchBar.Text));
                RecordNumber = MarvelComics.Count;
                DetailPanel.Offset(0, 1000).Start();
                if (cachedHeight != -1)
                {
                    DetailRow.Height = new GridLength(cachedHeight);
                }
                DetailPanel.CornerRadius = new CornerRadius(30, 30, 0, 0);
                isExpanded = false;
            }
        }
        private void GoSearch(object sender, RoutedEventArgs e)
        {
            CopyCollection(SearchCoreHelper.Search(Query));
            RecordNumber = MarvelComics.Count;
            DetailPanel.Offset(0, 1000).Start();
            if (cachedHeight != -1)
            {
                DetailRow.Height = new GridLength(cachedHeight);
            }
            DetailPanel.CornerRadius = new CornerRadius(30, 30, 0, 0);
            isExpanded = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void CopyCollection(ObservableCollection<MarvelComic> marvelComics)
        {
            MarvelComics.Clear();
            foreach (var item in marvelComics)
            {
                MarvelComics.Add(item);
            }
        }

        private void ChooseComic(object sender, ItemClickEventArgs e)
        {
            CurrentComic = e.ClickedItem as MarvelComic;
            DetailPanel.Offset(0, -1000).Start();
        }

        private void ImageDownloadProgress(object sender, Windows.UI.Xaml.Media.Imaging.DownloadProgressEventArgs e)
        {
            if(e.Progress < 100)
            {
                ImageLoadMask.Opacity = 1;
            }
            else
            {
                ImageLoadMask.Opacity = 0;
            }
        }

        private double cachedHeight = -1;
        private bool isExpanded = false;

        private void DropPanel(object sender, RoutedEventArgs e)
        {
            if (!isExpanded)
            {
                DetailPanel.Offset(0, 1000).Start();   
            }
            if (cachedHeight != -1)
            {
                DetailRow.Height = new GridLength(cachedHeight);
            }
            DetailPanel.CornerRadius = new CornerRadius(30,30,0,0);
            isExpanded = false;
        }


        private void NavigationBack(object sender, RoutedEventArgs e)
        {
            if (Uri.IsWellFormedUriString(CurrentComic.Detail, UriKind.Absolute))
                DetailView.Navigate(new Uri(CurrentComic.Detail));
            else
                DetailView.NavigateToString(CurrentComic.Detail);
        }

        private void ExpandPanel(object sender, RoutedEventArgs e)
        {
            if (!isExpanded)
            {
                cachedHeight = DetailRow.ActualHeight;
                isExpanded = true;
            }
            
            DetailPanel.CornerRadius = new CornerRadius(0);
            DetailRow.Height = new GridLength(0);
        }
        
        #endregion


    }
}
