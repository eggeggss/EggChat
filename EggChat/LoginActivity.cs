using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Common;
using PublicStruct.cs;
using Common.Util;
using Newtonsoft.Json;

namespace EggChat
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : Activity
    {
        int source_id=0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);

            this.SetContentView(Resource.Layout.Login);
      
            var loginLinear= FindViewById<LinearLayout>(Resource.Id.loginLayout);

            loginLinear.SetBackgroundColor(Android.Graphics.Color.DarkGray);

            var gridview = FindViewById<GridView>(Resource.Id.gridview);

            gridview.Adapter = new ImageAdapter(this);         

            gridview.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                ImageAdapter adapter = (ImageAdapter)gridview.Adapter;

                if (gridview.GetChildAt(oldPosition) !=null)
                  gridview.GetChildAt(oldPosition).SetBackgroundColor(Android.Graphics.Color.DarkGray);

                gridview.GetChildAt(args.Position).SetBackgroundColor(Android.Graphics.Color.Red);

                oldPosition = args.Position;

                source_id= ((ImageAdapter)gridview.Adapter).thumbIds[args.Position];
            };

            UserInfo user_info = new UserInfo();
        
            var editUserName= this.FindViewById<EditText>(Resource.Id.editUserName);
           
            var editEmail = this.FindViewById<EditText>(Resource.Id.editEmail);

            var btnLogin= this.FindViewById<Button>(Resource.Id.btnLogin);

            btnLogin.Click += (s1, e1) => {

                user_info.UserName = editUserName.Text;
                user_info.Email = editEmail.Text;
                user_info.ImagePath = source_id.ToString();

                if (String.IsNullOrEmpty(user_info.UserName))
                {
                    Toast.MakeText(this, "Please Input UserName", ToastLength.Short).Show();
                    return;
                }

                if (String.IsNullOrEmpty(user_info.Email))
                {
                    Toast.MakeText(this, "Please Input Email", ToastLength.Short).Show();
                    return;
                }
                if (source_id==0)
                {
                    Toast.MakeText(this, "Please Choose a Picture", ToastLength.Short).Show();
                    return;
                }

                
                try
                {
                    String jsonUserInfo = JsonConvert.SerializeObject(user_info);

                    try
                    {
                        WebApi.UploadJsonData("RegistUserInfo", jsonUserInfo);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(String.Format("{0}=>{1}","Please Check NetworkState!",ex.Message));
                    }

                    EggApp.eggChatDB.InsertUserInfo(user_info);

                    Intent intent = new Intent();
                    intent.SetClass(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                catch (Exception ex)
                {
                    AndroidUtil.ToastHander(this, ex.Message);
                }
               

                //System.Diagnostics.Debug.Write(string.Format("{0},{1},{2}",user_info.UserName,user_info.Email,user_info.ImagePath), "Debug");

            };
            
            // Create your application here
        }
        private int oldPosition=0;
    }
    
    public class ImageAdapter : BaseAdapter
    {
        private Context context;

     
        public ImageAdapter(Context c)
        {
            context = c;
        }

        public override int Count
        {
            
             get { return thumbIds.Length; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return 0;
        }

        // create a new ImageView for each item referenced by the Adapter
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if (convertView == null)
            {  // if it's not recycled, initialize some attributes
                imageView = new ImageView(context);
                imageView.LayoutParameters = new GridView.LayoutParams(80,80);
                imageView.SetScaleType(ImageView.ScaleType.FitStart);
                //imageView.SetPadding(0, 2, 2, 2);
            }
            else
            {
                imageView = (ImageView)convertView;
            }


            imageView.SetImageResource(thumbIds[position]);
            imageView.SetBackgroundColor(Android.Graphics.Color.DarkGray);    
            imageView.SetBaseline(1);


            return imageView;
        }

        
        // references to our images
        public int[] thumbIds = {
        Resource.Drawable.b1, Resource.Drawable.b2,
        Resource.Drawable.b3, Resource.Drawable.b4,
        Resource.Drawable.b5, Resource.Drawable.b6,
        Resource.Drawable.b7, Resource.Drawable.b8,
        Resource.Drawable.b9,Resource.Drawable.b10,
        Resource.Drawable.b11,Resource.Drawable.b12,
        Resource.Drawable.b13,Resource.Drawable.b14
      };
    }
}