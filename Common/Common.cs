using Microsoft.AspNet.SignalR.Client;
using PublicStruct.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public interface ICrossDevice
    {
        void SoundPlay();

        //跨執行緒
        void Dispatcher(EventHandler eventhandler, object obj, EventArgs e);

        ///message
        void PostMessage(String message);
    }

    public class SignalRProxy
    {
        private readonly ICrossDevice _crossDevice;
        public HubConnection _hubconnection;
        private IHubProxy _IhubProxy;

        public event EventHandler GotMsgEvent;

        public event EventHandlerUserList RefreschUserListEvent;

        public event EventHandler ReceiveLocationEvent;

        public SignalRProxy(ICrossDevice _crossDevice)
        {
            this._crossDevice = _crossDevice;
            Initial();
        }

        public void PostMessage(String message)
        {
            _crossDevice.PostMessage(message);
        }

        public void Dispatcher(EventHandler eventhandler, object obj, EventArgs e)
        {
            eventhandler.Invoke(obj, e);
        }

        public void Initial()
        {
            this._hubconnection = new HubConnection(WebResource.signalR_path);
            
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
                            this.PostMessage("ConnectionFail");
                        }
                        else
                        {
                            this.RegisterUser(userinfo);

                            //PostMessage("跟伺服器連線");
                        }
                    }
                    catch (Exception ex)
                    {
                        PostMessage(String.Format("{0},{1}", "ConnectionContinueFail:", ex.Message));
                    }
                });
            }
            catch (Exception ex)
            {
                this.PostMessage(String.Format("{0},{1}", "與伺服器連線時發生錯誤:", ex.Message));
            }

            await result;

            Compelte(sender, null);
        }

        //註冊client方法
        private void RegisterAllEvent()
        {
            this._IhubProxy.On<SignalRMessage>("gotmsg", (msg) =>
            {
                this.Dispatcher((sender, e) =>
                {
                    if (this.GotMsgEvent != null)
                    {
                        this.GotMsgEvent(this, msg);
                        _crossDevice.SoundPlay();
                    }
                }, null, null);
            });

            //boardcast
            this._IhubProxy.On<List<UserInfo>>("refreschuserlist", (userinfo) =>
            {
                this.Dispatcher((sender, e) =>
                {
                    if (this.RefreschUserListEvent != null)
                        this.RefreschUserListEvent(this, userinfo);
                }, null, null);
            });

            //receiveFriendLocation
            this._IhubProxy.On<UserInfo>("receiveLocation", (userinfo) =>
            {
                this.Dispatcher((sender, e) =>
                {
                    if (this.ReceiveLocationEvent != null)
                        this.ReceiveLocationEvent(this, userinfo);
                }, null, null);
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
                PostMessage("與server尚未連線");
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
                PostMessage("與server尚未連線");
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
                PostMessage("與server尚未連線");
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
                PostMessage("與server尚未連線");
                //Toast.MakeText(this.context, "與server尚未連線", ToastLength.Short).Show();
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
                PostMessage("與server尚未連線");
                //Toast.MakeText(this.context, "與server尚未連線", ToastLength.Short).Show();
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
                PostMessage("與server尚未連線");
                //Toast.MakeText(this.context, "與server尚未連線", ToastLength.Short).Show();
            }
        }
    }

    public delegate void EventHandlerUserList(object sender, List<UserInfo> e);

}