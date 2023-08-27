
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using FYP.Models;
using System.Data;

using RouteAttribute = System.Web.Mvc.RouteAttribute;


namespace FYPAPI.Controllers
{
    [EnableCors("*", "*", "*")]
    public class StudentController : ApiController
    {
        // FYPEntities Db = new FYPEntities();
        FYP1Entities Db = new FYP1Entities();
        [System.Web.Http.HttpGet]
        public IHttpActionResult DownloadSolution(int solutionId)
        {
            try
            {
                // Retrieve the solution from the database
                using (var dbContext = new FYP1Entities())
                {
                    var solution = dbContext.Solutions.Find(solutionId);
                    if (solution == null)
                        return NotFound();

                    // Get the file path of the solution
                    var uploadsFolder = HttpContext.Current.Server.MapPath("~/Uploads"); 
                    var filePath = Path.Combine(uploadsFolder, solution.SolutionFilePath);

                    // Check if the file exists
                    if (!File.Exists(filePath))
                        return NotFound();

                    // Generate the file name for download
                    var fileName = Path.GetFileName(filePath);

                    // Return the file for download
                    var fileBytes = File.ReadAllBytes(filePath);
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(fileBytes)
                    };
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Prompt the user to save the file
                    var responseMessage = Request.CreateResponse(HttpStatusCode.OK);
                    responseMessage.Content = new StreamContent(new MemoryStream(fileBytes));
                    responseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                    return ResponseMessage(responseMessage);
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                // Also, consider returning a more informative error message to the client
                return InternalServerError(ex);
            }
        }



