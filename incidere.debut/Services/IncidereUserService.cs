using incidere.debut.Models.LocalUser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;

namespace incidere.debut.Services
{
    public class IncidereUserService
    {
        enum OperationType { GetAll, GetOne, Create, Edit, Delete };
        private string m_incidereServiceUrl;
        private string m_incidereServiceApiEndpoint;
        private HttpClient m_incidereServiceClient;

        public IncidereUserService()
        {
            m_incidereServiceUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451";
            m_incidereServiceClient = new HttpClient { BaseAddress = new Uri(m_incidereServiceUrl) };
            m_incidereServiceApiEndpoint = "api/local-users";
        }

        public List<LocalUser> GetUsers()
        {
            var localUsers = new List<LocalUser>();
            try
            {
                var output = m_incidereServiceClient.GetStringAsync(m_incidereServiceApiEndpoint).Result;
                try
                {
                    var json = JObject.Parse(output).SelectToken("$._results");
                    foreach (var jtok in json)
                    {
                        var localUser = jtok.ToObject<LocalUser>();
                        localUsers.Add(localUser);
                    }
                }
                catch (Exception)
                {
                    return localUsers;
                }
            }
            catch (Exception)
            {
                return localUsers;
            }

            return localUsers;
        }

        public LocalUser GetUser(string id = null)
        {
            var localUser = new LocalUser();

            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var output = m_incidereServiceClient.GetStringAsync($"{m_incidereServiceApiEndpoint}/{id}").Result;
                    try
                    {
                        var json = JObject.Parse(output).SelectToken("$._result");
                        localUser = json.ToObject<LocalUser>();
                    }
                    catch (Exception)
                    {
                        return localUser;
                    }
                }
                catch (Exception)
                {
                    return localUser;
                }
            }

            return localUser;
        }

        public LocalUser CreateUser(LocalUser item)
        {
            return CreateOrEditUser(item, OperationType.Create);
        }

        public LocalUser EditUser(LocalUser item, string id)
        {
            return CreateOrEditUser(item, OperationType.Edit, id);
        }

        public bool DeleteUser(string id)
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

        private LocalUser CreateOrEditUser(LocalUser item, OperationType operation, string id = null)
        {
            var localUser = new LocalUser();
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
                    return localUser;

                if (response.IsSuccessStatusCode)
                {
                    var output = response.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var result = JObject.Parse(output).SelectToken("_result");
                        localUser = result.ToObject<LocalUser>();
                    }
                    catch (Exception)
                    {
                        return localUser;
                    }
                }
            }
            catch (Exception)
            {
                return localUser;
            }

            return localUser;
        }
    }
}