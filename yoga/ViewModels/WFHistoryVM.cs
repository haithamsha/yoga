namespace yoga.ViewModels
{
    public class WFHistoryVM
    {
        public int HistoryId { get; set; }
        public string HistoryType { get; set; }
        public int RecordId { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}