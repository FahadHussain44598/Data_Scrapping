using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using RestSharp;
using System.Collections;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebScrapper.Model;
using WebScrappingApi.Utilities;

namespace WebScrappingApi.Scrapping_Manager.Ali_Baba_Scrapping_Manager
{
    public class Scrapping_Manager : IScrapping_Manager
    {
        private ProxyDataReponses Responses { get; set; }
        public Scrapping_Manager()
        {
            Responses = new ProxyDataReponses();
        }   

        public async Task<IEnumerable<ScrappingData_Model>> GetAliBabaData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    //var Url = $"https://app.scrapingbee.com/api/v1/?api_key=70E99U1CM2EW7379SCUHB2BI2TW1BSWP6BG6XVBXK9GL22FT8BC5WJYHD8AO3N84CPDPDWAABNBD01EK&url=https%3A%2F%2Fwww.alibaba.com%2Fproducts%2F{product}.html%3FIndexArea%3Dproduct_en";
                    //var client = new RestClient();
                    //var request = new RestRequest(Url);
                    //RestResponse response = await client.ExecuteGetAsync(request);
                    var url = $"https://www.alibaba.com/products/{product}.html?IndexArea=product_en";
                    var Content = await Responses.GetWebDataFromZenrowsAsync(url);
                    #endregion

                    //if (response.IsSuccessStatusCode)
                    //{
                        if (!string.IsNullOrEmpty(Content))
                        {
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(Content);

                            #region XPATHS
                            var TitleXpath = "*//h2[@class=\"search-card-e-title\"]/a/@href";
                            var PriceXPath = "*//div[@class='search-card-e-price-main']";
                            var QuantityXPath = "*//div[contains(@class, 'search-card-m-sale-features__item')]";

                            #endregion
                            #region GET DATA 
                            var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                            var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                            var QuantityNodes = doc.DocumentNode.SelectNodes(QuantityXPath);

                            #endregion


                            if (titleNodes != null && PriceNodes != null && QuantityNodes != null)
                            {
                                for (int i = 0; i < titleNodes.Count; i++)
                                {
                                    var ScrapData = new ScrappingData_Model();

                                    #region PARSE DATA

                                    var link = titleNodes[i].GetAttributeValue("href", "");
                                    var title = titleNodes[i].InnerText;
                                    var Price = PriceNodes[i].InnerText;
                                    var Quantity = QuantityNodes[i].InnerText.Replace("Min. order:", "").Trim();

                                    #endregion
                                    #region SET MODEL
                                    ScrapData.Website = "Ali Baba";
                                    ScrapData.product = product;
                                    ScrapData.Product_Description = title == string.Empty ? string.Empty : title;
                                    ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");

                                    #region SET QUANTITY AND UNIT

                                    var splitQuantity = Quantity.Split(' ').ToList();
                                    if (splitQuantity.Count > 0)
                                    {
                                        ScrapData.Quantity = splitQuantity[0];
                                        ScrapData.Unit = splitQuantity[1];

                                    }
                                    else
                                    {
                                        ScrapData.Quantity = Quantity == string.Empty ? string.Empty : Quantity;
                                    }
                                    #endregion
                                    #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                    if (!string.IsNullOrEmpty(Price))
                                    {
                                        if (Price.Contains('-'))
                                        {
                                            var splitPrice = Price.Split('-').ToList();
                                            ScrapData.Max_Price = splitPrice[0];
                                            ScrapData.Min_Price = splitPrice[1];

                                            if (Price.Contains("$"))
                                            {
                                                ScrapData.Currency = "USD";
                                            }
                                            else
                                            {
                                                ScrapData.Currency = "PKR";
                                            }
                                        }
                                        else
                                        {
                                            ScrapData.Max_Price = Price;
                                            ScrapData.Min_Price = Price;

                                            if (Price.Contains("$"))
                                            {
                                                ScrapData.Currency = "USD";
                                            }
                                            else
                                            {
                                                ScrapData.Currency = "PKR";
                                            }
                                        }
                                    }

                                    #endregion

                                    #endregion

                                    lstScrapedData.Add(ScrapData);
                                }

                                return lstScrapedData;
                            }
                            else
                            {
                                Console.WriteLine("Error: Required product data not found");
                                return lstScrapedData;
                            }

                        }
                        else
                        {
                            Console.WriteLine("Error: No content recieved from the website");

                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;

                    }
                //}
                //else
                //{
                //    Console.WriteLine("Error: Enter Product Name!!");
                //    return lstScrapedData;
                //}
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetAliExpressData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.aliexpress.com/premium/{product}.html?SearchText=";
                    var client = new RestClient();
                    var request = new RestRequest(Url);
                    var proxy = "http://5454336363fce73e64a05912ade422803cf45d07:@proxy.zenrows.com:8001";
                    request.AddQueryParameter("https", proxy);

