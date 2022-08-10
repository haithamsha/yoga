namespace yoga.ViewModels
{
    public class UserSetting
    {
        public UserSetting()
        {
            User_Subscribtions = new UserSubscribtions();
            MemshipCard = new MemCardStatusVM();
            TeacherLic = new TeacherLicStatusVM();
        }
        public virtual UserSubscribtions? User_Subscribtions { get; set; }
        public virtual MemCardStatusVM? MemshipCard { get; set; }
        public virtual TeacherLicStatusVM? TeacherLic { get; set; }
        
    }
}