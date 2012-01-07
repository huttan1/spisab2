using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.SessionState;
using System.Web.UI;

namespace ProfIT
{
    public class SiteUtilities
    {
        
         public static SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        /// <summary>
        /// Gets the current week start date.
        /// </summary>
        /// <returns>DateTime with current week start date</returns>
        public static DateTime GetCurrentWeekStartDate(int week)
        {
            DateTime result = DateTime.Now;

            if (result.DayOfWeek.ToString().ToLower().IndexOf("monday") != -1)
            {
            }
            else if (result.DayOfWeek.ToString().ToLower().IndexOf("tuesday") != -1)
                result = result.Subtract(new TimeSpan(1, 0, 0, 0));
            else if (result.DayOfWeek.ToString().ToLower().IndexOf("wednesday") != -1)
                result = result.Subtract(new TimeSpan(2, 0, 0, 0));
            else if (result.DayOfWeek.ToString().ToLower().IndexOf("thursday") != -1)
                result = result.Subtract(new TimeSpan(3, 0, 0, 0));
            else if (result.DayOfWeek.ToString().ToLower().IndexOf("friday") != -1)
                result = result.Subtract(new TimeSpan(4, 0, 0, 0));
            else if (result.DayOfWeek.ToString().ToLower().IndexOf("saturday") != -1)
                result = result.Subtract(new TimeSpan(5, 0, 0, 0));
            else if (result.DayOfWeek.ToString().ToLower().IndexOf("sunday") != -1)
                result = result.Subtract(new TimeSpan(6, 0, 0, 0));

            return DateTime.Parse(result.ToString("yyyy-MM-dd 00:00:00"));

        }

        public static DateTime GetFirstDayOfWeek(int year, int weekNumber)
        {
            return GetFirstDayOfWeek(year, weekNumber, new System.Globalization.CultureInfo("sv-SE"));
        }

        public static DateTime GetFirstDayOfWeek(int year, int weekNumber,
            System.Globalization.CultureInfo culture)
        {
            System.Globalization.Calendar calendar = culture.Calendar;
            DateTime firstOfYear = new DateTime(year, 1, 1, calendar);
            DateTime targetDay = calendar.AddWeeks(firstOfYear, weekNumber);
            DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

            while (targetDay.DayOfWeek != firstDayOfWeek)
            {
                targetDay = targetDay.AddDays(-1);
            }

            return targetDay;
        }

        public static int GetWeekNumber(DateTime dtPassed, int week)
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(dtPassed, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            int realdate = weekNum - week;

            weekNum = realdate + week;

            return weekNum;
        }

        public static List<Job> GetRentalOverDue(DateTime dateTime)
        {
            return DataLayer.GetJobsWithRentalOverDue(dateTime, sqlConnection);
        }

        public static List<Person> GetAvaliablePersonal(DateTime datetime)
        {
            var personlist = GetAllPersonal();

            try
            {
                var tmpPersons = GetAllPersonalByDate(datetime);
                for (int i = 0; i < personlist.Count; i++)
                {
                    for (int j = 0; j < tmpPersons.Count; j++)
                    {
                        if (tmpPersons[j].Fullday && tmpPersons[j].PersonId == personlist[i].PersonId)
                        {
                            personlist.Remove(personlist[i]);
                        }
                    }
                }
            }
            catch
            {
            }

            return personlist;
        }


        public static void UpdateOrCreateJob(Guid guid, List<Job> list, List<Person> persons, List<Car> cars)
        {
            List<Job> tmplist = new List<Job>();

            
            for (int i = 0; i < list.Count; i++)
            {
                tmplist = GetJobsByJobId(list[i].JobId);
                    if (tmplist.Count > 0)
                    {
                        // Create new
                        DataLayer.UpdateJob(list[i], sqlConnection);
                    }
                    else
                    {
                        // Update
                        DataLayer.CreateJob(list[i], sqlConnection);
                    }

                DataLayer.CleanJobBeforeAdd(list[i].JobId, sqlConnection);

                for (int j = 0; j < persons.Count; j++)
                {
                    DataLayer.AddPersonToJob(list[i].JobId, persons[j].PersonId, sqlConnection);
                }

                for (int j = 0; j < cars.Count; j++)
                {
                    DataLayer.AddCarToJob(list[i].JobId, cars[j].CarId, sqlConnection);
                }
            }
        }

        public static List<Job> GetJobsByDate(DateTime dateTime)
        {
            return DataLayer.GetJobsByDate(dateTime, sqlConnection);
        }