                    RestResponse response = await client.ExecuteAsync(request);
                    #endregion

                    if (response.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(response.Content))
                        {
                            HtmlDocument doc = new HtmlDocument();
                            doc.LoadHtml(response.Content);

                            #region XPATHS
                            var TitleXpath = "//*[contains(concat(' ', normalize-space(@class), ' '), ' manhattan--titleText--WccSjUS ')]";
                            var PriceXPath = "//*[contains(concat(' ', normalize-space(@class), ' '), ' manhattan--price-sale--1CCSZfK ')]";
                            var LinkXPath = "//*[contains(concat(' ', normalize-space(@class), ' '), ' manhattan--container--1lP57Ag ')] [contains(concat(' ', normalize-space(@class), ' '), ' cards--gallery--2o6yJVt ')]";
                            #endregion
                            #region GET DATA 
                            var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                            var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                            var linkNodes = doc.DocumentNode.SelectNodes(LinkXPath);
                            #endregion


                            if (titleNodes != null && PriceNodes != null && linkNodes != null && titleNodes.Count == PriceNodes.Count && titleNodes.Count == linkNodes.Count)
                            {
                                for (int i = 0; i < titleNodes.Count; i++)
                                {
                                    var ScrapData = new ScrappingData_Model();

                                    #region PARSE DATA

                                    var link = linkNodes[i].GetAttributeValue("href", "");
                                    var title = titleNodes[i].InnerText;
                                    var Price = PriceNodes[i].InnerText;

                                    #endregion
                                    #region SET MODEL
                                    ScrapData.Website = "Ali Express";
                                    ScrapData.product = product;
                                    ScrapData.Product_Description = title == string.Empty ? string.Empty : title;
                                    ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");
                                    ScrapData.Quantity = string.Empty;
                                    ScrapData.Unit = string.Empty;

                                    #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                    if (!string.IsNullOrEmpty(Price))
                                    {
                                        if (Price.Contains('-'))
                                        {
                                            var splitPrice = Price.Split('-').ToList();
                                            ScrapData.Max_Price = splitPrice[0];
                                            ScrapData.Min_Price = splitPrice[1];

                                            if (Price.Contains("$"))
                                            {
                                                ScrapData.Currency = "USD";
                                            }
                                            else if (Price.Contains("PKR"))
                                            {
                                                ScrapData.Currency = "PKR";
                                            }
                                        }
                                        else
                                        {
                                            ScrapData.Max_Price = Price;
                                            ScrapData.Min_Price = Price;

                                            if (Price.Contains("$"))
                                            {
                                                ScrapData.Currency = "USD";
                                            }
                                            else if (Price.Contains("PKR"))
                                            {
                                                ScrapData.Currency = "PKR";
                                            }
                                        }
                                    }

                                    #endregion

                                    #endregion

                                    lstScrapedData.Add(ScrapData);
                                }

                                return lstScrapedData;
                            }
                            else
                            {
                                Console.WriteLine("Error: Required product data not found");
                                return lstScrapedData;
                            }

                        }
                        else
                        {
                            
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;

                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetMadeInChinaData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.made-in-china.com/productdirectory.do?word={product}&file=&subaction=hunt&style=b&mode=and&code=0&comProvince=nolimit&order=0&isOpenCorrection=1";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);
                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//*[contains(concat(' ', normalize-space(@class), ' '), ' product-name ')]";
                        var PriceXPath = "//div[contains(@class, 'product-property')]";
                        var QuantityXPath = "//div[contains(concat(' ', normalize-space(@class), ' '), 'prod-info')]//div[contains(concat(' ', normalize-space(@class), ' '), 'info') and not(contains(concat(' ', normalize-space(@class), ' '), 'price-info'))]";
                        var linkXPaths = "//h2[contains(@class, 'product-name')]/a";
                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        var QuantityNodes = doc.DocumentNode.SelectNodes(QuantityXPath);
                        var linkNodes = doc.DocumentNode.SelectNodes(linkXPaths);

                        #endregion


                        if (titleNodes != null && PriceNodes != null && QuantityNodes != null && linkNodes != null && titleNodes.Count == PriceNodes.Count && titleNodes.Count == QuantityNodes.Count && titleNodes.Count == linkNodes.Count)
                        {
                            for (int i = 0; i < titleNodes.Count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = linkNodes[i].GetAttributeValue("href", " ");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                var Quantity = QuantityNodes[i].InnerText;

                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Made In China";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ").Replace("Recommended product from this supplier. ", "");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");

                                #region SET QUANTITY AND UNIT

                                var splitQuantity = Quantity.Split(' ').ToList();
                                if (splitQuantity.Count > 0)
                                {
                                    ScrapData.Quantity = splitQuantity[0];
                                    ScrapData.Unit = splitQuantity[1];

                                }
                                else
                                {
                                    ScrapData.Quantity = Quantity == string.Empty ? string.Empty : Quantity;
                                }
                                #endregion
                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    var pattern = "US\\s*\\$\\s*(\\d+(?:\\.\\d+)?|\\.\\d+)-(\\d+(?:\\.\\d+)?|\\.\\d+)";
                                    var filteredPrice = Regex.Match(Price, pattern);
                                    if (filteredPrice.Success)
                                    {
                                        var initialPrice = Regex.Replace(filteredPrice.Value, @"\s+", "");
                                        if (!string.IsNullOrEmpty(initialPrice))
                                        {
                                            if (initialPrice.Contains('-'))
                                            {
                                                var splitPrice = initialPrice.Split('-').ToList();
                                                ScrapData.Max_Price = splitPrice[0].Replace("US$", "");
                                                ScrapData.Min_Price = splitPrice[1].Replace("US$", "");

                                                if (initialPrice.Contains("$"))
                                                {
                                                    ScrapData.Currency = "USD";
                                                }
                                                else
                                                {
                                                    ScrapData.Currency = "PKR";
                                                }
                                            }
                                            else
                                            {
                                                ScrapData.Max_Price = initialPrice.Replace("US$", "");
                                                ScrapData.Min_Price = initialPrice.Replace("US$", "");

                                                if (initialPrice.Contains("$"))
                                                {
                                                    ScrapData.Currency = "USD";
                                                }
                                                else
                                                {
                                                    ScrapData.Currency = "PKR";
                                                }
                                            }
                                        }
                                    }
                                   
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            lstScrapedData.RemoveAll(x => string.IsNullOrEmpty(x.Min_Price));
                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetAmazonData(string product)
        {
            try 
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.amazon.com/s?k={product}&crid=25QBZPTQS5RUB&sprefix=calciu%2Caps%2C490&ref=nb_sb_noss_2";
                    var content = await Responses.GetWebDataFromRestClient(Url);
                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//*[contains(concat(' ', normalize-space(@class), ' '), ' a-size-mini ')] [contains(concat(' ', normalize-space(@class), ' '), ' a-spacing-none ')] [contains(concat(' ', normalize-space(@class), ' '), ' a-color-base ')] [contains(concat(' ', normalize-space(@class), ' '), ' s-line-clamp-4 ')]";
                        var PriceXPath = "//*[contains(concat(' ', normalize-space(@class), ' '), ' a-row ')] [contains(concat(' ', normalize-space(@class), ' '), ' a-size-base ')] [contains(concat(' ', normalize-space(@class), ' '), ' a-color-base ')]";
                        var linkXpath = "//*[contains(concat(' ', normalize-space(@class), ' '), 'a-size-mini')]  [contains(concat(' ', normalize-space(@class), ' '), 'a-spacing-none')] [contains(concat(' ', normalize-space(@class), ' '), 'a-color-base')]  [contains(concat(' ', normalize-space(@class), ' '), 's-line-clamp-4')] //*[contains(concat(' ', normalize-space(@class), ' '), 'a-link-normal')]  [contains(concat(' ', normalize-space(@class), ' '), 's-underline-text')]  [contains(concat(' ', normalize-space(@class), ' '), 's-underline-link-text')] [contains(concat(' ', normalize-space(@class), ' '), 's-link-style')] [contains(concat(' ', normalize-space(@class), ' '), 'a-text-normal')]";
                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        var linkNodes = doc.DocumentNode.SelectNodes(linkXpath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = linkNodes[i].GetAttributeValue("href", " ");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Amazon";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY
                                string pattern = @"\$\d+(\.\d+)?";

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = Regex.Match(splitPrice[0], pattern).Value.Replace("$", "");
                                        ScrapData.Min_Price = Regex.Match(splitPrice[1], pattern).Value.Replace("$", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Regex.Match(Price, pattern).Value.Replace("$","");
                                        ScrapData.Min_Price = Regex.Match(Price, pattern).Value.Replace("$", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetDarazData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://app.scrapingbee.com/api/v1/?api_key=70E99U1CM2EW7379SCUHB2BI2TW1BSWP6BG6XVBXK9GL22FT8BC5WJYHD8AO3N84CPDPDWAABNBD01EK&url=https%3A%2F%2Fwww.daraz.pk%2Fcatalog%2F%3Fspm%3Da2a0e.pdp.search.1.2c21MOuvMOuvHP%26q%3D{product}%26_keyori%3Dss%26from%3Dsearch_history%26sugg%3Dcalcium_0_1";
                    var client = new RestClient();
                    var request = new RestRequest(Url);
                    RestResponse response = await client.ExecuteGetAsync(request);
                    var content = response.Content;

                    #endregion
                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//*[contains(@class, 'title--wFj93')]";
                        var PriceXPath = "//div[contains(@class, 'price--NVB62')]//span[contains(@class, 'currency--GVKjl')]";
                        var linkXpath = "//div[contains(@class, 'title--wFj93')]/a/@href";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        var linkNodes = doc.DocumentNode.SelectNodes(linkXpath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)   
                        {
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = linkNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Daraz";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");
                                ScrapData.Quantity = string.Empty;
                                ScrapData.Unit = string.Empty;

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace("Rs. ", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace("Rs. ", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace("Rs. ", ""); ;
                                        ScrapData.Min_Price = Price.Replace("Rs. ", ""); ;

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetMedOnlineData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://medonline.pk/?s={product}'&search_posttype=product";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion
                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//div[contains(@class, 'item-content')]/h4/a/@href";
                        var PriceXPath = "//div[@class='item-price']/span/ins/span[@class='woocommerce-Price-amount amount']";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Med Online";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");
                                ScrapData.Quantity = string.Empty;
                                ScrapData.Unit = string.Empty;

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace("&#8360;&nbsp;", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace("&#8360;&nbsp;", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace("&#8360;&nbsp;", ""); ;
                                        ScrapData.Min_Price = Price.Replace("&#8360;&nbsp;", ""); ;

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetChaseValueData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://chasevalue.pk/search?q={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion
                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//a[@class=\"grid-link__title\"]";
                        var PriceXPath = "//div[@class=\"grid-link__org_price\"]/span[@class=\"money\"]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Chase Value Centre";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");
                                ScrapData.Quantity = string.Empty;
                                ScrapData.Unit = string.Empty;

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace("Rs. ", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace("Rs. ", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace("Rs. ", ""); ;
                                        ScrapData.Min_Price = Price.Replace("Rs. ", ""); ;

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetChemistDirectData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://chemistdirect.pk/search?type=product&options%5Bprefix%5D=last&q={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//h3[@class='card__heading h5']/a[@class='full-unstyled-link']";
                        var PriceXPath = "//span[contains(@class, 'price-item') and contains(@class, 'price-item--sale') and contains(@class, 'price-item--last')]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Chemist Direct";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");
                                ScrapData.Quantity = string.Empty;
                                ScrapData.Unit = string.Empty;

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0] == string.Empty ? string.Empty : Regex.Replace(splitPrice[0].Trim(), @"\s+", " ").Replace("PKR\n", "");
                                        ScrapData.Min_Price = splitPrice[1] == string.Empty ? string.Empty : Regex.Replace(splitPrice[0].Trim(), @"\s+", " ").Replace("PKR\n", ""); 

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price == string.Empty ? string.Empty : Regex.Replace(Price.Trim(), @"\s+", " ").Replace("PKR\n", "");
                                        ScrapData.Min_Price = Price == string.Empty ? string.Empty : Regex.Replace(Price.Trim(), @"\s+", " ").Replace("PKR\n", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetBimedisData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://bimedis.com/search/search-items?skw={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//div[@class='e-product-image sjs-quick-open-link data-background-image']/img";
                        var PriceXPath = "//span[@class='e-product-cost']";
                        var linkXPath = "//div[contains(concat(' ', normalize-space(@class), ' '), ' e-product-content-open-link ')]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        var linkNodes = doc.DocumentNode.SelectNodes(linkXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = linkNodes[i].GetAttributeValue("data-href", "");
                                var title = titleNodes[i].GetAttributeValue("title", "");
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Bimedis";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");
                                ScrapData.Quantity = string.Empty;
                                ScrapData.Unit = string.Empty;

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0] == string.Empty ? string.Empty : Regex.Replace(splitPrice[0].Trim(), @"\s+", " ").Replace("$", "");
                                        ScrapData.Min_Price = splitPrice[1] == string.Empty ? string.Empty : Regex.Replace(splitPrice[0].Trim(), @"\s+", " ").Replace("$", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price == string.Empty ? string.Empty : Regex.Replace(Price.Trim(), @"\s+", " ").Replace("$", "");
                                        ScrapData.Min_Price = Price == string.Empty ? string.Empty : Regex.Replace(Price.Trim(), @"\s+", " ").Replace("$", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetDawaaiData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://dawaai.pk/search/index?search={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//h2[contains(@class, 'header') and contains(@class, 'text-overflow--hidden') and contains(@class, 'mb-0')]/a/@href";
                        var PriceXPath = "//div[contains(@class, 'description')]//h4/text()";
                        var QuantityXPath = "//div[@class='pack-size-control']//p[@class='pack-size-size']";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        var QuantityNodes = doc.DocumentNode.SelectNodes(QuantityXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null && QuantityNodes != null)
                        {
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                var Quantity = QuantityNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Dawaai.pk";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");
                                ScrapData.Unit = "Unit";

                                #region SET QUANTITY
                                var QuanityRegex = @"(\d+)\D+(\d+)";
                                Match match = Regex.Match(Quantity, QuanityRegex);
                                if (match.Success)
                                {
                                    if (match.Groups.Count > 0)
                                    {
                                        ScrapData.Quantity = (int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value)).ToString();
                                    }
                                }
                                else
                                {
                                    ScrapData.Quantity = Regex.Match(Quantity, @"\b\d+\b").Success ? Regex.Match(Quantity, @"\b\d+\b").Value : string.Empty;
                                }
                                

                                #endregion

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace(" ", "").Replace("Rs.", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace(" ", "").Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace(" ", "").Replace("Rs.", "");
                                        ScrapData.Min_Price = Price.Replace(" ", "").Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetNaheedData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.naheed.pk/catalogsearch/result/?q={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//h2[contains(@class, 'product-name')]/a[contains(@class, 'product-item-link')]";
                        var PriceXPath = "//span[contains(@class, 'price-wrapper')]/span[@class='price' and ancestor::span[contains(@class, 'price-wrapper') and @data-price-type='finalPrice']]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Naheed.pk";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");
                                ScrapData.Unit = "Unit";
                                ScrapData.Quantity = string.Empty;
                                ScrapData.Unit = string.Empty;
                              

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace(" ", "").Replace("Rs.", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace(" ", "").Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace(" ", "").Replace("Rs.", "");
                                        ScrapData.Min_Price = Price.Replace(" ", "").Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetChinaCNData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://en.china.cn/search/{product}.html";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//div[contains(@class, 'detail-top')]/a";
                        var PriceXPath = "//div[@class='detail-top']/dl/dd";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {   
                            var count = titleNodes.Count > PriceNodes.Count ? PriceNodes.Count : titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].GetAttributeValue("title", "");
                                var Price = i % 2 == 0 ? PriceNodes[i].InnerText : PriceNodes[i + 1].InnerText;
                                var Quantity = i % 2 != 0 ? PriceNodes[i].InnerText.Replace("Min. Order : ", "") : PriceNodes[i + 1].InnerText.Replace("Min. Order : ", "");
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "China CN";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");

                                #region SET QUANTITY AND UNIT

                                if (!string.IsNullOrEmpty(Quantity))
                                {
                                    if (Quantity.Contains(" "))
                                    {
                                        var splitQuantity = Quantity.Split(" ").ToList();
                                        if (splitQuantity.Count > 0)
                                        {
                                            ScrapData.Quantity = splitQuantity[0];
                                            ScrapData.Unit = splitQuantity[1];
                                        }
                                        else
                                        {
                                            ScrapData.Quantity = Quantity;
                                            ScrapData.Unit = string.Empty;
                                        }
                                    }
                                }
                                #endregion
                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace(" ", "").Replace("Rs.", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace(" ", "").Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else if (Price.Contains(":"))
                                    {
                                        var splitPrice = Price.Split(':').ToList();
                                        ScrapData.Max_Price = splitPrice[1].Replace(" ", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace(" ", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = string.Empty;
                                        }
                                        else
                                        {
                                            ScrapData.Currency = string.Empty;
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace(" ", "").Replace("Rs.", "");
                                        ScrapData.Min_Price = Price.Replace(" ", "").Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetEc21Data(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.ec21.com/ec-market/{product}.html";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//div[contains(@class, 'front') and .//li[contains(@class, 'txt') and contains(@class, 'prTxt')]]//h2[contains(@class, 'pdtName')]/a";
                        var PriceXPath = "//ol[contains(@class, 'itemLs')]/li[contains(@class, 'txt') and contains(@class, 'prTxt')]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count ;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].GetAttributeValue("title", "");
                                var Price = i % 2 == 0 ? PriceNodes[i].InnerText : PriceNodes[i + 1].InnerText;
                                var Quantity = i % 2 != 0 ? PriceNodes[i].InnerText.Replace("(Min. Order)", "") : PriceNodes[i + 1].InnerText.Replace("(Min. Order)", "");
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "ec 21";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");

                                #region SET QUANTITY AND UNIT

                                if (!string.IsNullOrEmpty(Quantity))
                                {
                                    #region SET UNIT
                                    var UnitRegex = "[A-Za-z]+";
                                    var Unit = Regex.Match(Quantity, UnitRegex); 
                                    if (Unit.Success)
                                    {
                                        ScrapData.Unit = Unit.Value;
                                    }
                                    #endregion
                                    #region SET QUANTITY
                                    var QuantityRegex = "[-+]?\\d+(?:\\.\\d+)?";
                                    var quantity = Regex.Match(Quantity, QuantityRegex);
                                    if (quantity.Success)
                                    {
                                        ScrapData.Quantity = quantity.Value;
                                    }
                                    #endregion


                                }
                                #endregion
                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('~'))
                                    {
                                        var splitPrice = Price.Split('~').ToList();
                                        ScrapData.Max_Price = Regex.Match(splitPrice[0], "[-+]?\\d+(?:\\.\\d+)?").Value;
                                        ScrapData.Min_Price = Regex.Match(splitPrice[1], "[-+]?\\d+(?:\\.\\d+)?").Value;

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Regex.Match(Price, "[-+]?\\d+(?:\\.\\d+)?").Value;
                                        ScrapData.Min_Price = Regex.Match(Price, "[-+]?\\d+(?:\\.\\d+)?").Value;

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }

                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetDvagoData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.dvago.pk/search?search={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//*[contains(concat(' ', normalize-space(@class), ' '), ' MuiTypography-root ')]    [contains(concat(' ', normalize-space(@class), ' '), ' MuiTypography-body1 ')]    [contains(concat(' ', normalize-space(@class), ' '), ' css-19tcczm ')]/a/@href";
                        var PriceXPath = "//p[contains(concat(' ', normalize-space(@class), ' '), ' MuiTypography-root ')] [contains(concat(' ', normalize-space(@class), ' '), ' MuiTypography-body1 ')] [contains(concat(' ', normalize-space(@class), ' '), ' ProductCard_salePrice___b0BY ')] [contains(concat(' ', normalize-space(@class), ' '), ' css-19tcczm ')]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                //var Quantity = i % 2 != 0 ? PriceNodes[i].InnerText.Replace("(Min. Order)", "") : PriceNodes[i + 1].InnerText.Replace("(Min. Order)", "");
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Dvago";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");

                                #region SET QUANTITY AND UNIT

                                if (!string.IsNullOrEmpty(title))
                                {
                                    var QuantityRegex = "\\b\\d+\\b";
                                    var quantity = Regex.Match(title, QuantityRegex);
                                    if (quantity.Success)
                                    {
                                        ScrapData.Quantity = quantity.Value;
                                        var splitTitle = title.Split(" ").ToList();
                                        var UnitIndex = splitTitle.FindIndex(x => x.Equals(quantity.Value));
                                        if (UnitIndex != -1)
                                        {
                                            if (splitTitle.Count >= UnitIndex + 1)
                                            {
                                                ScrapData.Unit = splitTitle[UnitIndex + 1];
                                            }
                                            else
                                            {
                                                ScrapData.Unit = string.Empty;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Quantity = string.Empty;
                                        ScrapData.Unit = string.Empty;
                                    }
                                }
                                #endregion
                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace(" ", "").Replace("Rs.", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace(" ", "").Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace(" ", "").Replace("Rs.", "");
                                        ScrapData.Min_Price = Price.Replace(" ", "").Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }



                                }
                                #endregion

                                #endregion


                                lstScrapedData.Add(ScrapData);

                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetTradeWheelData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.tradewheel.com/search/product/?keyword={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//div[contains(@class, 'col-product')]/h2/a/@href";
                        var PriceXPath = "//ul[contains(@class, 'attr-product')]/li[1]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Trade Wheel";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");

                                #region SET QUANTITY AND UNIT

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains("/"))
                                    {
                                        var split = Price.Split("/").ToList();
                                        if (split.Count > 0)
                                        {
                                            ScrapData.Quantity = "1";
                                            ScrapData.Unit = split[1].Replace('\n', ' ').Trim();
                                        }
                                    }
                                }
                                #endregion
                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains("FOB Price:"))
                                    {
                                        Price.Replace("FOB Price:", "");
                                        var pattern = "[^0-9]";

                                        if (Price.Contains('-'))
                                        {
                                            var splitPrice = Price.Split('-').ToList();
                                            ScrapData.Max_Price = Regex.Replace(splitPrice[0], pattern, "");
                                            ScrapData.Min_Price = Regex.Replace(splitPrice[1], pattern, "");

                                            if (Price.Contains("USD"))
                                            {
                                                ScrapData.Currency = "USD";
                                            }
                                            else
                                            {
                                                ScrapData.Currency = "PKR";
                                            }
                                        }
                                        else
                                        {
                                            ScrapData.Max_Price = Regex.Replace(Price, pattern, "");
                                            ScrapData.Min_Price = Regex.Replace(Price, pattern, ""); 

                                            if (Price.Contains("USD"))
                                            {
                                                ScrapData.Currency = "USD";
                                            }
                                            else
                                            {
                                                ScrapData.Currency = "PKR";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Min_Price = string.Empty;
                                    }
                                 
                                }
                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            lstScrapedData.RemoveAll(x => string.IsNullOrEmpty(x.Min_Price));

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetPakWheelData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.pakwheels.com/used-cars/search/-/?q={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//a[contains(@class, 'car-name')]/@href";
                        var PriceXPath = "//div[contains(@class, 'price-details') and contains(@class, 'generic-dark-grey')]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].GetAttributeValue("title", "");
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Pak Wheels";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link == string.Empty ? string.Empty : link.Replace("//", "");

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0];
                                        ScrapData.Min_Price = splitPrice[1];

                                        if (Price.Contains("USD"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price;
                                        ScrapData.Min_Price = Price;

                                        if (Price.Contains("USD"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }
                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetMediPkData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://medipk.com/search.php?search_query={product}&x=0&y=0";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//div[contains(@class, 'ProductDetails')]/strong/a/@href";
                        var PriceXPath = "//div[contains(@class, 'ProductPriceRating')]/em";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Medi PK";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link;
                                ScrapData.Quantity = string.Empty;
                                ScrapData.Unit = string.Empty;

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains('-'))
                                    {
                                        var splitPrice = Price.Split('-').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace("$", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace("$", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace("$", "");
                                        ScrapData.Min_Price = Price.Replace("$", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }
                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetSehatData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://sehat.com.pk/search.php?search_query={product}";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//div[contains(@class, 'col-md-12') and contains(@class, 'd-table-cell') and contains(@class, 'text-center') and contains(@class, 'pl-1') and contains(@class, 'pr-1') and contains(@class, 'noSwipe')]/strong/a/@href";
                        var PriceXPath = "//div[contains(@class, 'ProductPriceRating') and contains(@class, 'd-table-row') and contains(@class, 'text-center') and contains(@class, 'pl-1') and contains(@class, 'pr-1') and contains(@class, 'align-middle')]/em";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null)
                        {
                            var count = titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = titleNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Sehat";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link;
                                ScrapData.Quantity = string.Empty;
                                ScrapData.Unit = string.Empty;

                                #region SET MINIMUM PRICE, MAXIMUM PRICE AND CURRENCY

                                if (!string.IsNullOrEmpty(Price))
                                {
                                    if (Price.Contains(' '))
                                    {
                                        var splitPrice = Price.Split(' ').ToList();
                                        ScrapData.Max_Price = splitPrice[0].Replace("Rs.", "");
                                        ScrapData.Min_Price = splitPrice[1].Replace("Rs.", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else if(Price.Contains("Rs"))
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                    else
                                    {
                                        ScrapData.Max_Price = Price.Replace("$", "");
                                        ScrapData.Min_Price = Price.Replace("$", "");

                                        if (Price.Contains("$"))
                                        {
                                            ScrapData.Currency = "USD";
                                        }
                                        else if(Price.Contains("Rs"))
                                        {
                                            ScrapData.Currency = "PKR";
                                        }
                                    }
                                }
                                #endregion

                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetZaubaData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.zauba.com/import-{product}-hs-code.html";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//td[@class='desc']";
                        var linkXPath = "(//td[@scope=\"row\"])/a/@href";
                        var PriceXPath = "(//td[@scope=\"row\"][9])";
                        var QuantityXPath = "(//td[@scope=\"row\"][7])";
                        var UnitXPath = "(//td[@scope=\"row\"][6])";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        var linkNodes = doc.DocumentNode.SelectNodes(linkXPath);
                        var QuantityNodes = doc.DocumentNode.SelectNodes(QuantityXPath);
                        var UnitNodes = doc.DocumentNode.SelectNodes(UnitXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null && linkNodes != null && QuantityNodes != null && UnitNodes != null)
                        {
                            var count = titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = linkNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                var Quantity = QuantityNodes[i].InnerText;
                                var Unit = UnitNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Zauba";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link;
                                ScrapData.Quantity = Quantity;
                                ScrapData.Unit = Unit;
                                ScrapData.Currency = "USD";
                                ScrapData.Max_Price = Price;
                                ScrapData.Min_Price = Price;
                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetExportImportData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.exportimportdata.in/import-{product}-hs-code";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//td[@itemprop=\"Item-Description\"]";
                        var linkXPath = "//td[@itemprop=\"CTH\"]/a/@href";
                        var PriceXPath = "//td[7]";
                        var QuantityXPath = "//td[@itemprop=\"Quantity\"]";
                        var UnitXPath = "//td[6]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        var linkNodes = doc.DocumentNode.SelectNodes(linkXPath);
                        var QuantityNodes = doc.DocumentNode.SelectNodes(QuantityXPath);
                        var UnitNodes = doc.DocumentNode.SelectNodes(UnitXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null && linkNodes != null && QuantityNodes != null && UnitNodes != null)
                        {
                            var count = titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var link = linkNodes[i].GetAttributeValue("href", "");
                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                var Quantity = QuantityNodes[i].InnerText;
                                var Unit = UnitNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Export Import";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = link;
                                ScrapData.Quantity = Quantity;
                                ScrapData.Unit = Unit;
                                ScrapData.Currency = "USD";
                                ScrapData.Max_Price = Price;
                                ScrapData.Min_Price = Price;
                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ScrappingData_Model>> GetVietnamData(string product)
        {
            try
            {
                var lstScrapedData = new List<ScrappingData_Model>();

                if (!string.IsNullOrEmpty(product))
                {
                    #region EXECUTE GET REQUEST
                    var Url = $"https://www.vietnamtrades.com/vietnam-import-data/{product}.html";
                    var content = await Responses.GetWebDataFromZenrowsAsync(Url);

                    #endregion

                    if (!string.IsNullOrEmpty(content))
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(content);

                        #region XPATHS
                        var TitleXpath = "//td[3]";
                        var PriceXPath = "//td[8]";
                        var QuantityXPath = "//td[6]";
                        var UnitXPath = "//td[5]";

                        #endregion
                        #region GET DATA 
                        var titleNodes = doc.DocumentNode.SelectNodes(TitleXpath);
                        var PriceNodes = doc.DocumentNode.SelectNodes(PriceXPath);
                        var QuantityNodes = doc.DocumentNode.SelectNodes(QuantityXPath);
                        var UnitNodes = doc.DocumentNode.SelectNodes(UnitXPath);
                        #endregion


                        if (titleNodes != null && PriceNodes != null && QuantityNodes != null && UnitNodes != null)
                        {
                            var count = titleNodes.Count;
                            for (int i = 0; i < count; i++)
                            {
                                var ScrapData = new ScrappingData_Model();

                                #region PARSE DATA

                                var title = titleNodes[i].InnerText;
                                var Price = PriceNodes[i].InnerText;
                                var Quantity = QuantityNodes[i].InnerText;
                                var Unit = UnitNodes[i].InnerText;
                                #endregion
                                #region SET MODEL
                                ScrapData.Website = "Vietnam";
                                ScrapData.product = product;
                                ScrapData.Product_Description = title == string.Empty ? string.Empty : Regex.Replace(title.Trim(), @"\s+", " ");
                                ScrapData.Product_Link = string.Empty;
                                ScrapData.Quantity = Quantity;
                                ScrapData.Unit = Unit;
                                ScrapData.Currency = "USD";
                                ScrapData.Max_Price = Price;
                                ScrapData.Min_Price = Price;
                                #endregion

                                lstScrapedData.Add(ScrapData);
                            }

                            return lstScrapedData;
                        }
                        else
                        {
                            return lstScrapedData;
                        }
                    }
                    else
                    {
                        return lstScrapedData;
                    }
                }
                else
                {
                    return lstScrapedData;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
