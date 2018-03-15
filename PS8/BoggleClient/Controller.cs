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
        private IBoggleView window;
        private string userToken;
        private string domain;
        private string gameID;
        private bool isPlayer1;
        private string gameState;
        System.Windows.Forms.Timer t;

        public Controller(IBoggleView window)
        {
            this.window = window;
            t = new System.Windows.Forms.Timer();
            t.Tick += delegate
            {
                fetchDataFromServer();
            };
            t.Interval = 1000;
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
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic temp = JsonConvert.DeserializeObject(result);
                        gameID = (string)(temp.GameID);
                        
                        response = await client.GetAsync("games/"+gameID);
                        result = await response.Content.ReadAsStringAsync();
                        temp = JsonConvert.DeserializeObject(result);

                        gameState = "pending";
                        if (response.StatusCode.Equals(202))
                            isPlayer1 = true;
                        else
                            isPlayer1 = false;

                        t.Enabled = true;                
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
            }
        }

        private void HandleCancelRequest()
        {
            tokenSource.Cancel();
        }

        private async void HandleWordEntered(string word)
        {
            try
            {
                using (HttpClient client = CreateClient(domain))
                {
                    dynamic user = new ExpandoObject();
                    user.UserToken = userToken;
                    user.Word = word;

                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("games/"+gameID, content, tokenSource.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic temp = JsonConvert.DeserializeObject(result);
                        //window.Score = (string)temp.Score;
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
        }

        private async void fetchDataFromServer()
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

                        if ("pending".Equals((string)temp.GameState))
                        {
                           
                        }
                        else if("active".Equals((string)temp.GameState) && gameState.Equals("pending"))
                        {
                            window.BoardEnabled = true;
                            window.CancleRequestEnabled = false;
                            window.Time = (string)temp.TimeLeft;
                            window.LoadBoard((string)temp.Board);

                            if (isPlayer1)
                            {
                                window.Score = (string)temp.Player1.Score;
                                window.Player2 = (string)temp.Player2.Nickname;
                                window.Player2Score = (string)temp.Player2.Score;
                            }
                            else
                            {
                                window.Score = (string)temp.Player2.Score;
                                window.Player2 = (string)temp.Player1.Nickname;
                                window.Player2Score = (string)temp.Player1.Score;
                            }
                            gameState = "active";
                        }
                        else if("active".Equals((string)temp.GameState) && gameState.Equals("active"))
                        {
                            window.Time = temp.TimeLeft;
                            if (isPlayer1)
                            {
                                window.Score = (string)temp.Player1.Score;
                                window.Player2Score = (string)temp.Player2.Score;
                            }
                            else
                            {
                                window.Score = (string)temp.Player2.Score;
                                window.Player2Score = (string)temp.Player1.Score;
                            }
                        }
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

            return client;
        }
    }
}

