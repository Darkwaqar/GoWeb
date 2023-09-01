using System.Collections.Generic;

namespace Nop.Web.Areas.Mservices.Models.Common
{
    public class CountryandStatesModel
    {
        public CountryandStatesModel()
        {
            Countries = new List<Country>();
        }
        public IList<Country> Countries { get; set; }

        public partial class Country
        {
            public Country()
            {
                States = new List<StateProvince>();
            }

            public int CountryId { get; set; }

            public string Name { get; set; }

            public IList<StateProvince> States { get; set; }
        }

        public partial class StateProvince
        {
            public int ProvinceId { get; set; }

            public string Name { get; set; }
            
        }

    }
}