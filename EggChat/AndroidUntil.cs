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
    //±±¨î¤ÏÂà
    public class AndroidUtil : ICrossDevice
    {
        public void Dispatcher(EventHandler eventhandler, object obj, EventArgs e)
        {
            var Handler = new Handler(Looper.MainLooper);

            Handler.Post(() =>
            {
                eventhandler.Invoke(obj, e);
            });
            //throw new NotImplementedException();
        }

        public void PostMessage(string message)
        {
            this.Dispatcher((s, e) =>
            {
                Toast.MakeText(EggApp.Context, message, ToastLength.Short).Show();
            }, null, null);
            //throw new NotImplementedException();
        }

        public static void ToastHander(Context context, String msg)
        {
            var handler = new Handler(Looper.MainLooper);

            handler.Post(() =>
            {
                Toast.MakeText(context, msg, ToastLength.Short).Show();
            });
        }

        public static void Dialog(Context context, String tile, String message, EventHandler<DialogClickEventArgs> ok, EventHandler<DialogClickEventArgs> cancel)
        {
            var dialog = new AlertDialog.Builder(context);
            dialog.SetTitle(tile);
            dialog.SetMessage(message);
            dialog.SetPositiveButton("OK", ok);
            dialog.SetNegativeButton("Cancel", cancel);

            dialog.Create();
            dialog.Show();
        }

        public static void InputDialog(Context context, View view, EventHandler<DialogClickEventArgs> ok, EventHandler<DialogClickEventArgs> cancel)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);

            builder.SetView(view);

            builder.SetPositiveButton("OK", ok);
            builder.SetNegativeButton("Cancel", cancel);
            builder.Create();
            builder.Show();
        }
    }
}