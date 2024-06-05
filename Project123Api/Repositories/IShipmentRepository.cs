using AuthenticationPlugin;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project123.Dto;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Project123Api.Repositories
{

    public interface IShipmentRepository
    {
        Task<IEnumerable<ShipmentLocationModel>> GetShipmentLocationAsync();
        Task<IEnumerable<ShipmentLocationModel>> GetShipmentStatusAsync();
        Task<IEnumerable<ShipmentModel>> SearchShipmentAsync(ShipmentModel ShipmentData);
        Task<ResponseModel> CreateShipmentAsync(ShipmentModel ShipmentData);
        Task<IEnumerable<ShipmentModel>> UpdateShipmentAsync(ShipmentModel ShipmentData);
        Task<ResponseModel> DeleteShipmentAsync(int id);         
    }

    public class ShipmentRepository : IShipmentRepository
    {
        private readonly IConfiguration _configuration;

        public ShipmentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        //    public async Task<IEnumerable<ShipmentLocationModel>> GetShipmentLocationAsync() { 

        //   List<ShipmentLocationModel> shipmentList = new List<ShipmentLocationModel>();
        //    string sqlSelect = @"SELECT CONVERT(VARCHAR(10), ShipmentStorageID) AS ShipmentItemID, ShipmentStorageName AS ShipmentItemText
        //                 FROM ShipmentLocation
        //                 ORDER BY ShipmentStorageID";
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();

        //        try
        //        {


        //            using (SqlDataAdapter adapter = new SqlDataAdapter(sqlSelect, connection))
        //            {
        //                DataTable dtResult = new DataTable();
        //                adapter.Fill(dtResult);


        //                foreach (DataRow row in dtResult.Rows)
        //                {
        //                    ShipmentLocationModel model = new ShipmentLocationModel();
        //                    model.ShipmentItemID = row["ShipmentItemID"].ToString();
        //                    model.ShipmentItemText = row["ShipmentItemText"].ToString();
        //                    shipmentList.Add(model);
        //                }

        //            }

        //                connection.Close();

        //            }

        //        catch (Exception ex)
        //        {
        //            var msg = ex.Message;
        //        }
        //    }

        //    return await Task.FromResult(shipmentList);
        //}

        public async Task<IEnumerable<ShipmentLocationModel>> GetShipmentLocationAsync()
        {
            List<ShipmentLocationModel> shipmentList = new List<ShipmentLocationModel>();
            string sqlSelect = @"SELECT CONVERT(VARCHAR(10), ShipmentStorageID) AS ShipmentItemID, 
                                ShipmentStorageName AS ShipmentItemText
                                FROM ShipmentLocation
                                ORDER BY ShipmentStorageID";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlSelect, connection))
                    {
                        DataTable dtResult = new DataTable();
                        adapter.Fill(dtResult);


                        foreach (DataRow row in dtResult.Rows)
                        {
                            ShipmentLocationModel model = new ShipmentLocationModel
                            {
                                ShipmentItemID = row["ShipmentItemID"].ToString(),
                                ShipmentItemText = row["ShipmentItemText"].ToString()
                            };
                            shipmentList.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                var msg = ex.Message;
                // Optionally, rethrow or handle the exception as needed
            }

            return shipmentList;
        }


        public async Task<IEnumerable<ShipmentLocationModel>> GetShipmentStatusAsync()
        {
            List<ShipmentLocationModel> statusList = new List<ShipmentLocationModel>();
            string sqlSelect = @"SELECT CONVERT(VARCHAR(10), ShipmentStatusID) AS ShipmentItemID, 
                                ShipmentStatusName AS ShipmentItemText
                                FROM ShipmentStatus
                                ORDER BY ShipmentStatusID";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlSelect, connection))
                    {
                        DataTable dtResult = new DataTable();
                        adapter.Fill(dtResult);

                        foreach (DataRow row in dtResult.Rows)
                        {
                            ShipmentLocationModel model = new ShipmentLocationModel
                            {
                                ShipmentItemID = row["ShipmentItemID"].ToString(),
                                ShipmentItemText = row["ShipmentItemText"].ToString()
                            };
                            statusList.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                var msg = ex.Message;
                // Optionally, rethrow or handle the exception as needed
            }

            return statusList;
        }


        public async Task<ResponseModel> CreateShipmentAsync(ShipmentModel ShipmentData)
        {
            ResponseModel response = new ResponseModel();
            List<ShipmentModel> shipmentList = new List<ShipmentModel>();
            string sqlCreate = @"INSERT INTO Shipment(OrderNumber, FullName, MobileNumber, Storage, ShipmentStatus, ShipDate, ShipDateFR, ShipDateTO, CreateDate)
                                VALUES(@OrderNumber, @FullName, @MobileNumber, @Storage, @ShipmentStatus, @ShipDate, @ShipDateFR, @ShipDateTO, @CreateDate)";

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlCreate, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@OrderNumber", ShipmentData.OrderNumber);
                        command.Parameters.AddWithValue("@FullName", ShipmentData.FullName);
                        command.Parameters.AddWithValue("@MobileNumber", ShipmentData.MobileNumber);
                        command.Parameters.AddWithValue("@Storage", ShipmentData.Storage);
                        command.Parameters.AddWithValue("@ShipmentStatus", ShipmentData.ShipmentStatus);
                        command.Parameters.AddWithValue("@ShipDate", ShipmentData.ShipDate);
                        command.Parameters.AddWithValue("@ShipDateFR", ShipmentData.ShipDateFR ?? "");
                        command.Parameters.AddWithValue("@ShipDateTO", ShipmentData.ShipDateTO ?? "");
                        command.Parameters.AddWithValue("@CreateDate", ShipmentData.CreateDate);
                        command.ExecuteNonQuery();

                        response.Status = "S";
                        response.Message = ""+ShipmentData.OrderNumber;
                                                             
                    }
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
        }   public async Task<IEnumerable<ShipmentModel>> UpdateShipmentAsync (ShipmentModel ShipmentData)
        {
            ResponseModel response = new ResponseModel();
            List<ShipmentModel> shipmentList = new List<ShipmentModel>();
            string sqlCreate = @"UPDATE Shipment
                                SET
                                   
                                    FullName = @FullName,
                                    MobileNumber = @MobileNumber,
                                    Storage = @Storage,
                                    ShipmentStatus = @ShipmentStatus,
                                    ShipDate = @ShipDate,
                                    ShipDateFR = @ShipDateFR,
                                    ShipDateTO = @ShipDateTO,
                                    CreateDate = @CreateDate
                                WHERE                               
                                    OrderNumber = @OrderNumber;
                                ";

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlCreate, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@OrderNumber", ShipmentData.OrderNumber);
                        command.Parameters.AddWithValue("@FullName", ShipmentData.FullName);
                        command.Parameters.AddWithValue("@MobileNumber", ShipmentData.MobileNumber);
                        command.Parameters.AddWithValue("@Storage", ShipmentData.Storage);
                        command.Parameters.AddWithValue("@ShipmentStatus", ShipmentData.ShipmentStatus);
                        command.Parameters.AddWithValue("@ShipDate", ShipmentData.ShipDate);
                        command.Parameters.AddWithValue("@ShipDateFR", ShipmentData.ShipDateFR ?? "");
                        command.Parameters.AddWithValue("@ShipDateTO", ShipmentData.ShipDateTO ?? "");
                        command.Parameters.AddWithValue("@CreateDate", ShipmentData.CreateDate);
                        command.ExecuteNonQuery();

                        response.Status = "S";
                        response.Message = "Update Shipment Success Your Ordernumber are "+ShipmentData.OrderNumber;
                                                             
                    }
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

            return await Task.FromResult(shipmentList);
        }

        //public async Task<IEnumerable<ShipmentModel>> SearchShipmentAsync(ShipmentModel ShipmentData)
        //{
        //    ResponseModel response = new ResponseModel();
        //    List<ShipmentModel> shipmentList = new List<ShipmentModel>();
        //    string sqlWhere = string.Empty;
        //    string sqlSelect = @"SELECT s.*, sl.ShipmentStorageID, sl.ShipmentStorageName ,st.ShipmentStatusID, st.ShipmentStatusName
        //                 FROM dbo.Shipment s
        //                 INNER JOIN ShipmentLocation sl
        //                 ON s.Storage = sl.ShipmentStorageID
        //                 INNER JOIN ShipmentStatus st
        //                 ON s.ShipmentStatus = st.ShipmentStatusID
        //                 ";
        //    if (!string.IsNullOrEmpty(ShipmentData.OrderNumber))
        //    {        
        //        sqlWhere = "WHERE s.OrderNumber = @OrderNumber";
        //    }
        //    sqlSelect +=" "+ sqlWhere;
        //    string connectionString = _configuration.GetConnectionString("DefaultConnection");

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            await connection.OpenAsync();


        //            using (SqlDataAdapter adapter = new SqlDataAdapter(sqlSelect, connection))
        //            {
        //                adapter.SelectCommand.Parameters.AddWithValue("@OrderNumber", ShipmentData.OrderNumber);

        //                DataTable dtResult = new DataTable();
        //                adapter.Fill(dtResult);

        //                foreach (DataRow row in dtResult.Rows)
        //                {
        //                    ShipmentModel model = new ShipmentModel
        //                    {
        //                        ShipmentId = Convert.ToInt32(row["ShipmentId"]),
        //                        OrderNumber = row["OrderNumber"].ToString(),
        //                        FullName = row["FullName"].ToString(),
        //                        MobileNumber = row["MobileNumber"].ToString(),
        //                        Storage = row["Storage"].ToString(),
        //                        ShipmentStatus = row["ShipmentStatus"] != DBNull.Value ? Convert.ToInt32(row["ShipmentStatus"]) : (int?)null,
        //                        ShipDate = row["ShipDate"].ToString(),
        //                        ShipDateFR = row["ShipDateFR"].ToString(),
        //                        ShipDateTO = row["ShipDateTO"].ToString(),
        //                        CreateDate = row["CreateDate"].ToString(),
        //                    };

        //                    ShipmentLocationModel location = new ShipmentLocationModel
        //                    {
        //                        ShipmentItemID = row["ShipmentStorageID"].ToString(),
        //                        ShipmentItemText = row["ShipmentStorageName"].ToString()
        //                    };  
        //                    ShipmentStatusModel status = new ShipmentStatusModel
        //                    {
        //                        ShipmentItemID = row["ShipmentStatusID"].ToString(),
        //                        ShipmentItemText = row["ShipmentStatusName"].ToString()
        //                    };

        //                    model.ShipmentLocation.Add(location);
        //                    model.ShipmentStatusList.Add(status);
        //                    shipmentList.Add(model);
        //                }
        //                response.Status = "S";
        //                response.Message = "Success";

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception or handle it as needed
        //        response.Status = "E";
        //        response.Message = ex.Message;

        //        // Optionally, rethrow or handle the exception as needed
        //    }


        //    return shipmentList;
        //}


        public async Task<IEnumerable<ShipmentModel>> SearchShipmentAsync(ShipmentModel ShipmentData)
        {
            ResponseModel response = new ResponseModel();
            // Check if all fields in ShipmentData are null or empty
            if (string.IsNullOrEmpty(ShipmentData.OrderNumber) &&
                string.IsNullOrEmpty(ShipmentData.FullName) &&
                string.IsNullOrEmpty(ShipmentData.MobileNumber) &&
                string.IsNullOrEmpty(ShipmentData.Storage) &&
                (ShipmentData.ShipmentStatus == null))
            {
              
                
                    response.Status = "E";
                    response.Message = "Error";
                
                return new List<ShipmentModel>(); // Return empty list indicating no data found
            }

            List<ShipmentModel> shipmentList = new List<ShipmentModel>();
            string sqlSelect = @"SELECT s.*, sl.ShipmentStorageID, sl.ShipmentStorageName, st.ShipmentStatusID, st.ShipmentStatusName
                         FROM dbo.Shipment s
                         INNER JOIN ShipmentLocation sl ON s.Storage = sl.ShipmentStorageID
                         INNER JOIN ShipmentStatus st ON s.ShipmentStatus = st.ShipmentStatusID";
            List<string> sqlWhereClauses = new List<string>();
            List<SqlParameter> sqlParameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(ShipmentData.OrderNumber))
            {
                sqlWhereClauses.Add("s.OrderNumber = @OrderNumber");
                sqlParameters.Add(new SqlParameter("@OrderNumber", ShipmentData.OrderNumber));
            }

            if (!string.IsNullOrEmpty(ShipmentData.FullName))
            {
                sqlWhereClauses.Add("s.FullName = @FullName");
                sqlParameters.Add(new SqlParameter("@FullName", ShipmentData.FullName));
            }

            if (!string.IsNullOrEmpty(ShipmentData.MobileNumber))
            {
                sqlWhereClauses.Add("s.MobileNumber = @MobileNumber");
                sqlParameters.Add(new SqlParameter("@MobileNumber", ShipmentData.MobileNumber));
            }

            if (!string.IsNullOrEmpty(ShipmentData.Storage))
            {
                sqlWhereClauses.Add("s.Storage = @Storage");
                sqlParameters.Add(new SqlParameter("@Storage", ShipmentData.Storage));
            }

            if (ShipmentData.ShipmentStatus != null)
            {
                sqlWhereClauses.Add("s.ShipmentStatus = @ShipmentStatus");
                sqlParameters.Add(new SqlParameter("@ShipmentStatus", ShipmentData.ShipmentStatus));
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
                                ShipmentModel model = new ShipmentModel
                                {
                                    ShipmentId = Convert.ToInt32(reader["ShipmentId"]),
                                    OrderNumber = reader["OrderNumber"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    MobileNumber = reader["MobileNumber"].ToString(),
                                    Storage = reader["Storage"].ToString(),
                                    ShipmentStatus = reader["ShipmentStatus"] != DBNull.Value ? Convert.ToInt32(reader["ShipmentStatus"]) : (int?)null,
                                    ShipDate = reader["ShipDate"].ToString(),
                                    ShipDateFR = reader["ShipDateFR"].ToString(),
                                    ShipDateTO = reader["ShipDateTO"].ToString(),
                                    CreateDate = reader["CreateDate"].ToString(),
                                };

                                ShipmentLocationModel location = new ShipmentLocationModel
                                {
                                    ShipmentItemID = reader["ShipmentStorageID"].ToString(),
                                    ShipmentItemText = reader["ShipmentStorageName"].ToString()
                                };
                                ShipmentStatusModel status = new ShipmentStatusModel
                                {
                                    ShipmentItemID = reader["ShipmentStatusID"].ToString(),
                                    ShipmentItemText = reader["ShipmentStatusName"].ToString()
                                };

                                model.ShipmentLocation.Add(location);
                                model.ShipmentStatusList.Add(status);
                                shipmentList.Add(model);
                            }
                        }
                    }
                }

                if (shipmentList.Count == 0)
                {

                    response.Status = "E";
                    response.Message = "Error";
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

            return shipmentList;
        }


        public async Task<ResponseModel> DeleteShipmentAsync(int id)
        {
            ResponseModel response = new ResponseModel();
            string sqlDelete = @"DELETE FROM Shipment WHERE ShipmentId = @ShipmentId";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(sqlDelete, connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@ShipmentId", id);
                        command.ExecuteNonQuery();
                       
                        response.Status = "S";
                        response.Message = "Delete Shipment "+ "Success";
                    }
                }
                catch (Exception ex)
                {
                  response.Status="E";
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

