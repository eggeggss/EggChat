using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Common;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EggChat
{
    [Activity(Label = "EggChat", MainLauncher = false, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private int count = 1;
        public SignalRProxy mySignalR;
        private MyList adapter;

        private SwipeRefreshLayout refresh;

        protected override void OnCreate(Bundle bundle)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(bundle);

           

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            OverridePendingTransition(Resource.Animation.pull_in_left, Resource.Animation.push_out_right);

        }

        protected override void OnStart()
        {
            base.OnStart();

            var listview = this.FindViewById<ListView>(Resource.Id.listview);

            this.adapter = new MyList();
            this.adapter.Context = this;

            UserInfo userinfo = EggApp.eggChatDB.SelectUserInfo()[0];

            this.adapter.List = new List<UserInfo>() { userinfo };

            listview.Adapter = adapter;

            mySignalR = EggApp.mySignalR;// new SignalRProxy(this);

            // mySignalR.GotMsgEvent += MySignalR_GotMsgEvent;

            mySignalR.RefreschUserListEvent += (s, e) =>
            {
                var hander = new Handler(Looper.MainLooper);
                hander.Post(() =>
                {
                    var k = from q in e
                            where q.Email != userinfo.Email
                            select q;

                    this.adapter.List = k.ToList();
                    this.adapter.NotifyDataSetChanged();
                });
            };

            refresh = this.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher1);

            RefreshReConnet(userinfo);

            refresh.Refresh += (s1, e1) =>
            {
                RefreshReConnet(userinfo);
            };
        }


       

        private void RefreshReConnet(UserInfo userinfo)
        {
            refresh.Refreshing = true;

            mySignalR.OpenConnection(null, userinfo, (s1, e1) =>
            {
                mySignalR.TriggerGotList();
                refresh.Refreshing = false;
            });
        }

        public override bool OnKeyDown([GeneratedEnum] Keycode keyCode, KeyEvent e)
        {
            if (keyCode == Keycode.Back)
            {
                AndroidUtil.Dialog(this, "Information", "確定要離開嗎?", (s1,e1) => {
                    mySignalR.CloseConnection();
                    this.Finish();
                }, null);
            }
            return base.OnKeyDown(keyCode, e);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //mySignalR.CloseConnection();
        }
    }

    public class MyList : BaseAdapter<UserInfo>
    {
        public MainActivity Context;
        public List<UserInfo> List;

        public override UserInfo this[int position]
        {
            get
            {
                return this.List[position];
            }
        }

        public override int Count
        {
            get
            {
                return this.List.Count;
                //throw new NotImplementedException();
            }
        }

        public void DeleteItem(UserInfo item)
        {
            //throw new NotImplementedException();
        }

        public override long GetItemId(int position)
        {
            return position;
            //throw new NotImplementedException();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //前面幾筆進來時回收區還是空的
            //註冊在這邊是因為後面的view都用這個回收的所以初始註冊一次即可
            if (convertView == null)
            {
                convertView = this.Context.LayoutInflater.Inflate(Resource.Layout.ItemRow, null);

                var btn_open = convertView.FindViewById<Button>(Resource.Id.btn_open);

                btn_open.Click += (sender, obj) =>
                {
                    var thisbtn = sender as Button;
                    Int32 ll_position = Convert.ToInt32(thisbtn.Tag);
                    var item_row = this.List[ll_position];
                    String itemRowJson = JsonConvert.SerializeObject(item_row);

                    //Context.mySignalR.SendMsgTo(new SignalRMessage() { To = item_row.UserName, Content = "Hello" });

                    Intent intent = new Intent();
                    intent.SetClass(this.Context, typeof(ChatActivity));
                    intent.PutExtra("ItemRow", itemRowJson);

                    this.Context.StartActivity(intent);
                };
            }
            UserInfo item = List[position];
            var btn = convertView.FindViewById<Button>(Resource.Id.btn_open);
            btn.Tag = position;
            convertView.FindViewById<TextView>(Resource.Id.txtDescrip).Text = item.UserName;
            convertView.FindViewById<ImageView>(Resource.Id.head_image).SetImageResource(Convert.ToInt32(item.ImagePath));
            return convertView;

            //throw new NotImplementedException();
        }
    }
}