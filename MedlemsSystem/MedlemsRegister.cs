using System;
using System.Messaging;
using Newtonsoft.Json;
using Microsoft.MessageQueue.Messaging;

namespace Eksamens_Projekt_SystemIntegration_Jens_Ulrik
{
    public class MedlemsRegister
    {
        private List<Medlem> memberships;
        private readonly string requestQueuePath = @".\Private$\RequestQueue";

        public MedlemsRegister(List<Medlem> memberships)
        {
            this.memberships = memberships;

            if (!MessageQueue.Exists(requestQueuePath))
            {
                MessageQueue.Create(requestQueuePath);
            }
        }

        public void StartListening()
        {
            using (var requestQueue = new MessageQueue(requestQueuePath))
            {
                requestQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
                requestQueue.ReceiveCompleted += OnRequestReceived;
                requestQueue.BeginReceive();
            }
        }

        private void OnRequestReceived(object sender, ReceiveCompletedEventArgs e)
        {
            var requestQueue = (MessageQueue)sender;
            var requestMessage = requestQueue.EndReceive(e.AsyncResult);
            var requestMessageJson = requestMessage.Body.ToString();
            var request = JsonConvert.DeserializeObject<dynamic>(requestMessageJson);

            var membershipNumber = request.payload.medlemsNummer.ToString();
            var membership = CheckMembership(membershipNumber);

            var responseMessage = new
            {
                messageType = "MedlemsskabTjekResponse",
                payload = new
                {
                    medlemsNummer = membershipNumber,
                    erMedlem = membership != null
                }
            };

            var responseMessageJson = JsonConvert.SerializeObject(responseMessage);

            using (var replyQueue = requestMessage.ResponseQueue)
            {
                var message = new Message
                {
                    Body = responseMessageJson,
                    Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
                };

                replyQueue.Send(message);
            }

            requestQueue.BeginReceive();
        }

        public Medlem CheckMembership(string membershipNumber)
        {
            foreach (var membership in memberships)
            {
                if (membership.MedlemsNummer.ToString() == membershipNumber)
                {
                    return membership;
                }
            }
            return null;
        }
    }
}