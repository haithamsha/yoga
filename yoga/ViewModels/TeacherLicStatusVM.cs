namespace yoga.ViewModels
{
    public class TeacherLicStatusVM
    {
        public int Status { get; set; }
        public bool FinalApprove { get; set; }
        public string? ExpireDate { get; set; }
        public string? IssueDate { get; set; }
        public bool PayFees { get; set; }
        public bool PayExamFees { get; set; }
        public bool TakeExam { get; set; }
        public bool PassExam { get; set; }
        public string ReceiptCopy { get; set; }
        public string ReceiptCopyLic { get; set; }
        public string Serial { get; set; }
        public string ExamLocation { get; set; }
        public string RejectReason { get; set; }
        public List<TechearMemberShipTestVM> TechearMemberShipTests { get; set; }
    }
}