using System;
using System.Collections.Generic;

namespace Eksamens_Projekt_SystemIntegration_Jens_Ulrik
{
    class Program
    {
        static void Main(string[] args)
        {
            var memberships = new List<Medlem>
            {
                new Medlem { MedlemsNummer = 12345, Fornavn = "John", Efternavn = "Doe", AktivPassiv = true },
                new Medlem { MedlemsNummer = 67890, Fornavn = "Jane", Efternavn = "Smith", AktivPassiv = false }
            };

            var activities = new List<Activity> 
            {
                new Activity("Swimming", 50, 100),
                new Activity("Tennis", 30, 60),
                new Activity("Yoga", 20, 40)
            };

            var medlemsRegister = new MedlemsRegister(memberships);
            var place2book = new Place2book(activities);

            // Start listening for membership requests
            medlemsRegister.StartListening();

            // Simulate getting the price for an activity
            double price = place2book.GetActivityPrice("12345", "Swimming");
            Console.WriteLine($"The price for Swimming is: {price}");
        }
    }
}