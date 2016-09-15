
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PublicStruct.cs
{
    public class UserInfoBase : EventArgs
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int IDUserInfo { set; get; }
        public String UserName { set; get; }
        public String Email { set; get; }
        public String ImageContent { set; get; }
        public int StatVoid { set; get; }
        public DateTime DtCreate { set; get; }
        public DateTime DtUpdate { set; get; }
    }


    public class UserInfo : UserInfoBase
    {
        public String ContextId { set; get; }
        public String ImagePath { set; get; }
        public bool HaveImage { set; get; }
        public bool HaveNewMsg { set; get; }
        public double Lat { set; get; }
        public double Lon { set; get; }
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

    public class AppSetting
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ID { set; get; }
        public String WebAPI { set; get; }
    }

    public class Item : IComparable<Item>
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int ID { set; get; }

        public string Descrip { set; get; }

        public string Link { set; get; }

        public int CompareTo(Item other)
        {
            return this.ID > other.ID ? -1 : 1;
            // throw new NotImplementedException();
        }
    }

}
