using AuthenticationPlugin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project123.Dto;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project123Api.Repositories
{

    public interface ISpotRepository
    {
        Task<ResponseModel> CreateSong(SongModel SongData);
        
    }

    public class SpotRepository : ISpotRepository
    {
        private readonly IConfiguration _configuration;

        public SpotRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public async Task<ResponseModel> CreateSong(SongModel SongData)
        //{
        //    ResponseModel response = new ResponseModel();
        //    string sqlCreateSong = @"INSERT INTO Song (AlbumId, ArtistName,SongName,SongFile,SongGenres,SongImage) VALUES (@AlbumId, @ArtistName, @SongName,@SongFile,@SongGenres,@SongImage)";
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        try
        //        {
        //            SqlCommand command = new SqlCommand(sqlCreateSong, connection);
        //            command.Parameters.AddWithValue("@AlbumId", SongData.AlbumId);
        //            command.Parameters.AddWithValue("@ArtistName", SongData.ArtistName);
        //            command.Parameters.AddWithValue("@SongName", SongData.SongName);
        //            command.Parameters.AddWithValue("@SongFile", SongData.SongFilePath);
        //            command.Parameters.AddWithValue("@SongGenres", SongData.SongGenres);
        //            command.Parameters.AddWithValue("@SongImage", SongData.SongImagePath);
        //            await command.ExecuteNonQueryAsync();

        //            response.Status = "S";
        //            response.Message = "User created successfully.";
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Status = "E";
        //            response.Message = ex.Message;
        //        }
        //        finally
        //        {
        //            connection.Close();
        //        }
        //    }

        //    return await Task.FromResult(response);
        //}

        public async Task<ResponseModel> CreateSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateSong = @"INSERT INTO Song (AlbumId, ArtistName, SongName, SongFile, SongGenres, SongImage) 
                             VALUES (@AlbumId, @ArtistName, @SongName, @SongFile, @SongGenres, @SongImage)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateSong, connection);
                    command.Parameters.AddWithValue("@AlbumId", SongData.AlbumId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ArtistName", SongData.ArtistName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SongName", SongData.SongName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SongFile", SongData.SongFilePath ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SongGenres", SongData.SongGenres ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@SongImage", SongData.SongImagePath ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "User created successfully.";
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }

            return response;
        }







    }
}
