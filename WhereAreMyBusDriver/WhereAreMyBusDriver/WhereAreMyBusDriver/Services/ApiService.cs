namespace WhereAreMyBusDriver.Services
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Plugin.Connectivity;
    using Models;
    using Newtonsoft.Json.Linq;
    using System.Linq;

    public class ApiService
    {



        public async Task<TokenResponse> GetToken(string urlBase, string uri, string username, string password)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);

                var request = JsonConvert.SerializeObject(new LoginRequest
                {
                    Email = username,
                    Password = password,
                    ReturnSecureToken = true
                });
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(uri, content);
                var resultJSON = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<TokenResponse>(resultJSON);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        public async Task<Response> GetList<T>(string urlBase, string servicePrefix, string controller)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                dynamic data = JObject.Parse((string)result);
                        var list = ((IDictionary<string, JToken>)data).Select(k =>
                JsonConvert.DeserializeObject<T>(k.Value.ToString())).ToList();
                //List<T> list = new List<T>();
                //foreach (var itemDynamic in data)
                //{
                //    list.Add(JsonConvert.DeserializeObject<T>(((JProperty)itemDynamic).Value.ToString()));
                //}
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = list,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> GetByField<T>(string urlBase, string servicePrefix, string field, object value,
            string accessToken)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}?auth={1}&orderBy={2}{3}{4}&equalTo={5}{6}{7}", servicePrefix, 
                    accessToken, '\u0022', field, '\u0022', '\u0022', value, '\u0022');
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                //var model = JsonConvert.DeserializeObject<T>(result);
                dynamic data = JObject.Parse((string)result);
                //var model = JsonConvert.DeserializeObject<T>(((JProperty)data).Value.ToString());
                JObject parsedJson = JObject.Parse(result);
                var key = parsedJson.First.Children().ToString();
                var model = ((IDictionary<string, JToken>)data).Select(k =>
                JsonConvert.DeserializeObject<T>(k.Value.ToString())).First();
                //dynamic data = JObject.Parse((string)result);
                //List<T> list = new List<T>();
                //foreach (var itemDynamic in data)
                //{
                //    list.Add(JsonConvert.DeserializeObject<T>(((JProper,ty)itemDynamic).Value.ToString()));
                //}
                return new Response
                {
                    IsSuccess = true,
                    Message = "Ok",
                    Result = model,
                    Extra = key
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> Post<T>(string urlBase, string servicePrefix, string controller, T model, 
            string accessToken)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}?auth={2}", servicePrefix, controller, accessToken);
                var response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                var newRecord = JsonConvert.DeserializeObject<PostResponse>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Record added OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> Put<T>(string urlBase, string servicePrefix, string controller,  T model)
        {
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(request, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                client.BaseAddress = new Uri(urlBase);
                var url = string.Format("{0}{1}", servicePrefix, controller);
                var response = await client.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                var result = await response.Content.ReadAsStringAsync();
                var newRecord = JsonConvert.DeserializeObject<T>(result);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Record updated OK",
                    Result = newRecord,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> Delete<T>(string urlBase, string servicePrefix, string controller,
            string tokenType, string accessToken, T model)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
                var url = string.Format("{0}{1}/{2}", servicePrefix, controller, model.GetHashCode());
                var response = await client.DeleteAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = response.StatusCode.ToString(),
                    };
                }

                return new Response
                {
                    IsSuccess = true,
                    Message = "Record deleted OK",
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<Response> CheckConnection()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Please turn on your internet settings.",
                };
            }

            var isReachable = await CrossConnectivity.Current.IsRemoteReachable("google.com");
            if (!isReachable)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "Check your internet connection.",
                };
            }
            return new Response
            {
                IsSuccess = true,
                Message = "Ok.",
            };
        }
    }
}
