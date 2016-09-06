using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EggChat
{
    //控制反轉
    public class AndroidUtil : ICrossDevice
    {
        #region ICrossDevice.SoundPlay

        public void SoundPlay()
        {
            SoundPool sound = new SoundPool(1, Android.Media.Stream.Music, 5);

            Int32 alert_id = sound.Load(Application.Context, Resource.Raw.sound2, 1);

            System.Threading.Thread.Sleep(600);

            sound.Play(alert_id, 1.0f, 1.0f, 0, 0, 1.0f);
        }

        #endregion ICrossDevice.SoundPlay

        #region ICrossDevice.Dispatcher

        public void Dispatcher(EventHandler eventhandler, object obj, EventArgs e)
        {
            var Handler = new Handler(Looper.MainLooper);

            Handler.Post(() =>
            {
                eventhandler.Invoke(obj, e);
            });
            //throw new NotImplementedException();
        }

        #endregion ICrossDevice.Dispatcher

        #region ICrossDevice.PostMessage

        public void PostMessage(string message)
        {
            this.Dispatcher((s, e) =>
            {
                Toast.MakeText(EggApp.Context, message, ToastLength.Short).Show();
            }, null, null);
            //throw new NotImplementedException();
        }

        #endregion ICrossDevice.PostMessage

        #region ToastHander
        public static void ToastHander(Context context, String msg)
        {
            var handler = new Handler(Looper.MainLooper);

            handler.Post(() =>
            {
                Toast.MakeText(context, msg, ToastLength.Short).Show();
            });
        }
        #endregion

        #region Dialog
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
        #endregion
          
        public static void InputDialog(Context context, View view, EventHandler<DialogClickEventArgs> ok, EventHandler<DialogClickEventArgs> cancel)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);

            builder.SetView(view);

            builder.SetPositiveButton("OK", ok);
            builder.SetNegativeButton("Cancel", cancel);
            builder.Create();
            builder.Show();
        }

        #region EncryptionUtil/DeCrptytionUtil

        public String EncryptionUtil(String msg)
        {
            //1.產生金鑰&初始化向量
            byte[] key = System.Text.Encoding.UTF8.GetBytes("12345678");
            byte[] iv = System.Text.Encoding.UTF8.GetBytes("87654321");

            //2.選擇加密演算法
            DESCryptoServiceProvider provider = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            provider.Key = key;
            provider.IV = iv;

            //3.建立加密物件
            ICryptoTransform transform = provider.CreateEncryptor();

            //4.加密
            MemoryStream ms = new MemoryStream();
            using (CryptoStream crypto = new CryptoStream(ms, transform, CryptoStreamMode.Write))
            {
                byte[] data = System.Text.Encoding.Default.GetBytes(msg);
                crypto.Write(data, 0, data.Length);
            }
            ms.Position = 1;
            BinaryReader brs = new BinaryReader(ms);
            byte[] result = brs.ReadBytes(409600);

            return System.Text.Encoding.UTF8.GetString(result);
        }

        public String DeCrptytionUtil(String msg)
        {
            //1.產生金鑰&初始化向量
            byte[] key = System.Text.Encoding.UTF8.GetBytes("12345678");
            byte[] iv = System.Text.Encoding.UTF8.GetBytes("87654321");

            //2.選擇加密演算法
            DESCryptoServiceProvider provider = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();
            provider.Key = key;
            provider.IV = iv;

            //3.建立加密物件
            ICryptoTransform transform = provider.CreateDecryptor();
            byte[] data;
            int count;
            String result = "";
            //4.加密
            try
            {
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream crypto = new CryptoStream(ms, transform, CryptoStreamMode.Read))
                {
                    data = new byte[409600];
                    count = crypto.Read(data, 0, 409600);
                    result = System.Text.Encoding.UTF8.GetString(data, 0, count);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("DeCrptytionUtil Fail :{0}", ex.Message));
            }

            return result;
        }
        #endregion

        public void HideKeyBoard(Context context, View view)
        {
            InputMethodManager imm = (InputMethodManager)context.GetSystemService(Context.InputMethodService);

            imm.HideSoftInputFromWindow(view.WindowToken, 0);
        }
    }
}