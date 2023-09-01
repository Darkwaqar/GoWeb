using System.Collections.Generic;

namespace Nop.Web.Areas.MServices.Models.Product
{
    public class SpecificationAttributeModelAPI
    {
        public int id { get; set; }
        public string name { get; set; }
        public int displayOrder { get; set; }
        public List<SpecificationAttributeOptionModelAPI> options { get; set; }
        public SpecificationAttributeModelAPI()
        {
            options = new List<SpecificationAttributeOptionModelAPI>();
        }
    }

    public class SpecificationAttributeOptionModelAPI
    {
        public int id { get; set; }
        public string name { get; set; }
        public int displayOrder { get; set; }
    }

}