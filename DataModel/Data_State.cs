using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using System.ComponentModel;
using System.Linq.Expressions;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SJCAM_Zone.DataModel
{
    public class Data_State : INotifyPropertyChanged
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

        private int _camera_mode;

        /// <summary>
        /// 0 Photo
        /// 1 Video
        /// 2 FileExplorer
        /// 4 PhotoLaps
        /// 3 VideoLaps
        /// </summary>
        public int CameraMode
        {
            get { return _camera_mode; }
            set
            {
                switch(value)
                {
                    case 0://photo
                        {
                            this.Mode_Video_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_Photo_Brush = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                            this.Mode_VideoLaps_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_PhotoLaps_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            break;
                        }
                    case 1://video
                        {
                            this.Mode_Video_Brush = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                            this.Mode_Photo_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_VideoLaps_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_PhotoLaps_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            break;
                        }
                    case 2:
                        {
                            //todo?
                            break;
                        }
                    case 3://video laps
                        {
                            this.Mode_Video_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_Photo_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_VideoLaps_Brush = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                            this.Mode_PhotoLaps_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            break;
                        }
                    case 4:
                        {
                            this.Mode_Video_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_Photo_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_VideoLaps_Brush = new SolidColorBrush(Windows.UI.Colors.White);
                            this.Mode_PhotoLaps_Brush = new SolidColorBrush(Windows.UI.Colors.DodgerBlue);
                            break;
                        }
                }

                this.NotifyPropertyChanged<SolidColorBrush>((System.Linq.Expressions.Expression<Func<SolidColorBrush>>)(() => this.Mode_Video_Brush));
                this.NotifyPropertyChanged<SolidColorBrush>((System.Linq.Expressions.Expression<Func<SolidColorBrush>>)(() => this.Mode_Photo_Brush));
                this.NotifyPropertyChanged<SolidColorBrush>((System.Linq.Expressions.Expression<Func<SolidColorBrush>>)(() => this.Mode_VideoLaps_Brush));
                this.NotifyPropertyChanged<SolidColorBrush>((System.Linq.Expressions.Expression<Func<SolidColorBrush>>)(() => this.Mode_PhotoLaps_Brush));

                _camera_mode = value;
            }
        }

        public SolidColorBrush Mode_Video_Brush { get; set; }
        public SolidColorBrush Mode_Photo_Brush { get; set; }
        public SolidColorBrush Mode_VideoLaps_Brush { get; set; }
        public SolidColorBrush Mode_PhotoLaps_Brush { get; set; }

        public string cameraVersion { get; set; }
        public bool new_hardware_result_fg { get; set; }

        private string _WIFI_name;
        public string WIFI_Name
        {
            get
            {
                return this._WIFI_name;
            }
            set
            {
                if (this._WIFI_name == value)
                    return;

                this._WIFI_name = value;
                this.NotifyPropertyChanged<string>((System.Linq.Expressions.Expression<Func<string>>)(() => this.WIFI_Name));
            }
        }

        public void UpdateBattery(string s)
        {
            if(this._battery==null)
            {
                this.Battery = new BitmapImage(new Uri(s));
                return;
            }

            string a = this._battery.UriSource.ToString();
            if (a != s)
            {
                this.Battery = new BitmapImage(new Uri(s));
            }
        }

        private BitmapImage _battery;
        public BitmapImage Battery
        {
            get
            {
                return this._battery;
            }
            set
            {
                if (this._battery == value)
                    return;

                this._battery = value;
                this.NotifyPropertyChanged<BitmapImage>((System.Linq.Expressions.Expression<Func<BitmapImage>>)(() => this.Battery));
            }
        }

        public void UpdateWiFi(string s)
        {
            if (this._wifi == null)
            {
                this.WiFi = new BitmapImage(new Uri(s));
                return;
            }

            string a = this._wifi.UriSource.ToString();
            if (a != s)
            {
                this.WiFi = new BitmapImage(new Uri(s));
            }
        }

        private BitmapImage _wifi;
        public BitmapImage WiFi
        {
            get
            {
                return this._wifi;
            }
            set
            {
                if (this._wifi == value)
                    return;

                this._wifi = value;
                this.NotifyPropertyChanged<BitmapImage>((System.Linq.Expressions.Expression<Func<BitmapImage>>)(() => this.WiFi));
            }
        }
    }
}
