using yoga.Data;

namespace yoga.Models
{
    public class WFHistoryManager : IWFHistoryManager
    {
        private readonly YogaAppDbContext _db;
        public WFHistoryManager(YogaAppDbContext db)
        {
            _db = db;
        }
        public int Save(WFHistory wfHistory)
        {
            try
            {
                _db.WFHistory.Add(wfHistory);
                return _db.SaveChanges();
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }

    }
}