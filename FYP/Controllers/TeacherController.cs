using FYP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using static FYPAPI.Controllers.StudentController;
using static FYPAPI.Controllers.TeacherController;

namespace FYPAPI.Controllers
{
    [EnableCors("*", "*", "*")]
  
    public class TeacherController : ApiController
    {
        FYP1Entities Db = new FYP1Entities();


        //
        [System.Web.Http.HttpPost]
        public IHttpActionResult UploadAssignmentSolution()
        {
            var httpRequest = HttpContext.Current.Request;

            // Validate if the request contains a file
            if (httpRequest.Files.Count == 0)
                return BadRequest("No file found in the request.");

            var file = httpRequest.Files[0];

            // Create a byte array to store the file content
            byte[] fileData;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }

            // Convert string values to appropriate types
            int assignmentNumber = int.Parse(httpRequest["AssignmentNumber"]);
            int smester = int.Parse(httpRequest["Smester"]);
            string section = httpRequest["Section"];

            // Save the file data to the database
            using (var db = new FYP1Entities())
            {
                var assignmentSolution = new AssignmentSolution
                {
                    AssignmentNumber = assignmentNumber,
                    Solution = Convert.ToBase64String(fileData),
                    Smester = smester,
                    Section = section
                };

                db.AssignmentSolutions.Add(assignmentSolution);
                db.SaveChanges();
            }

            return Ok("Assignment solution uploaded successfully.");
        }

        //
        [System.Web.Http.HttpGet]
        public IHttpActionResult Login(string Temail, string Tpassword)
        {
            var emp = Db.Teachers.Where(model => model.Temail == Temail).FirstOrDefault();
            var emp1 = Db.Teachers.Where(model => model.Tpassword == Tpassword).FirstOrDefault();
            if (emp != null && emp1 != null)
            {

                return Ok("Successfully LogIn");
            }
            else
            {
                return Ok(" Invalid User plz check Email & password");
            }

            return Ok(emp);
        }
        //[System.Web.Http.HttpGet]
        //public IHttpActionResult GetStudentInfoByEmail(string email)
        //{
        //    Student student = Db.Students.FirstOrDefault(s => s.Semail == email);
        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    string fullName = string.Format("{0} {1}", student.Fname, student.Lname);
        //    string info = string.Format("{0} {1} {2} {3}", fullName, student.Semester, student.Section, student.Semail);

        //    return Ok(info);
        //}
        //
        [System.Web.Http.HttpGet]

        public IHttpActionResult GetStudentInfoByEmail(string email)
        {
            Student student = Db.Students.FirstOrDefault(s => s.Semail == email);
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }
        //
        [System.Web.Http.HttpGet]
        [Route("api/Teacher/GetAssgs")]
        public IHttpActionResult GetAssgs()
        {
            List<Assignment> list = Db.Assignments.ToList();
            return Ok(list);
        }
        //
        [System.Web.Http.HttpGet]
        [Route("api/Teacher/GetTStudentInfo")]
        public IHttpActionResult GetTStudentInfo()
        {
            List<Student> students = Db.Students.ToList();
            List<Dictionary<string, object>> results = new List<Dictionary<string, object>>();

            // Retrieve column names from the Student entity
            var columnNames = typeof(Student).GetProperties().Select(p => p.Name).ToList();

            // Add column names as the first item in the results list
            Dictionary<string, object> columnData = new Dictionary<string, object>();
            foreach (string columnName in columnNames)
            {
                columnData[columnName] = columnName;
            }
            results.Add(columnData);

            foreach (Student student in students)
            {
                Dictionary<string, object> studentData = new Dictionary<string, object>();

                foreach (string columnName in columnNames)
                {
                    var columnValue = typeof(Student).GetProperty(columnName)?.GetValue(student);
                    studentData[columnName] = columnValue;
                }

                results.Add(studentData);
            }

            return Ok(results);
        }
        //
        [System.Web.Http.HttpPut]
        public IHttpActionResult UpdateStudentInfo(int Sid, Student updatedStudent)
        {
            // Retrieve the student from the database based on the provided Sid
            Student student = Db.Students.FirstOrDefault(s => s.Sid == Sid);
            if (student == null)
            {
                return NotFound();
            }

            // Update the student information with the provided values
            student.Fname = updatedStudent.Fname;
            student.Lname = updatedStudent.Lname;
            student.Semester = updatedStudent.Semester;
            student.Section = updatedStudent.Section;
            student.Semail = updatedStudent.Semail;
            student.Spassword = updatedStudent.Spassword;

            Db.SaveChanges();

            return Ok("Successfully updated");
        }
        //
        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteStudent(int Sid)
        {
            // Retrieve the student from the database based on the provided Sid
            Student student = Db.Students.FirstOrDefault(s => s.Sid == Sid);
            if (student == null)
            {
                return NotFound();
            }

            // Delete the student from the database
            Db.Students.Remove(student);
            Db.SaveChanges();

            return Ok("Student deleted successfully");
        }
        //
        
      
        [System.Web.Http.HttpGet]
        
