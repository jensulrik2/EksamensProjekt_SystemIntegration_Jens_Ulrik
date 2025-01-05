using System;
using System.Messaging;
using Microsoft.MessageQueue.Messaging;
using Newtonsoft.Json;

namespace Eksamens_Projekt_SystemIntegration_Jens_Ulrik
{
    public class Place2book
    {
        private List<Activity> activities;
        private readonly string requestQueuePath = @".\Private$\RequestQueue";
        private readonly string replyQueuePath = @".\Private$\ReplyQueue";

        public Place2book(List<Activity> activities)
        {
            this.activities = activities;

            if (!MessageQueue.Exists(requestQueuePath))
            {
                MessageQueue.Create(requestQueuePath);
            }

            if (!MessageQueue.Exists(replyQueuePath))
            {
                MessageQueue.Create(replyQueuePath);
            }
        }

        public double GetActivityPrice(string membershipNumber, string activityName)
        {
            using (var requestQueue = new MessageQueue(requestQueuePath))
            using (var replyQueue = new MessageQueue(replyQueuePath))
            {
                var requestMessage = new
                {
                    messageType = "MedlemsskabTjekRequest",
                    payload = new { medlemsNummer = membershipNumber }
                };

                var requestMessageJson = JsonConvert.SerializeObject(requestMessage);
                var message = new Message
                {
                    Body = requestMessageJson,
                    ResponseQueue = replyQueue,
                    Formatter = new XmlMessageFormatter(new Type[] { typeof(string) })
                };

                requestQueue.Send(message);

                var replyMessage = replyQueue.Receive();
                replyMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                var replyMessageJson = replyMessage.Body.ToString();
                var response = JsonConvert.DeserializeObject<dynamic>(replyMessageJson);

                var activity = activities.Find(a => a.Name == activityName);

                if (activity == null)
                {
                    throw new ArgumentException("Activity not found");
                }

                if (response.payload.erMedlem)
                {
                    return activity.MedlemsPris;
                }
                else
                {
                    return activity.IkkeMedlemsPris;
                }
            }
        }
    }
}