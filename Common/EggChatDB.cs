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
        public EggChatDB (String folder)
        {
            _conn = new SQLiteConnection(System.IO.Path.Combine(folder, "droid.db"));
            //setting
            
            _conn.CreateTable<UserInfo>();
            _conn.CreateTable<UserInfoLog>();
            _conn.CreateTable<UserInfoList>();
        }

        /************SelectUserInfo****************/


        #region SelectUserInfo()
        public List<UserInfo> SelectUserInfo()
        {
            return _conn.Table<UserInfo>().ToList<UserInfo>();
        }
        #endregion

        #region InsertUserInfo
        public void InsertUserInfo(UserInfo userinfo)
        {
            _conn.Insert(userinfo);
        }
        #endregion


        /************UserInfoLogs****************/

        #region SelectUserInfoLogs
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
        #endregion

        #region InsertUserInfoLogs
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
        #endregion


        /************UserInfoList****************/

        #region InsertAllUserInfoList

        public void InsertAllUserInfoList(List<UserInfoList> users)
        {
            _conn.DeleteAll<UserInfoList>();

            _conn.InsertAll(users);
        }

        #endregion

        #region SelectAllUserList

        public List<UserInfoList> SelectAllUserList()
        {
            return _conn.Table<UserInfoList>().ToList<UserInfoList>();          
        }

        #endregion

        #region InserOneUserInforList()

        public void InserOneUserInfoList(UserInfoList user)
        {
            if (!(FindOneUserInfoList(user)))
            {
                _conn.Insert(user);
            }
        }

        #endregion

        #region FindOneUserInfoList

        public bool FindOneUserInfoList(UserInfoList user)
        {
            var users = SelectAllUserList();

            var result= users.Find((e) => { return e.Email == user.Email; });

            return (result == null) ? false : true;

        }


        #endregion

    }

    public class UserInfoList:UserInfo 
    {
       
    }
    
}