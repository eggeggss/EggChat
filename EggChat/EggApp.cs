using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Common;

namespace EggChat
{
    public class EggApp:Application
    {
        //public static SignalRProxy MySignalRProxy { get; set; }
        public static EggChatDB eggChatDB;
        public static SignalRProxy mySignalR = new SignalRProxy(Application.Context);

        public EggApp(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
          

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