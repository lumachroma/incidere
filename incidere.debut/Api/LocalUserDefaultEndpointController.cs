using incidere.debut.Models.LocalUser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace incidere.debut.Api
{
    [RoutePrefix("api/local-users")]
    public class LocalUserDefaultEndpointController : ApiController
    {
        private string m_entityName;
        private string m_entityRoute;
        private HttpClient m_firebaseClient;
        private string m_baseUrl;
        private string m_firebaseUrl;
        private string m_firebaseDatabaseSecret;

        public LocalUserDefaultEndpointController()
        {
            m_entityName = nameof(LocalUser);
            m_entityRoute = "local-users";
            m_baseUrl = ConfigurationManager.AppSettings["IncidereBaseUrl"] ?? "http://localhost:50451";
            m_firebaseUrl = ConfigurationManager.AppSettings["FirebaseUrl"];
            m_firebaseDatabaseSecret = ConfigurationManager.AppSettings["FirebaseDatabaseSecret"];
            m_firebaseClient = new HttpClient { BaseAddress = new Uri(m_firebaseUrl) };
        }

        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAction([FromUri(Name = "q")]string q = null,
            [FromUri(Name = "next")]string next = null,
            [FromUri(Name = "prev")]string prev = null,
            [FromUri(Name = "size")]int size = 10)
        {
            var resultSuccess = true;
            var resultStatus = "OK";

            var prevSize = size + 2;
            var nextSize = size + 1;

            var items = new List<LocalUser>();
            var isPrevPage = !string.IsNullOrEmpty(prev);
            var isNextPage = !string.IsNullOrEmpty(next);
            var firstPage = !isPrevPage && !isNextPage;

            var queryString = $"orderBy=\"$key\"";
            if (isPrevPage)
            {
                queryString += $"&limitToLast={prevSize}&endAt=\"{prev}\"";
                prev = null;
            }
            else
            {
                queryString += $"&limitToFirst={nextSize}&startAt=\"{next}\"";
                next = null;
            }

            var response = await m_firebaseClient.GetAsync($"{m_entityName}.json?auth={m_firebaseDatabaseSecret}&{queryString}");
            if (!response.IsSuccessStatusCode)
            {
                resultSuccess = false;
                resultStatus = $"Firebase: {(int)response.StatusCode} {response.ReasonPhrase.ToString()}";
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = resultSuccess, status = resultStatus });
            }

            var output = await response.Content.ReadAsStringAsync();
            if (output == "null")
            {
                resultSuccess = false;
                resultStatus = $"No data found for {m_entityName}";
                return Request.CreateResponse(HttpStatusCode.NotFound, new { success = resultSuccess, status = resultStatus });
            }

            var json = new JObject();
            try
            {
                json = JObject.Parse(output);
            }
            catch (Exception ex)
            {
                resultSuccess = false;
                resultStatus = $"Error: {ex.Message}";
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { success = resultSuccess, status = resultStatus });
            }

            var itemIndex = 1;
            var pageStartAt = 0;
            var pageEndAt = 0;
            var hasMaxPrevSize = (json.Count == prevSize);

            if (hasMaxPrevSize)
            {
                pageStartAt = 2;
                pageEndAt = prevSize;
            }
            else
            {
                pageStartAt = 1;
                pageEndAt = nextSize;
            }
            foreach (var jtok in json)
            {
                if (hasMaxPrevSize)
                {
                    if (itemIndex < pageStartAt)
                    {
                        itemIndex++;
                        continue;
                    }
                    if (itemIndex == pageStartAt) prev = jtok.Key;
                }
                if (isNextPage && !firstPage)
                {
                    if (itemIndex == pageStartAt) prev = jtok.Key;
                }
                if (itemIndex == pageEndAt)
                {
                    next = jtok.Key;
                    break;
                }

                var item = jtok.Value.ToObject<LocalUser>();
                item.FirebaseKey = jtok.Key;
                items.Add(item);

                itemIndex++;
            }

            var status = new { success = resultSuccess, status = resultStatus };
            var count = await PrivateGetCountAsync();
            var links = new List<object>();
            var selfLink = new
            {
                method = "GET",
                rel = "self",
                href = $"{m_baseUrl}/api/{m_entityRoute}?size={size}",
                desc = "Issue a GET request to get the first page of the result"
            };
            links.Add(selfLink);
            var prevLink = new
            {
                method = "GET",
                rel = "prev",
                href = $"{m_baseUrl}/api/{m_entityRoute}?size={size}&prev={prev}",
                desc = "Issue a GET request to get the previous page of the result"
            };
            if (prev != null) links.Add(prevLink);
            var nextLink = new
            {
                method = "GET",
                rel = "next",
                href = $"{m_baseUrl}/api/{m_entityRoute}?size={size}&next={next}",
                desc = "Issue a GET request to get the next page of the result"
            };
            if (next != null) links.Add(nextLink);

            var result = new
            {
                _results = items,
                _status = status,
                _count = count,
                _size = size,
                _next = next,
                _links = links
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetOneByIdAsync(string id)
        {
            var resultSuccess = true;
            var resultStatus = "OK";

            var query = $"orderBy=\"$key\"&equalTo=\"{id}\"";
            var response = await m_firebaseClient.GetAsync($"{m_entityName}.json?auth={m_firebaseDatabaseSecret}&{query}");
            if (!response.IsSuccessStatusCode)
            {
                resultSuccess = false;
                resultStatus = $"Firebase: {(int)response.StatusCode} {response.ReasonPhrase.ToString()}";
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = resultSuccess, status = resultStatus, id = id });
            }

            var output = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(output).SelectToken($"{id}");
            if (json == null)
            {
                resultSuccess = false;
                resultStatus = $"Cannot find {m_entityName} with id: {id}";
                return Request.CreateResponse(HttpStatusCode.NotFound, new { success = resultSuccess, status = resultStatus, id = id });
            }

            var item = json.ToObject<LocalUser>();
            item.FirebaseKey = id;

            var status = new { success = resultSuccess, status = resultStatus, id = id };
            var links = new List<object>();
            var selfLink = new
            {
                method = "GET",
                rel = "self",
                href = $"{m_baseUrl}/api/{m_entityRoute}/{id}",
                desc = "Issue a GET request to get the item"
            };
            links.Add(selfLink);

            var result = new
            {
                _result = item,
                _status = status,
                _links = links
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        [Route("")]
        public async Task<HttpResponseMessage> PostDefault([FromBody]LocalUser item)
        {
            var resultSuccess = true;
            var resultStatus = "OK";

            item.FirebaseKey = null;
            item.CreatedBy = string.Empty; //TODO:
            item.ChangedBy = string.Empty; //TODO:

            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await m_firebaseClient.PostAsync($"{m_entityName}.json?auth={m_firebaseDatabaseSecret}", content);
            if (!response.IsSuccessStatusCode)
            {
                resultSuccess = false;
                resultStatus = $"Firebase: {(int)response.StatusCode} {response.ReasonPhrase.ToString()}";
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = resultSuccess, status = resultStatus });
            }

            var output = await response.Content.ReadAsStringAsync();
            var itemId = JObject.Parse(output).SelectToken("name");

            var status = new { success = resultSuccess, status = resultStatus, id = itemId };
            var links = new List<object>();
            var selfLink = new
            {
                method = "GET",
                rel = "self",
                href = $"{m_baseUrl}/api/{m_entityRoute}/{itemId}",
                desc = "Issue a GET request to get the item"
            };
            links.Add(selfLink);
            var result = new
            {
                _id = itemId,
                _status = status,
                _result = item,
                _links = links
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<HttpResponseMessage> PutDefault([FromBody]LocalUser item, string id)
        {
            var resultSuccess = true;
            var resultStatus = "OK";

            var itemFromSource = await PrivateGetOneByIdAsync(id);
            item.FirebaseKey = null;
            item.Id = itemFromSource.Id;
            item.CreatedDate = itemFromSource.CreatedDate;
            item.CreatedBy = itemFromSource.CreatedBy;
            item.ChangedDate = DateTime.Now;
            item.ChangedBy = string.Empty; //TODO:

            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await m_firebaseClient.PutAsync($"{m_entityName}/{id}.json?auth={m_firebaseDatabaseSecret}", content);
            if (!response.IsSuccessStatusCode)
            {
                resultSuccess = false;
                resultStatus = $"Firebase: {(int)response.StatusCode} {response.ReasonPhrase.ToString()}";
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = resultSuccess, status = resultStatus });
            }

            var output = await response.Content.ReadAsStringAsync();
            item = JObject.Parse(output).ToObject<LocalUser>();
            item.FirebaseKey = id;

            var status = new { success = resultSuccess, status = resultStatus, id = id };
            var links = new List<object>();
            var selfLink = new
            {
                method = "GET",
                rel = "self",
                href = $"{m_baseUrl}/api/{m_entityRoute}/{id}",
                desc = "Issue a GET request to get the item"
            };
            links.Add(selfLink);
            var result = new
            {
                _id = id,
                _status = status,
                _result = item,
                _links = links
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<HttpResponseMessage> DeleteDefault(string id)
        {
            var resultSuccess = true;
            var resultStatus = "OK";

            var response = await m_firebaseClient.DeleteAsync($"{m_entityName}/{id}.json?auth={m_firebaseDatabaseSecret}");
            if (!response.IsSuccessStatusCode)
            {
                resultSuccess = false;
                resultStatus = $"Firebase: {(int)response.StatusCode} {response.ReasonPhrase.ToString()}";
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { success = resultSuccess, status = resultStatus, id = id });
            }

            var status = new { success = resultSuccess, status = resultStatus, id = id };
            var result = new
            {
                _id = id,
                _status = status
            };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        private async Task<int> PrivateGetCountAsync()
        {
            var count = 0;
            var query = "shallow=true&print=pretty";
            try
            {
                var response = await m_firebaseClient.GetAsync($"{m_entityName}.json?auth={m_firebaseDatabaseSecret}&{query}");
                if (response.IsSuccessStatusCode)
                {
                    var output = await response.Content.ReadAsStringAsync();
                    try
                    {
                        var json = JObject.Parse(output);
                        count = json.Count;
                    }
                    catch (Exception)
                    {
                        return count;
                    }
                }
            }
            catch (Exception)
            {
                return count;
            }
            return count;
        }

        private async Task<LocalUser> PrivateGetOneByIdAsync(string id)
        {
            var item = new LocalUser();
            var query = $"orderBy=\"$key\"&equalTo=\"{id}\"";
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    var response = await m_firebaseClient.GetAsync($"{m_entityName}.json?auth={m_firebaseDatabaseSecret}&{query}");
                    if (response.IsSuccessStatusCode)
                    {
                        var output = await response.Content.ReadAsStringAsync();
                        try
                        {
                            var json = JObject.Parse(output).SelectToken($"{id}");
                            if (null != json)
                            {
                                item = json.ToObject<LocalUser>();
                                item.FirebaseKey = id;
                            }
                        }
                        catch (Exception)
                        {
                            return item;
                        }
                    }
                }
                catch (Exception)
                {
                    return item;
                }

            }
            return item;
        }
    }
}