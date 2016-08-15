using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggChat
{
    [Application]
    public class EggApp : Application
    {
        //public static SignalRProxy MySignalRProxy { get; set; }
        public static EggChatDB eggChatDB;

        public static SignalRProxy mySignalR;

        //public static SignalRProxy mySignalR = new SignalRProxy(Application.Context);
        public static UnityContainer Container { get; set; }

        public static void Initialize()
        {
            try
            {
                EggApp.Container = new UnityContainer();
                EggApp.Container.RegisterType<ICrossDevice, AndroidUtil>();
                EggApp.mySignalR = EggApp.Container.Resolve(typeof(SignalRProxy), "signalRProxy") as SignalRProxy;
            }
            catch (Exception ex)
            {
                // System.Diagnostics.Debug.WriteLine(ex, "Debug");
            }
        }

        public EggApp(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            EggApp.Initialize();
            //System.Diagnostics.Debug.WriteLine("", "degub");
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
        }
    }
}