using System;
using System.Collections.Generic;
using System.Linq;

namespace OPMF.Database
{
    public static class DatabaseAuxillary
    {
        public static void RemoveDuplicateIds<TItem>(List<TItem> items) where TItem : Entities.IId
        {
            IEnumerable<string> duplicatedSSN = from i in items group i by i.Id into g where g.Count() > 1 select g.Key;
            List<TItem> duplicated = items.FindAll(i => duplicatedSSN.Contains(i.Id));
            duplicated.ForEach(d => items.Remove(d));
        }
    }
}
