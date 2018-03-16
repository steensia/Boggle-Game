// Written by Joe Zachary for CS 3500, March 2016
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace Boggle
{
    /// <summary>
    /// The struct that is returned by the RestClient methods below
    /// </summary>
    public struct Response
    {
        public HttpStatusCode Status { get; set; }

        public dynamic Data { get; set; }

        /// <summary>
        /// Converts an HttpStatusCode into a Response
        /// </summary>
        static public implicit operator Response(HttpStatusCode code)
        {
            return new Response { Status = code };
        }
    }

    /// <summary>
    /// Provides convenient ways to make HTTP requests
    /// </summary>
    public class RestTestClient
    {
        /// <summary>
        /// Creates a RestClient given the domain of a web server
        /// </summary>
        /// <param name="domain"></param>
        public RestTestClient(string domain)
        {
            this.domain = new Uri(domain);
        }

        // The domain used by the client
        private Uri domain;

        /// <summary>
        /// Creates an HttpClient for communicating with GitHub.  The GitHub API requires specific information
        /// to appear in each request header.
        /// </summary>
        private HttpClient CreateClient()
        {
            // Create a client whose base address is the GitHub server
            HttpClient client = new HttpClient();
            client.BaseAddress = domain;

            // There is more client configuration to do, depending on the request.
            return client;
        }

        /// <summary>
        /// Does a GET to the url, where the query string should have {0}, {1}, and so
        /// on in places where parameter values belong.  The parameterValues 
        /// should be those values.  The response code and the response object
        /// is returned if the request was successful; just the response code is
        /// returned if the request was unsuccessful.
        /// 
        /// For example:
        /// DoGetAsync("this/is/a/test?name={0}&age={1}, "James", "57")
        /// </summary>
        public async Task<Response> DoGetAsync(string url, params string[] parameterValues)
        {
            for (int i = 0; i < parameterValues.Length; i++)
            {
                parameterValues[i] = Uri.EscapeDataString(parameterValues[i]);
            }

            using (HttpClient client = CreateClient())
            {
                url = String.Format(url, parameterValues);
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    String result = await response.Content.ReadAsStringAsync();
                    return new Response { Status = response.StatusCode, Data = JsonConvert.DeserializeObject(result) };
                }
                else
                {
                    return response.StatusCode;
                }
            }
        }

        /// <summary>
        /// Does a POST to the url, where the data is send in the request body.
        /// The response code and the response object is returned if the request was 
        /// successful; just the response code is returned if the request was unsuccessful.
        /// </summary>
        public async Task<Response> DoPostAsync(string url, dynamic data)
        {
            using (HttpClient client = CreateClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return new Response { Status = response.StatusCode, Data = JsonConvert.DeserializeObject(result) };
                }
                else
                {
                    return response.StatusCode;
                }
            }
        }

        /// <summary>
        /// Does a PUT to the url, where the data is send in the request body.
        /// The response code and the response object is returned if the request was 
        /// successful; just the response code is returned if the request was unsuccessful.
        /// </summary>
        public async Task<Response> DoPutAsync(dynamic data, string url)
        {
            using (HttpClient client = CreateClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return new Response { Status = response.StatusCode, Data = JsonConvert.DeserializeObject(result) };
                }
                else
                {
                    return response.StatusCode;
                }
            }
        }

        public async Task<Response> DoDeleteAsync(string url)
        {
            using (HttpClient client = CreateClient())
            {
                HttpResponseMessage response = await client.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return new Response { Status = response.StatusCode, Data = JsonConvert.DeserializeObject(result) };
                }
                else
                {
                    return response.StatusCode;
                }
            }
        }
    }
}
