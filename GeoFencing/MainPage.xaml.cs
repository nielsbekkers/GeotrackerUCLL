using System;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GeoFencing
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        readonly GeofenceMonitor _monitor = GeofenceMonitor.Current;

        public MainPage()
        {
            InitializeComponent();

            var dwellTimes = TimeSpan.FromMilliseconds(0.1);
            var mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited;

            _monitor.GeofenceStateChanged += MonitorOnGeofenceStateChanged;

            var pos = new BasicGeoposition { Latitude = 50.929125, Longitude = 5.395189 };

            Geofence fence = new Geofence("UCLL", new Geocircle(pos, 100),mask,false,dwellTimes);

            try
            {
                _monitor.Geofences.Add(fence);
            }
            catch (Exception)
            {
                //geofence already added to system 
            }
        }

        private async void MonitorOnGeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var fences = sender.ReadReports();
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {

                foreach (GeofenceStateChangeReport report in fences)
                {
                    if (report.Geofence.Id != "UCLL")

                        continue;


                    if (report.NewState == GeofenceState.Entered)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                         {
                             Frame.Navigate(typeof(UCLLdiepenbeek));
                             MessageDialog dialog = new MessageDialog("Welkom op UCLL");
                             await dialog.ShowAsync();
                         });
                    }
                    if (report.NewState == GeofenceState.Exited)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                         {
                             Frame.Navigate(typeof(MainPage));
                             MessageDialog dialog = new MessageDialog("U verlaat UCLL!");
                             await dialog.ShowAsync();
                         });

                    }
                }
            });
        }
    }
}
