using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggChat
{
    public interface ICommunicateToMsg
    {
        void SendMessage(String msg);

        List<UserInfoLog> GetMessage();
    }

    [Activity(Label = "ChatActivity", ConfigurationChanges = Android.Content.PM.ConfigChanges.Keyboard | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ChatActivity : Activity, ICommunicateToMsg
    {
        public UserInfo FriedInfo;
        public UserInfo SelfInfo;
        public SignalRProxy SignalRProxy;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);

            SetContentView(Resource.Layout.Chat);
            this.SignalRProxy = EggApp.mySignalR;
        }

        protected override void OnResume()
        {
            base.OnResume();

            String UserInfoString = this.Intent.GetStringExtra("ItemRow");

            FriedInfo = JsonConvert.DeserializeObject<UserInfo>(UserInfoString);

            var SelfUserInfos = EggApp.eggChatDB.SelectUserInfo();

            SelfInfo = SelfUserInfos[0];

            var btnChat = this.FindViewById<Button>(Resource.Id.btnChat);

            First();

            btnChat.Click += (sender, obj) =>
            {
                First();
            };

            var btnMap = this.FindViewById<Button>(Resource.Id.btnMap);
            btnMap.Click += (sender, obj) =>
            {
                Second();
            };
            FragmentManager.PopBackStack(null, FragmentManager.PopBackStackInclusive);

            EggApp.mySignalR.GotMsgEvent += MySignalR_GotMsgEvent;
        }

        private void MySignalR_GotMsgEvent(object sender, EventArgs e)
        {
            var hander = new Handler(Looper.MainLooper);
            hander.Post(() =>
            {
                SignalRMessage msg = e as SignalRMessage;

                UserInfoLog log = new UserInfoLog()
                {
                    From = msg.From,
                    To = msg.To,
                    Email = msg.Email,
                    Content = msg.Content,
                    Galary = "left"
                };
                
                EggApp.eggChatDB.InsertUserInfoLogs(log);

                Toast.MakeText(this, String.Format("{0} Say:{1}", msg.From, msg.Content), ToastLength.Short).Show();
                                     
                MessageActivity msgFragemant = this.FragmentManager.FindFragmentByTag<MessageActivity>("first");

                if (msgFragemant != null)
                {
                    msgFragemant.NotifyListChange();
                }
            });
        }

        private void ClearStack()
        {
            FragmentManager.PopBackStack(null, FragmentManager.PopBackStackInclusive);
        }

        private void First()
        {
            FragmentManager.
            BeginTransaction().
            Replace(Resource.Id.frameLayout, new MessageActivity() { }, "first").
            Commit();

            this.ClearStack();
        }

        private void Second()
        {
            FragmentManager.
            BeginTransaction().
            Replace(Resource.Id.frameLayout, new MapActivity() { }, "second").
            Commit();
        }

        protected override void OnPause()
        {
            base.OnPause();
            //EggApp.mySignalR.GotMsgEvent -= MySignalR_GotMsgEvent;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void SendMessage(string msg)
        {
            SignalRMessage smsg = new SignalRMessage() { To = FriedInfo.UserName, From = SelfInfo.UserName, Content = msg, Email = SelfInfo.Email };
            EggApp.mySignalR.SendMsgTo(smsg);

            UserInfoLog logs = new UserInfoLog()
            {
                Email = FriedInfo.Email,
                From = FriedInfo.UserName,
                To = SelfInfo.UserName,
                Content = msg,
                DTCreate = DateTime.Now,
                Galary = "right"
            };
            EggApp.eggChatDB.InsertUserInfoLogs(logs);

            MessageActivity msgFragemant = this.FragmentManager.FindFragmentByTag<MessageActivity>("first");

            if (msgFragemant != null)
                msgFragemant.NotifyListChange();
            //throw new NotImplementedException();
        }

        public List<UserInfoLog> GetMessage()
        {
            return EggApp.eggChatDB.SelectUserInfoLogs(FriedInfo.Email);
            //throw new NotImplementedException();
        }
    }
}