using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Dapper;
using Project123Api.Repositories;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Project123.Dto;


namespace Project123Api.Hubs
{

    public class NotificationHub : Hub
    {
        private readonly DataDbContext _db;
        private readonly IConfiguration _configuration;

        public NotificationHub(IConfiguration configuration, DataDbContext dataDbContext)
        {
            _configuration = configuration;
            _db = dataDbContext;

          
        }

        //public override async Task OnConnectedAsync()
        //{
        //    //var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    //string? userId = Context.UserIdentifier; // Get the user ID
        //    // manual javascript 
        //    var userId = Context.GetHttpContext()?.Request.Query["userId"];
        //    await Clients.Caller.SendAsync("ReceiveMessage", $"Your UserID is: {userId}");
        //    await base.OnConnectedAsync();
        //}
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            string? userId = httpContext?.Request.Query["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                // Add user to a group with their user ID
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);

                // Fetch unread notifications
                await SendUnreadNotifications(userId);
            }

            await base.OnConnectedAsync();
        }

        //public async Task GetUnreadNotifications(string userId)
        //{
        //    using (var connection = new SqlConnection("YourConnectionString"))
        //    {
        //        string query = "SELECT Id, Message FROM Notifications WHERE UserId = @UserId AND IsRead = 0";
        //        var unreadNotifications = await connection.QueryAsync<NotificationModel>(query, new { UserId = userId });

        //        await Clients.Caller.SendAsync("ReceiveUnreadNotifications", unreadNotifications);
        //    }
        //}
        public async Task SendUnreadNotifications(string userId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    // Fetch unread notifications
                    //string sqlQuery = "SELECT UserId, NotiMessage FROM Notification WHERE UserId = @UserId AND IsRead = 0";
                    //string sqlQuery = @"

                    //                    WITH SongDedup AS (
                    //                        SELECT 
                    //                            s.SongName, s.SongImage, s.AlbumId, s.ArtistName,
                    //                            ROW_NUMBER() OVER (PARTITION BY s.SongName ORDER BY 
                    //                                CASE WHEN s.AlbumId IS NULL THEN 1 ELSE 0 END,  -- Prioritize songs without an album
                    //                                s.AlbumId DESC  -- Otherwise, pick the highest AlbumId
                    //                            ) AS RowNum
                    //                        FROM Song s
                    //                    )
                    //                    SELECT DISTINCT 
                    //                        noti.NotificationId, noti.UserId, noti.NotiMessage, noti.IsRead, noti.CreateAt,
                    //                        alb.AlbumId,
                    //                        art.ArtistId, art.ArtistImage, art.ArtistName, 
                    //                        COALESCE(alb.AlbumId, NULL) AS AlbumId, 
                    //                        COALESCE(alb.AlbumName, NULL) AS AlbumName, 
                    //                        COALESCE(alb.AlbumImage, NULL) AS AlbumImage, 
                    //                        s.SongName, s.SongImage
                    //                    FROM Notification noti 

                    //                    -- Join artists followed by the user
                    //                    INNER JOIN UserArtists usrart ON noti.UserId = usrart.UserId  
                    //                    INNER JOIN Artist art ON usrart.ArtistId = art.ArtistId  

                    //                    -- Deduplicated song selection
                    //                    LEFT JOIN SongDedup s ON s.ArtistName = art.ArtistName  
                    //                                          AND s.SongName = noti.NotiMessage  
                    //                                          AND s.RowNum = 1  -- Pick only one version of the song

                    //                    -- Albums should match only when AlbumId is available
                    //                    LEFT JOIN Albums alb ON alb.AlbumId = s.AlbumId  

                    //                    WHERE noti.UserId = @UserId 
                    //                    AND noti.IsRead = 0
                    //                    AND s.SongName IS NOT NULL;
                    //                    ";

                    string sqlQuery = @"                                                                     
                                         SELECT DISTINCT 
                                             noti.NotificationId, noti.UserId, noti.AlbumId, noti.NotiMessage, noti.IsRead, noti.CreateAt,
                                             alb.AlbumId, alb.AlbumName, alb.AlbumImage,
                                             art.ArtistId, art.ArtistImage, art.ArtistName
 
 
                                         FROM Notification noti 

                                         -- Join artists followed by the user
                                         INNER JOIN UserArtists usrart ON noti.UserId = usrart.UserId  
                                         INNER JOIN Artist art ON usrart.ArtistId = art.ArtistId  


                                         -- Albums should match only when AlbumId is available
                                         inner JOIN Albums alb ON alb.ArtistName = art.ArtistName  

                                         WHERE noti.UserId = 7003 
                                         AND noti.IsRead = 0
                                         AND noti.NotiMessage = alb.AlbumName
                                         AND noti.AlbumId = alb.AlbumId
                                        ";
                    var unreadNotifications = await connection.QueryAsync<NotiMessageModel>(sqlQuery, new { UserId = userId });

                    // Send unread notifications to the connected user
                    //await Clients.Caller.SendAsync("ReceiveUnreadNotifications", unreadNotifications);
                    await Clients.Group(userId.ToString()).SendAsync("ReceiveUnreadNotifications", unreadNotifications);
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Error sending notification: {ex.Message}");
                }
             
            }
        }

        public async Task SendNotification(string userId, string message)
         {
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("Invalid userId");
                return; // Return early if userId is invalid
            }

            try
            {
                // Sending notification to the specific user
                //await Clients.User(userId).SendAsync("ReceiveNotification", userId, message);
                //await Clients.All.SendAsync("ReceiveNotification", userId, message);
                await Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
            }
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                Console.WriteLine($"User {userId} removed from group.");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }


}
