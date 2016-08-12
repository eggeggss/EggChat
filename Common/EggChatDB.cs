using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Microsoft.AspNet.SignalR.Client;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class EggChatDB
    {
        private SQLiteConnection _conn;

        public EggChatDB(String folder)
        {
            _conn = new SQLiteConnection(System.IO.Path.Combine(folder, "droid.db"));
            //setting
            _conn.CreateTable<UserInfo>();
            _conn.CreateTable<UserInfoLog>();
        }

        public List<UserInfo> SelectUserInfo()
        {
            return _conn.Table<UserInfo>().ToList<UserInfo>();
        }

        public void InsertUserInfo(UserInfo userinfo)
        {
            _conn.Insert(userinfo);
        }

        public List<UserInfoLog> SelectUserInfoLogs(String Email)
        {
            var logs = _conn.Table<UserInfoLog>().ToList<UserInfoLog>();

            List<UserInfoLog> newLogs = new List<UserInfoLog>();
            foreach (var log in logs)
            {
                if (log.Email == Email)
                {
                    newLogs.Add(log);
                }
            }
            return newLogs;
            //return (q == null) ? null : q.ToList();
        }

        public void InsertUserInfoLogs(UserInfoLog log)
        {
            _conn.Insert(log);
        }
    }

    public class UserInfo : EventArgs
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int IDUserInfo { set; get; }

        public String UserName { set; get; }
        public String ContextId { set; get; }
        public String Email { set; get; }
        public String ImagePath { set; get; }
        public bool HaveImage { set; get; }
        public bool HaveNewMsg { set; get; }
    }

    public class UserInfoLog
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int IDUserInfoLog { set; get; }

        public String Email { set; get; } //relation key
        public String From { set; get; }
        public String To { set; get; }
        public String Content { set; get; }
        public String ImagePath { set; get; }
        public bool HaveImage { set; get; }
        public DateTime DTCreate { set; get; }
        public String Galary { set; get; }

        public double Lat { set; get; }
        public double Lon { set; get; }
    }
}