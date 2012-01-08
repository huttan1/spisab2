using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using gbaapi;
using ProfIT;

namespace ASP.App_Code
{
    public class Mamut
    {
        public static List<Job> GetOrdersFromMamut(Exception exception, HttpRequest request)
        {
            string mamutID = "";
            int mamutStartValue = 0;
            int mamutMaxValue = 0;
            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(request.ApplicationPath);

            if (rootWebConfig1.AppSettings.Settings.Count > 0)
            {
                KeyValueConfigurationElement mamutIDSetting =
                    rootWebConfig1.AppSettings.Settings["MAMUT_ID"];
                KeyValueConfigurationElement mamutStartNumber =
                    rootWebConfig1.AppSettings.Settings["MAMUT_START_VALUE"];
                KeyValueConfigurationElement mamutmaxNumber =
                    rootWebConfig1.AppSettings.Settings["MAMUT_MAX_VALUE"];

                mamutID = mamutIDSetting.Value;
                mamutStartValue = Convert.ToInt32(mamutStartNumber.Value);
                mamutMaxValue = Convert.ToInt32(mamutmaxNumber.Value);

            }



            List<Job> jobs = new List<Job>();
            try
            {
                // Setup Connection to Mamut
                dynamic gba = Activator.CreateInstance(Type.GetTypeFromProgID("GBAAPI.Gba"));
                dynamic connectinfo = Activator.CreateInstance(Type.GetTypeFromProgID("GBAAPI.Connectinfo"));
                connectinfo.SetConnectBasicInfo(".", "mamut", 1, mamutID);
                gba.openConnection(connectinfo);
                dynamic Order = Activator.CreateInstance(Type.GetTypeFromProgID("GBAAPI.Orderheader"));
                dynamic Contact = Activator.CreateInstance(Type.GetTypeFromProgID("GBAAPI.Contact"));
                dynamic ContactPerson = Activator.CreateInstance(Type.GetTypeFromProgID("GBAAPI.Cperson"));
                Order.NewInit(gba);
                Contact.NewInit(gba);
                ContactPerson.NewInit(gba);
                
                if (Order.NewInit(gba) != null)
                {
                    for (int i = mamutStartValue; i < mamutMaxValue; i++)
                    {
                        int retval = (int) Order.Get(i);

                        if (retval == 1)
                        {
                            Order.Get(i);

                            gbaapi.Iorderheader orderheader = Order;
                            
                            if (orderheader.Status == 5200 && orderheader.Lorderpicked)
                            {

                                 MamutOrder mamutOrder =  GetDatesFromMamut(orderheader);

                                

                                DateTime jobstartdate = DateTime.Now;
                                DateTime jobenddate = DateTime.Now;
                                string orderref = "";
                                string address = "";
                                string companyname = "";
                                string phone = "";
                                if (mamutOrder.JobStartDate.ToString() != "")
                                {
                                    try
                                    {
                                        if (mamutOrder.JobStartDate > DateTime.MinValue)
                                            jobstartdate = mamutOrder.JobStartDate;
                                    }
                                    catch
                                    {

                                        jobstartdate = DateTime.MaxValue;
                                    }

                                }
                                if (mamutOrder.JobEndDate.ToString() != "")
                                {
                                    try
                                    {
                                        if (mamutOrder.JobEndDate > DateTime.MinValue)
                                            jobenddate = mamutOrder.JobEndDate;
                                    }
                                    catch
                                    {

                                        jobenddate = DateTime.MaxValue;
                                    }
                                }


                                if (mamutOrder.YourRef != null)
                                {

                                    try
                                    {
                                        orderref = mamutOrder.YourRef.Trim();
                                    }
                                    catch
                                    {

                                        orderref = "";
                                    }

                                }
                                if (mamutOrder.Address != null)
                                {
                                    address = mamutOrder.Address;
                                }
                                Contact.Get(Order.ContID);
                                Contact.Getcpers();
                                if (Contact.Contname != null)
                                {
                                    companyname = Contact.Contname.Trim();
                                }
                                ContactPerson.Get(Contact.Cpersid);

                                if (Contact.Phone1 != null)
                                {
                                    try
                                    {
                                        phone = ContactPerson.Phone1.Trim();
                                    }
                                    catch
                                    {
                                        phone = "";
                                    }
                                }


                                int id = Order.Orderid;
                                Job job = new Job(Guid.NewGuid(), companyname, true, id.ToString(), jobstartdate, jobenddate,
                                                  orderref, address, string.Empty, DateTime.MaxValue,
                                                  DateTime.MaxValue, DateTime.MaxValue, DateTime.MaxValue, string.Empty,
                                                  false, false, Guid.Empty, string.Empty, phone, 5, string.Empty);

                                Job j = AddOrderToList(job);

                                // If j == null job allready exist dont add it again
                                if (j != null)
                                {
                                    jobs.Add(j);
                                }
                            }

                        }
                    }


                }
            }
            catch
            {
                return null;
            }

            return jobs;
        }

