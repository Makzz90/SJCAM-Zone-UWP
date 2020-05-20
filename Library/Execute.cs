using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;

namespace SJCAM_Zone.Library
{
    public class Execute
    {
        public static void ExecuteOnUIThread(Action action)
        {
            var Thread = Windows.UI.Core.CoreWindow.GetForCurrentThread();
            
            if(Window.Current!=null && (Window.Current.Content as Frame).Dispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                Execute.DO(action);
            }
        }

        private static async void DO(Action action)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                action();
            });
        }
    }
}
