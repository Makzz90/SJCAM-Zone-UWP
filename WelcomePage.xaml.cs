using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

using SJCAM_Zone.DataModel;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Text;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;

namespace SJCAM_Zone
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        private List<Data_Config> Configs = new List<Data_Config>();
        private PopUpService _flyout;
        private Data_New TopVM { get; set; }

        public WelcomePage()
        {
            this.TopVM = new Data_New();
            this.DataContext = new Data_State();
            this.InitializeComponent();
            this.Loaded += WelcomePage_Loaded;
 //           this.lvTop.ItemsSource = this.TopVM;


   //         this.TopVM.LoadAppTop();
  //          this.TopVM.LoadMore();
        }

        private async void WelcomePage_Loaded(object sender, RoutedEventArgs e)
        {
            this.LoadConfigs();
            var temp = await RequestsDispatcher.GetResponse<Banner>("banner?type=2");
            if(temp!=null)
                this.Banner.Source = new BitmapImage(new Uri(temp.model_list[0].images[0].url));
        }
        
        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            this.ProgressRing.IsActive = true;
            this.BtnConnect.IsEnabled = false;

            string mode = await HttpHelper.SendCmd_retStatus("cmd=3016");//Получаем режим камеры

            this.ProgressRing.IsActive = false;
            this.BtnConnect.IsEnabled = true;

            if (String.IsNullOrEmpty(mode))
                return;

//            this.VM.CameraMode = int.Parse(mode);

            List<string> info = await HttpHelper.SendCmd_retList("cmd=3012");
            string devicename = info[0] + "-" + info[1];
 //           this.VM.cameraVersion = info[1];

            Data_Config config = this.Configs.FirstOrDefault((cfg)=>cfg.devicename == devicename);
            //           this.pibovt.SelectedIndex = 1;
            //           this.vlcMediaElement.Source = "rtsp://192.168.1.254/sjcam.mov";
            if(config!=null)
            {
                config.CameraMode = int.Parse(mode);
                (Window.Current.Content as Frame).Navigate(typeof(CameraPage), config);
            }
            int i = 0;
        }

        private async void LoadConfigs()
        {
            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets\\Cparams");
            IReadOnlyList<StorageFile> files = await folder.GetFilesAsync();
            foreach(var file in files)
            {
                
                IBuffer buffer = await FileIO.ReadBufferAsync(file);
                DataReader reader = DataReader.FromBuffer(buffer);
                byte[] fileContent = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(fileContent);
                string text = Encoding.UTF8.GetString(fileContent, 0, fileContent.Length);
                
                Data_Config data = JsonConvert.DeserializeObject<Data_Config>(text);
                this.Configs.Add(data);
            }
        }

        private void ShowPopUp(FrameworkElement element)
        {
            this._flyout = new PopUpService(this.Overlay);
            this._flyout.OverrideBackKey = true;
            this._flyout.AnimationTypeChild = PopUpService.AnimationTypes.Slide;
            this._flyout.Child = element;

            this._flyout.Show();
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            this.MySplitView.IsPaneOpen = !this.MySplitView.IsPaneOpen;
        }
        
        private void pibovt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
 //           VisualStateManager.GoToState(this, (sender as Pivot).SelectedIndex == 0 ? "Camera" : "Find", true);
        }

        private void Camera_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.pibovt.SelectedIndex = 0;
        }

        private void Find_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.pibovt.SelectedIndex = 1;
        }

        private void MenuFaq_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ShowPopUp(new UC.FaqUC());
        }
        
        private void Banner_ImageOpened(object sender, RoutedEventArgs e)
        {
            FrameworkElement elemet = sender as FrameworkElement;
            elemet.Animate(0, 1, "Opacity", 600, new int?());
        }

        private void ConnectHelp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //this.ShowPopUp(new UC.ConnectFaqUC());
            
        }
    }
}
