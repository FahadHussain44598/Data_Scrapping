using RestSharp;
using System;

namespace WebScrappingApi.Utilities
{
    public class ProxyDataReponses
    {


        public async Task<string> GetWebDataFromRestClient(string Url)
        {
            try
            {
                if (!string.IsNullOrEmpty(Url))
                {
                    var client = new RestClient();
                    var request = new RestRequest(Url);
                    var response = await client.ExecuteGetAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        if (!string.IsNullOrEmpty(response.Content))
                        {
                            return response.Content;
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetWebDataFromScrappingBeeAsync(string Url)
        {
            try
            {
                var URL = $"https://app.scrapingbee.com/api/v1/?api_key=70E99U1CM2EW7379SCUHB2BI2TW1BSWP6BG6XVBXK9GL22FT8BC5WJYHD8AO3N84CPDPDWAABNBD01EK&url={Url}";
                var client = new RestClient();
                var request = new RestRequest(URL);
                RestResponse response = await client.ExecuteGetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(response.Content))
                    {
                        return response.Content;

                    }
                    else
                    {
                        return "No contenct found";
                    }
                }
                else
                {
                    return "Status code is 500";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetWebDataFromZenrowsAsync(string Url)
        {
            try
            {
                var client = new RestClient();
                var request = new RestRequest(Url);
                var proxy = "";
                request.AddQueryParameter("https", proxy);
                request.AddQueryParameter("js_render", true);
                RestResponse response = await client.ExecuteGetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(response.Content))
                    {
                        return response.Content;

                    }
                    else
                    {
                        return "No contenct found";
                    }
                }
                else
                {
                    return "Status code is 500";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetWebDataFromCrawlBaseAsync(string Url)
        {
            try
            {
                var client = new RestClient();
                string apiUrl = "https://api.crawlbase.com/scraper?token=9lSr1-gsDCgxAFz5luiZ8g&url=" + Url;
                var request = new RestRequest(apiUrl);
                request.AddQueryParameter("javascript", "true");
                request.AddQueryParameter("country", "US");
                request.AddQueryParameter("premium", "true");
                var response = await client.GetAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(response.Content))
                    {
                        return response.Content;

                    }
                    else
                    {
                        return "No contenct found";
                    }
                }
                else
                {
                    return "Status code is 500";
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
