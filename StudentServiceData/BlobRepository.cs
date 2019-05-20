using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace StudentServiceData
{
    public class BlobRepository
    {
        public CloudStorageAccount storageAccount;
        public CloudBlobClient blobStorage;
        public CloudBlobContainer container;

        public BlobRepository()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            blobStorage = storageAccount.CreateCloudBlobClient();
            container = blobStorage.GetContainerReference("pic");
            container.CreateIfNotExists();

            // Ovaj deo koda je neophodan da bi se slike prikazele u browser-u u vs2017 , 
            //=======================================================
            var perm = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            container.SetPermissions(perm);
            //=======================================================
        }

        public void ClearContainer()
        {
            container.Delete();
            container.Create();
        }
        public string InsertBlob(Stream stream, string ContentType, string RowKey)
        {
            var key = $"image_{RowKey}";
            var blob = container.GetBlockBlobReference(key);
            blob.Properties.ContentType = ContentType;
            try
            {
                blob.UploadFromStream(stream);
            }
            catch (System.Exception e)
            {
                Trace.WriteLine(e);
            }

            return blob.Uri.ToString();
        }

        public void RemoveImage(string RowKey)
        {
            var key = $"image_{RowKey}";
            var blob = container.GetBlockBlobReference(key);
            try
            {
                blob.Delete();
            }
            catch (System.Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        public Image GetImage(string RowKey)
        {
            var key = $"image_{RowKey}";
            var blob = container.GetBlockBlobReference(key);
            var ms = new MemoryStream();
            try
            {
                blob.DownloadToStream(ms);
            }
            catch (System.Exception e)
            {
                Trace.WriteLine(e);
            }

            return new Bitmap(ms);
        }
    }
}
