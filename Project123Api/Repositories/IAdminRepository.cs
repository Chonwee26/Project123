//using AuthenticationPlugin;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Project123.Dto;
//using System.ComponentModel.Design;
//using System.Data;
//using System.Data.SqlClient;
//using static System.Collections.Specialized.BitVector32;


//namespace Project123Api.Repositories
//{
//    public interface IAdminRepository
//    {
//        Task<ResponseModel> CreateUser(AdminModel UserData);
//        Task<ResponseModel> CreateUser(dataModel userData);
//    }
//    public class AdminRepository : DataDbContext, IAdminRepository

//    {
     
//        public async Task<ResponseModel> CreateUser(dataModel UserData)
//        {
//            ResponseModel response = new ResponseModel();
//            string sqlCreateUser = @"INSERT INTO Tb_User (Name, Age) VALUES (@Name , @Age)";

//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                connection.Open();

//                try
//                {
//                    SqlCommand command = connection.CreateCommand();
//                    command.Parameters.AddWithValue("@Name", UserData.Name);
//                    command.Parameters.AddWithValue("@Age", UserData.Age);
//                    response.Status = "S";
//                    response.Message = UserData.Name;
//                }
//                catch (Exception ex)
//                {
//                    response.Status = "E";
//                    response.Message = ex.Message;
//                }
//                finally
//                {
//                    connection.Close();
//                }
//            }

//            return await Task.FromResult(response);
//        }
//    }
//}
