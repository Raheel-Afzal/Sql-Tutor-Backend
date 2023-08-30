//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Data;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using FYP.Models;
//using System.Web.Http.Cors;
//using System.Reflection;
//using System.Data.Entity;
//using System.CodeDom.Compiler;
//using System.Xml.Linq;
//using System.Data.Entity.Infrastructure;
//using System.Security.Cryptography;
//using System.Web.UI.WebControls;

//namespace FYPAPI.Controllers
//{
//    [EnableCors("*", "*", "*")]

//    public class DatbaseController : Controller
//    {
//        // FYP2Entities Db = new FYP2Entities();

//        // GET: Datbase
//        string constr = "Data Source=DESKTOP-KIDCKCN;Integrated Security=True";

//        public List<string> GetDatabaseList()
//        {

//            List<string> databaseList = new List<string>();

//            // Extract the server name from the connection string
//            string serverName = new SqlConnectionStringBuilder(constr).DataSource;

//            // Build a connection string to the master database on the same server
//            string masterConnectionString = $"Server={serverName};Database=master;Trusted_Connection=True;";

//            // Query the master database to get the list of all databases
//            using (SqlConnection connection = new SqlConnection(masterConnectionString))
//            {
//                connection.Open();

//                using (SqlCommand command = new SqlCommand("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')", connection))
//                {
//                    using (SqlDataReader reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            string databaseName = reader.GetString(0);
//                            databaseList.Add(databaseName);
//                        }
//                    }
//                }
//            }

//            return databaseList;
//        }
//    }
//}