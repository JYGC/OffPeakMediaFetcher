using System;
using System.Collections.Generic;
using System.Text;

namespace OPMF.Database.Migration
{
    public class DatabaseChanges
    {
        // TODO: Implement database migration
        public static void Migrate()
        {
            Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.MigrateData();
            });
        }
    }
}
