namespace yoga.ViewModels
{
    public class TeacherLicStatusVM
    {
        public int Status { get; set; }
        public bool FinalApprove { get; set; }
        public string? ExpireDate { get; set; }
        public bool PayFees { get; set; }
        public bool PayExamFees { get; set; }
        public bool TakeExam { get; set; }
        public bool PassExam { get; set; }
    }
}