using AuthenticationPlugin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project123.Dto;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project123Api.Repositories
{

    public interface IAdminRepository
    {
        Task<ResponseModel> CreateUser(dataModel userData);
        Task<ResponseModel> SearchUser(dataModel userData);
        //Task<ResponseModel> Login(AdminModel userData);
    }

    public class AdminRepository : IAdminRepository
    {
        private readonly IConfiguration _configuration;

        public AdminRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ResponseModel> CreateUser(dataModel UserData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateUser = @"INSERT INTO Tb_User (Name, Age,RecordDate) VALUES (@Name, @Age, @RecordDate)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateUser, connection);
                    command.Parameters.AddWithValue("@Name", UserData.Name);
                    command.Parameters.AddWithValue("@Age", UserData.Age);
                    command.Parameters.AddWithValue("@RecordDate", UserData.RecordDate);
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


        public async Task<ResponseModel> SearchUser(dataModel UserData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateUser = @"INSERT INTO Tb_User (Name, Age,RecordDate) VALUES (@Name, @Age, @RecordDate)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateUser, connection);
                    command.Parameters.AddWithValue("@Name", UserData.Name);
                    command.Parameters.AddWithValue("@Age", UserData.Age);
                    command.Parameters.AddWithValue("@RecordDate", UserData.RecordDate);
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


    }
}
