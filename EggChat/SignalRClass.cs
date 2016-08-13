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
using System.Threading.Tasks;

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

        public event EventHandler ReceiveLocationEvent;

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
            Task result = null;
            try
            {
                result = this._hubconnection.Start().ContinueWith((hubStat) =>
                {
                    try
                    {
                        if (hubStat.IsFaulted)
                        {
                            Util.ToastHander(this.context, "ConnectionFail");
                        }
                        else
                        {
                            this.RegisterUser(userinfo);

                            Util.ToastHander(this.context, "Register Me");
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.ToastHander(this.context, String.Format("{0},{1}", "ConnectionContinueFail:", ex.Message));
                       
                    }
                });
            }
            catch (Exception ex)
            {
                Util.ToastHander(this.context, String.Format("{0},{1}", "�P���A���s�u�ɵo�Ϳ��~:", ex.Message));
            }

            await result;

            Compelte(sender, null);
        }

        //���Uclient��k
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
                    if (this.RefreschUserListEvent != null)
                        this.RefreschUserListEvent(this, userinfo);
                });
            });

            //receiveFriendLocation
            this._IhubProxy.On<UserInfo>("receiveLocation", (userinfo) =>
            {
                var handler = new Handler(Looper.MainLooper);

                handler.Post(() =>
                {
                    if (this.ReceiveLocationEvent != null)
                        this.ReceiveLocationEvent(this, userinfo);
                });
            });
        }

        #region call server side
        public void RegisterUser(UserInfo userinfo)
        {
            if (this._hubconnection.State == ConnectionState.Connected)
            {
                this._IhubProxy.Invoke<UserInfo>("RegisterUser", userinfo);
            }
            else
            {
                Toast.MakeText(this.context, "�Pserver�|���s�u", ToastLength.Short).Show();
            }
        }

        public void UnRegisterUser(UserInfo userinfo)
        {
            if (this._hubconnection.State == ConnectionState.Connected)
            {
                this._IhubProxy.Invoke<UserInfo>("UnRegisterUser", userinfo);
            }
            else
            {
                Toast.MakeText(this.context, "�Pserver�|���s�u", ToastLength.Short).Show();
            }
        }

        public void SendMsgTo(SignalRMessage msg)
        {
            if (this._hubconnection.State == ConnectionState.Connected)
            {
                this._IhubProxy.Invoke<SignalRMessage>("SendMsgTo", msg);
            }
            else
            {
                Toast.MakeText(this.context, "�Pserver�|���s�u", ToastLength.Short).Show();
            }
        }

        public void SendMsgToAll(SignalRMessage msg)
        {
            if (this._hubconnection.State == ConnectionState.Connected)
            {
                this._IhubProxy.Invoke<SignalRMessage>("SendMsgToAll", msg);
            }
            else
            {
                Toast.MakeText(this.context, "�Pserver�|���s�u", ToastLength.Short).Show();
            }
        }

        public void SendLocation(UserInfo myinfo)
        {
            if (this._hubconnection.State == ConnectionState.Connected)
            {
                this._IhubProxy.Invoke<UserInfo>("SendLocation", myinfo);
            }
            else
            {
                Toast.MakeText(this.context, "�Pserver�|���s�u", ToastLength.Short).Show();
            }
        }

        #endregion call server side

        public void CloseConnection()
        {
            if (this._hubconnection.State == ConnectionState.Connected)
                this._hubconnection.Stop();
        }

        public void TriggerGotList()
        {
            if (this._hubconnection.State == ConnectionState.Connected)
            {
                this._IhubProxy.Invoke("TriggerGotList");
            }
            else
            {
                Toast.MakeText(this.context, "�Pserver�|���s�u", ToastLength.Short).Show();
            }
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