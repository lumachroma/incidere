using incidere.debut.Models.Internals.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;

namespace incidere.debut.Services
{
    public class IncidereSettingService
    {
        enum OperationType { GetAll, GetOne, Create, Edit, Delete };
        private string m_incidereServiceUrl;
        private string m_incidereServiceApiEndpoint;
        private HttpClient m_incidereServiceClient;

        public IncidereSettingService()
        {
            m_incidereServiceUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451";
            m_incidereServiceClient = new HttpClient { BaseAddress = new Uri(m_incidereServiceUrl) };
            m_incidereServiceApiEndpoint = "api/internal/settings";
        }

        public List<Setting> GetSettings()
        {
            var settings = new List<Setting>();
            try
            {
                var output = m_incidereServiceClient.GetStringAsync(m_incidereServiceApiEndpoint).Result;
                try
                {
                    var json = JObject.Parse(output).SelectToken("$._results");
                    foreach (var jtok in json)
                    {
                        var setting = jtok.ToObject<Setting>();
                        settings.Add(setting);
                    }
                }
                catch (Exception)
                {
                    return settings;
                }
            }
            catch (Exception)
            {
                return settings;
            }

            return settings;
        }

        public Setting GetSetting(string id = null)
        {
            var setting = new Setting();

            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var output = m_incidereServiceClient.GetStringAsync($"{m_incidereServiceApiEndpoint}/{id}").Result;
                    try
                    {
                        var json = JObject.Parse(output).SelectToken("$._result");
                        setting = json.ToObject<Setting>();
                    }
                    catch (Exception)
                    {
                        return setting;
                    }
                }
                catch (Exception)
                {
                    return setting;
                }
            }

            return setting;
        }

        public Setting CreateSetting(Setting item)
        {
            return CreateOrEditSetting(item, OperationType.Create);
        }

        public Setting EditSetting(Setting item, string id)
        {
            return CreateOrEditSetting(item, OperationType.Edit, id);
        }

        public bool DeleteSetting(string id)
        {
            var result = false;
            var response = new HttpResponseMessage();

            try
            {
                response = m_incidereServiceClient.DeleteAsync($"{m_incidereServiceApiEndpoint}/{id}").Result;
                if (response.IsSuccessStatusCode)
                {
                    var output = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var success = JObject.Parse(output).SelectToken("_status").SelectToken("success");
                        result = success.Value<bool>();
                    }
                    catch (Exception)
                    {
                        return result;
                    }
                }
            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }

        private Setting CreateOrEditSetting(Setting item, OperationType operation, string id = null)
        {
            var setting = new Setting();
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            try
            {
                var response = new HttpResponseMessage();

                if (operation == OperationType.Create)
                    response = m_incidereServiceClient.PostAsync(m_incidereServiceApiEndpoint, content).Result;
                else if (operation == OperationType.Edit)
                    response = m_incidereServiceClient.PutAsync($"{m_incidereServiceApiEndpoint}/{id}", content).Result;
                else
                    return setting;

                if (response.IsSuccessStatusCode)
                {
                    var output = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var result = JObject.Parse(output).SelectToken("_result");
                        setting = result.ToObject<Setting>();
                    }
                    catch (Exception)
                    {
                        return setting;
                    }
                }
            }
            catch (Exception)
            {
                return setting;
            }

            return setting;
        }
    }
}