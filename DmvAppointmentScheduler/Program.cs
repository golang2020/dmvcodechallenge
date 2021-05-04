using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DmvAppointmentScheduler
{
    class Program
    {
        public static Random random = new Random();
        public static List<Appointment> appointmentList = new List<Appointment>();
        static void Main(string[] args)
        {
            CustomerList customers = ReadCustomerData();
            TellerList tellers = ReadTellerData();
            Calculation(customers, tellers);
            OutputTotalLengthToConsole();

        }
        private static CustomerList ReadCustomerData()
        {
            string fileName = "CustomerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            CustomerList customerData = JsonConvert.DeserializeObject<CustomerList>(jsonString);
            return customerData;

        }
        private static TellerList ReadTellerData()
        {
            string fileName = "TellerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            TellerList tellerData = JsonConvert.DeserializeObject<TellerList>(jsonString);
            return tellerData;

        }
        static void Calculation(CustomerList customers, TellerList tellers)
        {
            // Your code goes here .....
            
            //New Design: 
            //1. Add new variable assignedTask in Teller.
            //2. Sort tellers by assignedTask in order.
            //3. Assign the customer to the first teller in the list.
            //4. Update the assigned task value of teller after new appointment is created.
            int numTeller = tellers.Teller.Count;

            foreach(Customer customer in customers.Customer)
            {
                //sort the teller list 
                tellers.Teller.Sort((x, y) => x.assignedTask.CompareTo(y.assignedTask));
                // and assign customer to the first teller in the list
                var appointment = new Appointment(customer, tellers.Teller[0]);
                tellers.Teller[0].assignedTask += appointment.duration;
                appointmentList.Add(appointment);

            }
            /*/verify result
            List<Teller> sortedTellers = tellers.Teller.OrderBy(x => x.assignedTask).ToList();
            Console.WriteLine("Teller 1" + sortedTellers[0].id + " will work for " + sortedTellers[0].assignedTask + " minutes!");
            Console.WriteLine("Teller 150" + sortedTellers[numTeller-1].id + " will work for " + sortedTellers[numTeller-1].assignedTask + " minutes!");
            */
        }
        static void OutputTotalLengthToConsole()
        {
            var tellerAppointments =
                from appointment in appointmentList
                group appointment by appointment.teller into tellerGroup
                select new
                {
                    teller = tellerGroup.Key,
                    totalDuration = tellerGroup.Sum(x => x.duration),
                };
            var max = tellerAppointments.OrderBy(i => i.totalDuration).LastOrDefault();
            Console.WriteLine("Teller " + max.teller.id + " will work for " + max.totalDuration + " minutes!");
        }

    }
}
