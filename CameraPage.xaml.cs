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
using System.Threading.Tasks;

using SJCAM_Zone.DataModel;

namespace SJCAM_Zone
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class CameraPage : Page
    {
        private Data_Config Config;
        DispatcherTimer timer_wifi = new DispatcherTimer();
        DispatcherTimer timer_battery = new DispatcherTimer();
        private PopUpService _flyout;
        private bool mrecordSetting = false;//мы в записи?

        private Data_State VM
        {
            get { return this.DataContext as Data_State; }
        }
        public CameraPage()
        {
            this.DataContext = new Data_State();
            this.InitializeComponent();
            this.Loaded += CameraPage_Loaded;


            this.timer_battery.Interval = TimeSpan.FromSeconds(10);
            this.timer_battery.Tick += this.Update_Battery;
            this.Update_Battery(null, null);
            this.timer_battery.Start();


        }

        private void CameraPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (e.Handled == true)
                return;

            e.Handled = true;

            if(this.Overlay.Children.Count==0)
                this.Back_Tapped(null, null);
        }

        private void ActivateModesPanel(bool state)
        {
            this.PanelModes.IsHitTestVisible = state;
            this.CameraFiles.Opacity = this.CameraMenu.Opacity = this.textBlockQual.Opacity = this.PanelModes.Opacity = state ? 1.0 : 0.4;
        }

        private void CameraPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Config.CameraMode == 2)//file explorer
            {
                this.Mode_Video_Tapped(null, null);
                return;
            }

            this.VM.CameraMode = this.Config.CameraMode;
            this.InitMediaPlayer(this.Config.CameraMode == 0 || this.Config.CameraMode == 4);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Config = e.Parameter as Data_Config;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += CameraPage_BackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested -= CameraPage_BackRequested;
        }

        void InitMediaPlayer(bool is_photo)
        {
            this.Media.Stop();
            this.Media.Source = is_photo ? "http://192.168.1.254:8192" : "rtsp://192.168.1.254/sjcam.mov";
            this.Media.Play();
        }

        private async void Mode_Photo_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ActivateModesPanel(false);
            this.VM.CameraMode = 0;
            string temp = await HttpHelper.SendCmd_retStatus("cmd=3001&par=0");
            await Task.Delay(500);
            this.InitMediaPlayer(true);
            this.ActivateModesPanel(true);
        }

        private async void Mode_Video_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ActivateModesPanel(false);
            this.VM.CameraMode = 1;
            string temp = await HttpHelper.SendCmd_retStatus("cmd=3001&par=1");
            await Task.Delay(500);
            this.InitMediaPlayer(false);
            this.ActivateModesPanel(true);
        }

        private async void Mode_VideoLaps_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ActivateModesPanel(false);
            this.VM.CameraMode = 3;
            string temp = await HttpHelper.SendCmd_retStatus("cmd=3001&par=3");
            await Task.Delay(500);
            this.InitMediaPlayer(false);
            this.ActivateModesPanel(true);
        }

        private async void Mode_PhotoLaps_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.ActivateModesPanel(false);
            this.VM.CameraMode = 4;
            string temp = await HttpHelper.SendCmd_retStatus("cmd=3001&par=4");
            await Task.Delay(500);
            this.InitMediaPlayer(true);
            this.ActivateModesPanel(true);
        }

        async void Update_Battery(object sender, object e)
        {
            string battery = await HttpHelper.SendCmd_retValue("cmd=3019");
            this.VM.UpdateBattery("ms-appx:/Assets/Img_camera/ic_battery_" + battery + ".png");
        }

        private async void Power_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string temp = await HttpHelper.SendCmd_retStatus("cmd=3007&par=4");//Выключаем камеру
            this.Media.Stop();
            this.timer_battery.Stop();
            this.timer_wifi.Stop();
            (Window.Current.Content as Frame).GoBack();
        }

        private void ShowPopUp(FrameworkElement element)
        {
            this._flyout = new PopUpService(this.Overlay);
            this._flyout.OverrideBackKey = true;
            this._flyout.AnimationTypeChild = PopUpService.AnimationTypes.Slide;
            this._flyout.Child = element;
            this._flyout.Closed += _flyout_Closed;

            this._flyout.Show();
        }

        private void _flyout_Closed(object sender, EventArgs e)
        {
            this.Mode_Video_Tapped(null, null);
        }

        private void Settings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            International internation = this.Config.International.FirstOrDefault((inter) => inter.Language == "en");
            this.ShowPopUp(new UC.CameraSettingsUC(internation.section_array));
        }

        private void Files_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Media.Stop();
            this.ShowPopUp(new UC.FileListUC(this.Close));
            HttpHelper.SendCmd_retStatus("cmd=3001&par=2");//file pick mode
        }

        private void Close()
        {
            this._flyout.Hide();
        }

        private void Back_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Media.Stop();
            this.timer_battery.Stop();
            this.timer_wifi.Stop();
            (Window.Current.Content as Frame).GoBack();
        }

        private async void Record_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (this.mrecordSetting == false)
            {
                if (this.VM.CameraMode == 0 || this.VM.CameraMode == 4)
                {
                    this.SoundPhoto.Play();
                    await HttpHelper.SendCmd_retStatus("cmd=1001");//Сделать фотографию - нужен режим фото!
                }
                else
                {
                    this.ActivateModesPanel(false);


                    this.SoundVideo.Play();
                    await HttpHelper.SendCmd_retStatus("cmd=2001&par=1");//Начать запись

                    await Task.Delay(500);
                    this.InitMediaPlayer(false);
                }
                this.mrecordSetting = true;
            }
            else
            {
                await HttpHelper.SendCmd_retStatus("cmd=2001&par=0");
                this.mrecordSetting = false;


                await Task.Delay(500);
                this.InitMediaPlayer(false);

                this.ActivateModesPanel(true);
            }
        }


    }
}
