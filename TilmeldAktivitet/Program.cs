using System;
using System.Messaging;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace TilmeldAktivitet
{

    class Program
    {
        public static void Main(string[] args)
        {
            string requestQueuePath = @"laptop-bl7hbsrp\private$\MedlemsIdRequest";
            string responseQueuePath = @"laptop-bl7hbsrp\private$\MedlemsIdResponse";

            // Create queues if they do not exist
            if (!MessageQueue.Exists(requestQueuePath))
                MessageQueue.Create(requestQueuePath);
            if (!MessageQueue.Exists(responseQueuePath))
                MessageQueue.Create(responseQueuePath);

            Console.WriteLine("Vælg system: (1) Medlemsregister, (2) Tilmelding til aktivitet");
            string valg = Console.ReadLine();
            
            //kør først 1, derefter tast et eller andet, kør derefter 2 (1 kører i baggrunden)
            if (valg == "1")
            {
                // Run Medlemsregister system
                Task.Run(() => KørMedlemsRegister(requestQueuePath, responseQueuePath));
            }
            else if (valg == "2")
            {
                // Run Tilmelding system
                Task.Run(() => KørTilmelding(requestQueuePath, responseQueuePath));
            }
            else
            {
                Console.WriteLine("Ugyldigt valg.");
            }

            // This keeps the main thread alive while the tasks are running
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        
        }

        // Medlemsregister-system
        static void KørMedlemsRegister(string requestQueuePath, string responseQueuePath)
        {
            Console.WriteLine("Medlemsregister kører...");

            // Hardcoded medlemsdata
            var medlemmer = new List<Medlem>
            {
                new Medlem { MedlemsNummer = 1, Navn = "Alice", ErAktivt = true },
                new Medlem { MedlemsNummer = 2, Navn = "Bob", ErAktivt = false }
            };

            using (MessageQueue requestQueue = new MessageQueue(requestQueuePath))
            using (MessageQueue responseQueue = new MessageQueue(responseQueuePath))
            {
                // MESSAGE CHANNEL: Use requestQueue to receive messages and responseQueue to send replies.
                requestQueue.Formatter = new XmlMessageFormatter(new[] { typeof(int) });

                while (true)
                {
                    Console.WriteLine("Waiting for activity requests...");

                    // MESSAGE ENDPOINT: Membership system receives messages from the queue.
                    var message = requestQueue.Receive();
                    int medlemsNummer = (int)message.Body;

                    // MESSAGE TRANSLATOR: Membership system evaluates membership status based on data.
                    var medlem = medlemmer.Find(m => m.MedlemsNummer == medlemsNummer);
                    bool erMedlem = medlem != null && medlem.ErAktivt;

                    // MESSAGE ENDPOINT: Reply is sent back via responseQueue.
                    responseQueue.Send(erMedlem);
                    Console.WriteLine(
                        $"Request received for Member ID {medlemsNummer}. Response: {(erMedlem ? "Member" : "Not a member")}");
                }
            }
        }

        // Place2Book-system
        static void KørTilmelding(string requestQueuePath, string responseQueuePath)
        {
            Console.WriteLine("TilmeldAktivitet is running...");

            // Hardcoded aktivitetsdata
            var aktiviteter = new List<Aktivitet>
            {
                new Aktivitet { Id = 1, Navn = "Yoga Class", MedlemsPris = 50.00m, IkkeMedlemsPris = 100.00m },
                new Aktivitet { Id = 2, Navn = "Cooking Workshop", MedlemsPris = 75.00m, IkkeMedlemsPris = 150.00m }
            };

            using (MessageQueue requestQueue = new MessageQueue(requestQueuePath))
            using (MessageQueue responseQueue = new MessageQueue(responseQueuePath))
            {
                // MESSAGE CHANNEL: Use requestQueue to send requests and responseQueue to receive replies.
                responseQueue.Formatter = new XmlMessageFormatter(new[] { typeof(bool) });

                foreach (var aktivitet in aktiviteter)
                {
                    Console.WriteLine($"Processing activity: {aktivitet.Navn}");

                    int medlemsNummerToCheck = 1; // Hardcoded membership number

                    // MESSAGE ENDPOINT: Place2Book sends messages to the queue.
                    requestQueue.Send(medlemsNummerToCheck);

                    // REQUEST-REPLY: Waiting for response from Membership system.
                    var responseMessage = responseQueue.Receive();
                    bool erMedlem = (bool)responseMessage.Body;
                    
                    decimal pris = erMedlem ? aktivitet.MedlemsPris : aktivitet.IkkeMedlemsPris;
                    Console.WriteLine(
                        $"Price for activity '{aktivitet.Navn}': {(erMedlem ? "Member price" : "Non-member price")} {pris:C}");
                }
            }
        }
    }
}
