using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggChat
{
    [Activity(Label = "¾|¤Ò§ä¯Á¶©Lite", MainLauncher = true, Icon = "@drawable/Map")]
    public class WelcomeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.WelCome);

            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            EggApp.eggChatDB = new EggChatDB(folder);

            var imgvi = this.FindViewById<ImageView>(Resource.Id.img_icon);

            var progress = this.FindViewById<ProgressBar>(Resource.Id.progressbar_updown);

            imgvi.SetImageResource(Resource.Drawable.where2);

            System.Timers.Timer timer2 = new System.Timers.Timer();

            Int32 TimeOut = 0;
            timer2.Interval = 1000;
            timer2.Elapsed += (s, e) =>
            {
                TimeOut += 10;

                if (TimeOut >= 30)
                {
                    Intent intent = new Intent();

                    if (EggApp.eggChatDB.SelectUserInfo().Count == 0)
                    {
                        intent.SetFlags(ActivityFlags.NoHistory);
                        intent.SetClass(this, typeof(LoginActivity));
                    }
                    else
                    {
                        intent.SetClass(this, typeof(MainActivity));
                    }

                    StartActivity(intent);
                    this.Finish();

                    timer2.Stop();
                    timer2.Dispose();
                    timer2.Interval = 0;
                }
            };
            timer2.Start();
        }
    }
}