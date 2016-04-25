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
        Geolocator geolocator = new Geolocator();
        readonly GeofenceMonitor _monitor = GeofenceMonitor.Current;

        public MainPage()
        {
            InitializeComponent();

            var dwellTimes = TimeSpan.FromMilliseconds(0.1);
            var mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited;

            

            var pos = new BasicGeoposition { Latitude = 50.929125, Longitude = 5.395189 };

            Geofence fence = new Geofence("UCLL", new Geocircle(pos, 100),mask,false,dwellTimes);

            _monitor.GeofenceStateChanged += MonitorOnGeofenceStateChanged;

            try
            {
                _monitor.Geofences.Clear();
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
                        Frame.Navigate(typeof(UCLLdiepenbeek));
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                          {
                              MessageDialog dialog = new MessageDialog("Welkom op UCLL");
                              await dialog.ShowAsync();
                              //Frame.Navigate(typeof(UCLLdiepenbeek));
                          });
                    }
                    if (report.NewState == GeofenceState.Exited)
                    {
                        Frame.Navigate(typeof(MainPage));
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                         {
                             MessageDialog dialog = new MessageDialog("U verlaat UCLL!");
                             await dialog.ShowAsync();
                             //Frame.Navigate(typeof(MainPage));
                         });

                    }
                }
            });
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)//Locatie
        {
            Geoposition position = await geolocator.GetGeopositionAsync();  //Forceer het ophalen van de geolocaties om te simuleren in de emulator
            
        }
    }
}
