using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SOS.OrderTracking.Web.Client.Services
{
    public class ApiService
    {
        public HttpClient Http { get; private set; }
        protected readonly ILogger logger;
        protected readonly NavigationManager navigationManager;
        private readonly SignOutSessionStateManager sessionStateManager;

        public ApiService(
           HttpClient http,
           ILogger<ApiService> logger,
           NavigationManager navigationManager,
           SignOutSessionStateManager sessionStateManager)
        {
            this.Http = http;
            this.logger = logger;
            this.navigationManager = navigationManager;
            this.sessionStateManager = sessionStateManager;
        }

        /// <summary>
        /// Gets string from specified URL and deserializes into T type.
        /// Throws HttpRequestException and general Exeption. 
        /// Redirects to Login page in case of AccessToeken is not present or receives Unauthorized response code from server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task<T> GetFromJsonAsync<T>(string path)
        {
            logger.LogInformation($"GetFromJsonAsync -> {path}");
            HttpResponseMessage httpResponse = null;
            try
            {
                logger.LogInformation($"http is null {Http == null}");
               
                httpResponse = await Http.GetAsync(path);

                logger.LogInformation($"{path} success");
            }
            catch(AccessTokenNotAvailableException ex)
            {
                ex.Redirect();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }

            if (httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await httpResponse.Content.ReadAsStringAsync();
                    logger.LogInformation(content);
                    return await httpResponse.Content.ReadFromJsonAsync<T>();
                }
                else if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return default;
                } 
                else
                {
                    throw new Exception(httpResponse.StatusCode.ToString());
                }
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                logger.LogError("logging out");
                await sessionStateManager.SetSignOutState();
                navigationManager.NavigateTo("authentication/logout", true);
            }

            logger.LogError(await httpResponse.Content.ReadAsStringAsync());
            throw new Exception(await httpResponse.Content.ReadAsStringAsync());
        }

        public async Task<TKey> PostFromJsonAsync<TKey, TValue>(string path, TValue tValue)
        {
            HttpResponseMessage httpResponse = null;
            try
            {
                httpResponse = await Http.PostAsJsonAsync(path, tValue);

                logger.LogInformation($"{path} success");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
            }

            if (httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = await httpResponse.Content.ReadAsStringAsync();
                    logger.LogInformation($"{result} result");
                    if (!string.IsNullOrEmpty(result))
                    {
                        var obj =  Newtonsoft.Json.JsonConvert.DeserializeObject<TKey>(result);
                        if (obj == null)
                        {
                            logger.LogWarning("Serialization resulted in null");
                        }
                        return obj;
                    }
                }
                else if (httpResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return default;
                }
                else
                {
                    throw new Exception(httpResponse.StatusCode.ToString());
                }
            }
            else if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                navigationManager.NavigateTo("authentication/login");
            }

            logger.LogError(await httpResponse.Content.ReadAsStringAsync());
            throw new Exception(await httpResponse.Content.ReadAsStringAsync());
        }
    }
}
