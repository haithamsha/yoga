using System.ComponentModel.DataAnnotations;

namespace yoga.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        public string Code{get;set;}
        public string EnName {get;set;}
        public string ArName {get;set;}
        public string EnNationalityName {get;set;}
        public string ArNationalityName {get;set;}
    }
}