using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Boggle
{
    /// <summary>
    /// This interface defines a collection of operations provided by BoggleService.svc.  Each method that 
    /// is annotated as with [WebGet] or [WebInvoke] will be exposed by the service
    /// </summary>
    [ServiceContract]
    public interface IBoggleService
    {
        /// <summary>
        /// Sends back index.html as the response body.
        /// </summary>
        [WebGet(UriTemplate = "/api")]
        Stream API();

        /// <summary>
        /// Create a new user.
        /// If Nickname is null, or is empty when trimmed, or contains more than 50 characters when trimmed, responds with status 403 (Forbidden).
        /// Otherwise, creates a new user with a unique UserToken and the trimmed Nickname. The returned UserToken should be used to identify the 
        /// user in subsequent requests. Responds with status 201 (Created).
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/users")]
        string CreateUser(UserInfo user);

        /// <summary>
        /// Join a game.
        /// If UserToken is invalid, TimeLimit < 5, or TimeLimit > 120, responds with status 403 (Forbidden).
        /// Otherwise, if UserToken is already a player in the pending game, responds with status 409 (Conflict). 
        /// Otherwise, if there is already one player in the pending game, adds UserToken as the second player. The pending game becomes active and 
        /// a new pending game with no players is created. The active game's time limit is the integer average of the time limits requested by the 
        /// two players. Returns the new active game's GameID (which should be the same as the old pending game's GameID). Responds with status 201 (Created).
        /// Otherwise, adds UserToken as the first player of the pending game, and the TimeLimit as the pending game's requested time limit. Returns the 
        /// pending game's GameID. Responds with status 202 (Accepted).
        /// </summary>
        [WebInvoke(Method = "POST", UriTemplate = "/games")]
        string JoinGame(TimeInfo user);

        /// Cancel a pending request to join a game.
        /// If UserToken is invalid or is not a player in the pending game, responds with status 403 (Forbidden).
        /// Otherwise, removes UserToken from the pending game and responds with status 200 (OK).
        [WebInvoke(Method = "PUT", UriTemplate = "/games")]
        void CancelJoin(TimeInfo user);

        /// Play a word in a game.
        /// If Word is null or empty or longer than 30 characters when trimmed, or if GameID or UserToken is invalid, or if UserToken is not a player in the 
        /// game identified by GameID, responds with response code 403 (Forbidden).
        /// Otherwise, if the game state is anything other than "active", responds with response code 409 (Conflict).
        /// Otherwise, records the trimmed Word as being played by UserToken in the game identified by GameID. Returns the score for Word in the context of the 
        /// game (e.g. if Word has been played before the score is zero). Responds with status 200 (OK). Note: The word is not case sensitive.
        [WebInvoke(Method = "PUT", UriTemplate = "/games/{gameID}")]
        int PlayWord(Player user, string gameID);

        /// Get game status information.
        /// If GameID is invalid, responds with status 403 (Forbidden).
        /// Otherwise, returns information about the game named by GameID as illustrated below. Note that the information returned depends on whether "Brief=yes" 
        /// was included as a parameter as well as on the state of the game. Responds with status code 200 (OK). Note: The Board and Words are not case sensitive.
        [WebGet(UriTemplate = "/games/{gameID}?brief={brief}")]
        IList<GameStatus> GameStatus(string gameID, string brief);
    }
}
