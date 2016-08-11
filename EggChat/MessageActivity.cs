using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EggChat
{
    public class MessageActivity : Fragment
    {
        public ChatActivity _context;
        private ICommunicateToMsg _commuticate;
        public ListView _listMsg;
        public MsgAdapter _msgAdapter;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Message, null);

            _listMsg = view.FindViewById<ListView>(Resource.Id.listMsg);

            _msgAdapter = new MsgAdapter();

            _context = (ChatActivity)this.Activity;
            _commuticate = this._context;

            _msgAdapter.context = _context;

            var editMsg = view.FindViewById<EditText>(Resource.Id.editMsg);

            var btnSendMsg = view.FindViewById<Button>(Resource.Id.btnSendMsg);

            btnSendMsg.Click += (s1, e1) =>
            {
                _commuticate.SendMessage(editMsg.Text);
            };

            List<UserInfoLog> logs = _commuticate.GetMessage();

            _msgAdapter.UserInfoLogs = logs;

            _listMsg.Adapter = _msgAdapter;

            return view;
        }

        public void NotifyListChange()
        {
            var msg_list = _commuticate.GetMessage();

            _msgAdapter.UserInfoLogs = msg_list;

            _msgAdapter.NotifyDataSetChanged();

            _listMsg.Post(() =>
            {
                var hander = new Handler(Looper.MainLooper);
                hander.Post(() =>
                {
                    _listMsg.SetSelection(_msgAdapter.Count - 1);
                });
            });
        }
    }

    public class MsgAdapter : BaseAdapter<UserInfoLog>
    {
        public ChatActivity context;
        public List<UserInfoLog> UserInfoLogs = new List<UserInfoLog>();

        public override UserInfoLog this[int position]
        {
            get
            {
                return this.UserInfoLogs[position];
                // throw new NotImplementedException();
            }
        }

        public override int Count
        {
            get
            {
                return this.UserInfoLogs.Count;
                //throw new NotImplementedException();
            }
        }

        public override long GetItemId(int position)
        {
            return position;
            //throw new NotImplementedException();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = this.context.LayoutInflater.Inflate(Resource.Layout.MsgItemRow, null);
                var txtMsg1 = convertView.FindViewById<TextView>(Resource.Id.txtMsg);
                txtMsg1.Tag = position;
            }

            var txtMsg = convertView.FindViewById<TextView>(Resource.Id.txtMsg);

            var userInfoLog = this.UserInfoLogs[position];

            if (userInfoLog.Galary == "left")
            {
                txtMsg.Gravity = GravityFlags.Left;

                txtMsg.Text = "       " + userInfoLog.From + " :" + userInfoLog.Content;
                txtMsg.SetBackgroundResource(Resource.Drawable.leftmsg);
            }
            else
            {
                txtMsg.Gravity = GravityFlags.Right;

                txtMsg.Text = userInfoLog.Content + "       ";
                txtMsg.SetBackgroundResource(Resource.Drawable.rightmsg);
            }

            // txtMsg.TextAlignment = TextAlignment.Center;

            //txtMsg.SetBackgroundResource(Resource.Drawable.bubble);
            // txtMsg.Background.SetTintMode(Android.Graphics.PorterDuff.Mode.DstOver);
            return convertView;
            //throw new NotImplementedException();
        }
    }
}