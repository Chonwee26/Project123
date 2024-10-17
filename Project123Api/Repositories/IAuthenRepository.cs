//using AuthenticationPlugin;
//using Microsoft.Extensions.Configuration;
//using Project123.Dto;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Threading.Tasks;

//namespace Project123Api.Repositories
//{
//    public interface IAuthenRepository
//    {
//        Task<ResponseModel> Register(AdminModel userData);
//        Task<ResponseModel> Login(AdminModel userData);
//        Task<ResponseModel> CreateUser(dataModel userData);
//        Task<ResponseModel> DeleteUser(int id);
//        Task<IEnumerable<dataModel>> SearchUser(dataModel userData);
//    }

//    public class AuthenRepository : IAuthenRepository
//    {
//        private readonly IConfiguration _configuration;

//        public AuthenRepository(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task<ResponseModel> Register(AdminModel userData)
//        {
//            ResponseModel response = new ResponseModel();
//            string sqlCreateUser = @"INSERT INTO Tb_Admin (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, @Role)";
//            string connectionString = _configuration.GetConnectionString("DefaultConnection");
//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                await connection.OpenAsync();

//                try
//                {
//                    SqlCommand command = new SqlCommand(sqlCreateUser, connection);
//                    command.Parameters.AddWithValue("@Name", userData.Name);
//                    command.Parameters.AddWithValue("@Email", userData.Email);
//                    command.Parameters.AddWithValue("@Password", SecurePasswordHasherHelper.Hash(userData.Password));
//                    command.Parameters.AddWithValue("@Role", userData.Role);
//                    await command.ExecuteNonQueryAsync();

//                    response.Status = "S";
//                    response.Message = "User registered successfully.";
//                }
//                catch (Exception ex)
//                {
//                    response.Status = "E";
//                    response.Message = ex.Message;
//                }
//            }

//            return response;
//        }

//        public async Task<ResponseModel> Login(AdminModel userData)
//        {
//            ResponseModel response = new ResponseModel();
//            string sqlSelectUser = @"SELECT Password FROM Tb_Admin WHERE Email = @Email";
//            string connectionString = _configuration.GetConnectionString("DefaultConnection");
//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                await connection.OpenAsync();

//                try
//                {
//                    SqlCommand command = new SqlCommand(sqlSelectUser, connection);
//                    command.Parameters.AddWithValue("@Email", userData.Email);

//                    var passwordFromDb = await command.ExecuteScalarAsync() as string;

//                    if (passwordFromDb != null && SecurePasswordHasherHelper.Verify(userData.Password, passwordFromDb))
//                    {
//                        response.Status = "S";
//                        response.Message = "Login successful.";
//                    }
//                    else
//                    {
//                        response.Status = "E";
//                        response.Message = "Invalid email or password.";
//                    }
//                }
//                catch (Exception ex)
//                {
//                    response.Status = "E";
//                    response.Message = ex.Message;
//                }
//            }

//            return response;
//        }

//        public async Task<ResponseModel> CreateUser(dataModel userData)
//        {
//            ResponseModel response = new ResponseModel();
//            string sqlCreateUser = @"INSERT INTO Tb_User (Name, Age, RecordDate) VALUES (@Name, @Age, @RecordDate)";
//            string connectionString = _configuration.GetConnectionString("DefaultConnection");
//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                await connection.OpenAsync();

//                try
//                {
//                    SqlCommand command = new SqlCommand(sqlCreateUser, connection);
//                    command.Parameters.AddWithValue("@Name", userData.Name);
//                    command.Parameters.AddWithValue("@Age", userData.Age);
//                    command.Parameters.AddWithValue("@RecordDate", userData.RecordDate);
//                    await command.ExecuteNonQueryAsync();

//                    response.Status = "S";
//                    response.Message = "User created successfully.";
//                }
//                catch (Exception ex)
//                {
//                    response.Status = "E";
//                    response.Message = ex.Message;
//                }
//            }

//            return response;
//        }

//        public async Task<ResponseModel> DeleteUser(int id)
//        {
//            ResponseModel response = new ResponseModel();
//            string sqlDeleteUser = @"DELETE FROM Tb_User WHERE Id = @UserId";
//            string connectionString = _configuration.GetConnectionString("DefaultConnection");
//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                await connection.OpenAsync();

//                try
//                {
//                    SqlCommand command = new SqlCommand(sqlDeleteUser, connection);
//                    command.Parameters.AddWithValue("@UserId", id);
//                    await command.ExecuteNonQueryAsync();

//                    response.Status = "S";
//                    response.Message = "User deleted successfully.";
//                }
//                catch (Exception ex)
//                {
//                    response.Status = "E";
//                    response.Message = ex.Message;
//                }
//            }

//            return response;
//        }

//        public async Task<IEnumerable<dataModel>> SearchUser(dataModel userData)
//        {
//            List<dataModel> userList = new List<dataModel>();
//            string sqlSelect = @"SELECT Id, Name, Age, RecordDate FROM dbo.Tb_User";
//            List<string> sqlWhereClauses = new List<string>();
//            List<SqlParameter> sqlParameters = new List<SqlParameter>();

//            if (!string.IsNullOrEmpty(userData.Name))
//            {
//                sqlWhereClauses.Add("Name = @Name");
//                sqlParameters.Add(new SqlParameter("@Name", userData.Name));
//            }

//            if (!string.IsNullOrEmpty(userData.Age))
//            {
//                sqlWhereClauses.Add("Age = @Age");
//                sqlParameters.Add(new SqlParameter("@Age", userData.Age));
//            }

//            if (sqlWhereClauses.Count > 0)
//            {
//                sqlSelect += " WHERE " + string.Join(" AND ", sqlWhereClauses);
//            }

//            string connectionString = _configuration.GetConnectionString("DefaultConnection");
//            using (SqlConnection connection = new SqlConnection(connectionString))
//            {
//                await connection.OpenAsync();

//                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
//                {
//                    command.Parameters.AddRange(sqlParameters.ToArray());

//                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
//                    {
//                        while (await reader.ReadAsync())
//                        {
//                            dataModel user = new dataModel
//                            {
//                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
//                                Name = reader["Name"].ToString(),
//                                Age = reader["Age"].ToString(),
//                                RecordDate = reader.GetDateTime(reader.GetOrdinal("RecordDate"))
//                            };

//                            userList.Add(user);
//                        }
//                    }
//                }
//            }

//            return userList;
//        }
//    }
//}
