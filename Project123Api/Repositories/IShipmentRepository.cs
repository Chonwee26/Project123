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

    public interface IShipmentRepository
    {
        Task<IEnumerable<ShipmentLocationModel>> GetShipmentLocationAsync();
        Task<IEnumerable<ShipmentLocationModel>> GetShipmentStatusAsync();
      
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



        public async Task<IEnumerable<ShipmentModel>> SearchShipment()
        {
            List<ShipmentModel> shipmentList = new List<ShipmentModel>();
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
                            ShipmentModel model = new ShipmentModel
                            {
                                //ShipmentItemID = row["ShipmentItemID"].ToString(),
                                //ShipmentItemText = row["ShipmentItemText"].ToString()
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







    }
}
