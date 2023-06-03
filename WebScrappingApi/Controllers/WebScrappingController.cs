using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebScrapper.Model;
using WebScrappingApi.Controllers.Dto;
using WebScrappingApi.Scrapping_Manager.Ali_Baba_Scrapping_Manager;

namespace WebScrappingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebScrappingController : ControllerBase
    {
        public IScrapping_Manager Manager { get; set; }

        public WebScrappingController(IScrapping_Manager manager)
        {
            Manager = manager;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<WebScrappingData_Dto>> GetAllWebsitesBotData(string product)
        {
            try
            {
                if (!string.IsNullOrEmpty(product))
                {
                    var Return_Dto = new WebScrappingData_Dto();
                    var lstData = new List<ScrappingData_Model>();

                    #region MAKING EACH REQUEST HANDLER
                    List<Task> requestsTaskHandler = new List<Task>();

                    var taskAliBabaData = Task.Run(async () => await Manager.GetAliBabaData(product));
                    var taskMadeInChinaData = Task.Run(async () => await Manager.GetMadeInChinaData(product));
                    var taskAliExpressData = Task.Run(async () => await Manager.GetAliExpressData(product));
                    var taskAmazonData = Task.Run(async () => await Manager.GetAmazonData(product));
                    var taskDarazData = Task.Run(async () => await Manager.GetDarazData(product));
                    var taskMedOnlineData = Task.Run(async () => await Manager.GetMedOnlineData(product));
                    var taskChaseValueData = Task.Run(async () => await Manager.GetChaseValueData(product));
                    var taskChemistDirectData = Task.Run(async () => await Manager.GetChemistDirectData(product));
                    var taskBimedisData = Task.Run(async () => await Manager.GetBimedisData(product));
                    var taskDawaaiData = Task.Run(async () => await Manager.GetDawaaiData(product));
                    var taskNaheedData = Task.Run(async () => await Manager.GetNaheedData(product));
                    var taskChinaCNData = Task.Run(async () => await Manager.GetChinaCNData(product));
                    var taskEc21Data = Task.Run(async () => await Manager.GetEc21Data(product));
                    var taskDvagoData = Task.Run(async () => await Manager.GetDvagoData(product));
                    var taskTradeWheelData = Task.Run(async () => await Manager.GetTradeWheelData(product));
                    var taskPakWheelsData = Task.Run(async () => await Manager.GetPakWheelData(product));
                    var taskMediPkData = Task.Run(async () => await Manager.GetMediPkData(product));
                    var taskSehatData = Task.Run(async () => await Manager.GetSehatData(product));
                    var taskVietnameData = Task.Run(async () => await Manager.GetVietnamData(product));
                    var taskExportImportData = Task.Run(async () => await Manager.GetExportImportData(product));
                    var taskZaubaData = Task.Run(async () => await Manager.GetZaubaData(product));

                    #endregion  
                    #region ADDING THREADS INTO THREAD HANDLER

                    requestsTaskHandler.Add(taskAliBabaData);
                    requestsTaskHandler.Add(taskMadeInChinaData);
                    requestsTaskHandler.Add(taskAliExpressData);
                    requestsTaskHandler.Add(taskAmazonData);
                    requestsTaskHandler.Add(taskDarazData);
                    requestsTaskHandler.Add(taskMedOnlineData);
                    requestsTaskHandler.Add(taskChaseValueData);
                    requestsTaskHandler.Add(taskChemistDirectData);
                    requestsTaskHandler.Add(taskBimedisData);
                    requestsTaskHandler.Add(taskDawaaiData);
                    requestsTaskHandler.Add(taskNaheedData);
                    requestsTaskHandler.Add(taskChinaCNData);
                    requestsTaskHandler.Add(taskEc21Data);
                    requestsTaskHandler.Add(taskDvagoData);
                    requestsTaskHandler.Add(taskTradeWheelData);
                    requestsTaskHandler.Add(taskPakWheelsData);
                    requestsTaskHandler.Add(taskMediPkData);
                    requestsTaskHandler.Add(taskSehatData);
                    requestsTaskHandler.Add(taskVietnameData);
                    requestsTaskHandler.Add(taskExportImportData);
                    requestsTaskHandler.Add(taskZaubaData);

                    Task.WaitAll(requestsTaskHandler.ToArray());
                    #endregion
                    #region WAIT FOR DATA
                    var lstAliBabaData = await taskAliBabaData;
                    var lstMadeInChinaData = await taskMadeInChinaData;
                    var lstAliExpressData = await taskAliExpressData;
                    var lstAmazonData = await taskAmazonData;
                    var lstDarazData = await taskDarazData;
                    var lstMedOnlineData = await taskMedOnlineData;
                    var lstChaseValueData = await taskChaseValueData;
                    var lstChemistDirectData = await taskChemistDirectData;
                    var lstBimedisData = await taskBimedisData;
                    var lstDawaaiData = await taskDawaaiData;
                    var lstNaheedData = await taskNaheedData;
                    var lstChinaCNData = await taskChinaCNData;
                    var lstEc21Data = await taskEc21Data;
                    var lstDvagoData = await taskDvagoData;
                    var lstTradeWheelData = await taskTradeWheelData;
                    var lstPakwheelsData = await taskPakWheelsData;
                    var lstMediPKData = await taskMediPkData;
                    var lstSehatData = await taskSehatData;
                    var lstZaubaData = await taskZaubaData;
                    var lstVietnamData = await taskVietnameData;
                    var lstExportImportData = await taskExportImportData;
                    #endregion
                    #region ADD DATA TO MAIN LIST

                    if (lstAliBabaData.Count() > 0)
                    {
                        lstData.AddRange(lstAliBabaData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Ali Baba");
                    }
                    if (lstMadeInChinaData.Count() > 0)
                    {
                        lstData.AddRange(lstMadeInChinaData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Made In China");
                    }
                    if (lstAliExpressData.Count() > 0)
                    {
                        lstData.AddRange(lstAliExpressData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Ali Express");
                    }
                    if (lstAmazonData.Count() > 0)
                    {
                        lstData.AddRange(lstAmazonData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Amazon");
                    }
                    if (lstDarazData.Count() > 0)
                    {
                        lstData.AddRange(lstDarazData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Daraz");
                    }
                    if (lstMedOnlineData.Count() > 0)
                    {
                        lstData.AddRange(lstMedOnlineData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Med Online");
                    }
                    if (lstChaseValueData.Count() > 0)
                    {
                        lstData.AddRange(lstChaseValueData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Chase Value");
                    }
                    if (lstChemistDirectData.Count() > 0)
                    {
                        lstData.AddRange(lstChemistDirectData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Chemist Direct");
                    }
                    if (lstBimedisData.Count() > 0)
                    {
                        lstData.AddRange(lstBimedisData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Bimedis");
                    }
                    if (lstDawaaiData.Count() > 0)
                    {
                        lstData.AddRange(lstDawaaiData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Dawai");
                    }
                    if (lstNaheedData.Count() > 0)
                    {
                        lstData.AddRange(lstNaheedData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Naheed");
                    }
                    if (lstChinaCNData.Count() > 0)
                    {
                        lstData.AddRange(lstChinaCNData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("China CN");
                    }
                    if (lstEc21Data.Count() > 0)
                    {
                        lstData.AddRange(lstEc21Data);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Ec 21");
                    }
                    if (lstDvagoData.Count() > 0)
                    {
                        lstData.AddRange(lstDvagoData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Dvago");
                    }
                    if (lstTradeWheelData.Count() > 0)
                    {
                        lstData.AddRange(lstTradeWheelData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Trade Wheel");
                    }
                    if (lstPakwheelsData.Count() > 0)
                    {
                        lstData.AddRange(lstPakwheelsData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Pak Wheels");
                    }
                    if (lstMediPKData.Count() > 0)
                    {
                        lstData.AddRange(lstMediPKData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Medi PK");
                    }
                    if (lstSehatData.Count() > 0)
                    {
                        lstData.AddRange(lstSehatData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Sehat");
                    }
                    if (lstZaubaData.Count() > 0)
                    {
                        lstData.AddRange(lstZaubaData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Zauba");
                    }
                    if (lstVietnamData.Count() > 0)
                    {
                        lstData.AddRange(lstVietnamData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Vietnam");
                    }
                    if (lstExportImportData.Count() > 0)
                    {
                        lstData.AddRange(lstExportImportData);
                    }
                    else
                    {
                        Return_Dto.lstWebData_Not_Found.Add("Export Import");
                    }
                    #endregion
                    #region RETURN DATA

                    if (lstData.Count > 0)
                    {
                        Return_Dto.lstData = lstData;
                        Return_Dto.Data_Count = lstData.Count;
                        return Ok(Return_Dto);
                    }
                    else
                    {
                        return NotFound($"The data about the product {product} not found");
                    }
                    #endregion
                }
                else
                {
                    return BadRequest("Enter product name to search!!");
                }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
