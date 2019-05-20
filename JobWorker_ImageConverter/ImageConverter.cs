using StudentServiceData;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace JobWorker_ImageConverter
{
    public class ImageConverter
    {
        DataRepository dataRepository;
        BlobRepository blobRepository;
        QueueHelper queue;

        public ImageConverter(string queueName)
        {
            dataRepository = new DataRepository();
            blobRepository = new BlobRepository();
            queue = new QueueHelper(queueName);
        }

        public void DoImageConversion()
        {
            var msg = queue.GetMessage();
            if (msg != null)
                foreach (var item in dataRepository.RetrieveAllStudents())
                {
                    if (item.Index == msg.AsString)
                    {
                        Image newImage = new Bitmap(blobRepository.GetImage(item.RowKey), new Size(20, 20));

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            Bitmap bm = newImage as Bitmap;
                            bm.Save(memoryStream, ImageFormat.Bmp);
                            memoryStream.Position = 0;
                            blobRepository.InsertBlob(memoryStream, "image/bmp", item.RowKey);
                        }
                        break;
                    }
                }
        }
    }
}
