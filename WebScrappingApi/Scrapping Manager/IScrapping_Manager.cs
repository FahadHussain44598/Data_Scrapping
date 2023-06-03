using Microsoft.AspNetCore.Mvc;
using System.Collections;
using WebScrapper.Model;

namespace WebScrappingApi.Scrapping_Manager.Ali_Baba_Scrapping_Manager
{
    public interface IScrapping_Manager
    {
        public Task<IEnumerable<ScrappingData_Model>> GetAliBabaData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetAliExpressData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetMadeInChinaData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetAmazonData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetDarazData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetMedOnlineData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetChaseValueData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetChemistDirectData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetBimedisData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetDawaaiData(string product);
        public  Task<IEnumerable<ScrappingData_Model>> GetNaheedData(string product);
        public  Task<IEnumerable<ScrappingData_Model>> GetChinaCNData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetEc21Data(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetDvagoData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetTradeWheelData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetPakWheelData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetMediPkData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetSehatData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetExportImportData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetZaubaData(string product);
        public Task<IEnumerable<ScrappingData_Model>> GetVietnamData(string product);


    }
}
