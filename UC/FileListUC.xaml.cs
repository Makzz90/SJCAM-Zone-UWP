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

using SJCAM_Zone.DataModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SJCAM_Zone.UC
{
    public sealed partial class FileListUC : UserControl
    {
        public Action Close;

        public FileListUC(Action closeCallback)
        {
            this.DataContext = new Data_Items();
            this.InitializeComponent();
            this.Close = closeCallback;
        }

        private Data_Items VM
        {
            get
            {
                return this.DataContext as Data_Items;
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot pivot = sender as Pivot;
            if (pivot.SelectedIndex == 0)
            {
                if (this.VM.ItemsPhoto.Count == 0)
                {
                    this.VM.LoadPhotos();
                }
            }
            else
            {
                if (this.VM.ItemsVideo.Count == 0)
                {
                    this.VM.LoadVideos();
                }
            }
        }

        private void GridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //GridView gv = sender as GridView;
            //var panel = (ItemsWrapGrid)gv.ItemsPanelRoot;
            //panel.ItemWidth = e.NewSize.Width / 3;
            //panel.ItemHeight = panel.ItemWidth * (3 / 4);



            GridView gv = sender as GridView;
            var panel = (ItemsWrapGrid)gv.ItemsPanelRoot;
            //panel.Orientation = Orientation.Horizontal;

            double colums = e.NewSize.Width / 190.0;

            //panel.MaximumRowsOrColumns = (int)colums;

            //System.Diagnostics.Debug.WriteLine(colums + " " );

            panel.ItemWidth = e.NewSize.Width / (int)colums;
            panel.ItemHeight = panel.ItemWidth * 0.8;
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image img = sender as Image;/*
            string name = img.Tag.ToString();
            if (this.items.SelectMode == true)
            {
                DataModel.Data_Items.Item i = this.items.GetItemByPhoto(name);
                i.Is_Selected = !i.Is_Selected;
                return;
            }
            this.ShowAppBar(false);
            ImageViewer.HandlePhotoUpdate(items.ItemsPhoto, name, this.GetImageFunc(), new Action(() => { this.ShowAppBar(true); }));*/
        }

        private void Image_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            Image img = sender as Image;/*
            string name = img.Tag.ToString();
            DataModel.Data_Items.Item i = this.items.GetItemByVideo(name);

            if (this.items.SelectMode == true)
            {
                i.Is_Selected = !i.Is_Selected;
                return;
            }

            Grid_VideoPlayer.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.MediaElement.Source = new Uri(i.Original);
            this.ShowAppBar(false);*/
        }

        private bool InSelection
        {
            get
            {
                return this.GridViewPhotos.SelectionMode == ListViewSelectionMode.Multiple;
            }
        }

        private void ActivateSelect(bool status)
        {
            this.GridViewVideos.SelectionMode = this.GridViewPhotos.SelectionMode = status ? ListViewSelectionMode.Multiple : ListViewSelectionMode.None;

            CompositeTransform target = this.Btn_Delete.RenderTransform as CompositeTransform;
            target.Animate(target.TranslateY, status ? 0 : this.AppBar.Height, "TranslateY", 100, new int?(0));

            target = this.Btn_Download.RenderTransform as CompositeTransform;

            target.Animate(target.TranslateY, status ? 0 : this.AppBar.Height, "TranslateY", 100, new int?(0));
        }
        
        private void Close_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.Close != null)
                this.Close();
        }

        private async void Btn_Download_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (var item in this.GridViewPhotos.SelectedItems)
            {
                await this.VM.DownloadItem((Data_Items.Item)item);
            }
            await Task.Delay(200);
            foreach (var item in this.GridViewVideos.SelectedItems)
            {
                await this.VM.DownloadItem((Data_Items.Item)item);
            }
            this.ActivateSelect(false);
        }

        private void Select_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ActivateSelect(!this.InSelection);
        }

        private async void Btn_Delete_Tapped(object sender, TappedRoutedEventArgs e)
        {
            foreach (var item in this.GridViewPhotos.SelectedItems)
            {
                await this.VM.RemoveItem((Data_Items.Item)item);
            }
            await Task.Delay(500);
            foreach (var item in this.GridViewVideos.SelectedItems)
            {
                await this.VM.RemoveItem((Data_Items.Item)item);
            }
        }
    }
}
