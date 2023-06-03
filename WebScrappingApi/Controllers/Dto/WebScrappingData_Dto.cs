using WebScrapper.Model;

namespace WebScrappingApi.Controllers.Dto
{
    public class WebScrappingData_Dto
    {
        public List<string> lstWebData_Not_Found { get; set; }
        public int Data_Count { get; set; }
        public List<ScrappingData_Model> lstData { get; set; }
        
        public WebScrappingData_Dto()
        {
            lstData = new List<ScrappingData_Model>();
            lstWebData_Not_Found = new List<string>();
        }
    }
}