        public static List<Job> GetJobsByJobId(Guid guid)
        {
            return DataLayer.GetJobsByJobId(guid, sqlConnection);
        }

	    public static List<Person> GetAllPersonal()
	    {
	       return DataLayer.GetAllPersonalList(sqlConnection);
	    }

        public static List<Person> GetAllPersonalByDate(DateTime dateTime)
        {
            return DataLayer.GetBusyPersonalListForDate(dateTime, sqlConnection);
        }

		public static List<JobPersonList> GetJobPersonal(Guid guid)
		{
		    return DataLayer.GetPersonalListForJobByJobId(guid, sqlConnection);
		}

        public static Person GetPersonByID(Guid guid)
        {
            return DataLayer.GetPersonByPersonId(guid, sqlConnection);
        }

        public static List<Job> GetJobsByOrderId(string orderid)
        {
            return DataLayer.GetJobsByJobOrderId(orderid, sqlConnection);
        }

        public static List<Car> GetAllCars()
        {
            return DataLayer.GetAllCars(sqlConnection);
        }

        public static List<Car> GetCarFromJobID(Guid guid)
        {
            return DataLayer.GetCarsForJobByJobId(guid, sqlConnection);
        }

        public static List<Job> GetJobsByMonth(string month)
        {

            return DataLayer.GetJobsByMonth(month, sqlConnection);
           
        }

        public static Person GetPersonByName(string firstname, string lastname)
        {
            var personlist = DataLayer.GetAllPersonalList(sqlConnection);

            foreach (var person in personlist)
            {
                if (person.Firstname == firstname && person.Lastname == lastname)
                {
                    return person;
                }
            }
            return null;
            
        }

        public static void SetPersonInactive(Person person)
        {
            person.Active = false;
            DataLayer.UpdatePerson(person, sqlConnection);

        }

        public static Person AddPerson(PersonMamut personMamut)
        {

            var person = GetPersonByName(personMamut.FirstName, personMamut.LastName);

            if (person != null)
            {
                Person newperson = new Person(person.PersonId, personMamut.FirstName, personMamut.LastName, personMamut.Mobile, personMamut.Email, true, person.Active);

                DataLayer.UpdatePerson(newperson, sqlConnection);

                return newperson;
            }
            else
            {
                Person newperson = new Person(Guid.NewGuid(), personMamut.FirstName, personMamut.LastName, personMamut.Mobile, personMamut.Email, true, true);
                DataLayer.CreatePerson(newperson, sqlConnection);

                return newperson;
            }

        }

        public static void DeleteOldSession(List<Job> job, HttpSessionState session)
        {
            if (job[0] != null)
            {
                session.Remove("Job_" + job[0].JobId);
            }
        }

        public static void JobComplete(Guid guid, DateTime dateTime)
        {
            var joblist = GetJobsByJobId(guid);

            Job job = joblist[0];

            job.JobEndDate = dateTime;
            DataLayer.UpdateJob(job, sqlConnection);

            var cloneIDs = DataLayer.GetJobClonesByJobId(guid, sqlConnection);


            if (cloneIDs.Count > 0)
            {
                DataLayer.DeleteJobClones(job.JobId, sqlConnection);

                foreach (var cloneID in cloneIDs)
                {
                    DataLayer.CleanJobBeforeAdd(cloneID, sqlConnection);
                }
            }
            //Check so all jobs are cleared
            var cloneIDCheck = DataLayer.GetJobClonesByJobId(guid, sqlConnection);

            if (cloneIDCheck.Count > 0)
            {
                foreach (var iD in cloneIDCheck)
                {
                    DataLayer.CleanJobBeforeAdd(iD, sqlConnection);
                    
                }
            }

            job.Type = 20;
            job.JobId = Guid.NewGuid();
            job.JobStartDate = dateTime;
            job.JobEndDate = dateTime;
            job.RentEndDate = DateTime.MaxValue;
            job.RentStartDate = DateTime.MaxValue;
            job.PersonsInThisJob = string.Empty;
            job.Cars = string.Empty;


            DataLayer.CreateJob(job, sqlConnection);

        }
    }

	public class Person
	{
		public Guid PersonId { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public bool Fullday { get; set; }
        public bool Active { get; set; }

		public Person(Guid personid, string firstname, string lastname, string mobile, string email, bool fullday, bool active)
		{
            PersonId = personid;
			Firstname = firstname;
			Lastname = lastname;
		    Mobile = mobile;
		    Email = email;
		    Fullday = fullday;
		    Active = active;
		}
	}

