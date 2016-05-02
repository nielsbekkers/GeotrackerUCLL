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
        readonly GeofenceMonitor _monitor = GeofenceMonitor.Current;                                //Maak klasse aan

        public MainPage()                                                                           //Constructor
        {
            InitializeComponent();

            AppInfo appinfo = new AppInfo();                                                        //Binding appinfo
            appinfo.AppNaam = "GeoTrackerUCLL";                                                     //Binding appinfo
            TitlePanel.DataContext = appinfo;                                                       //Binding appinfo

            var dwellTimes = TimeSpan.FromMilliseconds(0.1);                                        //Verplicht aantal miliseconds dat men binnen de geolocatie moet zijn vooraleer code verder gaat
            var mask = MonitoredGeofenceStates.Entered | MonitoredGeofenceStates.Exited;            //mask meegeven, binnen de zone of buiten de zone

            geolocator.MovementThreshold = 10;

            var pos = new BasicGeoposition { Latitude = 50.929125, Longitude = 5.395189 };          //Geef longitude en latitude van positie mee

            Geofence fence = new Geofence("UCLL", new Geocircle(pos, 100),mask,false,dwellTimes);   //Maak nieuw geofence met behulp van de meegegeven variabelen

            _monitor.GeofenceStateChanged += MonitorOnGeofenceStateChanged;                         //ga naar functie (MonitorOnGeofenceStateChanged)

            try
            {
                _monitor.Geofences.Clear();                                                         //Verwijder eerdere toegevoegde geofences 
                _monitor.Geofences.Add(fence);
            }
            catch (Exception)
            {
                //geofence is al aan het systeem toegevoegd 
                
            }
        }

        private async void MonitorOnGeofenceStateChanged(GeofenceMonitor sender, object args)           
        {
            var fences = sender.ReadReports();
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {

                foreach (GeofenceStateChangeReport report in fences)
                {
                    if (report.Geofence.Id != "UCLL")                                                   //Indien Geofence ID gelijk is aan UCLL, voer code uit

                        continue;


                    if (report.NewState == GeofenceState.Entered)
                    {
                        Frame.Navigate(typeof(UCLLdiepenbeek));                                         //Navigeer naar de pagina UCLLdiepenbeek
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                          {
                              MessageDialog dialog = new MessageDialog("Welkom op UCLL Diepenbeek");    //Toon melding indien binnentreden van de geolocatie
                              await dialog.ShowAsync();
                              //Frame.Navigate(typeof(UCLLdiepenbeek));
                          });
                    }
                    if (report.NewState == GeofenceState.Exited)
                    {
                        Frame.Navigate(typeof(MainPage));                                                //Navigeer naar de pagina UCLLdiepenbeek
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
                         {
                             MessageDialog dialog = new MessageDialog("U verlaat UCLL Diepenbeek!");    //Toon melding iendien verlaten van de geolocatie
                             await dialog.ShowAsync();
                             //Frame.Navigate(typeof(MainPage));
                         });

                    }
                }
            });
        }

       
        private async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Geoposition position = await geolocator.GetGeopositionAsync();                               //Forceer het ophalen van de geolocaties om te simuleren in de emulator
        }
    }
}
