using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggChat
{
    public class MapActivity : Fragment
    {
        private LocationManager locMgr;
        private String locationProvider;
        private ChatActivity context;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var view = inflater.Inflate(Resource.Layout.Map, null);

            this.context = (ChatActivity)this.Activity;

            //Context.LocationService

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
        }
    }
}