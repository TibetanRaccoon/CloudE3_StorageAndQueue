using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Diagnostics;

namespace StudentServiceData
{
    public class QueueHelper
    {
        private static CloudQueue queue;
        private static CloudStorageAccount account;

        public QueueHelper(string queueName)
        {
            account = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            CloudQueueClient queueClient = new CloudQueueClient(new Uri(account.QueueEndpoint.AbsoluteUri), account.Credentials);
            queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExists();
        }
        public void ResetQueue()
        {
            queue.Delete();
            queue.Create();
        }

        public void AddMessage(string msg)
        {
            var q = new CloudQueueMessage(msg);

            // TimeSpan parametar postavlja pri kreiranju poruku ucini nevidljivom 30 sekundi, ovo se trazilo u zadatku nije obavezno
            queue.AddMessage(new CloudQueueMessage(msg), null, new TimeSpan(0, 0, 30), null);
            Trace.WriteLine(msg);
        }

        public void RemoveMessage(CloudQueueMessage msg)
        {
            queue.DeleteMessage(msg, null, null);
        }

        public CloudQueueMessage GetMessage()
        {
            var msg = queue.GetMessage();

            if (msg == null)
            {
                Trace.WriteLine("There's no messages");
            }
            else
            {

                Trace.WriteLine($"Processing message {msg.AsString}.");
                // Brise poruku ako je procitana vise od 3 puta, koristiti samo ako se u trazi u zadatku
                if (msg.DequeueCount == 3)
                    RemoveMessage(msg);
            }

            return msg;
        }
    }
}
