using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ProfIT
{
    public class DataLayer
    {

        public static void AddJobsToList(List<Job> list, IEnumerable iteratorObject)
        {
            list.AddRange(from DataRowView record in iteratorObject
                          select new Job(new Guid(record["JobID"].ToString()), record["JobName"].ToString(), Convert.ToBoolean(record["Active"].ToString()), record["OrderID"].ToString(), Convert.ToDateTime(record["JobStartDate"].ToString()), Convert.ToDateTime(record["JobEndDate"].ToString()), record["ContactPerson"].ToString(), record["Address"].ToString(), record["Comments"].ToString(), Convert.ToDateTime(record["RentStartDate"].ToString()), Convert.ToDateTime(record["RentEndDate"].ToString()), Convert.ToDateTime(record["JobStartTime"].ToString()), Convert.ToDateTime(record["JobEndTime"].ToString()), string.Empty, Convert.ToBoolean(record["FullDay"].ToString()), Convert.ToBoolean(record["NeedUpdate"].ToString()), new Guid(record["ParentJobID"].ToString()), string.Empty, record["ContactPhone"].ToString(), Convert.ToByte(record["Type"].ToString()), record["Month"].ToString()));
        }


        /// <summary>
        /// Create new job
        /// </summary>
        /// <param name="job"></param>
        /// <param name="connection"></param>
        public static void CreateJob(Job job, SqlConnection connection)
        {
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_CreateJob", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                SetParametersForJob(cmd, job);

                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }	
            
        }

        /// <summary>
        /// Delete Job by jobId
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="connection"></param>
        public static void DeleteJob(Guid jobId, SqlConnection connection)
        {
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_DeleteJobByJobID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = "@JobID";
                parameter.Value = jobId;
                cmd.Parameters.Add(parameter);

                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }

        /// <summary>
        /// Delete Job by jobId
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="connection"></param>
        public static void DeleteJobClones(Guid jobId, SqlConnection connection)
        {
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_DeleteCloneJobsByJobID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = "@JobID";
                parameter.Value = jobId;
                cmd.Parameters.Add(parameter);

                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }

        /// <summary>
        /// Update current job with new values
        /// </summary>
        /// <param name="job"></param>
        /// <param name="connection"></param>
        public static void UpdateJob(Job job, SqlConnection connection)
        {
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_UpdateJob", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SetParametersForJob(cmd, job);


                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }

        /// <summary>
        /// Update Current Person with new values
        /// </summary>
        /// <param name="person"></param>
        /// <param name="connection"></param>
        public static void UpdatePerson(Person person, SqlConnection connection)
        {
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_UpdatePerson", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SetParametersForPerson(cmd, person);


                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }

        private static void SetParametersForPerson(SqlCommand cmd, Person person)
        {
            cmd.Parameters.Add(new SqlParameter("@PersonID", person.PersonId));
            cmd.Parameters.Add(new SqlParameter("@FirstName", person.Firstname));
            cmd.Parameters.Add(new SqlParameter("@LastName", person.Lastname));
            cmd.Parameters.Add(new SqlParameter("@Mobile", person.Mobile));
            cmd.Parameters.Add(new SqlParameter("@Email", person.Email));
            cmd.Parameters.Add(new SqlParameter("@Active", person.Active));
        }

        /// <summary>
        /// Add Car to the Job
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="CarId"></param>
        /// <param name="connection"></param>
        public static void AddCarToJob(Guid JobId, Guid CarId, SqlConnection connection)
        {
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_JobAddCar", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@JobID", JobId));
                cmd.Parameters.Add(new SqlParameter("@CarID", CarId));


                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }

        /// <summary>
        /// Get Car/Cars that the Current JobID has assigned
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Car> GetCarsForJobByJobId(Guid jobId, SqlConnection connection)
        {
            List<Car> list = new List<Car>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetCarsForJobByJobID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = "@JobID";
                parameter.Value = jobId;
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Car car = new Car(
                        new Guid(rdr.GetValue(1).ToString()),
                        new Guid(rdr.GetValue(0).ToString()),
                        rdr.GetValue(2).ToString(),
                        rdr.GetValue(3).ToString()
                        );

                    // Add To list
                    list.Add(car);
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Gets All Cars
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Car> GetAllCars(SqlConnection connection)
        {
            List<Car> list = new List<Car>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetAllCars", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                //SqlParameter parameter = new SqlParameter();
                //parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                //parameter.ParameterName = "@JobID";
                //parameter.Value = jobId;
                //cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var car = new Car
                        (
                        new Guid(rdr.GetValue(0).ToString()),
                        Guid.Empty, 
                        rdr.GetValue(1).ToString(),
                        rdr.GetValue(2).ToString()
                        );

                    // Add To list
                    list.Add(car);
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Get a List of Jobs by specific dateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Job> GetJobsByDate(DateTime dateTime, SqlConnection connection)
        {
            var list = new List<Job>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetJobsByDate", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                var parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.Date;
                parameter.ParameterName = "@JobStartDate";
                parameter.Value = dateTime.ToShortDateString();
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                    Job job = BindJob(rdr);


                    // Add To list
                    if (job.Type == SiteUtilities.IMPORT || job.Type == SiteUtilities.CONSTRUCTION || job.Type == SiteUtilities.DESTRUCTION)
                    {
                        list.Add(job);
                    }
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Get Jobs that has RentalEndDate Higher(Overdue) than dateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Job> GetJobsWithRentalOverDue(DateTime dateTime, SqlConnection connection)
        {
            var list = new List<Job>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetJobWithRentalOverDue", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                var parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.Date;
                parameter.ParameterName = "@DateTime";
                parameter.Value = dateTime.ToShortDateString();
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Job job = BindJob(rdr);

                    // Add To list
                    list.Add(job);
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Get Job by JobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Job> GetJobsByJobId(Guid jobId, SqlConnection connection)
        {
            List<Job> list = new List<Job>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetJobByJobID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = "@JobID";
                parameter.Value = jobId;
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Job job = BindJob(rdr);

                    // Add To list
                    list.Add(job);
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Get JobClones JobIDs by JobID
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Guid> GetJobClonesByJobId(Guid jobId, SqlConnection connection)
        {
            List<Guid> list = new List<Guid>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetJobClonesByJobID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = "@JobID";
                parameter.Value = jobId;
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    // Add To list
                    list.Add(new Guid(rdr.GetValue(0).ToString()));
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Get Jobs by specific year-month (ex. 2011-05)
        /// </summary>
        /// <param name="date"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Job> GetJobsByMonth(string date, SqlConnection connection)
        {
            List<Job> list = new List<Job>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetJobByDate", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.ParameterName = "@DateTime";
                parameter.Value = date;
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Job job = BindJob(rdr);

                    // Add To list
                    if (job.Type == SiteUtilities.IMPORT || job.Type == SiteUtilities.CONSTRUCTION || job.Type == SiteUtilities.DESTRUCTION)
                    {
                        list.Add(job);
                    }
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Get Jobs By OrderID
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Job> GetJobsByJobOrderId(string orderId, SqlConnection connection)
        {
            List<Job> list = new List<Job>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetJobByOrderID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.NVarChar;
                parameter.ParameterName = "@OrderID";
                parameter.Value = orderId;
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Job job = BindJob(rdr);

                    // Add To list
                    list.Add(job);
                }
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        private static Job BindJob(SqlDataReader rdr)
        {
            return new Job(
                new Guid(rdr.GetValue(0).ToString()),
                rdr.GetValue(1).ToString(),
                Convert.ToBoolean(rdr.GetValue(2).ToString()),
                rdr.GetValue(3).ToString(),
                Convert.ToDateTime(rdr.GetValue(4).ToString()),
                Convert.ToDateTime(rdr.GetValue(6).ToString()),
                rdr.GetValue(10).ToString(),
                rdr.GetValue(11).ToString(),
                rdr.GetValue(12).ToString(),
                Convert.ToDateTime(rdr.GetValue(8).ToString()),
                Convert.ToDateTime(rdr.GetValue(9).ToString()),
                Convert.ToDateTime(rdr.GetValue(5).ToString()),
                Convert.ToDateTime(rdr.GetValue(7).ToString()),
                string.Empty,
                Convert.ToBoolean(rdr.GetValue(13).ToString()),
                Convert.ToBoolean(rdr.GetValue(14).ToString()),
                new Guid(rdr.GetValue(15).ToString()),
                string.Empty,
                rdr.GetValue(16).ToString(),
                Convert.ToByte(rdr.GetValue(17).ToString()),
                rdr.GetValue(18).ToString());
        }

        /// <summary>
        /// Get Personal for Current Job
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<JobPersonList> GetPersonalListForJobByJobId(Guid jobId, SqlConnection connection)
        {
            var list = new List<JobPersonList>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_JobPersonalByJobID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = "@JobID";
                parameter.Value = jobId;
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                    JobPersonList person = new JobPersonList
                        (
                            new Guid(rdr.GetValue(0).ToString()),
                            new Guid(rdr.GetValue(1).ToString()),
                            rdr.GetValue(2).ToString(),
                            rdr.GetValue(3).ToString(),
                            rdr.GetValue(4).ToString(),
                            rdr.GetValue(5).ToString(),
                            Convert.ToBoolean(rdr.GetValue(6).ToString())
                        );
                    // Add To list
                    list.Add(person);
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Get Personal that is busy for a specific date
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Person> GetBusyPersonalListForDate(DateTime dateTime, SqlConnection connection)
        {
            var list = new List<Person>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_JobBusyPersonalByDate", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.Date;
                parameter.ParameterName = "@JobStartDate";
                parameter.Value = dateTime.ToShortDateString();
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                    Person person = new Person
                        (
                            new Guid(rdr.GetValue(0).ToString()),
                            rdr.GetValue(1).ToString(),
                            rdr.GetValue(2).ToString(),
                            rdr.GetValue(3).ToString(),
                            rdr.GetValue(4).ToString(),
                            Convert.ToBoolean(rdr.GetValue(5).ToString()),
                            true
                        );
                    // Add To list
                    list.Add(person);
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }

        /// <summary>
        /// Get All Persons
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<Person> GetAllPersonalList(SqlConnection connection)
        {
            var list = new List<Person>();

            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetAllPersons", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                //// will be passed to the stored procedure
                //SqlParameter parameter = new SqlParameter();
                //parameter.SqlDbType = SqlDbType.Date;
                //parameter.ParameterName = "@JobStartDate";
                //parameter.Value = dateTime.ToShortDateString();
                //cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                    Person person = new Person
                        (
                            new Guid(rdr.GetValue(0).ToString()),
                            rdr.GetValue(1).ToString(),
                            rdr.GetValue(2).ToString(),
                            rdr.GetValue(3).ToString(),
                            rdr.GetValue(4).ToString(),
                            false,
                            Convert.ToBoolean(rdr.GetValue(5).ToString())
                        );
                    // Add To list
                    list.Add(person);
                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list;

        }


        /// <summary>
        /// Get Specific person
        /// </summary>
        /// <param name="personId"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static Person GetPersonByPersonId(Guid personId, SqlConnection connection)
        {
            List<Person> list = new List<Person>();
            try
            {
                SqlDataReader rdr = null;
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_GetPersonByPersonID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                //// will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = "@PersonID";
                parameter.Value = personId;
                cmd.Parameters.Add(parameter);
                // execute the command
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                   Person person = new Person
                        (
                            new Guid(rdr.GetValue(0).ToString()),
                            rdr.GetValue(1).ToString(),
                            rdr.GetValue(2).ToString(),
                            rdr.GetValue(3).ToString(),
                            rdr.GetValue(4).ToString(),
                            true,
                            Convert.ToBoolean(rdr.GetValue(5).ToString())
                        );

                    list.Add(person);

                }

            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

            return list[0];

        }

        /// <summary>
        /// Add Persons to specific Job
        /// </summary>
        /// <param name="JobId"></param>
        /// <param name="PersonId"></param>
        /// <param name="connection"></param>
        public static void AddPersonToJob(Guid JobId, Guid PersonId, SqlConnection connection)
        {
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_JobAddPerson", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@JobID", JobId));
                cmd.Parameters.Add(new SqlParameter("@PersonID", PersonId));


                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }

        private static void SetParametersForJob(SqlCommand cmd, Job job)
        {
            cmd.Parameters.Add(new SqlParameter("@JobID", job.JobId));
            cmd.Parameters.Add(new SqlParameter("@JobName", job.JobName));
            cmd.Parameters.Add(new SqlParameter("@Active", true));
            cmd.Parameters.Add(new SqlParameter("@OrderID", job.OrderID));
            cmd.Parameters.Add(new SqlParameter("@JobStartDate", job.JobStartDate));
            cmd.Parameters.Add(new SqlParameter("@JobStartTime", job.JobStartTime));
            cmd.Parameters.Add(new SqlParameter("@JobEndDate", job.JobEndDate));
            cmd.Parameters.Add(new SqlParameter("@JobEndTime", job.JobEndTime));
            cmd.Parameters.Add(new SqlParameter("@RentStartDate", job.RentStartDate));
            cmd.Parameters.Add(new SqlParameter("@RentEndDate", job.RentEndDate));
            cmd.Parameters.Add(new SqlParameter("@ContactPerson", job.ContactName));
            cmd.Parameters.Add(new SqlParameter("@Address", job.Address));
            cmd.Parameters.Add(new SqlParameter("@Comments", job.Comments));
            cmd.Parameters.Add(new SqlParameter("@FullDay", job.FullDay));
            cmd.Parameters.Add(new SqlParameter("@NeedUpdate", job.NeedUpdate));
            cmd.Parameters.Add(new SqlParameter("@ParentJobID", job.ParentJobID));
            cmd.Parameters.Add(new SqlParameter("@ContactPhone", job.Phone));
            cmd.Parameters.Add(new SqlParameter("@Type", job.Type));
            cmd.Parameters.Add(new SqlParameter("@Month", job.Month));
        }

        public static void CleanJobBeforeAdd(Guid jobId, SqlConnection connection)
        {
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_CleanJobByJobID", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                //// will be passed to the stored procedure
                SqlParameter parameter = new SqlParameter();
                parameter.SqlDbType = SqlDbType.UniqueIdentifier;
                parameter.ParameterName = "@JobId";
                parameter.Value = jobId;
                cmd.Parameters.Add(parameter);

                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }	
        }


        /// <summary>
        /// Create a new person
        /// </summary>
        /// <param name="person"></param>
        /// <param name="connection"></param>
        public static void CreatePerson(Person person, SqlConnection connection)
        {
        
            try
            {
                connection.Open();
                // 1. create a command object identifying
                // the stored procedure
                SqlCommand cmd = new SqlCommand(
                    "JOB_CreatePerson", connection);

                // 2. set the command object so it knows
                // to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which
                // will be passed to the stored procedure
                SetParametersForPerson(cmd, person);


                // execute the command
                cmd.ExecuteReader();
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }

        }
       
    }
}
