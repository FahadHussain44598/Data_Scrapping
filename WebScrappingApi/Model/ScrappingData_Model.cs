using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScrapper.Model
{
    public class ScrappingData_Model
    {
        public string Website { get; set; }
        public string product { get; set; }
        public string Product_Description { get; set; }
        public string Product_Link { get; set; }
        public string Quantity { get; set; }
        public string Currency { get; set; }
        public string Unit { get; set; }
        public string Max_Price { get; set; }
        public string Min_Price { get; set;}

    }
}
