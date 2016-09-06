using PublicStruct.cs;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;


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

        public  List<UserInfo> SelectUserInfo()
        {
            return _conn.Table<UserInfo>().ToList<UserInfo>();
        }

        public  void InsertUserInfo(UserInfo userinfo)
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
            var logs = this.SelectUserInfoLogs(log.Email);

            var findLog = logs.Find((userinfoHist) =>
            {
                return userinfoHist.Content == log.Content;
            });

            if (findLog == null)
                _conn.Insert(log);
        }
    }
    
}