using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Common;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggChat
{
    public delegate void EventHandlerUserList(object sender, List<UserInfo> e);

    public class SignalRProxy
    {
        public Context context { set; get; }
        public HubConnection _hubconnection;
        private IHubProxy _IhubProxy;

        public event EventHandler GotMsgEvent;

        public event EventHandlerUserList RefreschUserListEvent;

        public static SignalRProxy CreateSignalRProxyFactory(Context context)
        {
            return new SignalRProxy(context);
        }

        public SignalRProxy(Context context)
        {
            this.context = context;
            this._hubconnection = new HubConnection("http://eggeggss.ddns.net/chat/");
            this._IhubProxy = this._hubconnection.CreateHubProxy("Chathub");
            this.RegisterAllEvent();
        }

        public async void OpenConnection(object sender, UserInfo userinfo, EventHandler Compelte)
        {
            var result = this._hubconnection.Start().ContinueWith((hubStat) =>
            {
                if (hubStat.IsFaulted)
                {
                    var handle = new Handler(Looper.MainLooper);
                    handle.Post(() =>
                    {
                        Toast.MakeText(this.context, "ConnectionFail", ToastLength.Short).Show();
                    });
                    //System.Diagnostics.Debug.WriteLine("Fail Log", "error");
                }
                else
                {
                    this.RegisterUser(userinfo);

                    var handle = new Handler(Looper.MainLooper);
                    handle.Post(() =>
                    {
                        Toast.MakeText(this.context, "Register Me", ToastLength.Short).Show();
                    });
                }
            });

            await result;

            Compelte(sender, null);
        }

        //µù¥Uclient¤èªk
        private void RegisterAllEvent()
        {
            this._IhubProxy.On<SignalRMessage>("gotmsg", (msg) =>
            {
                var handler = new Handler(Looper.MainLooper);

                handler.Post(() =>
                {
                    if (this.GotMsgEvent != null)
                        this.GotMsgEvent(this, msg);
                });
            });

            //boardcast
            this._IhubProxy.On<List<UserInfo>>("refreschuserlist", (userinfo) =>
            {
                var handler = new Handler(Looper.MainLooper);

                handler.Post(() =>
                {
                    this.RefreschUserListEvent(this, userinfo);
                });
            });
        }

        #region call server side

        public void RegisterUser(UserInfo userinfo)
        {
            this._IhubProxy.Invoke<UserInfo>("RegisterUser", userinfo);
        }

        public void UnRegisterUser(UserInfo userinfo)
        {
            this._IhubProxy.Invoke<UserInfo>("UnRegisterUser", userinfo);
        }

        public void SendMsgTo(SignalRMessage msg)
        {
            this._IhubProxy.Invoke<SignalRMessage>("SendMsgTo", msg);
        }

        public void SendMsgToAll(SignalRMessage msg)
        {
            this._IhubProxy.Invoke<SignalRMessage>("SendMsgToAll", msg);
        }

        #endregion call server side

        public void CloseConnection()
        {
            this._hubconnection.Stop();
        }

        public void TriggerGotList()
        {
            this._IhubProxy.Invoke("TriggerGotList");
            //return null;
        }
    }

    public class SignalRMessage : EventArgs
    {
        public String From { set; get; }

        public String Email { set; get; }
        public String To { set; get; }
        public String Content { set; get; }
        public String ImagePath { set; get; }
        public bool HaveImage { set; get; }
    }
}