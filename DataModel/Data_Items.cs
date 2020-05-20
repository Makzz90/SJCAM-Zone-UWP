using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using Windows.UI.Xaml.Media;

using Windows.Networking.BackgroundTransfer;

using System.Threading;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

namespace SJCAM_Zone.DataModel
{
    public class Data_Items : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression.Body.NodeType != ExpressionType.MemberAccess)
                return;
            string propertyName = (propertyExpression.Body as MemberExpression).Member.Name;

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
        public Data_Items()
        {
            this.ItemsPhoto = new ObservableCollection<Item>();
            this.ItemsVideo = new ObservableCollection<Item>();
        }

        public ObservableCollection<Item> ItemsPhoto { get; private set; }

        public ObservableCollection<Item> ItemsVideo { get; private set; }

        public void Clear()
        {
            this.ItemsPhoto.Clear();
            this.ItemsVideo.Clear();
        }
        
        private void AddItem(string filename, string filesize, string filetime, bool is_photo)
        {
            Item i = new Item(filename, "http://192.168.1.254/DCIM/" + (is_photo ? "PHOTO" : "MOVIE") + "/" + filename, filesize, filetime);
            if (is_photo)
                this.ItemsPhoto.Add(i);
            else
                this.ItemsVideo.Add(i);
        }

        private Regex RegFiles = new Regex("<b>(?<name>.+)<\\/b>.+right>(?<size>.+)<.+right>(?<date>.+)\\s+<");

        public async void LoadPhotos()
        {
            string str = await HttpHelper.Explore("PHOTO");

            this.FillData(str, true);
        }

        public async void LoadVideos()
        {
            string str = await HttpHelper.Explore("MOVIE");

            this.FillData(str, false);
        }

        private void FillData(string str, bool is_photo)
        {
            MatchCollection mc = RegFiles.Matches(str);

            foreach(Match match in mc)
            {
                string filename = match.Groups["name"].Value;
                string size = match.Groups["size"].Value;
                string date = match.Groups["date"].Value;

                Item i = new Item(filename, "http://192.168.1.254/DCIM/" + (is_photo ? "PHOTO" : "MOVIE") + "/" + filename, size.Trim(), date);
                if (is_photo)
                    this.ItemsPhoto.Add(i);
                else
                    this.ItemsVideo.Add(i);
            }
        }

            public Item GetItemByPhoto(string name)
        {
            return this.ItemsPhoto.First((i) => { return i.Name == name; });
        }

        public Item GetItemByVideo(string name)
        {
            return this.ItemsVideo.First((i) => { return i.Name == name; });
        }

        public async Task DownloadItem(Item item)
        {
            StorageFolder folder;
            if (item.IsPhoto)
            {
                folder = await KnownFolders.PicturesLibrary.CreateFolderAsync("SJCAM", CreationCollisionOption.OpenIfExists);
            }
            else
            {
                folder = await KnownFolders.VideosLibrary.CreateFolderAsync("SJCAM", CreationCollisionOption.OpenIfExists);
            }
            
            await item.DownloadToPhone(folder);
        }

        public async Task RemoveItem(Item item)
        {
            if(item.IsPhoto)
            {
                this.ItemsPhoto.Remove(item);
                await HttpHelper.SendCmd_retStatus("cmd=4003&str=A:\\DCIM\\" + "PHOTO" + "\\" + item.Name);//todo:check secsess
            }
            else
            {
                this.ItemsVideo.Remove(item);
                await HttpHelper.SendCmd_retStatus("cmd=4003&str=A:\\DCIM\\" + "MOVIE" + "\\" + item.Name);//todo:check secsess
            }
        }

        public class Item : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
            {
                if (propertyExpression.Body.NodeType != ExpressionType.MemberAccess)
                    return;
                string propertyName = (propertyExpression.Body as MemberExpression).Member.Name;

                if (this.PropertyChanged != null)
                    this.PropertyChanged((object)this, new PropertyChangedEventArgs(propertyName));
            }

            public Item(string name, string original_uri, string filesize, string filetime)
            {
                this.ThumbUri = original_uri + "?custom=1&cmd=4001";
                this.Name = name;
                this.Original = original_uri;
                this.Filesize = filesize;
                this.Filetime = filetime;
                //this.Select_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                this.InDownload = Windows.UI.Xaml.Visibility.Collapsed;
                this.SelectionMode = Windows.UI.Xaml.Visibility.Collapsed;
                this.CheckExists();
                //this.Is_Selected = false;
            }

            public bool IsPhoto
            {
                get { return this.Original.Contains("PHOTO"); }
            }

            public async Task DownloadToPhone(StorageFolder folder)
            {
                this.InDownload = Windows.UI.Xaml.Visibility.Visible;
                string error = "";
                StorageFile file = null;
                var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                try
                {
                    file = await folder.CreateFileAsync(this.Name, CreationCollisionOption.GenerateUniqueName);
                    var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    /*
                    Uri durl = new Uri(this.Original);
                    RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromUri(durl);
                    IRandomAccessStreamWithContentType streamWithContent = await streamRef.OpenReadAsync();
                    Windows.Storage.Streams.Buffer buffer = new Windows.Storage.Streams.Buffer((uint)streamWithContent.Size);
                    
                    ulong readed = 0;
                    var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                    ulong total_for_read = streamWithContent.Size;
                    uint buffer_size = (uint)total_for_read/100;
                    while (readed < total_for_read)
                    {
                        var read = await streamWithContent.ReadAsync(buffer, buffer_size, InputStreamOptions.Partial);
                        await outputStream.WriteAsync(buffer);
                        readed += read.Length;
                        double progress = (100 * ((double)readed / (double)total_for_read));
                        this.DownloadProgress = progress;
                    }
                    this.Original = file.Path;
                    outputStream.Dispose();*/
                    this.DownloadToStream(this.Original, outputStream, (progress) => {

                        Library.Execute.ExecuteOnUIThread(()=> {
                            this.DownloadProgress = progress;


                            if (progress == 100)
                            {
                                this.Original = file.Path;
                                //outputStream.Dispose();



                                this.InDownload = Windows.UI.Xaml.Visibility.Collapsed;
                                this._download_progress = 0;//bugfix
                                string text = resourceLoader.GetString("Downloaded");
                                this.Filesize = text;
                            }
                        });
                        
                    });
                }
                catch (Exception e)
                {
                    error = e.Message;
                    
                }

                
                

                if (error.Length > 0)
                {
                    if (file!=null)
                        await file.DeleteAsync();
                    MessageDialog msgDialog = new MessageDialog(error,resourceLoader.GetString("Error"));
                    msgDialog.Commands.Add(new UICommand(resourceLoader.GetString("Yes"), null, "YES"));
                    await msgDialog.ShowAsync();
                }
                
            }

            private void DownloadToStream(string uri, IRandomAccessStream destinationStream, Action<double> progressCallback)
            {
                if (string.IsNullOrWhiteSpace(uri))
                {
                    
                }
                else
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                    request.BeginGetResponse((asyncRes =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(asyncRes);

                            using (Stream responseStream = response.GetResponseStream())
                            {
                                this.CopyStream(responseStream, destinationStream, progressCallback, response.ContentLength);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }), null);
                }
            }

            private void CopyStream(Stream input, IRandomAccessStream outputRandom, Action<double> progressCallback = null, long inputLength = 0)
            {
                if (inputLength == 0)
                {
                    try
                    {
                        inputLength = input.Length;
                    }
                    catch (Exception)
                    {
                    }
                }

                Stream output = outputRandom.AsStreamForWrite();

                //
                byte[] buffer = new byte[96768];
                int num = 0;
                int count;

                while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (!output.CanWrite)
                        throw new Exception("failed to copy stream");
                    //if (c != null && c.IsSet)
                    //    throw new Exception("CopyStream cancelled");
                    output.Write(buffer, 0, count);
                    num += count;
                    if (progressCallback != null)
                        progressCallback((double)num * 100.0 / (double)inputLength);
                }
                output.Position = 0;
                output.Dispose();
            }

            public async void CheckExists()
            {
                IStorageItem si;
                
                if(this.IsPhoto)
                {
                    si = await KnownFolders.PicturesLibrary.TryGetItemAsync("SJCAM");
                }
                else
                {
                    si = await KnownFolders.VideosLibrary.TryGetItemAsync("SJCAM");
                }

                if (si == null)
                    return;

                StorageFolder folder = si as StorageFolder;
                if (folder == null)
                    return;

                try
                {

                    IStorageItem item = await folder.TryGetItemAsync(this.Name);
                    if (item == null)
                        return;

                    StorageFile card = item as StorageFile;
                    this.Original = card.Path;
                    this.DownloadProgress = 100.0;
                    //
                    var resourceLoader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    string text = resourceLoader.GetString("Downloaded");
                    //
                    this.Filesize = text;
                }
                catch
                {

                }
                
            }

            public string ThumbUri { get; set; }
            public string Name { get; set; }

            private string _file_size;
            public string Filesize
            {
                get
                {
                    return this._file_size;
                }
                set
                {
                    if (this._file_size == value)
                        return;

                    this._file_size = value;
                    this.NotifyPropertyChanged<string>((System.Linq.Expressions.Expression<Func<string>>)(() => this.Filesize));
                }
            }

            public string Filetime { get; set; }
            public string Original { get; set; }
            
            public Guid guid;
            
            private Windows.UI.Xaml.Visibility _in_download;
            public Windows.UI.Xaml.Visibility InDownload
            {
                get { return this._in_download; }
                set
                {
                    if (this._in_download == value)
                        return;

                    this._in_download = value;
                    this.NotifyPropertyChanged<Windows.UI.Xaml.Visibility>((System.Linq.Expressions.Expression<Func<Windows.UI.Xaml.Visibility>>)(() => this.InDownload));
                }
            }

            private Windows.UI.Xaml.Visibility _selection_mode;
            public Windows.UI.Xaml.Visibility SelectionMode
            {
                get { return this._selection_mode; }
                set
                {
                    if (this._selection_mode == value)
                        return;

                    if (this._download_progress == 100.0 || this._in_download == Windows.UI.Xaml.Visibility.Visible)
                        return;

                    this._selection_mode = value;
                    this.NotifyPropertyChanged<Windows.UI.Xaml.Visibility>((System.Linq.Expressions.Expression<Func<Windows.UI.Xaml.Visibility>>)(() => this.SelectionMode));
                }
            }

            private double _download_progress;
            public double DownloadProgress
            {
                get { return this._download_progress; }
                set
                {
                    if (this._download_progress == value)
                        return;

                    this._download_progress = value;
                    this.NotifyPropertyChanged<double>((System.Linq.Expressions.Expression<Func<double>>)(() => this.DownloadProgress));
                }
            }
        }
    }
}
