using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelStory.Model
{
    public class MaintenanceDB<T>
    {
        public bool Save(T item)
        {
            try
            {
                ManageDB.localDB.BeginTransaction();
                ManageDB.localDB.Insert(item);
            }
            catch (Exception)
            {
                ManageDB.localDB.Rollback();
                return false;
            }

            ManageDB.localDB.Commit();
            return true;
        }

        public bool Delete(T item)
        {
            try
            {
                ManageDB.localDB.Delete(item);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool SaveEdit(T item)
        {
            try
            {
                ManageDB.localDB.BeginTransaction();
                ManageDB.localDB.InsertOrReplace(item);
            }
            catch (Exception)
            {
                ManageDB.localDB.Rollback();
                return false;
            }

            ManageDB.localDB.Commit();
            return true;
        }

    }
}
