using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggChat
{
    public class MapActivity : Fragment, ILocationListener
    {
        private LocationManager locMgr;
        private String locationProvider;
        private ChatActivity context;
        private WebView wvMap;
        private static String MAP_URL = "file:///android_asset/googlemap.html";
        private String provider;
        private Location mostRecentLocation;
        private bool lbSendMyLocation;
        private UserInfo selfUserInfo, friendUserInfo;
        private MyWebViewClient myWbClient;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Map, null);

            this.context = (ChatActivity)this.Activity;
            selfUserInfo = this.context.SelfInfo;
            friendUserInfo = this.context.FriedInfo;
            GetLocation();

            wvMap = view.FindViewById<WebView>(Resource.Id.wvMap);

            wvMap.Settings.JavaScriptEnabled = true;
            wvMap.Settings.SetSupportZoom(true);
            wvMap.Settings.BuiltInZoomControls = true;
            wvMap.RequestFocus();

            myWbClient = new MyWebViewClient()
            {
                mostRecentLocation = this.mostRecentLocation,
                webViewReady = false
            };

            wvMap.SetWebViewClient(myWbClient);

            wvMap.SetWebChromeClient(new WebChromeClient());

            wvMap.LoadUrl(MAP_URL);

            var chkMap = view.FindViewById<CheckBox>(Resource.Id.chkMap);

            //打開flag開始傳送我的位置
            chkMap.Click += (sender, obj) =>
            {
                if (chkMap.Checked)
                {
                    lbSendMyLocation = true;

                    if (mostRecentLocation != null)
                        SetMyLocationInfoAndSent(mostRecentLocation.Latitude, mostRecentLocation.Longitude);
                }
                else
                {
                    lbSendMyLocation = false;
                }
            };

            //取得對方的位置
            EggApp.mySignalR.ReceiveLocationEvent += (s, e) =>
            {
                var userInfo = e as UserInfo;

                //Toast.MakeText(this.context, "Recieve Friend" + userInfo.Lat + "/" + userInfo.Lon, ToastLength.Short).Show();
                LoadFriendLocation(userInfo.Lat, userInfo.Lon);
            };

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

            provider = this.locMgr.GetBestProvider(criteria, true);

            this.locMgr.RequestLocationUpdates(provider, 1, 0, this);

            mostRecentLocation = this.locMgr.GetLastKnownLocation(provider);

            if (mostRecentLocation == null)
            {
                mostRecentLocation = locMgr.GetLastKnownLocation(LocationManager.NetworkProvider);
            }
            else
            {
                String log = String.Format("Send=>{0},{1}", mostRecentLocation.Latitude, mostRecentLocation.Longitude);

                System.Diagnostics.Debug.WriteLine(log, "Location");
            }

            if (mostRecentLocation == null)
            {
                Toast.MakeText(this.context, "無法取得定位", ToastLength.Short).Show();
                //mostRecentLocation = locMgr.GetLastKnownLocation(LocationManager.NetworkProvider);
            }
        }

        private void LoadMyLocation(double Lat, double Lon)
        {
            if (myWbClient.webViewReady)
            {
                String centerURL = "javascript:centerAt("
                          + Lat + ","
                          + Lon + ")";

                wvMap.LoadUrl(centerURL);
            }
        }

        private void LoadFriendLocation(double Lat, double Lon)
        {
            if (myWbClient.webViewReady)
            {
                String centerURL = "javascript:centerAt2("
                          + Lat + ","
                          + Lon + ")";

                wvMap.LoadUrl(centerURL);
            }
        }

        private void SetMyLocationInfoAndSent(double lat, double lon)
        {
            selfUserInfo.Lat = lat;
            selfUserInfo.Lon = lon;

            selfUserInfo.UserName = friendUserInfo.UserName;
            // Toast.MakeText(this.context, "SendMyLoc" + 10 + "/" + 10, ToastLength.Short).Show();

            EggApp.mySignalR.SendLocation(selfUserInfo);
        }

        public void OnLocationChanged(Location location)
        {
            // String log = String.Format("{0},{1}", location.Latitude, location.Longitude);
            // System.Diagnostics.Debug.WriteLine(log, "Location");
            if (location != null)
            {
                LoadMyLocation(location.Latitude, location.Longitude);

                if (lbSendMyLocation)
                {
                    //改變自己位置
                    //myWbClient.ResetLocation(wvMap, location.Latitude, location.Longitude);
                    //打自己的位置出去
                    SetMyLocationInfoAndSent(location.Latitude, location.Longitude);
                }
            }
            else
            {
                Toast.MakeText(this.context, "無法取得定位", ToastLength.Short).Show();
            }
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }
    }

    public class MyWebViewClient : WebViewClient
    {
        public bool webViewReady = false;

        public Location mostRecentLocation;

        public override void OnPageFinished(WebView view, string url)
        {
            base.OnPageFinished(view, url);
            if (mostRecentLocation != null)
                ResetLocation(view, mostRecentLocation.Latitude, mostRecentLocation.Longitude);
        }

        public void ResetLocation(WebView view, double lat, double lon)
        {
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