        public IHttpActionResult GetStudentInfo()
        {
            List<Student> students = Db.Students.ToList();
            List<string> results = new List<string>();
            foreach (Student student in students)
            {
                string fullName = string.Format("{0} {1}", student.Fname, student.Lname);
                string info = string.Format("{0} {1} {2} {3}", fullName, student.Semester, student.Section, student.Semail);
                results.Add(info);
            }
            return Ok(results);
        }
        //
        [System.Web.Http.HttpPost]
        public IHttpActionResult AddStudent(Student student)
        {
            try
            {
                using (FYP1Entities Db = new FYP1Entities())
                {
                    Db.Students.Add(student);
                    Db.SaveChanges();
                }
                return Ok("Student added successfully");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessage = string.Join("; ", ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                return BadRequest("Error adding student: " + errorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest("Error adding student: " + ex.Message);
            }
        }

        //
        [System.Web.Http.HttpPost]
        public IHttpActionResult AssgEntry(Assignment a)
        {

            Db.Assignments.Add(a);
            Db.SaveChanges();
            return Ok("Succesfulyy insert");
        }

        //[System.Web.Http.HttpPost]
        //public IHttpActionResult AssgEntry(Assignment a)
        //{
        //    using (var Db = new FYP1Entities())
        //    {
        //        Db.Assignments.Add(a);
        //        Db.SaveChanges();
        //        return Ok("Successfully inserted assignment.");
        //    }
        //}

        //
        [System.Web.Http.HttpPost]
        [Route("api/teacher/InsertAmarkRecord")]
        public IHttpActionResult InsertAmarkRecord(Amark amark)
        {

            Db.Amarks.Add(amark);
            Db.SaveChanges();
            return Ok("Succesfulyy insert");
        }
        //
        [System.Web.Http.HttpGet]
        public IHttpActionResult marks()
        {

            List<Amark> list = Db.Amarks.ToList();
            return Ok(list);


            return Ok();
        }
        //
        [System.Web.Http.HttpGet]
        public IHttpActionResult Query()
        {

            List<Query> list = Db.Queries.ToList();
            return Ok(list);


            return Ok();
        }
        //

        string constr = "data source=DESKTOP-UBEKQ4F;initial catalog=FYPDB;user id=sa;password=123;MultipleActiveResultSets=True";
        [Route("api/Teacher/GetDatabaseList")]
        public List<string> GetDatabaseList()
        {

            List<string> databaseList = new List<string>();

            // Extract the server name from the connection string
            string serverName = new SqlConnectionStringBuilder(constr).DataSource;

            // Build a connection string to the master database on the same server
            string masterConnectionString = $"Server={serverName};Database=master;Trusted_Connection=True;";

            // Query the master database to get the list of all databases
            using (SqlConnection connection = new SqlConnection(constr))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT name FROM sys.databases WHERE name NOT IN ('master', 'tempdb', 'model', 'msdb')", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string databaseName = reader.GetString(0);
                            databaseList.Add(databaseName);
                        }
                    }
                }
            }

            return databaseList;
        }
        //
        [System.Web.Http.HttpGet]
        public IHttpActionResult GetTableColumns(string databaseName)
        {
            var tableColumns = new List<TableColumnInfo>();

            // Build a connection string to the specified database on the same server
            string constr = "Server=DESKTOP-UBEKQ4F;Database=" + databaseName + ";user id=sa;password=123;MultipleActiveResultSets=True";

            // Query the specified database to get the list of all tables and their columns
            using (SqlConnection connection = new SqlConnection(constr))
            {
                connection.Open();

                // Get the list of tables in the database
                var tables = connection.GetSchema("Tables");

                foreach (DataRow table in tables.Rows)
                {
                    string tableName = table[2].ToString();

                    // Get the list of columns in each table
                    var columns = connection.GetSchema("Columns", new[] { null, null, tableName });

                    var columnNames = new List<string>();
                    foreach (DataRow column in columns.Rows)
                    {
                        columnNames.Add(column[3].ToString());
                    }

                    tableColumns.Add(new TableColumnInfo
                    {
                        TableName = tableName,
                        ColumnNames = columnNames
                    });
                }
            }

            return Ok(tableColumns);
        }
        //
        [System.Web.Http.HttpGet]
        [Route("api/teacher/GetStudentQueryInfo")]
        public IHttpActionResult GetStudentQueryInfo()
        {
            List<Student> students = Db.Students.ToList();
            List<Query> queries = Db.Queries.ToList();

            var results = new List<Dictionary<string, object>>();

            foreach (var student in students)
            {
                var studentQueries = queries.Where(q => q.Sid == student.Sid).ToList();

                if (studentQueries.Count > 0)
                {
                    foreach (var query in studentQueries)
                    {
                        var result = new Dictionary<string, object>();

                        // Add Student table columns to the result dictionary
                        var studentColumns = typeof(Student).GetProperties();
                        foreach (var column in studentColumns)
                        {
                            result[column.Name] = column.GetValue(student);
                        }

                        // Add Query table columns to the result dictionary
                        var queryColumns = typeof(Query).GetProperties();
                        foreach (var column in queryColumns)
                        {
                            result[column.Name] = column.GetValue(query);
                        }

                        results.Add(result);
                    }
                }
            }

            return Ok(results);
        }

        [System.Web.Http.HttpPost]
        [Route("api/Teacher/AssgmentEntry")]

        public IHttpActionResult AssignmentEntry()
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

                // Extract other form data parameters
                var assignmentNumber = Convert.ToInt32(httpRequest.Form["assignmentNumber"]);
                var title = httpRequest.Form["title"];
                var deadline = Convert.ToDateTime(httpRequest.Form["deadline"]);
                var databaseName = httpRequest.Form["databaseName"];
                var section = httpRequest.Form["section"];
               
                var semester = Convert.ToInt32(httpRequest.Form["semester"]);


                // Generate a unique filename
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                string path = HttpContext.Current.Server.MapPath("~/Uploads");

                // Define the file path to save the uploaded file
                var filePath = Path.Combine(path, fileName);

                // Save the file to the server
                file.SaveAs(filePath);

                // Create a new instance of the Assignment class
                var assignment = new Assignment
                {
                    AssignmentNumber = int.Parse(assignmentNumber.ToString()),
                    Title = title,
                    QuestionText = filePath,
                    Deadline = deadline,
                    DatabaseName = databaseName,
                    Section = section,
                    Smester = semester
                };

                // Add the assignment to the database and save changes
                using (var dbContext = new FYP1Entities())
                {
                    dbContext.Assignments.Add(assignment);
                    dbContext.SaveChanges();
                }

                return Ok("Assignment created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [System.Web.Http.HttpGet]
        public IHttpActionResult marks(int Sid)
        {
            var emp = Db.Amarks.Where(model => model.Sid == Sid).ToList();


            if (emp != null && emp.Count > 0)
            {

                return Ok(emp);

            }
            else
            {
                return Ok("User not found");
            }

            return Ok();
        }
        [System.Web.Http.HttpDelete]
        public IHttpActionResult DeleteAssignment(int Aid)
        {
            var assignment = Db.Assignments.FirstOrDefault(a => a.Aid == Aid);
            if (assignment != null)
            {
                Db.Assignments.Remove(assignment);
                Db.SaveChanges();
                return Ok("Deleted successfully!");
            }
            else
            {
                return Ok("Record not found");
            }
        }
        //[System.Web.Http.HttpPost]
        //public IHttpActionResult InsertAmarkRecord(Amark amark)
        //{

        //    Db.Amarks.Add(amark);
        //    Db.SaveChanges();
        //    return Ok("Succesfulyy insert");
        //}
    }
}
