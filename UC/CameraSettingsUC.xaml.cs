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
using System.Collections.ObjectModel;

namespace SJCAM_Zone.UC
{
    public sealed partial class CameraSettingsUC : UserControl
    {
        private bool Initialized;
        private ObservableCollection<SectionArray> VM { get; set; }

        public CameraSettingsUC(List<SectionArray> section)
        {
            this.VM = new ObservableCollection<SectionArray>(section);
            this.InitializeComponent();
            this.lv.DataContext = this.VM;
            this.Loaded += CameraSettingsUC_Loaded;
        }

        private void CameraSettingsUC_Loaded(object sender, RoutedEventArgs e)
        {
            this.UpdateSettings();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.Initialized)
                return;

            ComboBox cb = sender as ComboBox;
            RowArray vm = cb.DataContext as RowArray;
            
            this.ApplyCmd(vm.cmd, cb.SelectedIndex);
        }

        private async void ApplyCmd(string cmd, int value)
        {
            await HttpHelper.SendCmd_retStatus("cmd=" + cmd + "&par=" + value);

            foreach (var section in this.VM)
            {
                foreach (var row in section.row_array)
                {
                    if (row.cmd == cmd)
                    {
                        row.Selected = value;
                    }
                }
            }
        }

        /// <summary>
        /// Получаем настройки у камеры и применяем их значения к текущему конфигу
        /// </summary>
        public async void UpdateSettings()
        {
            Dictionary<string, string> cameraSettings = await HttpHelper.SendCmd_retDictionary("cmd=3014");

            foreach (var section in this.VM)
            {
                foreach (var row in section.row_array)
                {
                    if (cameraSettings.ContainsKey(row.cmd))
                    {
                        row.Selected = int.Parse(cameraSettings[row.cmd]);
                    }
                    else
                    {
                        int i = 0;
                    }
                }
            }

            this.Initialized = true;
        }
    }
}
