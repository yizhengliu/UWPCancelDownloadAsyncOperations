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
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CancelDownloadAsyncOperationsUWP
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly CancellationTokenSource s_cts = new CancellationTokenSource();

        readonly HttpClient s_client = new HttpClient
        {
            MaxResponseContentBufferSize = 1_000_000
        };

        readonly IEnumerable<string> s_urlList = new string[]
        {
            "https://learn.microsoft.com",
            "https://learn.microsoft.com/aspnet/core",
            "https://learn.microsoft.com/azure",
            "https://learn.microsoft.com/azure/devops",
            "https://learn.microsoft.com/dotnet",
            "https://learn.microsoft.com/dynamics365",
            "https://learn.microsoft.com/education",
            "https://learn.microsoft.com/enterprise-mobility-security",
            "https://learn.microsoft.com/gaming",
            "https://learn.microsoft.com/graph",
            "https://learn.microsoft.com/microsoft-365",
            "https://learn.microsoft.com/office",
            "https://learn.microsoft.com/powershell",
            "https://learn.microsoft.com/sql",
            "https://learn.microsoft.com/surface",
            "https://learn.microsoft.com/system-center",
            "https://learn.microsoft.com/visualstudio",
            "https://learn.microsoft.com/windows",
            "https://learn.microsoft.com/xamarin"
        };


        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task SumPageSizesAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            int total = 0;
            foreach (string url in s_urlList)
            {
                int contentLength = await ProcessUrlAsync(url, s_client, s_cts.Token);
                total += contentLength;
            }

            stopwatch.Stop();

            AddListItem($"\nTotal bytes returned:  {total:#,#}");
            AddListItem($"Elapsed time:          {stopwatch.Elapsed}\n");
        }

        private async Task<int> ProcessUrlAsync(string url, HttpClient client, CancellationToken token)
        {
            HttpResponseMessage response = await client.GetAsync(url, token);
            byte[] content = await response.Content.ReadAsByteArrayAsync();
            AddListItem($"{url,-60} {content.Length,10:#,#}");

            return content.Length;
        }

        private void AddListItem(string text) 
        {
            ListViewItem listViewItem = new ListViewItem();
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            listViewItem.Content = textBlock;
            lvwOutput.Items.Add(listViewItem);
        }

        private async void btnDownloadContent_Click(object sender, RoutedEventArgs e)
        {
            //perform downloading of relevant content, the calculation of the size of the ocntent as well as the outputting 
            //information about the content
            try
            {
                //
                //custom if defined time (3 and half seconds) is passed for the downloading 
                s_cts.CancelAfter(3500);
                await SumPageSizesAsync();
            }
            //if the task is cancelled
            catch (OperationCanceledException) 
            {
                AddListItem("Download Operations Cancelled");
            } 
            
        }

        private void btnCancelDownload_Click(object sender, RoutedEventArgs e)
        {
            //cancel the task
            s_cts.Cancel();
        }
    }
}
