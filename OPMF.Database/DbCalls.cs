using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LiteDB;

namespace OPMF.Database
{
    public interface IDbCalls
    {
        LiteDatabase DB { set; }
    }

    public class DbCalls<TItem> : IDbCalls where TItem : Entities.IId
    {
        protected readonly string _keyName = "_id";

        protected LiteDatabase _db;
        protected ILiteCollection<TItem> _collection;

        public LiteDatabase DB
        {
            set
            {
                _db = value;
                _collection = _db.GetCollection<TItem>();
            }
        }
    }
}
