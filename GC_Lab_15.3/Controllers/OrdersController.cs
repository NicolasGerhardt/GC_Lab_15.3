using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GC_Lab_15._3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GC_Lab_15._3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly string connectionString;

        public OrdersController(IConfiguration config)
        {
            connectionString = config.GetConnectionString("default");
        }

        /// <summary>
        /// api/Order/all
        /// End point to get a list of all orders.
        /// This will be a large pull when used. 
        /// </summary>
        /// <returns></returns>
        [HttpGet("all")]
        public IEnumerable<OrderShipment> Get()
        {
            IEnumerable<OrderShipment> orders = new OrderShipment[0];

            try
            {


                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string queryString = "SELECT  OrderID, CustomerID, OrderDate, ShippedDate,ShipName, ";
                    queryString += "ShipAddress, ShipCity, ShipPostalCode, ShipRegion, ShipCountry ";
                    queryString += "FROM Orders ";
                    queryString += "ORDER BY CustomerID ";

                    orders = conn.Query<OrderShipment>(queryString);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("V**************************************************V");
                Console.WriteLine(e);
                Console.WriteLine("|**************************************************|");
                Console.WriteLine(e.Message); ;
                Console.WriteLine("^**************************************************^");
            }


            return orders;
        }

        /// <summary>
        /// api/Order/{offset}
        /// End point to get 25 orders at a time
        /// </summary>
        /// <param name="offset">Represents the offset to start the orders lookup</param>
        /// <returns></returns>
        [HttpGet("{offset}")]
        public IEnumerable<OrderShipment> Get(int offset = 0)
        {
            if (offset < 0)
            {
                offset = 0;
            }

            IEnumerable<OrderShipment> orders = new OrderShipment[0];
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string queryString = "SELECT  OrderID, CustomerID, OrderDate, ShippedDate,ShipName, ";
                    queryString += "ShipAddress, ShipCity, ShipPostalCode, ShipRegion, ShipCountry ";
                    queryString += "FROM Orders ";
                    queryString += "ORDER BY CustomerID ";
                    queryString += "OFFSET @Offset ROWS ";
                    queryString += "FETCH NEXT 25 ROWS ONLY";

                    orders = conn.Query<OrderShipment>(queryString, new { Offset = offset });
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("V**************************************************V");
                Console.WriteLine(e);
                Console.WriteLine("|**************************************************|");
                Console.WriteLine(e.Message); ;
                Console.WriteLine("^**************************************************^");
            }

            return orders;
        }

        /// <summary>
        /// api/Order/?CustomerID={CustomerID}
        /// Pulls up all of the orders by CustomerID.
        /// </summary>
        /// <param name="CustomerID"></param>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<OrderShipment> Get(string CustomerID = null)
        {
            IEnumerable<OrderShipment> orders = new OrderShipment[0];

            if (CustomerID != null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string queryString = "SELECT  OrderID, CustomerID, OrderDate, ShippedDate,ShipName, ";
                        queryString += "ShipAddress, ShipCity, ShipPostalCode, ShipRegion, ShipCountry ";
                        queryString += "FROM Orders ";
                        queryString += "WHERE CustomerID = @CustomerID ";

                        orders = conn.Query<OrderShipment>(queryString, new { CustomerID });
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("V**************************************************V");
                    Console.WriteLine(e);
                    Console.WriteLine("|**************************************************|");
                    Console.WriteLine(e.Message); ;
                    Console.WriteLine("^**************************************************^");
                }
            }


            return orders;
        }



        [HttpPost]
        public object Post(OrderShipment order)
        {
            int rows = 0;
            string[] allCustomerIDs = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string SQLallCustomerIDs = "select DISTINCT CustomerID from Customers";
                allCustomerIDs = conn.Query<string>(SQLallCustomerIDs).ToArray();
            }

            if (!allCustomerIDs.Contains(order.CustomerID))
            {
                return new { success = false, message = "Invalid CustomerID", ValidCustomerIDs = allCustomerIDs };
            }

            return new { success = false, message = "Valid CustomerID" };
        }


        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            int rows = 0;

            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string queryString = "delete ";
                    queryString += "From[Order Details] ";
                    queryString += "where OrderID = @OrderID ";
                    queryString += "delete ";
                    queryString += "from Orders ";
                    queryString += "where OrderID = @OrderID; ";

                    rows = conn.Execute(queryString, new { OrderID = id });
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("V**************************************************V");
                Console.WriteLine(e);
                Console.WriteLine("|**************************************************|");
                Console.WriteLine(e.Message); ;
                Console.WriteLine("^**************************************************^");
            }

            if (rows == 0)
            {
                return new { success = false };
            }

            return new { success = true };
        }


    }
}