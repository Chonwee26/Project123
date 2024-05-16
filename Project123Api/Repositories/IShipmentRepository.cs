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
        Task<IEnumerable<ShipmentLocationModel>> GetShipmentLocation();
      
    }

    public class ShipmentRepository : IShipmentRepository
    {
        private readonly IConfiguration _configuration;

        public ShipmentRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }


            public async Task<IEnumerable<ShipmentLocationModel>> GetShipmentLocation() { 
        
           List<ShipmentLocationModel> shipmentList = new List<ShipmentLocationModel>();
            string sqlSelect = @"SELECT CONVERT(VARCHAR(10), ShipmentStorageID) AS ShipmentItemID, ShipmentStorageName AS ShipmentItemText
                         FROM ShipmentLocation
                         ORDER BY ShipmentStorageID";
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                try
                {


                    using (SqlDataAdapter adapter = new SqlDataAdapter(sqlSelect, connection))
                    {
                        DataTable dtResult = new DataTable();
                        adapter.Fill(dtResult);


                        foreach (DataRow row in dtResult.Rows)
                        {
                            ShipmentLocationModel model = new ShipmentLocationModel();
                            model.ShipmentItemID = row["ShipmentItemID"].ToString();
                            model.ShipmentItemText = row["ShipmentItemText"].ToString();
                            shipmentList.Add(model);
                        }

                    }

                        connection.Close();

                    }
                
                catch (Exception ex)
                {
                    var msg = ex.Message;
                }
            }

            return await Task.FromResult(shipmentList);
        }


    


    }
}
