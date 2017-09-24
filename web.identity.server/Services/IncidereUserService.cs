using incidere.debut.Models.LocalUser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;

namespace web.identity.server.Services
{
    public class IncidereUserService
    {
        enum OperationType { GetAll, GetOne, Create, Edit, Delete };
        private string m_incidereServiceUrl;
        private HttpClient m_incidereServiceClient;

        public IncidereUserService()
        {
            m_incidereServiceUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451";
            m_incidereServiceClient = new HttpClient { BaseAddress = new Uri(m_incidereServiceUrl) };
        }

        public List<LocalUser> GetUsers()
        {
            var localUsers = new List<LocalUser>();
            try
            {
                var output = m_incidereServiceClient.GetStringAsync("api/local-users").Result;
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
                    var output = m_incidereServiceClient.GetStringAsync($"api/local-users/{id}").Result;
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

        public bool CreateUser(LocalUser user)
        {
            return CreateOrEditUser(user, OperationType.Create);
        }

        public bool EditLocalUser(LocalUser user, string id)
        {
            return CreateOrEditUser(user, OperationType.Edit, id);
        }

        public bool DeleteUser(string id)
        {
            var result = false;
            var response = new HttpResponseMessage();

            try
            {
                response = m_incidereServiceClient.DeleteAsync($"api/local-users/{id}").Result;
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

        private bool CreateOrEditUser(LocalUser localUser, OperationType operation, string id = null)
        {
            var result = false;
            var json = JsonConvert.SerializeObject(localUser);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            try
            {
                var response = new HttpResponseMessage();

                if (operation == OperationType.Create)
                    response = m_incidereServiceClient.PostAsync("api/local-users", content).Result;
                else if (operation == OperationType.Edit)
                    response = m_incidereServiceClient.PutAsync($"api/local-users/{id}", content).Result;
                else
                    return result;

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
    }
}