    public class Car
    {
        public Guid CarId { get; set; }
        public Guid JobID { get; set; }
        public string Name { get; set; }
        public string Serial { get; set; }
        public Car(Guid carid, Guid jobid, string name, string serial)
        {
            CarId = carid;
            JobID = jobid;
            Name = name;
            Serial = serial;
        }
    }


	public class JobPersonList
	{
		public Guid PersonId { get; set; }
        public Guid JobID { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
        public bool Active { get; set; }

        public JobPersonList(Guid personid, Guid jobid, string firstname, string lastname, string mobile, string email, bool active)
		{
            PersonId = personid;
		    JobID = jobid;
			Firstname = firstname;
			Lastname = lastname;
            Mobile = mobile;
            Email = email;
            Active = active;

		}
	}

    public class JobList
    {
        public DateTime Date { get; set; }
        public List<Job> Jobs { get; set; }

        public JobList(DateTime date, List<Job> jobs)
        {
            Date = date;
            Jobs = jobs;
        }
    }


	public class Job
	{
		public Guid JobId { get; set; }
		public string JobName { get; set; }
        public bool Active { get; set; }
        public string OrderID { get; set; }
        public DateTime JobStartDate { get; set; }
		public DateTime JobEndDate { get; set; }
		public string ContactName { get; set; }
		public string Address { get; set; }
		public string Comments { get; set; }
        public DateTime RentStartDate { get; set; }
	    public DateTime RentEndDate { get; set; }
        public DateTime JobStartTime { get; set; }
        public DateTime JobEndTime { get; set; }
        public string PersonsInThisJob { get; set; }
        public bool FullDay { get; set; }
        public bool NeedUpdate { get; set; }
        public Guid ParentJobID { get; set; }
        public string Cars { get; set; }
        public string Phone { get; set; }
        public Byte Type { get; set; }
        public string Month { get; set; }
        public Job(Guid jobid, string jobname, bool active, string orderid, DateTime jobstartdate, DateTime jobenddate, string contactname, string address, string comments, DateTime rentstartdate, DateTime rentenddate, DateTime jobstarttime, DateTime jobendtime, string persons, bool fullday, bool needupdate, Guid parentJobId, string cars, string phone, byte type, string month)
		{
			JobId = jobid;
			JobName = jobname;
		    Active = active;
            OrderID = orderid;
			JobStartDate = jobstartdate;
			JobEndDate = jobenddate;
			ContactName = contactname;
			Address = address;
			Comments = comments;
		    RentStartDate = rentstartdate;
		    RentEndDate = rentenddate;
		    JobStartTime = jobstarttime;
		    JobEndTime = jobendtime;
		    PersonsInThisJob = persons;
		    FullDay = fullday;
		    NeedUpdate = needupdate;
		    ParentJobID = parentJobId;
		    Cars = cars;
		    Phone = phone;
		    Type = type;
		    Month = month;
		}
	}

    public class PersonMamut
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int ID { get; set; }


        public PersonMamut(string firstname, string lastname, string email, string mobile, int id)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            Mobile = mobile;
            ID = id;
        }
    }

    public class OrderMamut
    {
        public DateTime RegDate { get; set; }
        public DateTime DelivDate { get; set; }
        public int Status { get; set; }
        public string CompanyName { get; set; }
        public string OrderRef { get; set; }
        public string Address { get; set; }
        public int Project { get; set; }
        public int ID { get; set; }


        public OrderMamut(DateTime regdate, DateTime delivdate, int status, string companyname, string orderref, string address, int project, int id)
        {
            RegDate = regdate;
            DelivDate = delivdate;
            CompanyName = companyname;
            Status = status;
            OrderRef = orderref;
            Address = address;
            Project = project;
            ID = id;
        }
    }

    public class OrderMamutExist
    {
        public DateTime RegDate { get; set; }
        public DateTime DelivDate { get; set; }
        public int Status { get; set; }
        public string CompanyName { get; set; }
        public string OrderRef { get; set; }
        public string Address { get; set; }
        public int Project { get; set; }
        public int ID { get; set; }
        public Page Page { get; set; }


        public OrderMamutExist(DateTime regdate, DateTime delivdate, int status, string companyname, string orderref, string address, int project, int id, Page page)
        {
            RegDate = regdate;
            DelivDate = delivdate;
            Status = status;
            CompanyName = companyname;
            OrderRef = orderref;
            Address = address;
            Project = project;
            ID = id;
            Page = page;
        }
    }
    
}
