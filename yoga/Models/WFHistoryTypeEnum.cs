namespace yoga.Models
{
    public enum WFHistoryTypeEnum
    {
        ApproveMembership = 1,
        RejectMembership = 2,
        ApproveTeacherLicense_step1_BasicInformation = 3,
        ApproveTeacherLicense_step2_PayExamFees = 4,
        ApproveTeacherLicense_step3_TakeTheExam = 5,
        ApproveTeacherLicense_step4_PassTheExam = 6,
        ApproveTeacherLicense_step5_PayLicenceFees = 7,

        RejectTeacherLicense_step1_1_BasicInformation = 8,
        RejectTeacherLicense_step2_2_PayExamFees = 9,
        RejectTeacherLicense_step3_3_TakeTheExam = 10,
        RejectTeacherLicense_step4_4_PassTheExam = 11,
        RejecteacherLicense_step5_5_PayLicenceFees = 12,

        CreateMembership =13,
        CreatTeacherLicense = 14
    }
}