using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ProfIT
{
    public class MamutV2
    {

        public static List<Job> GetOrdersFromMamut(Exception exception, HttpRequest request)
        {
            List<Job> jobs = new List<Job>();

            var jobsFromMamut = GetJobsFromMamut();

            try
            {
                foreach (var order in jobsFromMamut)
                {
                    Job job = new Job(Guid.NewGuid(), order.ContactName.Trim(), true, order.OrderId.ToString(), order.JobStartDate, order.JobEndDate, order.YourRef.Trim(), order.Address.Trim(), string.Empty, DateTime.MaxValue, DateTime.MaxValue, DateTime.MaxValue, DateTime.MaxValue, string.Empty, false, false, Guid.Empty, string.Empty, order.Phone.Trim(), 5, string.Empty);

                    Job j = CheckIfJobExist(job);

                    // If j == null job allready exist dont add it again
                    if (j != null)
                    {
                        jobs.Add(j);
                    }
                }

                return jobs;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public static List<Person> GetEmployeesFromMamut(Exception exception, HttpRequest request)
        {
            try
            {
                var employeesFromMamut = GetEmployeesFromMamut();
                
                var persons = AddOrUpdatePerson(employeesFromMamut);

                return persons;

            }
            catch (Exception)
            {

                return null;
            }
        }

        private static List<Person> AddOrUpdatePerson(IEnumerable<MamutEmployee> emp)
        {
            List<Person> persons = new List<Person>();

            foreach (var personMamut in emp)
            {
                Person person = SiteUtilities.AddPerson(personMamut);

                persons.Add(person);
            }

            return persons;
        }

        private static IEnumerable<MamutOrder> GetJobsFromMamut()
        {
            List<MamutOrder> list = new List<MamutOrder>();

            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringMamut"].ConnectionString);
            SqlDataSource sqlDataSource = new SqlDataSource();
            sqlDataSource.ConnectionString = sqlConnection.ConnectionString;

            sqlDataSource.SelectCommand = "SELECT G_ORDER.STATUSID, G_ORDER.ORDERID, G_ORDER.CONTNAME, G_ORDER.ADRDELIV, G_ORDER.DATEDELIV, G_ORDER.DATEPROD, G_ORDER.REFYOUR, G_ORDER.LORDERPICKED, G_ORDER.CUSTID, G_CONTAC.PHONE1 FROM G_ORDER INNER JOIN G_CONTAC ON G_ORDER.CUSTID = G_CONTAC.CUSTID WHERE (G_ORDER.LORDERPICKED = 'true') AND (G_ORDER.STATUSID = '5200')";
            sqlDataSource.DataBind();

            IEnumerable iteratorObject = sqlDataSource.Select
                (DataSourceSelectArguments.Empty);

            try
            {
                list.AddRange(from DataRowView record in iteratorObject
                              select new MamutOrder(Convert.ToInt32(record.Row.ItemArray[0]), Convert.ToInt32(record.Row.ItemArray[1]), record.Row.ItemArray[2].ToString(), record.Row.ItemArray[3].ToString(), record.Row.ItemArray[4].ToString(), record.Row.ItemArray[5].ToString(), record.Row.ItemArray[6].ToString(), Convert.ToBoolean(record.Row.ItemArray[7]), record.Row.ItemArray[9].ToString()));

                //list.AddRange(from DataRowView record in iteratorObject
                //              select new MamutOrder(Convert.ToInt32(record.Row.ItemArray["G_ORDER.STATUSID"].ToString()), Convert.ToInt32(record["G_ORDER.ORDERID"].ToString()), record["G_ORDER.CONTNAME"].ToString(), record["G_ORDER.ADRDELIV"].ToString(), record["G_ORDER.DATEDELIV"].ToString(), record["G_ORDER.DATEPROD"].ToString(), record["G_ORDER.REFYOUR"].ToString(), Convert.ToBoolean(record["G_ORDER.LORDERPICKED"].ToString()), (record["G_CONTAC.PHONE1"].ToString())));
            }
            catch
            {
                return null;
            }


            return list;
        }

        private static IEnumerable<MamutEmployee> GetEmployeesFromMamut()
        {
            List<MamutEmployee> list = new List<MamutEmployee>();

            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringMamut"].ConnectionString);
            SqlDataSource sqlDataSource = new SqlDataSource();
            sqlDataSource.ConnectionString = sqlConnection.ConnectionString;

            sqlDataSource.SelectCommand = "SELECT EMPID, FIRSTNAME, LASTNAME, PHONE_WORK, DATA15 FROM G_EMP WHERE (DATA15 = '3')";
            sqlDataSource.DataBind();

            IEnumerable iteratorObject = sqlDataSource.Select
                (DataSourceSelectArguments.Empty);

            try
            {
                list.AddRange(from DataRowView record in iteratorObject
                              select
                                  new MamutEmployee(record.Row.ItemArray[1].ToString(),
                                                    record.Row.ItemArray[2].ToString(),
                                                    Convert.ToInt32(record.Row.ItemArray[0].ToString()),
                                                    record.Row.ItemArray[3].ToString(),
                                                    Convert.ToInt32(record.Row.ItemArray[4].ToString())));

                //list.AddRange(from DataRowView record in iteratorObject
                //              select new MamutOrder(Convert.ToInt32(record.Row.ItemArray["G_ORDER.STATUSID"].ToString()), Convert.ToInt32(record["G_ORDER.ORDERID"].ToString()), record["G_ORDER.CONTNAME"].ToString(), record["G_ORDER.ADRDELIV"].ToString(), record["G_ORDER.DATEDELIV"].ToString(), record["G_ORDER.DATEPROD"].ToString(), record["G_ORDER.REFYOUR"].ToString(), Convert.ToBoolean(record["G_ORDER.LORDERPICKED"].ToString()), (record["G_CONTAC.PHONE1"].ToString())));
            }
            catch
            {
                return null;
            }


            return list;
        }

        public static Job CheckIfJobExist(Job job)
        {
            if (job.OrderID != null)
            {
                var tmplist = SiteUtilities.GetJobsByOrderId(job.OrderID);

                if (tmplist.Count == 0)
                {
                    return job;
                }
            }
            return null;
        }
        public class MamutOrder : IEnumerable
        {
            //public int LinkId { get; set; }
            public int StatusId { get; set; }
            public int OrderId { get; set; }
            public string ContactName { get; set; }
            public string Address { get; set; }
            public DateTime JobEndDate { get; set; }
            public DateTime JobStartDate { get; set; }
            public string YourRef { get; set; }
            public string Phone { get; set; }
            public bool OrderReady { get; set; }


            public MamutOrder(int statusid, int orderid, string contactname, string address, string jobenddate, string jobstartdate, string yourref, bool orderready, string phone)
            {
                //LinkId = linkid;
                StatusId = statusid;
                OrderId = orderid;
                ContactName = contactname;
                Address = address;
                JobEndDate = !string.IsNullOrEmpty(jobenddate) ? Convert.ToDateTime(jobenddate) : DateTime.MinValue;
                JobStartDate = !string.IsNullOrEmpty(jobstartdate) ? Convert.ToDateTime(jobstartdate) : DateTime.MinValue;
                YourRef = yourref;
                OrderReady = orderready;
                Phone = phone;
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public class MamutEmployee : IEnumerable
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int EMPID { get; set; }
            public string Phone { get; set; }
            public int DATA15 { get; set; }

            public MamutEmployee(string firstname, string lastname, int empid, string phone, int data15)
            {
                FirstName = firstname;
                LastName = lastname;
                EMPID = empid;
                Phone = phone;
                DATA15 = data15;
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
