using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DataVerse_MVC
{
    public class DataVerse
    {
        public class TableResponse
        {
            public string title { get; set; }
            public string casetypecode { get; set; }
            public string ticketnumber { get; set; }
            public string incidentid { get; set; }
            public string ownerid { get; set; }
            public string isAccess { get; set; }


        }

        #region #Fetch AccessToken
        public static string FetchToken(string resource, string clientId, string secret, string authority)
        {
            try
            {
                AuthenticationContext authContext = new AuthenticationContext(authority);
                ClientCredential credential = new ClientCredential(clientId, secret);
                AuthenticationResult result = authContext.AcquireTokenAsync(resource, credential).Result;
                return result.AccessToken;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region #Fetch Choice Column Metadata
        public static List<string> FetchChoiceColumnMetadata(string accessToken, string resource, string entityInternalName, string choiceColumn)
        {
            try
            {
                var response = new List<string>();
                var url = resource + "/api/data/v9.2/" + entityInternalName + "?$select=" + choiceColumn;
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Accept = "application/json";
                httpRequest.Headers["OData-MaxVersion"] = "4.0";
                httpRequest.Headers["OData-Version"] = "4.0";
                httpRequest.Headers["Prefer"] = "odata.include-annotations=OData.Community.Display.V1.FormattedValue";
                httpRequest.Headers["Authorization"] = "Bearer " + accessToken;
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JObject keyValuePairs = JObject.Parse(result);
                    foreach (var item in keyValuePairs["value"])
                    {
                        response.Add((string)item["casetypecode@OData.Community.Display.V1.FormattedValue"]);
                    }
                }
                if (httpResponse.StatusCode == HttpStatusCode.OK)
                    return response;
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }
        #endregion

        #region #Fetch Table Records
        public static DataVerse.TableResponse CallOut(string resource, string accessToken, string recordId, string entityName, string[] columns)
        {
            try
            {
                TableResponse tableResponse = new TableResponse();
                var client = new HttpClient
                {
                    BaseAddress = new Uri(resource + "/api/data/v9.2/"),
                    Timeout = new TimeSpan(0, 2, 0)
                };
                HttpRequestHeaders headers = client.DefaultRequestHeaders;
                headers.Add("OData-MaxVersion", "4.0");
                headers.Add("OData-Version", "4.0");
                headers.Add("Prefer","odata.include-annotations=OData.Community.Display.V1.FormattedValue");
                headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string queryParam = entityName + "(" + recordId + ")" + "?$select=";
                foreach (var column in columns)
                {
                    queryParam += column + ",";
                }
                queryParam = queryParam.Remove(queryParam.Length - 1);

                
                var response = client.GetAsync(resource + "/api/data/v9.2/" + queryParam).Result;
                if (response.IsSuccessStatusCode)
                {
                    JObject body = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    tableResponse.title = (string)body["title"];
                    tableResponse.casetypecode = (string)body["casetypecode@OData.Community.Display.V1.FormattedValue"];
                    tableResponse.ticketnumber = (string)body["ticketnumber"];
                    tableResponse.incidentid = (string)body["incidentid"];
                    tableResponse.ownerid = (string)body["_ownerid_value@OData.Community.Display.V1.FormattedValue"];
                    tableResponse.isAccess = (string)body["cr2a3_isapprovalreceived"];
                    return tableResponse;
                }
                else return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region #Update Field
        public static bool UpdateField(bool isOk, string resource, string accessToken, string entityName, string recordId)
        {
            try
            {
                var data = string.Empty;
                var url = resource + "/api/data/v9.2/" + entityName + "(" + recordId + ")";
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "PATCH";
                httpRequest.Headers["If-None-Match"] = "null";
                httpRequest.Headers["OData-Version"] = "4.0";
                httpRequest.Headers["OData-MaxVersion"] = "4.0";
                httpRequest.ContentType = "application/json";
                httpRequest.Accept = "application/json";
                httpRequest.Headers["scope"] = "https://vestatech.crm4.dynamics.com/.default";
                httpRequest.Headers["Authorization"] = "Bearer " + accessToken;
                if (isOk == true)
                {
                    data = @"{
                                ""isescalated"": true,
                                ""cr2a3_isapprovalreceived"" : true
                             }";
                }
                else if (isOk == false)
                {
                    data = @"{
                                ""isescalated"": false,
                                ""cr2a3_isapprovalreceived"" : false
                             }";
                }
                using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
                {
                    streamWriter.Write(data);
                }

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }
        #endregion







        #region #UnTouched Code
        public static string GetToken(string response, int index)
        {
            return response.Split(",")[index].Split(":")[1].Replace('"', ' ').Replace('}', ' ').Trim();
        }

        //public static string getAccessToken()
        //{
        //    string endpoint = "https://login.microsoftonline.com/3e8e53be-a48f-4147-adf8-7e90a6e46b57/oauth2/token";
        //    var client = new HttpClient();
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        //    var formEncodedData = new[]
        //    {
        //        new KeyValuePair<string, string>("client_assertion_type","urn:ietf:params:oauth:client-assertion-type:jwt-bearer"),
        //        new KeyValuePair<string, string>("client_id", "799a05ac-a21e-48cb-af72-27b8093faacd"),
        //        new KeyValuePair<string, string>("scope", "https://graph.microsoft.com/.default"),
        //        new KeyValuePair<string, string>("grant_type", "client_credentials"),
        //        new KeyValuePair<string, string>("client_secret", "JCu8Q~38xq2v5eItLvscIaVlIpak~V.ax4r35cJr")
        //    };  
        //    var response = client.PostAsync(endpoint, new FormUrlEncodedContent(formEncodedData)).GetAwaiter().GetResult();
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var result = response.Content.ReadAsStringAsync().Result;
        //        return GetToken(result, 6);
        //    }
        //    return null;
        //}
        #endregion

    }
}
