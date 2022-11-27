using yoga.Data;
using yoga.Models;

namespace yoga.Helpers
{
    public class NotificationHelper
    {
        private readonly YogaAppDbContext _db;
        public NotificationHelper(YogaAppDbContext db)
        {
            _db = db;
        }
        public void AddNotify(Notification obj)
        {
            
            try
            {
                _db.Notification.Add(obj);
                _db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                
                // log ex.message
            }
        }
    }
}