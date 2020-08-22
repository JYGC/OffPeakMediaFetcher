using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using LiteDB;

namespace OPMF.Database
{
    public class DatabaseAdapter<TDerivedCalls> where TDerivedCalls : IDbCalls, new()
    {
        private LiteDatabase __db;
        private TDerivedCalls __dbCalls;

        public DatabaseAdapter(string dbname)
        {
            __db = new LiteDatabase(Path.Join(Config.ConfigHelper.GetConfig().AppDataDirectory, dbname));
            __dbCalls = new TDerivedCalls();
            __dbCalls.DB = __db;
        }

        public TDerivedCalls DBCall
        {
            get
            {
                return __dbCalls;
            }
        }
    }
}
