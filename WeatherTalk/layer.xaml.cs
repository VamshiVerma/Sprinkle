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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WeatherTalk
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class layer : Page
    {
        public layer()
        {
            this.InitializeComponent();
            BitmapImage image = new BitmapImage(new Uri(
                $"http://api.wunderground.com/api/9270a990901ad2ce/animatedradar/animatedsatellite/image.gif?num=5&delay=50&rad.maxlat=47.709&rad.maxlon=-69.263&rad.minlat=31.596&rad.minlon=-97.388&rad.width=640&rad.height=480&rad.rainsnow=1&rad.reproj.automerc=1&rad.num=5&sat.maxlat=47.709&sat.maxlon=-69.263&sat.minlat=31.596&sat.minlon=-97.388&sat.width=900&sat.height=900&sat.key=sat_ir4_bottom&sat.gtt=107&sat.proj=me&sat.timelabel=0&sat.num=5", UriKind.Absolute));
            
            hi.Source = image;
            //    BitmapImage i2 = new BitmapImage(new Uri($"", UriKind.Absolute));
            string k = String.Format("http://api.wunderground.com/api/9270a990901ad2ce/radar/image.gif?centerlat={0}&centerlon={1}&radius=100&width=280&height=280&newmaps=1", MainPage.Lat,MainPage.Lng);
            BitmapImage i2 = new BitmapImage(new Uri(k, UriKind.Absolute));

            h1.Source = i2;
            string l= string.Format("http://api.wunderground.com/api/9270a990901ad2ce/satellite/image.gif?lat={0}&lon={1}&radius=100&width=280&height=280&key=sat_ir4_bottom&basemap=1", MainPage.Lat, MainPage.Lng);

            BitmapImage i3= new BitmapImage(new Uri(l, UriKind.Absolute));
            h2.Source = i3;
           // string l = string.Format("http://api.wunderground.com/api/9270a990901ad2ce/satellite/image.gif?lat={0}&lon={1}&radius=100&width=280&height=280&key=sat_ir4_bottom&basemap=1", MainPage.Lat, MainPage.Lng);

           // BitmapImage i4 = new BitmapImage(new Uri($"", UriKind.Absolute));
            //h3.Source = i4;

        }




    }
}
