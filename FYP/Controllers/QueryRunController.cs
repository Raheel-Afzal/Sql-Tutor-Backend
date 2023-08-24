using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using FYP.Models;
namespace FYPAPI.Controllers
{
    [EnableCors("*", "*", "*")]
    public class QueryRunController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> RunQuery(string databaseName, [FromUri] string query)
        {
            try
            {
                if (string.IsNullOrEmpty(databaseName) || string.IsNullOrEmpty(query))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid database name or query.");
                }

                // Connection string for the SQL Server
                string constr = "Server=DESKTOP-UBEKQ4F;Database=" + databaseName +";user id=sa;password=123;MultipleActiveResultSets=True";

                string connectionString = "Server=DESKTOP-KIDCKCN;Database=" + databaseName + ";Trusted_Connection=True;";

                using (SqlConnection connection = new SqlConnection(constr))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

                            while (await reader.ReadAsync())
                            {
                                Dictionary<string, object> row = new Dictionary<string, object>();

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row[reader.GetName(i)] = reader.GetValue(i);
                                }

                                result.Add(row);
                            }

                            return Request.CreateResponse(HttpStatusCode.OK, result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error executing query: " + ex.Message);
            }
        }
    }
}
