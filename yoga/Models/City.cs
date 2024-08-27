using System.ComponentModel.DataAnnotations;
using yoga.Migrations;

namespace yoga.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }
        public string Code{get;set;}
        public string EnName {get;set;}
        public string ArName {get;set;}
        public Country Country { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int CityDataId { get; set; }
    }
}