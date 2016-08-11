using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggChat
{
    public class MapActivity : Fragment,ILocationListener
    {
        private LocationManager locMgr;
        private String locationProvider;
        private ChatActivity context;
        private WebView wvMap;
        private static String MAP_URL = "file:///android_asset/googlemap.html";
        private String provider;
        private Location mostRecentLocation;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Map, null);

            this.context = (ChatActivity)this.Activity;

            GetLocation();

            wvMap=view.FindViewById<WebView>(Resource.Id.wvMap);

            wvMap.Settings.JavaScriptEnabled = true;
            wvMap.Settings.SetSupportZoom(true);
            wvMap.Settings.BuiltInZoomControls = true;
            wvMap.RequestFocus();

            wvMap.SetWebViewClient(new MyWebView() {
                mostRecentLocation = this.mostRecentLocation,
                webViewReady = false            
            });

            wvMap.SetWebChromeClient(new WebChromeClient());

            wvMap.LoadUrl(MAP_URL);


            return view;
            //return base.OnCreateView(inflater, container, savedInstanceState);
        }

      
        private void GetLocation()
        {
            this.locMgr = (LocationManager)this.context.GetSystemService(Context.LocationService);
            Criteria criteria = new Criteria();
            criteria.Accuracy = Accuracy.Fine;
            criteria.SpeedRequired = true;
            criteria.SpeedAccuracy = Accuracy.High;
            criteria.AltitudeRequired = true;
            criteria.BearingRequired = true;
            criteria.CostAllowed = true;

            provider= this.locMgr.GetBestProvider(criteria, true);

            this.locMgr.RequestLocationUpdates(provider, 1, 0,this);

            mostRecentLocation= this.locMgr.GetLastKnownLocation(provider);



            if (mostRecentLocation == null)
            {
                mostRecentLocation = locMgr.GetLastKnownLocation(LocationManager.NetworkProvider);
            }
            else
            {
                String log = String.Format("{0},{1}", mostRecentLocation.Latitude, mostRecentLocation.Longitude);
                System.Diagnostics.Debug.WriteLine(log, "Location");

            }

            if (mostRecentLocation == null)
            {
                Toast.MakeText(this.context, "無法取得定位", ToastLength.Short).Show();
                //mostRecentLocation = locMgr.GetLastKnownLocation(LocationManager.NetworkProvider);
            }

        }

        private void LoadWebView()
        {

        }


        public void OnLocationChanged(Location location)
        {
            String log = String.Format("{0},{1}",location.Latitude,location.Longitude);
            System.Diagnostics.Debug.WriteLine(log, "Location");
            //throw new NotImplementedException();
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }
    }

    public class MyWebView:WebViewClient
    {
        public bool webViewReady=false;

        public Location mostRecentLocation;
        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);

            webViewReady = true;

            String centerURL = "javascript:centerAt("
                        + mostRecentLocation.Latitude + ","
                        + mostRecentLocation.Longitude + ")";

            if (webViewReady)
            {

                view.LoadUrl(centerURL);
            }
        }
    }

}