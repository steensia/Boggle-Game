using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoggleClient
{
    class Controller
    {
        char[][] board;
        private IBoggleView window;
        private string userToken;
        private string domain;
        private int time;
        private string gameID;

        public Controller(IBoggleView window)
        {
            this.window = window;
            EventSetup();
        }

        private CancellationTokenSource tokenSource;

        void EventSetup()
        {
            window.RegisterEvent += HandleRegisterAsync;
            window.CancelRegisterEvent += HandleCancelRegister;
            window.RequestEvent += HandleRequestAsync;
            window.CancelRequestEvent += HandleCancelRequest;
            window.WordEnteredEvent += HandleWordEntered;
        }

        private async void HandleRegisterAsync(string domain, string name)
        {
            try
            {
                window.RegisterEnabled = false;

                //needs a method to diable proper controles
                using (HttpClient client = CreateClient(domain))
                {
                    dynamic user = new ExpandoObject();
                    user.Nickname = name;

                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    window.CancelEnabled = true;
                    HttpResponseMessage response = await client.PostAsync("users", content, tokenSource.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        window.CancelEnabled = false;
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic temp = JsonConvert.DeserializeObject(result);
                        userToken = (string)(temp.UserToken);
                        this.domain = domain;
                        window.TimeEnabled = true;
                        window.RequestEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Error registering: " + response.StatusCode + "\n" + response.ReasonPhrase);
                    }
                }
            }
            catch (TaskCanceledException)
            {

            }
            finally
            {
                window.CancelEnabled = false;
                window.RegisterEnabled = true;
            }
        }

        private void HandleCancelRegister()
        {
            tokenSource.Cancel();
        }

        private async void HandleRequestAsync(int time)
        {
            try
            {
                window.RequestEnabled = false;
                //needs a method to diable proper controles
                using (HttpClient client = CreateClient(domain))
                {
                    dynamic user = new ExpandoObject();
                    user.UserToken = userToken;
                    user.TimeLimit = time;

                    window.CancleRequestEnabled = true;

                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("games", content, tokenSource.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        window.CancleRequestEnabled = false;
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic temp = JsonConvert.DeserializeObject(result);
                        gameID = (string)(temp.GameID);

                        
                        response = await client.GetAsync("games/"+gameID);
                        result = await response.Content.ReadAsStringAsync();
                        temp = JsonConvert.DeserializeObject(result);
                        string gameState = (string)temp.GameState;
                        bool isPlayer1 = false;
                        string opponent = "";

                        if (gameState.Equals("pending"))
                        {
                            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
                            t.Interval = 1000;
                            
                        }
                        else
                        {
                            window.Player2 = (string)temp.Player1;
                        }
                        

                        window.BoardEnabled = true;                  
                    }
                    else
                    {
                        MessageBox.Show("Error registering: " + response.StatusCode + "\n" + response.ReasonPhrase);
                    }
                }
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                window.CancleRequestEnabled = false;
                window.RequestEnabled = true;
            }
        }

        private void HandleCancelRequest()
        {
            tokenSource.Cancel();
        }

        private async void HandleWordEntered(string word)
        {
            throw new NotImplementedException();
        }

        private async Task StartGameAsync()
        {
            try
            {
                using (HttpClient client = CreateClient(domain))
                {
                    HttpResponseMessage response = await client.GetAsync("games/" + gameID);
                    if (response.IsSuccessStatusCode)
                    {
                       
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic temp = JsonConvert.DeserializeObject(result);

                        string player2 = (string)(temp.Player2);
                        while (true)
                        {

                        }

                        window.BoardEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("Error registering: " + response.StatusCode + "\n" + response.ReasonPhrase);
                    }
                }
            }
            catch
            {

            }
        }

        private static HttpClient CreateClient(string domain)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(domain);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            return client;
        }
    }
}

