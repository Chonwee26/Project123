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
        Task<IEnumerable<dataModel>> SearchUser(dataModel userData);
        Task<ResponseModel> Register(AdminModel userData);
        Task<ResponseModel> DeleteUser(int id);

        //Task<ResponseModel> Login(AdminModel userData);
    }

    public class SpotRepository : ISpotRepository
    {
        private readonly IConfiguration _configuration;

        public SpotRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseModel> CreateSong(SongModel SongData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateSong = @"INSERT INTO Song (AlbumId, ArtistName,SongName,SongFile,SongGenres,SongImage) VALUES (@AlbumId, @ArtistName, @SongName,@SongFile,@SongGenres,@SongImage)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateSong, connection);
                    command.Parameters.AddWithValue("@AlbumId", SongData.AlbumId);
                    command.Parameters.AddWithValue("@ArtistName", SongData.ArtistName);
                    command.Parameters.AddWithValue("@SongName", SongData.SongName);
                    command.Parameters.AddWithValue("@SongFile", SongData.SongFile);
                    command.Parameters.AddWithValue("@SongGenres", SongData.SongGenres);
                    command.Parameters.AddWithValue("@SongImage", SongData.SongImage);
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

            return await Task.FromResult(response);
        }

        public async Task<ResponseModel> DeleteUser(int id)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateUser = @"DELETE Tb_User WHERE Tb_User.Id = @UserId";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateUser, connection);
                    command.Parameters.AddWithValue("@UserId", id);

                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "User Delete successfully.";
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

        public async Task<ResponseModel> Register(AdminModel UserData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateUser = @"INSERT INTO Tb_Admin (Name, Email,Password,Role) VALUES (@Name, @Email, @Password,@Role)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateUser, connection);
                    command.Parameters.AddWithValue("@Name", UserData.Name);
                    command.Parameters.AddWithValue("@Email", UserData.Email);
                    command.Parameters.AddWithValue("@Password", SecurePasswordHasherHelper.Hash(UserData.Password));
                    command.Parameters.AddWithValue("@Role", UserData.Role);
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

            return await Task.FromResult(response);
        }
        public async Task<IEnumerable<dataModel>> SearchUser(dataModel UserData)
        {
            ResponseModel response = new ResponseModel();

            // Check if all fields in UserData are null or empty
            if (string.IsNullOrEmpty(UserData.Name) && string.IsNullOrEmpty(UserData.Age))
            {
                response.Status = "E";
                response.Message = "Error: UserData fields are empty";

                return new List<dataModel>(); // Return empty list indicating no data found
            }

            List<dataModel> userList = new List<dataModel>();
            string sqlSelect = @"SELECT s.Id,s.Name, s.Age, s.RecordDate
                         FROM dbo.Tb_User s ";
            List<string> sqlWhereClauses = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(UserData.Name))
            {
                sqlWhereClauses.Add("s.Name = @Name");
                sqlParameters.Add(new SqlParameter("@Name", UserData.Name));
            }


            if (!string.IsNullOrEmpty(UserData.Age))
            {
                sqlWhereClauses.Add("s.Age = @Age");
                sqlParameters.Add(new SqlParameter("@Age", UserData.Age));
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
                                dataModel user = new dataModel
                                {
                                    Id = reader.GetInt32("Id"),
                                    Name = reader["Name"].ToString(),
                                    Age = reader["Age"].ToString(),
                                    RecordDate = Convert.ToDateTime(reader["RecordDate"])
                                };

                                userList.Add(user);
                            }
                        }
                    }
                }

                if (userList.Count == 0)
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

            return userList;
        }


    }
}
