using AuthenticationPlugin;
using Microsoft.Extensions.Configuration;
using Project123.Dto;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net.Mail;
using System.Net;
using Project123Api.Services;
using System.Drawing.Printing;
using System.Text;
using System.Linq;
using Microsoft.IdentityModel.Tokens;

namespace Project123Api.Repositories
{
    public interface IAuthenRepository
    {
        Task<ResponseModel> ChangePassword(AdminModel userData);
        Task<ResponseModel> ChangePasswordByToken(AdminModel userData, string token);

        Task<ResponseModel> ForgetPassword(AdminModel userData);
        Task<ResponseModel> Register(AdminModel userData);
        Task<ResponseModel> Login(AdminModel userData);
        Task<ResponseModel> CreateUser(dataModel userData);
        Task<ResponseModel> DeleteUser(int id);
        Task<IEnumerable<dataModel>> SearchUser(dataModel userData);
    }

    public class AuthenRepository : IAuthenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        public AuthenRepository(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<ResponseModel> Register(AdminModel userData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateUser = @"INSERT INTO Tb_Admin (Name, Email, Password, Role) VALUES (@Name, @Email, @Password, @Role)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateUser, connection);
                    command.Parameters.AddWithValue("@Name", userData.Name);
                    command.Parameters.AddWithValue("@Email", userData.Email);
                    command.Parameters.AddWithValue("@Password", SecurePasswordHasherHelper.Hash(userData.Password));
                    command.Parameters.AddWithValue("@Role", userData.Role);
                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "User registered successfully.";
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return response;
        }


        public async Task<ResponseModel> ChangePassword1(AdminModel userData)
        {
            ResponseModel response = new ResponseModel();
            string sqlGetUserPassword = @"SELECT Password FROM Tb_Admin WHERE Email = @Email";
            string sqlUpdatePassword = @"UPDATE Tb_Admin SET Password = @Password WHERE Email = @Email";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    // Retrieve the current password hash from the database
                    SqlCommand getPasswordCommand = new SqlCommand(sqlGetUserPassword, connection);
                    getPasswordCommand.Parameters.AddWithValue("@Email", userData.Email);
                    var currentPasswordHash = await getPasswordCommand.ExecuteScalarAsync();

                    if (currentPasswordHash != null)
                    {
                        // Check if the old password matches the stored hash
                        if (SecurePasswordHasherHelper.Verify(userData.OldPassword, currentPasswordHash.ToString()))
                        {
                            // Check if the new password is different from the old one
                            if (userData.Password == userData.OldPassword)
                            {
                                response.Status = "E";
                                response.Message = "New password cannot be the same as the old password.";
                                return response;
                            }

                            // Proceed to update the password
                            SqlCommand updatePasswordCommand = new SqlCommand(sqlUpdatePassword, connection);
                            updatePasswordCommand.Parameters.AddWithValue("@Password", SecurePasswordHasherHelper.Hash(userData.Password));
                            updatePasswordCommand.Parameters.AddWithValue("@Email", userData.Email);
                            await updatePasswordCommand.ExecuteNonQueryAsync();

                            response.Status = "S";
                            response.Message = "Password changed successfully.";
                        }
                        else
                        {
                            response.Status = "E";
                            response.Message = "Old password is incorrect.";
                        }
                    }
                    else
                    {
                        response.Status = "E";
                        response.Message = "User not found.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return response;
        }



        public async Task<ResponseModel> ChangePassword(AdminModel userData)
        {
            ResponseModel response = new ResponseModel();
            string sqlOldPassword = @"SELECT Password FROM TB_Admin WHERE Email = @Email";
            string sqlCurrentPassword = @"UPDATE Tb_Admin SET Password = @Password WHERE Email = @Email";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    SqlCommand getPassword = new SqlCommand(sqlOldPassword, connection);
                    getPassword.Parameters.AddWithValue("@Email", userData.Email);
                   var oldPasswordHash = await getPassword.ExecuteScalarAsync();
                    userData.OldPassword = oldPasswordHash.ToString();
                    if (oldPasswordHash != null)
                    {
                        if (SecurePasswordHasherHelper.Verify(userData.OldPassword, oldPasswordHash.ToString()))
                        {
                            response.Status = "E";
                            response.Message = "Old password is incorrect.";
                            return response;
                        }

                        if (userData.Password == userData.OldPassword)
                        {

                            response.Status = "E";
                            response.Message = "New password cannot be the same as the old password.";
                            return response;
                        }

                        SqlCommand command = new SqlCommand(sqlCurrentPassword, connection);
                        command.Parameters.AddWithValue("@Email", userData.Email);
                        command.Parameters.AddWithValue("@Password", SecurePasswordHasherHelper.Hash(userData.Password));
                        await command.ExecuteNonQueryAsync();

                        response.Status = "S";
                        response.Message = "User change password successfully.";


                    }
                    else
                    {
                        response.Status = "E";
                        response.Message = "User passsword not found.";
                    }

                  
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return response;
        }


        public async Task<ResponseModel> ChangePasswordByToken(AdminModel userData, string token)
        {
            ResponseModel response = new ResponseModel();
            string sqlSearchUser = @"SELECT * FROM TB_Admin WHERE TokenResetPassword = @TokenResetPassword";
            string sqlCurrentPassword = @"UPDATE Tb_Admin SET Password = @Password WHERE Email = @Email";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userData.Password))
            {
                response.Status = "E";
                response.Message = "Token and new password are required.";
                return response;
            }
         
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    SqlCommand commandUser = new SqlCommand(sqlSearchUser, connection);
                    commandUser.Parameters.AddWithValue("@TokenResetPassword", token);

                    var sqlUserData = await connection.QueryFirstOrDefaultAsync<AdminModel>(sqlSearchUser, new { TokenResetPassword = token });
                    //var sqlUserEmail= await commandUser.ExecuteScalarAsync();
                    string hashedPassword = SecurePasswordHasherHelper.Hash(userData.Password);
                    if (SecurePasswordHasherHelper.Verify(userData.Password, sqlUserData.Password))
                    {
                        response.Status = "E";
                        response.Message = "Cannot use the same old password.";
                        return response;
                    }
                    if (sqlUserData == null)
                    {

                        response.Status = "E";
                        response.Message = "User not found.";
                        return response;
                    }
                    if (sqlUserData.ExpireDate == null || sqlUserData.ExpireDate < DateTime.UtcNow)
                    {
                        response.Status = "E";
                        response.Message = "User token expired.";
                        return response;
                    }
              
                    SqlCommand command = new SqlCommand(sqlCurrentPassword, connection);
                    command.Parameters.AddWithValue("@Email", sqlUserData.Email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "User reset password successfully.";

                  
                    //await SendEmailAsync(user.Email, "Reset Your Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");

                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return response;

        }

        public async Task<ResponseModel> ForgetPassword(AdminModel userData)
        {
            ResponseModel response = new ResponseModel();
            string sqlSearchUser = @"SELECT * FROM TB_Admin WHERE Email = @Email";
            string sqlCurrentPassword = @"UPDATE Tb_Admin SET TokenResetPassword = @TokenResetPassword, TokenResetExpireDate = @TokenResetExpireDate WHERE Email = @Email";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    SqlCommand commandUser = new SqlCommand(sqlSearchUser, connection);
                    commandUser.Parameters.AddWithValue("@Email", userData.Email);
                    //var sqlUserData = await commandUser.ExecuteScalarAsync();
                    var sqlUserData = await connection.QueryFirstOrDefaultAsync<AdminModel>(sqlSearchUser, new { Email = userData.Email });

                    if (sqlUserData == null)
                    {

                        response.Status = "E";
                        response.Message = "User not found.";
                        return response;
                    }
                    // Generate a secure token
                    string resetToken = Guid.NewGuid().ToString();
                    userData.TokenResetPassword = resetToken;
                    userData.TokenResetExpireDate = DateTime.UtcNow.AddMinutes(30);
                
                        SqlCommand command = new SqlCommand(sqlCurrentPassword, connection);
                        command.Parameters.AddWithValue("@Email", userData.Email);
                        command.Parameters.AddWithValue("@TokenResetPassword", userData.TokenResetPassword);
                        command.Parameters.AddWithValue("@TokenResetExpireDate", userData.TokenResetExpireDate);
                        await command.ExecuteNonQueryAsync();

                        response.Status = "S";
                        response.Message = "User reset token successfully.";

                    string resetLink = $"https://localhost:7166/Admin/ForgetPasswordPage?token={resetToken}";
                    string strEmail = userData.Email ?? "";
                    string userName = sqlUserData.Name ?? "";
                    StringBuilder htmlform = new StringBuilder();
                    htmlform.Append($@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Reset Your Password</title>
</head>
<body style='margin: 0; padding: 0; background-color: #f4f4f4;'>
    <table role='presentation' style='width: 100%; max-width: 600px; height:80%; margin: 0 auto; background: #fff; padding: 20px; border-radius: 10px; font-family: Arial, sans-serif;'>
        <tr>
            <td align='center' style='padding: 20px 0;'>
                <img src='https://i.ibb.co/0j7LcsLY/music-note-icon-1.png' alt='Logo' style='width: 80px; height: 80px;'>
            </td>
        </tr>
        <tr>
            <td align='center'>
                <h2 style='margin: 0; color: #333;'>การขอเปลี่ยนรหัสผ่าน</h2>
            </td>
        </tr>
        <tr>
            <td style='padding: 15px; text-align: center; font-size: 16px; color: #555;'>
                เราได้รับคำขอเปลี่ยนรหัสผ่านสำหรับเว็บไซต์ <b>Project123</b> ของคุณแล้ว <br>
                ลิงก์นี้จะหมดอายุใน <b>24 ชั่วโมง</b> หากคุณไม่ได้ร้องขอการเปลี่ยนรหัสผ่าน โปรดละเว้นอีเมลฉบับนี้ บัญชีของคุณจะไม่มีการเปลี่ยนแปลงใดๆ
            </td>
        </tr>
        <tr>
            <td align='center' style='padding: 20px 0;'>
                <h3 style='margin: 0; color: #333;'>{userName}</h3>
            </td>
        </tr>
        <tr>
            <td align='center' style='padding: 20px 0;'>
                <a href='{resetLink}' target='_blank' style='background-color: #007bff; color: #fff; padding: 12px 20px; text-decoration: none; border-radius: 5px; font-size: 16px;'>
                    Reset Password
                </a>
            </td>
        </tr>
        <tr>
            <td style='padding: 20px; text-align: center; font-size: 12px; color: #888;'>
                หากคุณไม่ได้ร้องขอการเปลี่ยนรหัสผ่านนี้ โปรดละเว้นอีเมลนี้
            </td>
        </tr>
    </table>
</body>
</html>");

                    await _emailService.SendEmailAsync(strEmail, "Reset Your Password", htmlform.ToString());



                    //await _emailService.SendEmailAsync(strEmail, "Reset Your Password", $"Click <a href='{resetLink}'>here</a> to reset your password.");
                    //await _emailService.SendEmailAsync(strEmail, "Reset Your Password", htmlform.ToString());

                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return response;
        }
        //private async Task SendEmailAsync(string toEmail, string subject, string body)
        //{
        //    var smtpClient = new SmtpClient("smtp.your-email-provider.com")
        //    {
        //        Port = 587,
        //        Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
        //        EnableSsl = true,
        //    };

        //    var mailMessage = new MailMessage
        //    {
        //        From = new MailAddress("your-email@example.com"),
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true,
        //    };

        //    mailMessage.To.Add(toEmail);

        //    await smtpClient.SendMailAsync(mailMessage);
        //}

        public async Task<ResponseModel> Login(AdminModel userData)
        {
            ResponseModel response = new ResponseModel();
            string sqlSelectUser = @"SELECT Password FROM Tb_Admin WHERE Email = @Email";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    SqlCommand command = new SqlCommand(sqlSelectUser, connection);
                    command.Parameters.AddWithValue("@Email", userData.Email);

                    var passwordFromDb = await command.ExecuteScalarAsync() as string;

                    if (passwordFromDb != null && SecurePasswordHasherHelper.Verify(userData.Password, passwordFromDb))
                    {
                        response.Status = "S";
                        response.Message = "Login successful.";
                    }
                    else
                    {
                        response.Status = "E";
                        response.Message = "Invalid email or password.";
                    }
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return response;
        }


   

        public async Task<ResponseModel> CreateUser(dataModel userData)
        {
            ResponseModel response = new ResponseModel();
            string sqlCreateUser = @"INSERT INTO Tb_User (Name, Age, RecordDate) VALUES (@Name, @Age, @RecordDate)";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    SqlCommand command = new SqlCommand(sqlCreateUser, connection);
                    command.Parameters.AddWithValue("@Name", userData.Name);
                    command.Parameters.AddWithValue("@Age", userData.Age);
                    command.Parameters.AddWithValue("@RecordDate", userData.RecordDate);
                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "User created successfully.";
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return response;
        }

        public async Task<ResponseModel> DeleteUser(int id)
        {
            ResponseModel response = new ResponseModel();
            string sqlDeleteUser = @"DELETE FROM Tb_User WHERE Id = @UserId";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                try
                {
                    SqlCommand command = new SqlCommand(sqlDeleteUser, connection);
                    command.Parameters.AddWithValue("@UserId", id);
                    await command.ExecuteNonQueryAsync();

                    response.Status = "S";
                    response.Message = "User deleted successfully.";
                }
                catch (Exception ex)
                {
                    response.Status = "E";
                    response.Message = ex.Message;
                }
            }

            return response;
        }

        public async Task<IEnumerable<dataModel>> SearchUser(dataModel userData)
        {
            List<dataModel> userList = new List<dataModel>();
            string sqlSelect = @"SELECT Id, Name, Age, RecordDate FROM dbo.Tb_User";
            List<string> sqlWhereClauses = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(userData.Name))
            {
                sqlWhereClauses.Add("Name = @Name");
                sqlParameters.Add(new SqlParameter("@Name", userData.Name));
            }

            if (!string.IsNullOrEmpty(userData.Age))
            {
                sqlWhereClauses.Add("Age = @Age");
                sqlParameters.Add(new SqlParameter("@Age", userData.Age));
            }

            if (sqlWhereClauses.Count > 0)
            {
                sqlSelect += " WHERE " + string.Join(" AND ", sqlWhereClauses);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
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
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader["Name"].ToString(),
                                Age = reader["Age"].ToString(),
                                RecordDate = reader.GetDateTime(reader.GetOrdinal("RecordDate"))
                            };

                            userList.Add(user);
                        }
                    }
                }
            }

            return userList;
        }
    }
}