        [System.Web.Http.HttpGet]
        [Route("api/Student/GetAssignmentSolutions")]
        public IHttpActionResult GetAssignmentSolutions()
        {
            try
            {
                using (var dbContext = new FYP1Entities()) // Replace FYP1Entities with your actual DbContext class
                {
                    var assignmentSolutions = dbContext.Solutions.ToList();
                    return Ok(assignmentSolutions);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[System.Web.Http.HttpPost]

        //public IHttpActionResult UploadSolution(int assignmentNumber, int semester, string section)
        //{
        //    try
        //    {
        //        var httpRequest = HttpContext.Current.Request;

        //        // Check if a file is uploaded
        //        if (httpRequest.Files.Count == 0)
        //            return BadRequest("No file uploaded.");

        //        var file = httpRequest.Files[0];

        //        // Read the file data and convert it into a byte array
        //        byte[] fileData;
        //        using (var binaryReader = new BinaryReader(file.InputStream))
        //        {
        //            fileData = binaryReader.ReadBytes(file.ContentLength);
        //        }

        //        // Create a new instance of the Solution class
        //        var solution = new Solution
        //        {
        //            AssignmentNumber = assignmentNumber,
        //            SolutionFilePath = file.FileName, // Save the file name or path as per your requirement
        //            Semester = semester,
        //            Section = section
        //        };

        //        // Add the solution to the database and save changes
        //        using (var dbContext = new FYP1Entities())
        //        {
        //            dbContext.Solutions.Add(solution);
        //            dbContext.SaveChanges();
        //        }

        //        return Ok("Assignment solution uploaded successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetSolutions(string section, int Semester)
        {
            var matchingSolutions = Db.Solutions
                .Where(s => s.Section == section && s.Semester == Semester)
                .ToList();

            return Ok(matchingSolutions);
        }

        [System.Web.Http.HttpPost]
        public IHttpActionResult UploadSolution(int assignmentNumber, int semester, string section)
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;

                // Check if a file is uploaded
                if (httpRequest.Files.Count == 0)
                    return BadRequest("No file uploaded.");

                var file = httpRequest.Files[0];

                // Check if the file has content
                if (file.ContentLength == 0)
                    return BadRequest("Uploaded file has no content.");

                // Generate a unique filename
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);


                var uploadsFolder = HttpContext.Current.Server.MapPath("~/Uploads");
                // Define the file path to save the uploaded file
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the file to the server
                file.SaveAs(filePath);

                // Create a new instance of the Solution class
                var solution = new Solution
                {
                    AssignmentNumber = assignmentNumber,
                    SolutionFilePath = fileName,
                    Semester = semester,
                    Section = section
                };

                // Add the solution to the database and save changes
                using (var dbContext = new FYP1Entities())
                {
                    dbContext.Solutions.Add(solution);
                    dbContext.SaveChanges();
                }

                return Ok("Assignment solution uploaded successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [System.Web.Http.HttpGet]
        public IHttpActionResult RunQuery(string sqlQuery)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Db.Database.Connection.ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            var dataTable = new System.Data.DataTable();
                            dataTable.Load(reader);

                            return Ok(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //
        //[System.Web.Http.HttpGet]
        //[Route("api/Student/GetTableColumns")]
        //public IHttpActionResult GetTableColumns()
        //{

        //    {
        //        var tables = Db.Database.SqlQuery<string>(
        //            "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME"
        //        ).ToList();

        //        var tableColumns = new List<TableColumnInfo>();

        //        foreach (var table in tables)
        //        {
        //            var columns = Db.Database.SqlQuery<string>(
        //                $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{table}'"
        //            ).ToList();

        //            tableColumns.Add(new TableColumnInfo
        //            {
        //                TableName = table,
        //                ColumnNames = columns
        //            });
        //        }

        //        return Ok(tableColumns);
        //    }
        //}


        public class TableColumnInfo
        {
            public string TableName { get; set; }
            public List<string> ColumnNames { get; set; }
        }




        [System.Web.Http.HttpGet]
        public IHttpActionResult Login(string Semail, string Spassword)
        {
            var emp = Db.Students.Where(model => model.Semail == Semail).FirstOrDefault();
            var emp1 = Db.Students.Where(model => model.Spassword == Spassword).FirstOrDefault();
            if (emp != null && emp1 != null)
            {

                return Ok("Successfully LogIn");

            }
            else
            {
                //return Ok(" Invalid User plz check Email & password");
                return BadRequest("Invalid User plz check Email & password");
            }

        }

        //[System.Web.Http.HttpGet]
        //public IHttpActionResult GetAssgs()
        //{
        //    List<Assignment> list = Db.Assignments.ToList();
        //    return Ok(list);
        //}
        
        
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetAssgs(int Smester, string section)
        {
            // Filter the assignments based on the provided semester and section
            List<Assignment> list = Db.Assignments
                .Where(a => a.Smester == Smester && a.Section == section)
                .ToList();

            return Ok(list);
        }



        [System.Web.Http.HttpGet]
   
        public IHttpActionResult GetSavedQueryData(int sid)
        {
            Query savedQuery = Db.Queries.FirstOrDefault(q => q.Sid == sid);
            if (savedQuery == null)
            {
                return NotFound();
            }

            // Fetch the data of the saved query based on the provided Sid
            List<Query> queryData = Db.Queries.Where(d => d.Sid == sid).ToList();

            // Return the data
            return Ok(queryData);
        }

        //[system.web.http.httppost]
        //public ihttpactionresult assgentry(assignment a)
        //{

        //    db.assignments.add(a);
        //    db.savechanges();
        //    return ok("succesfulyy insert");
        //}

        [System.Web.Http.HttpPost]
        [Route("FYPAPI/api/Student/QueryEntry")]
        public IHttpActionResult QueryEntry(Query a)
        {

            Db.Queries.Add(a);
            Db.SaveChanges();
            return Ok("Succesfulyy insert");
        }

        [System.Web.Http.HttpPut]
        public IHttpActionResult QueryUpdate(int Qid, Query a)
        {

            //Db.Entry(a).State = System.Data.Entity.EntityState.Modified;
            //Db.SaveChanges();
            var emp = Db.Queries.Where(model => model.Qid == Qid).FirstOrDefault();
            if (emp != null)
            {
                emp.Qdetails = a.Qdetails;
                Db.SaveChanges();
            }
            else
            {
                return Ok("Record not found on this  Qid");
            }
            return Ok("Successfully updated");
        }


        [System.Web.Http.HttpDelete]
        public IHttpActionResult QueryDelete(int Qid)
        {
            var emp = Db.Queries.Where(model => model.Qid == Qid).FirstOrDefault();
            if (emp != null)
            {
                Db.Queries.Remove(emp);
                Db.SaveChanges();
                return Ok("Delet Succesfully ...!");
            }
            else
            {
                return Ok("Record not found");
            }
        }


       

        //[System.Web.Http.HttpPost]
        //[Route("FYPAPI/api/Student/Amarks")]
        //public IHttpActionResult Amarks(Amark m)
        //{

        //    Db.Amarks.Add(m);
        //    Db.SaveChanges();
        //    return Ok("Succesfulyy insert");
        //}


       




        [System.Web.Http.HttpGet]
        public IHttpActionResult DownloadAssignment(int assignmentId)
        {
            try
            {
                // Retrieve the assignment from the database based on the assignment ID
                using (var dbContext = new FYP1Entities())
                {
                    var assignment = dbContext.Assignments.Find(assignmentId);
                    if (assignment == null)
                        return NotFound();

                    // Retrieve the file path from the assignment
                    var filePath = assignment.QuestionText;

                    // Check if the file exists
                    if (!File.Exists(filePath))
                        return NotFound();

                    // Get the file name from the file path
                    var fileName = Path.GetFileName(filePath);

                    // Read the file content
                    var fileBytes = File.ReadAllBytes(filePath);

                    // Create a new memory stream with the file content
                    var memoryStream = new MemoryStream(fileBytes);

                    // Create a new file content result with the memory stream and content type
                    var fileContentResult = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK)
                    {
                        Content = new StreamContent(memoryStream)
                    };

                    // Set the content type and file name for the response
                    fileContentResult.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    fileContentResult.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                    return ResponseMessage(fileContentResult);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }


}