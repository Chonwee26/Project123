using ApacheTech.Common.Extensions.System;
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
        Task<ResponseModel> CreateAlbum(AlbumModel AlbumData);
        Task<IEnumerable<SongModel>>SearchSong(SongModel SongData);
        Task<IEnumerable<AlbumModel>>SearchAlbum(AlbumModel AlbumData);
        
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

        public async Task<ResponseModel> CreateAlbum(AlbumModel AlbumData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateSong = @"INSERT INTO Albums (AlbumName, ArtistName, AlbumImage) 
                             VALUES (@AlbumName, @ArtistName, @AlbumImage)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateSong, connection);
                    command.Parameters.AddWithValue("@AlbumName", AlbumData.AlbumName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ArtistName", AlbumData.ArtistName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@AlbumImage", AlbumData.AlbumImagePath ?? (object)DBNull.Value);
                   

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


        public async Task<ResponseModel> CreateSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateSong = @"INSERT INTO Song (AlbumId, ArtistName, SongName, SongFile, SongGenres, SongImage,SongLength) 
                             VALUES (@AlbumId, @ArtistName, @SongName, @SongFile, @SongGenres, @SongImage, @SongLength)";
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
                    command.Parameters.AddWithValue("@SongLength", SongData.SongLength ?? (object)DBNull.Value);

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


        public async Task<IEnumerable<SongModel>> SearchSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();

            // Check if all fields in UserData are null or empty
            if (string.IsNullOrEmpty(SongData.SongName) && string.IsNullOrEmpty(SongData.ArtistName)&&SongData.AlbumId ==null)
            {
                response.Status = "E";
                response.Message = "Error: Cant'find song";

                return new List<SongModel>(); // Return empty list indicating no data found
            }

            List<SongModel> songList = new List<SongModel>();
            string sqlSelect = @"SELECT s.AlbumId,s.SongId,s.SongName, s.ArtistName, s.SongFile,s.SongGenres,s.SongImage,s.SongLength
                     FROM dbo.Song s";
                  
            List<string> sqlWhereClauses = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (SongData.SongId.HasValue)
            {
                sqlWhereClauses.Add("s.SongId = @SongId");
                sqlParameters.Add(new SqlParameter("@SongId", SongData.SongId.Value));
            }

            if (SongData.AlbumId.HasValue)
            {
                sqlWhereClauses.Add("s.AlbumId = @AlbumId");
                sqlParameters.Add(new SqlParameter("@AlbumId", SongData.AlbumId.Value));
            }

            if (SongData.SongLength.HasValue)
            {
                sqlWhereClauses.Add("s.SongLength = @SongLength");
                sqlParameters.Add(new SqlParameter("@SongLength", SongData.SongLength.Value));
            }

            if (!string.IsNullOrEmpty(SongData.SongName))
            {
                sqlWhereClauses.Add("s.SongName = @SongName");
                sqlParameters.Add(new SqlParameter("@SongName", SongData.SongName));
            }

            if (!string.IsNullOrEmpty(SongData.ArtistName))
            {
                sqlWhereClauses.Add("s.ArtistName = @ArtistName");
                sqlParameters.Add(new SqlParameter("@ArtistName", SongData.ArtistName));
            }

            if (!string.IsNullOrEmpty(SongData.SongGenres))
            {
                sqlWhereClauses.Add("s.SongGenres = @SongGenres");
                sqlParameters.Add(new SqlParameter("@SongGenres", SongData.SongGenres));
            }

            if (!string.IsNullOrEmpty(SongData.SongFilePath))
            {
                sqlWhereClauses.Add("s.SongFile = @SongFile");
                sqlParameters.Add(new SqlParameter("@SongFile", SongData.SongFilePath));
            }

            if (!string.IsNullOrEmpty(SongData.SongImagePath))
            {
                sqlWhereClauses.Add("s.SongImage = @SongImage");
                sqlParameters.Add(new SqlParameter("@SongImage", SongData.SongImagePath));
            }

            string sqlWhere = sqlWhereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", sqlWhereClauses) : "";
            sqlSelect += sqlWhere;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        command.Parameters.AddRange(sqlParameters.ToArray());

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                SongModel song = new SongModel
                                {
                                    SongId = reader.GetInt32("SongId"),
                                    AlbumId = reader.IsDBNull(reader.GetOrdinal("AlbumId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AlbumId")),
                                    SongLength = reader.IsDBNull(reader.GetOrdinal("SongLength")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("SongLength")),
                                    SongName = reader["SongName"].ToString(),
                                    ArtistName = reader["ArtistName"].ToString(),
                                    SongGenres = reader["SongGenres"].ToString(),
                                    SongFilePath = reader["SongFile"].ToString(),
                                    SongImagePath = reader["SongImage"].ToString(),
                                };

                                songList.Add(song);
                            }
                        }
                    }
                }

                if (songList.Count == 0)
                {
                    response.Status = "E";
                    response.Message = "No data found";
                }
                else
                {
                    response.Status = "S";
                    response.Message = "Success";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                response.Status = "E";
                response.Message = ex.Message;
            }

            return songList;
        }


        public async Task<IEnumerable<AlbumModel>> SearchAlbum(AlbumModel AlbumData)
        {
            ResponseModel response = new ResponseModel();

            // Check if all fields in UserData are null or empty
            if (string.IsNullOrEmpty(AlbumData.AlbumName) && string.IsNullOrEmpty(AlbumData.ArtistName))
            {
                response.Status = "E";
                response.Message = "Error: Cant'find Album";

                return new List<AlbumModel>(); // Return empty list indicating no data found
            }

            List<AlbumModel> albumList = new List<AlbumModel>();
            string sqlSelect = @"SELECT s.AlbumId,s.AlbumName, s.ArtistName,s.AlbumImage
                     FROM dbo.Albums s";

            List<string> sqlWhereClauses = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (AlbumData.AlbumId.HasValue)
            {
                sqlWhereClauses.Add("s.SoAlbumIdngId = @AlbumId");
                sqlParameters.Add(new SqlParameter("@AlbumId", AlbumData.AlbumId.Value));
            }



            if (!string.IsNullOrEmpty(AlbumData.AlbumName))
            {
                sqlWhereClauses.Add("s.AlbumName = @AlbumName");
                sqlParameters.Add(new SqlParameter("@AlbumName", AlbumData.AlbumName));
            }

            if (!string.IsNullOrEmpty(AlbumData.ArtistName))
            {
                sqlWhereClauses.Add("s.ArtistName = @ArtistName");
                sqlParameters.Add(new SqlParameter("@ArtistName", AlbumData.ArtistName));
            }

            if (!string.IsNullOrEmpty(AlbumData.AlbumImagePath))
            {
                sqlWhereClauses.Add("s.AlbumImage = @AlbumImage");
                sqlParameters.Add(new SqlParameter("@AlbumImage", AlbumData.AlbumImagePath));
            }




            string sqlWhere = sqlWhereClauses.Count > 0 ? " WHERE " + string.Join(" AND ", sqlWhereClauses) : "";
            sqlSelect += sqlWhere;

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        command.Parameters.AddRange(sqlParameters.ToArray());

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                AlbumModel album = new AlbumModel
                                {
                                    AlbumId = reader.GetInt32("AlbumId"),                            
                                    AlbumName = reader["AlbumName"].ToString(),
                                    ArtistName = reader["ArtistName"].ToString(),                                                             
                                    AlbumImagePath = reader["AlbumImage"].ToString(),
                                };
                                albumList.Add(album);
                            }
                        }
                    }
                }

                if (albumList.Count == 0)
                {
                    response.Status = "E";
                    response.Message = "No data found";
                }
                else
                {
                    response.Status = "S";
                    response.Message = "Success";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                response.Status = "E";
                response.Message = ex.Message;
            }

            return albumList;
        }





    }
}
