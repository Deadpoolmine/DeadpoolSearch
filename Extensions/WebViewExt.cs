using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DeadpoolSearch.Extensions
{
    public class WebViewExt
    {
        public static readonly DependencyProperty HtmlSourceProperty =
               DependencyProperty.RegisterAttached("HtmlSource", typeof(string), typeof(WebViewExt), new PropertyMetadata("", OnHtmlSourceChanged));
        public static string GetHtmlSource(DependencyObject obj) { return (string)obj.GetValue(HtmlSourceProperty); }
        public static void SetHtmlSource(DependencyObject obj, string value) { obj.SetValue(HtmlSourceProperty, value); }
        private static void OnHtmlSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebView webView = d as WebView;
            if (webView != null)
            {
                System.Diagnostics.Debug.WriteLine((string)e.NewValue);
                if (Uri.IsWellFormedUriString((string)e.NewValue, UriKind.Absolute))
                    webView.Navigate(new Uri((string)e.NewValue));
                else
                    webView.NavigateToString((string)e.NewValue);
            }
        }
    }
}
