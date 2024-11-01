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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Project123Api.Repositories
{

    public interface ISpotRepository
    {
        Task<ResponseModel> CreateSong(SongModel SongData);
        Task<ResponseModel> UpdateSong(SongModel SongData);
        Task<ResponseModel> FavoriteSong(SongModel songData);
        Task<ResponseModel> FavoriteArtist(SpotSidebarModel artistData);
        Task<ResponseModel> DeleteSong(SongModel SongData);
        Task<ResponseModel> RemoveSong(SongModel SongData);
     
        Task<ResponseModel> CreateAlbum(AlbumModel AlbumData);
        Task<ResponseModel> CreateArtist(ArtistModel artistData);
        Task<ResponseModel> CreateGenre(GenreModel genreData);
        Task<ResponseModel>EditAlbum(AlbumModel AlbumData);
        Task<ResponseModel> UpdateArtist(ArtistModel artistData);
        Task<ResponseModel> UpdateGenre(GenreModel genreData);
        Task<ResponseModel> DeleteAlbum(AlbumModel AlbumData);
        Task<ResponseModel> DeleteArtist(ArtistModel artistData);
        Task<ResponseModel> DeleteGenre(GenreModel genreData);
        Task<IEnumerable<SongModel>>SearchSong(SongModel SongData);
        Task<IEnumerable<SongModel>> SearchSongNotInAlbum(SongModel SongData);
        Task<IEnumerable<AlbumModel>>SearchAlbum(AlbumModel AlbumData);
        Task<IEnumerable<ArtistModel>>SearchArtist(ArtistModel artistData);
        Task<IEnumerable<GenreModel>>SearchGenre(GenreModel genreData);
        Task<IEnumerable<AlbumModel>>GetAlbum(AlbumModel AlbumData);
        Task<IEnumerable<SongModel>> GetFavoriteSongs(SongModel SongData);
        Task<List<SongModel>> GetFavSongByUser(string userId);
        Task<List<SpotSidebarModel>> GetFavAlbumAndArtistByUser(string userId);
        Task<IEnumerable<SearchSpotModal>> SearchSpot(SearchSpotModal searchData);
        
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

        public async Task<ResponseModel> CreateGenre(GenreModel genreData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateSong = @"INSERT INTO Genre (GenreName, GenreImage) 
                             VALUES (@GenreName, @GenreImage)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateSong, connection);
                    command.Parameters.AddWithValue("@GenreName", genreData.GenreName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GenreImage", genreData.GenreImagePath ?? (object)DBNull.Value);
                 


                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Genre created successfully.";
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


        public async Task<ResponseModel> CreateArtist(ArtistModel artistData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateSong = @"INSERT INTO Artist (ArtistName, ArtistImage, ArtistBio,ArtistGenres) 
                             VALUES (@ArtistName, @ArtistImage, @ArtistBio,@ArtistGenres)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateSong, connection);
                    command.Parameters.AddWithValue("@ArtistName", artistData.ArtistName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ArtistImage", artistData.ArtistImagePath ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ArtistBio", artistData.ArtistBio ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ArtistGenres", artistData.ArtistGenres ?? (object)DBNull.Value);


                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Artist created successfully.";
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
        public async Task<ResponseModel> EditAlbum(AlbumModel AlbumData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateSong = @" UPDATE Albums
                                    SET AlbumName = @AlbumName,
                                    ArtistName = @ArtistName,
                                    AlbumImage = @AlbumImage
                                    WHERE AlbumId = @AlbumId";
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
                    command.Parameters.AddWithValue("@AlbumId", AlbumData.AlbumId ?? (object)DBNull.Value);
                   

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Edit Album successfully.";
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
            string sqlCreateSong = @"INSERT INTO Song (AlbumId, ArtistName, SongName, SongFile, SongGenres, SongImage,SongLength,FavoriteSong,CreateSongDate) 
                             VALUES (@AlbumId, @ArtistName, @SongName, @SongFile, @SongGenres, @SongImage, @SongLength,@FavoriteSong,@CreateSongDate)";
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
                    command.Parameters.AddWithValue("@FavoriteSong", SongData.FavoriteSong ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreateSongDate", SongData.CreateSongDate ?? (object)DBNull.Value);

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


        public async Task<ResponseModel> UpdateGenre(GenreModel genreData)
        {
            ResponseModel response = new ResponseModel();
            List<string> updateFields = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            // Build dynamic SQL for non-null fields
      

            if (!string.IsNullOrEmpty(genreData.GenreName))
            {
                updateFields.Add("GenreName = @GenreName");
                parameters.Add(new SqlParameter("@GenreName", genreData.GenreName));
            }
    

            if (!string.IsNullOrEmpty(genreData.GenreImagePath))
            {
                updateFields.Add("GenreImage = @GenreImagePath");
                parameters.Add(new SqlParameter("@GenreImagePath", genreData.GenreImagePath));
            }


            // If no fields are updated, return early
            if (updateFields.Count == 0)
            {
                response.Status = "E";
                response.Message = "No fields provided for update.";
                return response;
            }

            // Build the final SQL query
            string sqlUpdateArtist = $@"UPDATE Genre SET {string.Join(", ", updateFields)} 
                                WHERE GenreId = @GenreId";

            parameters.Add(new SqlParameter("@GenreId", genreData.GenreId));

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlUpdateArtist, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Edit Genre successfully.";
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

        public async Task<ResponseModel> UpdateArtist(ArtistModel artistData)
        {
            ResponseModel response = new ResponseModel();
            List<string> updateFields = new List<string>();
            List<SqlParameter> parameters = new List<SqlParameter>();

            // Build dynamic SQL for non-null fields
            if (!string.IsNullOrEmpty(artistData.ArtistBio))
            {
                updateFields.Add("ArtistBio = @ArtistBio");
                parameters.Add(new SqlParameter("@ArtistBio", artistData.ArtistBio));
            }

            if (!string.IsNullOrEmpty(artistData.ArtistName))
            {
                updateFields.Add("ArtistName = @ArtistName");
                parameters.Add(new SqlParameter("@ArtistName", artistData.ArtistName));
            }

            if (!string.IsNullOrEmpty(artistData.ArtistGenres))
            {
                updateFields.Add("ArtistGenres = @ArtistGenres");
                parameters.Add(new SqlParameter("@ArtistGenres", artistData.ArtistGenres));
            }

            if (!string.IsNullOrEmpty(artistData.ArtistImagePath))
            {
                updateFields.Add("ArtistImage = @ArtistImage");
                parameters.Add(new SqlParameter("@ArtistImage", artistData.ArtistImagePath));
            }


            // If no fields are updated, return early
            if (updateFields.Count == 0)
            {
                response.Status = "E";
                response.Message = "No fields provided for update.";
                return response;
            }

            // Build the final SQL query
            string sqlUpdateArtist = $@"UPDATE Artist SET {string.Join(", ", updateFields)} 
                                WHERE ArtistId = @ArtistId";

            parameters.Add(new SqlParameter("@ArtistId", artistData.ArtistId));

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlUpdateArtist, connection);
                    command.Parameters.AddRange(parameters.ToArray());

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Edit Artist successfully.";
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


        //
        //public async Task<ResponseModel> UpdateArtist(ArtistModel artistData)
        //{
        //    ResponseModel response = new ResponseModel();
        //    string sqlCreateSong = @" UPDATE Artist
        //                            SET ArtistBio = @ArtistBio,
        //                            ArtistName = @ArtistName,
        //                            ArtistGenres = @ArtistGenres,
        //                            ArtistImage = @ArtistImage
        //                            WHERE ArtistId = @ArtistId";
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        try
        //        {
        //            SqlCommand command = new SqlCommand(sqlCreateSong, connection);
        //            command.Parameters.AddWithValue("@ArtistBio", artistData.ArtistBio ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@ArtistName", artistData.ArtistName ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@ArtistGenres", artistData.ArtistGenres ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@ArtistImage", artistData.ArtistImagePath ?? (object)DBNull.Value);
        //            command.Parameters.AddWithValue("@ArtistId", artistData.ArtistId ?? (object)DBNull.Value);


        //            await command.ExecuteNonQueryAsync();

        //            response.Status = "S";
        //            response.Message = "Edit Artist successfully.";
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

        //    return response;
        //}

        public async Task<ResponseModel> UpdateSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();
            List<string> updateFields = new List<string>();
            List<SqlParameter> sqlParams = new List<SqlParameter>();

            if (SongData.AlbumId != null)
            {
                updateFields.Add("AlbumId = @AlbumId");
                sqlParams.Add(new SqlParameter("@AlbumId", SongData.AlbumId));
            }
            if (!string.IsNullOrEmpty(SongData.ArtistName))
            {
                updateFields.Add("ArtistName = @ArtistName");
                sqlParams.Add(new SqlParameter("@ArtistName", SongData.ArtistName));
            }
            if (!string.IsNullOrEmpty(SongData.SongName))
            {
                updateFields.Add("SongName = @SongName");
                sqlParams.Add(new SqlParameter("@SongName", SongData.SongName));
            }
            if (!string.IsNullOrEmpty(SongData.SongFilePath))
            {
                updateFields.Add("SongFile = @SongFile");
                sqlParams.Add(new SqlParameter("@SongFile", SongData.SongFilePath));
            }
            if (!string.IsNullOrEmpty(SongData.SongGenres))
            {
                updateFields.Add("SongGenres = @SongGenres");
                sqlParams.Add(new SqlParameter("@SongGenres", SongData.SongGenres));
            }
            if (!string.IsNullOrEmpty(SongData.SongImagePath))
            {
                updateFields.Add("SongImage = @SongImage");
                sqlParams.Add(new SqlParameter("@SongImage", SongData.SongImagePath));
            }

            if (SongData.FavoriteSong.HasValue && SongData.FavoriteSong.Value)
            {
                updateFields.Add("FavoriteSong = @FavoriteSong");
                sqlParams.Add(new SqlParameter("@FavoriteSong", SongData.FavoriteSong));


                updateFields.Add("FavoriteDate = @FavoriteDate");
                sqlParams.Add(new SqlParameter("@FavoriteDate", SongData.FavoriteDate));

            }
               


            if (SongData.SongLength != null)
            {
                updateFields.Add("SongLength = @SongLength");
                sqlParams.Add(new SqlParameter("@SongLength", SongData.SongLength));
            }
            
          

            if (updateFields.Count == 0)
            {
                response.Status = "E";
                response.Message = "No data to update.";
                return response;
            }

            string sqlUpdateSong = $"UPDATE Song SET {string.Join(", ", updateFields)} WHERE SongId = @SongId";
            sqlParams.Add(new SqlParameter("@SongId", SongData.SongId));

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    SqlCommand command = new SqlCommand(sqlUpdateSong, connection);
                    command.Parameters.AddRange(sqlParams.ToArray());

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Song updated successfully.";
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


        public async Task<ResponseModel> DeleteAlbum(AlbumModel AlbumData)
        {
            ResponseModel response = new ResponseModel();
            string sqlDeleteAlbum = @"DELETE FROM dbo.Albums WHERE AlbumId = @AlbumId";
            string sqlSetSongNull = @"UPDATE dbo.Song SET AlbumId = NULL WHERE AlbumId = @AlbumId";

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    SqlCommand updateCommand = new SqlCommand(sqlSetSongNull, connection, transaction);
                    updateCommand.Parameters.AddWithValue("@AlbumId", AlbumData.AlbumId);
                    await updateCommand.ExecuteNonQueryAsync();

                    SqlCommand deleteCommand = new SqlCommand(sqlDeleteAlbum, connection, transaction);
                    deleteCommand.Parameters.AddWithValue("@AlbumId", AlbumData.AlbumId);
                    await deleteCommand.ExecuteNonQueryAsync();

                    transaction.Commit();

                    response.Status = "S";
                    response.Message = "Album and associated songs updated successfully.";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    response.Status = "E";
                    response.Message = ex.Message;
                }
                finally
                {
                    await connection.CloseAsync();
                }
            }

            return response;
        }

        public async Task<ResponseModel> DeleteGenre(GenreModel genreData)
        {
            ResponseModel response = new ResponseModel();
            string sqlDeleteSong = @"DELETE dbo.genre WHERE GenreId = @GenreId";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlDeleteSong, connection);
                    command.Parameters.AddWithValue("@GenreId", genreData.GenreId);

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Genre Delete successfully.";
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

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel> DeleteArtist(ArtistModel artistData)
        {
            ResponseModel response = new ResponseModel();
            string sqlDeleteSong = @"DELETE dbo.artist WHERE ArtistId = @ArtistId";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlDeleteSong, connection);
                    command.Parameters.AddWithValue("@ArtistId", artistData.ArtistId);

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Artist Delete successfully.";
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

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel> DeleteSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();
            string sqlDeleteSong = @"DELETE dbo.Song WHERE SongId = @SongId";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlDeleteSong, connection);
                    command.Parameters.AddWithValue("@SongId", SongData.SongId);

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Song Delete successfully.";
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

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel> RemoveSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();
            string sqlRemoveSong = @"UPDATE dbo.Song 
                                    SET AlbumId = NULL
                                    WHERE SongId = @SongId";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlRemoveSong, connection);
                    command.Parameters.AddWithValue("@SongId", SongData.SongId);

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Song Remove successfully.";
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

            return await Task.FromResult(response);
        }

        public async Task<IEnumerable<SongModel>> SearchSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();

            // Check if all fields in SongData and UserId are null or empty
            if (string.IsNullOrEmpty(SongData.SongName) && string.IsNullOrEmpty(SongData.ArtistName) && SongData.AlbumId == null && !SongData.UserId.HasValue)
            {
                response.Status = "E";
                response.Message = "Error: Can't find song";
                return new List<SongModel>(); // Return empty list indicating no data found
            }

            List<SongModel> songList = new List<SongModel>();
            string sqlSelect = @"SELECT s.AlbumId, 
                           s.SongId, 
                           s.SongName, 
                           s.ArtistName, 
                           s.SongFile, 
                           s.SongGenres, 
                           s.SongImage, 
                           s.SongLength, 
                           us.FavoriteDate, 
                           us.FavoriteSong
                    FROM dbo.Song s
                    LEFT JOIN dbo.UserSongs us 
                           ON s.SongId = us.SongId AND us.UserId = @UserId";

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

            // Check if UserId is provided for filtering based on user's favorite songs
            if (SongData.UserId.HasValue)
            {
            
                sqlParameters.Add(new SqlParameter("@UserId", SongData.UserId.Value));
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
                                    FavoriteSong = reader.IsDBNull(reader.GetOrdinal("FavoriteSong")) ? false : reader.GetBoolean(reader.GetOrdinal("FavoriteSong")),
                                    FavoriteDate = reader.IsDBNull(reader.GetOrdinal("FavoriteDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FavoriteDate")),
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


        public async Task<IEnumerable<SongModel>> SearchSongNotInAlbum(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();

            // Check if all fields in UserData are null or empty
            if (string.IsNullOrEmpty(SongData.SongName) && string.IsNullOrEmpty(SongData.ArtistName) && SongData.AlbumId == null)
            {
                response.Status = "E";
                response.Message = "Error: Cant'find song";

                return new List<SongModel>(); // Return empty list indicating no data found
            }

            List<SongModel> songList = new List<SongModel>();
            string sqlSelect = @"SELECT s.AlbumId,s.SongId,s.SongName, s.ArtistName, s.SongFile,s.SongGenres,s.SongImage,s.SongLength,s.FavoriteSong
                     FROM dbo.Song s";

            List<string> sqlWhereClauses = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (SongData.SongId.HasValue)
            {
                sqlWhereClauses.Add("s.SongId = @SongId");
                sqlParameters.Add(new SqlParameter("@SongId", SongData.SongId.Value));
            }
            ///////////////ไม่เอาเพลงในalbum//////////
            if (SongData.AlbumId.HasValue)
            {
                sqlWhereClauses.Add("(s.AlbumId IS NULL OR s.AlbumId <> @AlbumId)");
                sqlParameters.Add(new SqlParameter("@AlbumId",SongData.AlbumId.Value));
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

           
            if (SongData.FavoriteSong.HasValue)
            {
                sqlWhereClauses.Add("s.FavoriteSong = @FavoriteSong");
                sqlParameters.Add(new SqlParameter("@FavoriteSong", SongData.FavoriteSong.Value));
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
                                    FavoriteSong = reader.IsDBNull(reader.GetOrdinal("FavoriteSong")) ? (bool?)null : reader.GetBoolean(reader.GetOrdinal("FavoriteSong")) // Handle Favorite column
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

        public async Task<IEnumerable<GenreModel>> SearchGenre(GenreModel genreData)
        {
            ResponseModel response = new ResponseModel();

            List<GenreModel> genreList = new List<GenreModel>();
            string sqlSelect = @"SELECT s.GenreId, s.GenreName, s.GenreImage
                     FROM dbo.Genre s";

            List<string> sqlWhereClauses = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (genreData.GenreId.HasValue)
            {
                sqlWhereClauses.Add("s.GenreId= @GenreId");
                sqlParameters.Add(new SqlParameter("@GenreId", genreData.GenreId.Value));
            }

            if (!string.IsNullOrEmpty(genreData.GenreName))
            {
                sqlWhereClauses.Add("UPPER(s.GenreName) LIKE UPPER(@GenreName)");
                sqlParameters.Add(new SqlParameter("@GenreName", $"%{genreData.GenreName}%"));
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
                                GenreModel genre = new GenreModel
                                {
                                    GenreId = reader.GetInt32("GenreId"),
                                 
                                    GenreName= reader["GenreName"].ToString(),
                                    GenreImagePath = reader["GenreImage"].ToString(),

                                };
                                genreList.Add(genre);
                            }
                        }
                    }
                }

                if (genreList.Count == 0)
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

            return genreList;
        }

        public async Task<IEnumerable<ArtistModel>> SearchArtist(ArtistModel artistData)
        {
            ResponseModel response = new ResponseModel();

            // Check if all fields in UserData are null or empty
            //if (string.IsNullOrEmpty(artistData.ArtistName))
            //{
            //    response.Status = "E";
            //    response.Message = "Error: Cant'find Artist";

            //    return new List<ArtistModel>(); // Return empty list indicating no data found
            //}

            List<ArtistModel> artistList = new List<ArtistModel>();
            string sqlSelect = @"SELECT s.ArtistId, s.ArtistName,s.ArtistImage, s.ArtistBio, s.ArtistGenres
                     FROM dbo.Artist s";

            List<string> sqlWhereClauses = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (artistData.ArtistId.HasValue)
            {
                sqlWhereClauses.Add("s.ArtistId = @ArtistId");
                sqlParameters.Add(new SqlParameter("@ArtistId", artistData.ArtistId.Value));
            }
    
            if (!string.IsNullOrEmpty(artistData.ArtistName))
            {
                sqlWhereClauses.Add("UPPER(s.ArtistName) LIKE UPPER(@ArtistName)");
                sqlParameters.Add(new SqlParameter("@ArtistName", $"%{artistData.ArtistName}%"));
            }

            if (!string.IsNullOrEmpty(artistData.ArtistBio))
            {
                sqlWhereClauses.Add("s.ArtistBio = @ArtistBio");
                sqlParameters.Add(new SqlParameter("@ArtistBio", artistData.ArtistBio));
            }

            if (!string.IsNullOrEmpty(artistData.ArtistImagePath))
            {
                sqlWhereClauses.Add("s.ArtistImage = @ArtistImage");
                sqlParameters.Add(new SqlParameter("@ArtistImage", artistData.ArtistImagePath));
            }

            if (!string.IsNullOrEmpty(artistData.ArtistGenres))
            {
                sqlWhereClauses.Add("s.ArtistGenres = @ArtistGenres");
                sqlParameters.Add(new SqlParameter("@ArtistGenres", artistData.ArtistGenres));
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
                                ArtistModel artist = new ArtistModel
                                {
                                    ArtistId = reader.GetInt32("ArtistId"),
                                    ArtistBio = reader["ArtistBio"].ToString(),
                                    ArtistGenres = reader["ArtistGenres"].ToString(),
                                    ArtistName = reader["ArtistName"].ToString(),
                                    ArtistImagePath = reader["ArtistImage"].ToString(),
                                };
                                artistList.Add(artist);
                            }
                        }
                    }
                }

                if (artistList.Count == 0)
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

            return artistList;
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
                sqlWhereClauses.Add("s.AlbumId = @AlbumId");
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


        public async Task<IEnumerable<AlbumModel>> GetAlbum(AlbumModel AlbumData)
        {
            ResponseModel response = new ResponseModel();

            // Check if all fields in UserData are null or empty
          

            List<AlbumModel> albumList = new List<AlbumModel>();
            string sqlSelect = @"SELECT s.AlbumId,s.AlbumName, s.ArtistName,s.AlbumImage
                     FROM dbo.Albums s";

           
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

         

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

        public async Task<ResponseModel> FavoriteArtist(SpotSidebarModel artistData)
        {
            ResponseModel response = new ResponseModel();

            string sqlMerge = @"
        MERGE INTO UserArtists AS target
        USING (SELECT @UserId AS UserId, @ArtistId AS ArtistId) AS source
        ON target.UserId = source.UserId AND target.SongId = source.SongId
        WHEN MATCHED THEN
            UPDATE SET 
                FavoriteArtist = @FavoriteArtist,
                FavoriteDate = @FavoriteDate
        WHEN NOT MATCHED THEN
            INSERT (UserId, ArtistId, FavoriteArtist, FavoriteDate)
            VALUES (@UserId, @ArtistId, @FavoriteArtist, @FavoriteDate);";

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlMerge, connection);
                    command.Parameters.AddWithValue("@UserId", artistData.UserId);
                    command.Parameters.AddWithValue("@ArtistId", artistData.ArtistId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FavoriteArtist", artistData.FavoriteArtist ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FavoriteDate", artistData.FavoriteDate ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Favorite Artist successfully.";
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



        public async Task<ResponseModel> FavoriteSong(SongModel songData)
        {
            ResponseModel response = new ResponseModel();

            string sqlMerge = @"
        MERGE INTO UserSongs AS target
        USING (SELECT @UserId AS UserId, @SongId AS SongId) AS source
        ON target.UserId = source.UserId AND target.SongId = source.SongId
        WHEN MATCHED THEN
            UPDATE SET 
                FavoriteSong = @FavoriteSong,
                FavoriteDate = @FavoriteDate
        WHEN NOT MATCHED THEN
            INSERT (UserId, SongId, FavoriteSong, FavoriteDate)
            VALUES (@UserId, @SongId, @FavoriteSong, @FavoriteDate);";

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlMerge, connection);
                    command.Parameters.AddWithValue("@UserId", songData.UserId);
                    command.Parameters.AddWithValue("@SongId", songData.SongId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FavoriteSong", songData.FavoriteSong ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@FavoriteDate", songData.FavoriteDate ?? (object)DBNull.Value);

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "Favorite song successfully.";
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

      
                public async Task<List<SpotSidebarModel>> GetFavAlbumAndArtistByUser(string userId)
        {

            Convert.ToInt32(userId);

            ResponseModel response = new ResponseModel();
            List<SpotSidebarModel> sidebarList = new List<SpotSidebarModel>();
            string sqlSelect = @"
                                SELECT 
                            useral.UserId,
                            al.AlbumId,
                            al.AlbumName,
                            al.ArtistName AS AlbumArtistName,
                            al.AlbumImage,
                            ar.ArtistId,
                            ar.ArtistName AS ArtistName,
                            NULL AS ArtistImage, 
                            useral.FavoriteAlbum,
                            useral.FavoriteDate AS FavoriteAlbumDate,
                            0 AS FavoriteArtist, 
                            NULL AS FavoriteArtistDate,
                            useral.FavoriteDate AS FavoriteDate
                        FROM 
                            dbo.Albums al
                        INNER JOIN 
                            dbo.UserAlbums useral ON useral.AlbumId = al.AlbumId
                        INNER JOIN 
                            dbo.Artist ar ON ar.ArtistName = al.ArtistName
                        WHERE 
                            useral.UserId = @userId
                            AND useral.FavoriteAlbum IS NOT NULL
                            AND useral.FavoriteAlbum <> 0

                        UNION ALL

                        SELECT 
                            userar.UserId,
                            NULL AS AlbumId, 
                            NULL AS AlbumName,
                            NULL AS AlbumArtistName,
                            NULL AS AlbumImage,
                            ar.ArtistId,
                            ar.ArtistName AS ArtistName,
                            ar.ArtistImage,
                            0 AS FavoriteAlbum,  
                            NULL AS FavoriteAlbumDate,
                            userar.FavoriteArtist,
                            userar.FavoriteDate AS FavoriteArtistDate,
                            userar.FavoriteDate AS FavoriteDate
                        FROM 
                            dbo.Artist ar
                        INNER JOIN 
                            dbo.UserArtists userar ON userar.ArtistId = ar.ArtistId
                        WHERE 
                            userar.UserId = @UserId
                            AND userar.FavoriteArtist IS NOT NULL
                            AND userar.FavoriteArtist <> 0 
                        ORDER BY 
                            FavoriteDate DESC;
                        ";

            //List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //SqlCommand command = new SqlCommand(sqlRemoveSong, connection);
            //command.Parameters.AddWithValue("@SongId", SongData.SongId);

            List<SqlParameter> sqlParameters = new List<SqlParameter>
    {
        new SqlParameter("@userId", userId)
    };
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
                                SpotSidebarModel sidebarData = new SpotSidebarModel
                                {
                                    UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? 0 : reader.GetInt32(reader.GetOrdinal("UserId")),
                                    AlbumId = reader.IsDBNull(reader.GetOrdinal("AlbumId")) ? 0 : reader.GetInt32(reader.GetOrdinal("AlbumId")),
                                    AlbumName = reader["AlbumName"] as string ?? string.Empty,
                                    ArtistName = reader["ArtistName"] as string ?? string.Empty,
                                    AlbumImagePath = reader["AlbumImage"] as string ?? string.Empty,
                                    ArtistId = reader.IsDBNull(reader.GetOrdinal("ArtistId")) ? 0 : reader.GetInt32(reader.GetOrdinal("ArtistId")),
                                    ArtistImagePath = reader["ArtistImage"] as string ?? string.Empty,
                                    FavoriteAlbum = reader.IsDBNull(reader.GetOrdinal("FavoriteAlbum")) ? false : reader.GetInt32(reader.GetOrdinal("FavoriteAlbum")) != 0,
                                    FavoriteDate = reader.IsDBNull(reader.GetOrdinal("FavoriteDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FavoriteDate")),
                                    FavoriteAlbumDate = reader.IsDBNull(reader.GetOrdinal("FavoriteAlbumDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FavoriteAlbumDate")),
                                    FavoriteArtist = reader.IsDBNull(reader.GetOrdinal("FavoriteArtist")) ? false : reader.GetInt32(reader.GetOrdinal("FavoriteArtist")) != 0,
                                    FavoriteArtistDate = reader.IsDBNull(reader.GetOrdinal("FavoriteArtistDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FavoriteArtistDate")),
                                };
                                sidebarList.Add(sidebarData);
                            }
                        }
                    }
                }

                if (sidebarList.Count == 0)
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

            return sidebarList;
        }


        public async Task<List<SongModel>> GetFavSongByUser(string userId)
        {

            Convert.ToInt32(userId);

                  ResponseModel response = new ResponseModel();
            List<SongModel> songList = new List<SongModel>();
            string sqlSelect = @"SELECT s.SongId
                        ,s.AlbumId
                        ,s.ArtistName
                        ,s.SongName
                        ,s.SongFile
                        ,s.SongGenres
                        ,s.SongImage
                        ,s.SongLength                
	                    ,a.AlbumImage
	                    ,usr.FavoriteSong
	                    ,usr.FavoriteDate
                   FROM dbo.Song s
                   FULL JOIN dbo.Albums a ON s.AlbumId = a.AlbumId
                   INNER JOIN dbo.UserSongs usr ON s.SongId = usr.SongId
                   WHERE s.FavoriteSong = 1 AND  usr.UserId = @userId
                   ORDER BY usr.FavoriteDate desc ";

            //List<SqlParameter> sqlParameters = new List<SqlParameter>();
            //SqlCommand command = new SqlCommand(sqlRemoveSong, connection);
            //command.Parameters.AddWithValue("@SongId", SongData.SongId);

            List<SqlParameter> sqlParameters = new List<SqlParameter>
    {
        new SqlParameter("@userId", userId)
    };
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
                                    SongId = reader.IsDBNull(reader.GetOrdinal("SongId")) ? 0 : reader.GetInt32(reader.GetOrdinal("SongId")),
                                    AlbumId = reader.IsDBNull(reader.GetOrdinal("AlbumId")) ? 0 : reader.GetInt32(reader.GetOrdinal("AlbumId")),
                                    ArtistName = reader["ArtistName"] as string ?? string.Empty,
                                    SongFilePath = reader["SongFile"] as string ?? string.Empty,
                                    SongName = reader["SongName"] as string ?? string.Empty,
                                    SongGenres = reader["SongGenres"] as string ?? string.Empty,
                                    SongImagePath = reader["SongImage"] as string ?? string.Empty,
                                    SongLength = reader.IsDBNull(reader.GetOrdinal("SongLength")) ? 0 : reader.GetInt32(reader.GetOrdinal("SongLength")),
                                    FavoriteSong = reader.IsDBNull(reader.GetOrdinal("FavoriteSong")) ? false : reader.GetBoolean(reader.GetOrdinal("FavoriteSong")),
                                    FavoriteDate = reader.IsDBNull(reader.GetOrdinal("FavoriteDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FavoriteDate")),
                                    AlbumImagePath = reader["AlbumImage"] as string ?? string.Empty,
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


        public async Task<IEnumerable<SongModel>> GetFavoriteSongs(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();
            List<SongModel> songList = new List<SongModel>();
            string sqlSelect = @"SELECT s.SongId
                        ,s.AlbumId
                        ,s.ArtistName
                        ,s.SongName
                        ,s.SongFile
                        ,s.SongGenres
                        ,s.SongImage
                        ,s.SongLength
                        ,s.FavoriteSong
                        ,s.FavoriteDate
	                    ,a.AlbumImage
                   FROM dbo.Song s
                   FULL JOIN dbo.Albums a ON s.AlbumId = a.AlbumId
                   WHERE s.FavoriteSong = 1
                   ORDER BY s.FavoriteDate desc ";

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
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
                                    SongId = reader.IsDBNull(reader.GetOrdinal("SongId")) ? 0 : reader.GetInt32(reader.GetOrdinal("SongId")),
                                    AlbumId = reader.IsDBNull(reader.GetOrdinal("AlbumId")) ? 0 : reader.GetInt32(reader.GetOrdinal("AlbumId")),
                                    ArtistName = reader["ArtistName"] as string ?? string.Empty,
                                    SongFilePath = reader["SongFile"] as string ?? string.Empty,
                                    SongName = reader["SongName"] as string ?? string.Empty,
                                    SongGenres = reader["SongGenres"] as string ?? string.Empty,
                                    SongImagePath = reader["SongImage"] as string ?? string.Empty,
                                    SongLength = reader.IsDBNull(reader.GetOrdinal("SongLength")) ? 0 : reader.GetInt32(reader.GetOrdinal("SongLength")),
                                    FavoriteSong = reader.IsDBNull(reader.GetOrdinal("FavoriteSong")) ? false : reader.GetBoolean(reader.GetOrdinal("FavoriteSong")),
                                    FavoriteDate = reader.IsDBNull(reader.GetOrdinal("FavoriteDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FavoriteDate")),
                                    AlbumImagePath = reader["AlbumImage"] as string ?? string.Empty,
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

        public async Task<IEnumerable<SearchSpotModal>> SearchSpot(SearchSpotModal searchData)
        {
            ResponseModel response = new ResponseModel();
            List<SearchSpotModal> searchSpotList = new List<SearchSpotModal>();

            string sqlSelect = @"
    

            SELECT 
                so.SongId, so.SongName, so.SongGenres, so.SongFile, so.SongImage, 
                alb.AlbumId, alb.AlbumName, alb.AlbumImage, 
                art.ArtistId, art.ArtistName, art.ArtistImage, art.ArtistGenres, 
                usr.UserId, usr.FavoriteSong, usr.FavoriteDate
            FROM 
                Song AS so
            JOIN 
                Artist AS art ON so.ArtistName = art.ArtistName
            JOIN 
                Albums AS alb ON so.AlbumId = alb.AlbumId
            FULL JOIN 
                UserSongs AS usr ON usr.SongId = so.SongId
            WHERE 
                (UPPER(so.SongName) LIKE UPPER(@searchData)
                 OR UPPER(art.ArtistName) LIKE UPPER(@searchData)
                 OR UPPER(alb.AlbumName) LIKE UPPER(@searchData))
                 AND (usr.UserId = @UserId OR usr.UserId IS NULL);
                ";

            // Add wildcards to the search parameter in C#
            string searchParameter = $"%{searchData.ArtistName}%"; // Assuming searchData has a SearchTerm property

            // Create the list of parameters
            List<SqlParameter> sqlParameters = new List<SqlParameter>
    {
        new SqlParameter("@searchData", searchParameter),
           new SqlParameter("@userId",searchData.UserId)
    };

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
                                SearchSpotModal searchSong = new SearchSpotModal
                                {
                                    SongId = reader.IsDBNull(reader.GetOrdinal("SongId")) ? 0 : reader.GetInt32(reader.GetOrdinal("SongId")),
                                    AlbumId = reader.IsDBNull(reader.GetOrdinal("AlbumId")) ? 0 : reader.GetInt32(reader.GetOrdinal("AlbumId")),
                                    ArtistName = reader["ArtistName"] as string ?? string.Empty,
                                    SongFilePath = reader["SongFile"] as string ?? string.Empty,
                                    SongName = reader["SongName"] as string ?? string.Empty,
                                    SongGenres = reader["SongGenres"] as string ?? string.Empty,
                                    SongImagePath = reader["SongImage"] as string ?? string.Empty,
                                    AlbumImagePath = reader["AlbumImage"] as string ?? string.Empty,
                                    FavoriteSong = reader.IsDBNull(reader.GetOrdinal("FavoriteSong")) ? (bool?)false : reader.GetBoolean(reader.GetOrdinal("FavoriteSong")),
                                    FavoriteDate = reader.IsDBNull(reader.GetOrdinal("FavoriteDate")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FavoriteDate"))
                                };
                                searchSpotList.Add(searchSong);
                            }
                        }
                    }
                }

                if (searchSpotList.Count == 0)
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

            return searchSpotList;
        }







    }
}