        private static MamutOrder GetDatesFromMamut(Iorderheader orderheader)
        {
            List<MamutOrder> list = new List<MamutOrder>();

            SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionStringMamut"].ConnectionString);
            SqlDataSource sqlDataSource = new SqlDataSource();
            sqlDataSource.ConnectionString = sqlConnection.ConnectionString;

            sqlDataSource.SelectCommand = "SELECT [LINKID], [STATUSID] , [ORDERID], [CONTNAME], [ADRDELIV], [DATEDELIV], [DATEPROD], [REFYOUR], [LORDERPICKED]  FROM [G_ORDER] WHERE LINKID = '" + orderheader.Linkid + "'";
            sqlDataSource.DataBind();

            IEnumerable iteratorObject = sqlDataSource.Select
                (DataSourceSelectArguments.Empty);

            try
            {
                list.AddRange(from DataRowView record in iteratorObject
                              select new MamutOrder(
                                  Convert.ToInt32(record["LINKID"].ToString()), Convert.ToInt32(record["STATUSID"].ToString()), Convert.ToInt32(record["ORDERID"].ToString()), record["CONTNAME"].ToString(), record["ADRDELIV"].ToString(), record["DATEDELIV"].ToString(), record["DATEPROD"].ToString(), record["REFYOUR"].ToString(), Convert.ToBoolean(record["LORDERPICKED"].ToString())));
            }
            catch
            {
                return null;
            }

            
            return list[0];
        }

        public static List<Person> GetPersonsFromMamut(List<Person> persons, HttpRequest request)
        {
            string mamutID = "";
            int mamutStartValue = 0;
            int mamutMaxValue = 0;
            System.Configuration.Configuration rootWebConfig1 =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration(request.ApplicationPath);

            if (rootWebConfig1.AppSettings.Settings.Count > 0)
            {
                KeyValueConfigurationElement mamutIDSetting =
                    rootWebConfig1.AppSettings.Settings["MAMUT_ID"];
                KeyValueConfigurationElement mamutStartNumber =
                    rootWebConfig1.AppSettings.Settings["MAMUT_START_VALUE"];
                KeyValueConfigurationElement mamutmaxNumber =
                    rootWebConfig1.AppSettings.Settings["MAMUT_MAX_VALUE"];

                mamutID = mamutIDSetting.Value;
                mamutStartValue = Convert.ToInt32(mamutStartNumber.Value);
                mamutMaxValue = Convert.ToInt32(mamutmaxNumber.Value);

            }


            // Get All Employees from Mamut
            dynamic gba2 = Activator.CreateInstance(Type.GetTypeFromProgID("GBAAPI.Gba"));
            dynamic connectinfo2 = Activator.CreateInstance(Type.GetTypeFromProgID("GBAAPI.Connectinfo"));
            connectinfo2.SetConnectBasicInfo(".", "mamut", 1, mamutID);
            gba2.openConnection(connectinfo2);
            dynamic Employee = Activator.CreateInstance(Type.GetTypeFromProgID("GBAAPI.CEmployee"));
            Employee.NewInit(gba2);
            List<PersonMamut> emp = new List<PersonMamut>();

            if (Employee.NewInit(gba2) == true)
            {
                int id = 0;
                for (int j = mamutStartValue; j < mamutMaxValue; j++)
                {
                    try
                    {
                        Employee.Get(j);

                        Iemployee employee = Employee;


                        if (employee.Firstname.Trim() != "" && id != employee.Empid)
                        {

                            // Check if employee is "Anställd"
                            if (employee.Data15.ToString() == "3")
                            {
                                string firstname = employee.Firstname.Trim();
                                string lastname = employee.Surname.Trim();
                                string email = employee.Email1.Trim();
                                string mobile = employee.Mobilephone1.Trim();
                                id = employee.Empid;
                                PersonMamut personMamut = new PersonMamut(firstname, lastname, email, mobile, id);
                                emp.Add(personMamut);
                            }
                            // Check if employee is "Ingen" (Set person to inactive)
                            if (employee.Data15.ToString() == "1")
                            {
                                var person = SiteUtilities.GetPersonByName(employee.Firstname.Trim(), employee.Surname.Trim());

                                if (person != null)
                                {
                                    SiteUtilities.SetPersonInactive(person);
                                }
                            }
                        }
                        else
                        {
                            j = 100;
                        }
                    }
                    catch
                    {
                        mamutMaxValue = j;
                    }


                }

                persons = AddOrUpdatePerson(emp);
            }

            return persons;
        }

        private static List<Person> AddOrUpdatePerson(List<PersonMamut> emp)
        {
            List<Person> persons = new List<Person>();

            foreach (var personMamut in emp)
            {
                Person person = SiteUtilities.AddPerson(personMamut);
                
                persons.Add(person);
            }

            return persons;
        }


        public static Job AddOrderToList(Job job)
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

        public class MamutOrder
        {
            public int LinkId { get; set; }
            public int StatusId { get; set; }
            public int OrderId { get; set; }
            public string ContactName { get; set; }
            public string Address { get; set; }
            public DateTime JobEndDate { get; set; }
            public DateTime JobStartDate { get; set; }
            public string YourRef { get; set; }
            public bool OrderReady { get; set; }


            public MamutOrder(int linkid, int statusid, int orderid, string contactname, string address, string jobenddate, string jobstartdate, string yourref, bool orderready)
            {
                LinkId = linkid;
                StatusId = statusid;
                OrderId = orderid;
                ContactName = contactname;
                Address = address;
                JobEndDate = !string.IsNullOrEmpty(jobenddate) ? Convert.ToDateTime(jobenddate) : DateTime.MinValue;
                JobStartDate = !string.IsNullOrEmpty(jobstartdate) ? Convert.ToDateTime(jobstartdate) : DateTime.MinValue;
                YourRef = yourref;
                OrderReady = orderready;
        }
        }
    }
}