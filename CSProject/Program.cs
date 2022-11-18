using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSProject
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int year = 0, month = 0;

            while (year == 0)
            {
                Console.WriteLine("\nPlease enter the year: ");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("That was an invalid entry, please try again");
                    year = 0;
                }
            }

            while (month == 0)
            {
                Console.WriteLine("\nPlease enter the month: ");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if (month < 1 || month > 12)
                    {
                        month = 0;
                        Console.WriteLine("That was an invalid entry, please enter a month between 1 and 12");
                    }
                }
                catch (FormatException)
                {
                    Console.WriteLine("That was an invalid entry, please enter a month between 1 and 12");
                    month = 0;
                }
            }

            myStaff = fr.ReadFile();

            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.WriteLine("\nEnter hours worked for {0}", myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked = Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }
                catch
                {
                    Console.WriteLine("Invalid entry, please try again");
                    i--;
                }
            }

            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            ps.GenerateSummary(myStaff);

            Console.Read();


        }
    }
    class Staff
    {
        //Fields
        private float hourlyRate;
        private int hWorked;

        //Properties
        public float TotalPay { get; protected set; }
        public float BasicPay { get; private set; }
        public string NameOfStaff { get; private set; }
        public int HoursWorked
        {
            get
            {
                return hWorked;
            }
            set
            {
                if (value > 0)
                    hWorked = value;
                else
                    hWorked = 0;
            }
        }

        //Constructors
        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        //Methods
        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating Pay...");
            BasicPay = hourlyRate * hWorked;
            TotalPay = BasicPay;
        }

        public override string ToString()
        {
            return "\nName of Staff = " + NameOfStaff + "\nHourly Rate = " + hourlyRate + "\nHours Worked = " + hWorked + "\nTotal Pay = " + TotalPay + "\nBasic Pay = " + BasicPay;
        }
    }

    class Manager : Staff
    {
        //Fields
        private const float managerHourlyRate = 50;

        //Properties
        public int Allowance { get; private set; }

        //Constructors
        public Manager(string name) : base(name, managerHourlyRate) { }

        //Methods

        public override void CalculatePay()
        {
            base.CalculatePay();
            Allowance = 1000;
            if (HoursWorked > 160)
                TotalPay = BasicPay + Allowance;

        }

        public override string ToString()
        {
            return "\nName of Staff = " + NameOfStaff + "\nManager Hourly rate = " + managerHourlyRate + "\nHours Worked = " + HoursWorked + "\nTotal Pay = " + TotalPay + "\nBasic Pay = " + BasicPay + "\nAllowance = " + Allowance;
        }
    }

    class Admin : Staff
    {
        //Fields
        private const float overtimeRate = 15.5f;
        private const float adminHourlyRate = 30f;

        //Properties 
        public float Overtime { get; private set; }

        //Constructors 
        public Admin(string name) : base(name, adminHourlyRate) { }

        //Methods
        public override void CalculatePay()
        {
            base.CalculatePay();
            if (HoursWorked > 160)
                Overtime = overtimeRate * (HoursWorked - 160);
            TotalPay = BasicPay + Overtime;
        }
        public override string ToString()
        {
            return "\nName of Staff = " + NameOfStaff + "\nAdmin Hourly rate = " + adminHourlyRate + "\nHours Worked = " + HoursWorked + "\nBasic Pay = " + BasicPay + "\nOvertime = " + Overtime + "\nTotal Pay = " + TotalPay;
        }
    }

    class FileReader
    {
        //Fields

        //Properties 

        //Constructors 

        //Methods
        public List<Staff> ReadFile()
        {
            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] seperator = { ", " };

            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    while (!sr.EndOfStream)
                    {
                        result = sr.ReadLine().Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                        if (result[1] == "Manager")
                            myStaff.Add(new Manager(result[0]));
                        else if (result[1] == "Admin")
                            myStaff.Add(new Admin(result[0]));
                    }
                    sr.Close();
                }
            }
            else
            {
                Console.WriteLine("Error: File does not exist");
            }

            return myStaff;
        }
    }

    class PaySlip
    {
        //Fields
        private int month;
        private int year;

        //Enum
        enum MonthsOfYear
        {
            JAN = 1, FEB = 2, MAR = 3, APR = 4, MAY = 5, JUN = 6, JUL = 7, AUG = 8, SEP = 9, OCT = 10, NOV = 11, DEC = 12
        }

        //Properties 

        //Constructors 

        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }
        //Methods
        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string path;
            foreach (Staff f in myStaff)
            {
                path = f.NameOfStaff + ".txt";

                StreamWriter sw = new StreamWriter(path);
                sw.WriteLine("PAYSLIP FOR {0} {1}", (MonthsOfYear)month, year);
                sw.WriteLine("==========================");
                sw.WriteLine("Name of Staff: {0}", f.NameOfStaff);
                sw.WriteLine("Hours Worked: {0}", f.HoursWorked);
                sw.WriteLine("");
                sw.WriteLine("Basic Pay: {0:C}", f.BasicPay);
                if (f.GetType() == typeof(Manager))
                    sw.WriteLine("Allowance: {0:C}", ((Manager)f).Allowance);
                else if (f.GetType() == typeof(Admin))
                    sw.WriteLine("Overtime: {0:C}", ((Admin)f).Overtime);
                sw.WriteLine("");
                sw.WriteLine("==========================");
                sw.WriteLine("Total Pay: {0:C}", f.TotalPay);
                sw.WriteLine("==========================");
                sw.Close();
            }
        }

        public void GenerateSummary(List<Staff> myStaff)
        {
            var result =
                from f in myStaff
                where f.HoursWorked < 10
                orderby f.NameOfStaff ascending
                select new { f.NameOfStaff, f.HoursWorked };

            string path = "summary.txt";

            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("Staff with less than 10 working hours");
                sw.WriteLine("");
                foreach (var f in result)
                    sw.WriteLine("Name of Staff: {0}, Hours Worked: {1}", f.NameOfStaff, f.HoursWorked);
                sw.Close();
            }
        }

        public override string ToString()
        {
            return "month = " + month + "year = " + year;
        }

    }

}
