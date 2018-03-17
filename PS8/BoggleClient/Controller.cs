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
        /// The view controlled by this Controller
        /// </summary>
        private IBoggleView view;

        /// <summary>
        /// The timer for the game
        /// </summary>
        private System.Windows.Forms.Timer timer;

        /// <summary>
        /// For canceling the current operation
        /// </summary>
        private CancellationTokenSource tokenSource;

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
        /// Displays the state of the game (active, pending, completed)
        /// </summary>
        private string gameState;

        /// <summary>
        /// A user token is valid if it is non-null and identifies a user.
        /// A user consists of a nickname and a unique user token. A nickname can be any string, 
        /// such as "Joe" or "Spike". A nickname does not need to be unique.
        /// </summary>
        private string userToken;

        /// <summary>
        /// Words entered by users and displayed on the word list
        /// </summary>
        private string words;

        /// <summary>
        /// Identifies if event deals the first palyer
        /// </summary>
        private bool isPlayer1;

        /// <summary>
        /// Used for thread locking
        /// </summary>
        private object test;


        /// <summary>
        /// Creates controller for the provided view
        /// Commences a new Boggle game and sets up the event handlers
        /// </summary>
        /// <param name="view"></param>
        public Controller(IBoggleView view)
        {
            this.view = view;

            test = new Object();

            this.words = "";
            this.gameState = "completed";
            this.gameID = "";
            this.isPlayer1 = true;

            timer = new System.Windows.Forms.Timer();
            timer.Tick += delegate
            {
                FetchDataFromServer();
            };
            timer.Interval = 1000;
            EventSetup();
        }

        /// <summary>
        /// Handles all the events to be fired
        /// </summary>
        private void EventSetup()
        {
            view.RegisterEvent += HandleRegisterAsync;
            view.CancelRegisterEvent += HandleCancelRegister;
            view.RequestEvent += HandleRequestAsync;
            view.CancelRequestEvent += HandleCancelRequestAsync;
            view.WordEnteredEvent += HandleWordEntered;
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
                using (HttpClient client = CreateClient(domain))
                {
                    // Create the parameter
                    dynamic user = new ExpandoObject();
                    user.Nickname = name;

                    view.RegisterEnabled = false;

                    // Compose and send the request.
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    view.CancelEnabled = true;
                    HttpResponseMessage response = await client.PostAsync("users", content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        view.CancelEnabled = false;
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic temp = JsonConvert.DeserializeObject(result);

                        // Wait for thread to finish and enable some features
                        lock (test)
                        {
                            userToken = (string)(temp.UserToken);
                            this.domain = domain;
                            view.TimeEnabled = true;
                            view.RequestEnabled = true;
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

            finally
            {
                view.CancelEnabled = false;
                view.RegisterEnabled = true;
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
                using (HttpClient client = CreateClient(domain))
                {
                    // Create the parameter
                    dynamic user = new ExpandoObject();
                    user.UserToken = userToken;
                    user.TimeLimit = time;

                    view.RequestEnabled = false;
                    view.CancelRequestEnabled = true;

                    // Compose and send the request
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("games", content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic temp = JsonConvert.DeserializeObject(result);

                        lock (test)
                            gameID = (string)(temp.GameID);

                        response = await client.GetAsync("games/" + gameID);
                        result = await response.Content.ReadAsStringAsync();
                        temp = JsonConvert.DeserializeObject(result);

                        // Wait for thread to finish and enable/disable some features
                        lock (test)
                        {
                            gameState = "pending";

                            if ("active".Equals((string)temp.GameState))
                                isPlayer1 = false;
                            else
                                isPlayer1 = true;

                            view.RegisterEnabled = false;
                            view.TimeEnabled = false;
                            timer.Enabled = true;
                        }
                    }
                    else
                    {
                        view.RequestEnabled = true;
                        view.CancelRequestEnabled = false;
                        MessageBox.Show("Error joining game: " + response.StatusCode + "\n" + response.ReasonPhrase);
                    }
                }
            }
            catch (TaskCanceledException)
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

                    timer.Enabled = false;

                    // Compose and send the request
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("games", content);

                    // Wait for thread to finish and reset the game
                    lock (test)
                        Reset();
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
                    // Create the parameter
                    dynamic user = new ExpandoObject();
                    user.UserToken = userToken;
                    user.Word = word;

                    // Compose and send the request.
                    tokenSource = new CancellationTokenSource();
                    StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("games/" + gameID, content, tokenSource.Token);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        String result = await response.Content.ReadAsStringAsync();

                        // Wait for thread to finish and display score and words
                        lock (test)
                        {
                            dynamic temp = JsonConvert.DeserializeObject(result);
                            int score = int.Parse((string)temp.Score);
                            words += word + " (" + score + ")\n";
                            view.Wordlist = words;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error incorrect word: " + response.StatusCode + "\n" + response.ReasonPhrase);
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
        private async void FetchDataFromServer()
        {
            try
            {
                using (HttpClient client = CreateClient(domain))
                {
                    HttpResponseMessage response = await client.GetAsync("games/" + gameID);

                    // Deal with the response
                    if (response.IsSuccessStatusCode)
                    {
                        String result = await response.Content.ReadAsStringAsync();
                        dynamic temp = JsonConvert.DeserializeObject(result);
                        dynamic op, player;

                        // Wait for thread to finish and display game over features
                        lock (test)
                        {
                            if (isPlayer1)
                            {
                                player = temp.Player1;
                                op = temp.Player2;
                            }
                            else
                            {
                                player = temp.Player2;
                                op = temp.Player1;
                            }

                            if ("pending".Equals((string)temp.GameState))
                            {

                            }
                            else if ("active".Equals((string)temp.GameState) && gameState.Equals("pending"))
                            {
                                view.BoardEnabled = true;


                                view.Time = (string)temp.TimeLeft;
                                view.LoadBoard((string)temp.Board);

                                view.Score = (string)player.Score;
                                view.Player2 = (string)op.Nickname;
                                view.Player2Score = (string)op.Score;

                                gameState = "active";
                            }
                            else if ("active".Equals((string)temp.GameState) && gameState.Equals("active"))
                            {
                                view.Time = temp.TimeLeft;

                                view.Score = (string)player.Score;
                                view.Player2Score = (string)op.Score;

                            }
                            else if ("completed".Equals((string)temp.GameState) && gameState.Equals("active"))
                            {
                                gameState = "completed";
                                List<dynamic> myWords = new List<dynamic>(), opWords = new List<dynamic>();

                                myWords = new List<dynamic>(player.WordsPlayed);
                                opWords = new List<dynamic>(op.WordsPlayed);

                                string message = "GAMEOVER\n";

                                // Display both players' name, score, and wordlist
                                message += "\nPlayer1 - " + (string)player.Nickname + ": " + (string)player.Score + " pts.\n";

                                foreach (dynamic s in myWords)
                                    message += (string)s.Word + " (" + (int)s.Score + ")\n";

                                message += "\nPlayer2 - " + (string)op.Nickname + ": " + (string)op.Score + " pts.\n";

                                foreach (dynamic s in opWords)
                                    message += (string)s.Word + " (" + (int)s.Score + ")\n";

                                MessageBox.Show(message);

                                timer.Enabled = false;
                                view.BoardEnabled = false;
                            }
                            else if ("completed".Equals((string)temp.GameState) && gameState.Equals("completed"))
                            {
                            }
                            else
                            {
                                timer.Enabled = false;
                                MessageBox.Show("Something went wrong");
                            }
                        }
                    }

                    else
                    {
                        MessageBox.Show("Error game status: " + response.StatusCode + "\n" + response.ReasonPhrase);
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Helper method to reset all but username/domain for starting a new game
        /// </summary>
        private void Reset()
        {
            view.RequestEnabled = true;
            view.CancelRequestEnabled = false;
            view.TimeEnabled = true;
            view.Time = "";
            view.RegisterEnabled = true;
            view.Score = "";
            view.Player2 = "";
            view.Player2Score = "";
            view.BoardEnabled = false;
            view.Wordlist = "";
            view.EnterWordBox = "";
            view.LoadBoard("");
            this.words = "";
            this.gameState = "completed";
            this.gameID = "";
            this.isPlayer1 = true;
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

