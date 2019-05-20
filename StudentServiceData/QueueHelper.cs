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
            queue.AddMessage(new CloudQueueMessage(msg));
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
                Trace.WriteLine($"Processing message {msg.AsString}.\n=================================================================");
                RemoveMessage(msg);
            }

            return msg;
        }
    }
}
