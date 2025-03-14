using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using Project123Api.Hubs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Project123Api.Repositories;
using Project123.Dto;
using Azure;
using System.Security.Claims;
namespace Project123Api.Services
{
    public class NotificationService 
    {
        private readonly DataDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<NotificationHub> _hubContext; 

        public NotificationService(IConfiguration configuration, DataDbContext dataDbContext, IHubContext<NotificationHub> hubContext)
        {
            _configuration = configuration;
            _db = dataDbContext;
  
            _hubContext = hubContext;
        }

        //public async Task NotifyUsers(int? artistId, string message)
        //{
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        // Get all user IDs who follow the artist
        //        string sqlSearchUserFollow = @"SELECT UserId FROM UserArtists WHERE ArtistId = @ArtistId";
        //        var followers = (await connection.QueryAsync<int>(sqlSearchUserFollow, new { ArtistId = artistId })).ToList();
        //        // Insert notifications for each user
        //        string sqlInsertNotification = @"
        //    INSERT INTO Notification (UserId, NotiMessage, IsRead, CreateAt)
        //    VALUES (@UserId, @NotiMessage, @IsRead, @CreateAt)";
        //        try
        //        {
        //            foreach (var userId in followers)
        //            {

        //                await connection.ExecuteAsync(sqlInsertNotification, new
        //                {
        //                    UserId = userId,
        //                    NotiMessage = message,
        //                    IsRead = false, // Default to unread
        //                    CreateAt = DateTime.Now // Set the current timestamp
        //                });
        //                //  Send notification to each user
        //            
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //           var error = ex.Message;
        //        }

        //    }
        public async Task NotifyUsers(int? albumId, int? artistId, string message)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Get all user IDs who follow the artist
                string sqlSearchUserFollow = @"SELECT UserId FROM UserArtists WHERE ArtistId = @ArtistId";
                var followers = (await connection.QueryAsync<int>(sqlSearchUserFollow, new { ArtistId = artistId })).ToList();

                // Insert notifications for each user
                string sqlInsertNotification = @"
        INSERT INTO Notification (UserId, AlbumId, NotiMessage, IsRead, CreateAt)
        VALUES (@UserId, @AlbumId, @NotiMessage, @IsRead, @CreateAt)";

                try
                {
                    foreach (var userId in followers)
                    {
                        await connection.ExecuteAsync(sqlInsertNotification, new
                        {
                            UserId = userId,
                            AlbumId = albumId,
                            NotiMessage = message,
                            IsRead = false, // Default to unread
                            CreateAt = DateTime.Now // Set the current timestamp
                        });

                        //await connection.ExecuteAsync(sqlInsertNotificationIsRead, new
                        //{
                        //    NotificationId = notificaition,
                        //    UserId = userId,
                        //    NotiMessage = message,
                        //}

                        Console.WriteLine($"Sending notification to user {userId}");

                        // Send real-time notification via SignalR
                        //await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", message);
                        await _hubContext.Clients.All.SendAsync("ReceiveUnreadNotifications", userId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending notification: {ex.Message}");
                }
            }
        }




        public async Task NotifyUsers1(int? artistId, string message)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Get all user IDs who follow the artist
                string sqlSearchUserFollow = @"SELECT UserId FROM UserArtists WHERE ArtistId = @ArtistId";
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlSearchUserFollow, connection);
                dataAdapter.SelectCommand.Parameters.AddWithValue("@ArtistId", artistId);

                DataTable followersTable = new DataTable();
                dataAdapter.Fill(followersTable);

                // Insert notifications for each user
                string sqlInsertNotification = @"
        INSERT INTO Notification (UserId, NotiMessage, IsRead, CreateAt)
        VALUES (@UserId, @NotiMessage, @IsRead, @CreateAt)";

                try
                {
                    foreach (DataRow row in followersTable.Rows)
                    {
                        int userId = Convert.ToInt32(row["UserId"]);

                        SqlCommand insertCommand = new SqlCommand(sqlInsertNotification, connection);
                        insertCommand.Parameters.AddWithValue("@UserId", userId);
                        insertCommand.Parameters.AddWithValue("@NotiMessage", message);
                        insertCommand.Parameters.AddWithValue("@IsRead", false); // Default to unread
                        insertCommand.Parameters.AddWithValue("@CreateAt", DateTime.Now); // Set the current timestamp

                        await insertCommand.ExecuteNonQueryAsync();

                        Console.WriteLine($"Sending notification to user {userId}");

                        // Send real-time notification via SignalR
                        //await _hubContext.Clients.Group(userId.ToString()).SendAsync("ReceiveNotification", message);
                        await _hubContext.Clients.All.SendAsync("ReceiveUnreadNotifications", userId);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending notification: {ex.Message}");
                }
            }
        }

        //public async Task GetUnreadNotifications(string userId)
        //{
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        string query = "SELECT Id, Message FROM Notifications WHERE UserId = @UserId AND IsRead = 0";
        //        var unreadNotifications = await connection.QueryAsync<SpotSidebarModel>(query, new { UserId = userId });
        //        await _hubContext.Clients.Caller.SendAsync("ReceiveUnreadNotifications", unreadNotifications);

        //    }
        //}



    }
}
