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
    /// <summary>
    /// The controller for the BoggleClient
    /// </summary>
    class Controller
    {
        /// <summary>
        /// The window controlled by this Controller
        /// </summary>
        private IBoggleView window;

        /// <summary>
        /// A user token is valid if it is non-null and identifies a user.
        /// A user consists of a nickname and a unique user token. A nickname can be any string, 
        /// such as "Joe" or "Spike". A nickname does not need to be unique.
        /// </summary>
        private string userToken;

        /// <summary>
        /// The user provides the domain name of a Boggle Server
        /// </summary>
        private string domain;

        /// <summary>
        /// A user is provided with a game ID after registering.
        /// A pending game contains a gameID.
        /// </summary>
        private string gameID;

        /// <summary>
        /// Identifies if event deals the first palyer
        /// </summary>
        private bool isPlayer1;

        /// <summary>
        /// Displays the state of the game (active, pending, completed)
        /// </summary>
        private string gameState;

        /// <summary>
        /// Words entered by users and displayed on the word list
        /// </summary>
        private string words;

        /// <summary>
        /// The timer for the game
        /// </summary>
        System.Windows.Forms.Timer t;

        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;

        /// <summary>
        /// Creates controller for the provided window
        /// Commences a new Boggle game and sets up the event handlers
        /// </summary>
        /// <param name="window"></param>
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

        /// <summary>
        /// Handles all the events to be fired
        /// </summary>
        private void EventSetup()
        {
            window.RegisterEvent += HandleRegisterAsync;
            window.CancelRegisterEvent += HandleCancelRegister;
            window.RequestEvent += HandleRequestAsync;
            window.CancelRequestEvent += HandleCancelRequestAsync;
            window.WordEnteredEvent += HandleWordEntered;
        }

        /// <summary>
        /// Registers a user to play Boggle with the given Boggle domain and username
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="name"></param>
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

        /// <summary>
        /// Cancels the current register operation
        /// </summary>
        private void HandleCancelRegister()
        {
            tokenSource.Cancel();
        }

        /// <summary>
        /// Register the time the user wishes to play the game for
        /// Request a game with another player and start a game with them.
        /// </summary>
        /// <param name="time"></param>
        private async void HandleRequestAsync(int time)
        {
            try
            {
                window.RequestEnabled = false;
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

                        if ("active".Equals((string)temp.GameState))
                            isPlayer1 = false;
                        else
                            isPlayer1 = true;

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

        /// <summary>
        /// Cancels the request for a user and re-enables the buttons
        /// </summary>
        private async void HandleCancelRequestAsync()
        {
            try
            {
                using (HttpClient client = CreateClient(domain))
                {
                    dynamic user = new ExpandoObject();
                    user.UserToken = userToken;

                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("games" , content);

                    if (response.IsSuccessStatusCode)
                    {
                        window.RequestEnabled = true;
                        window.CancleRequestEnabled = false;
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
            tokenSource.Cancel();
        }

        /// <summary>
        /// Displays the words entered by the users in the Boggle game
        /// </summary>
        /// <param name="word"></param>
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
                        int score = int.Parse((string)temp.Score);
                        {
                            words+=word+" ("+score+")\n";
                            window.Wordlist = words;
                        }
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

        /// <summary>
        /// Communicates with the server and displays the appropriate time, game status,
        /// player names and scores.
        /// </summary>
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
                        else if ("complete".Equals((string)temp.GameState) && gameState.Equals("active"))
                        {
                            gameState = "complete";

                        }
                        else
                        {
                            
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

        /// <summary>
        /// Creates an HttpClient for communicating with the server.
        /// </summary>
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

