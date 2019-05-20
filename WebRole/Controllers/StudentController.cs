using StudentServiceData;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace WebRole.Controllers
{
    public class StudentController : Controller
    {
        DataRepository dataRepository = new DataRepository();
        BlobRepository blobRepository = new BlobRepository();
        QueueHelper queueHelper = new QueueHelper("queue");

        public ActionResult Index()
        {
            return View(dataRepository.RetrieveAllStudents());
        }
        public ActionResult Create()
        {
            return View("CreateStudent");
        }
        public ActionResult Delete(string RowKey)
        {
            dataRepository.Remove(RowKey);
            blobRepository.RemoveImage(RowKey);
            queueHelper.ResetQueue();
            return RedirectToAction("Index");
        }

        public ActionResult Details(string RowKey)
        {
            var s = dataRepository.GetStudent(RowKey);
            if (s != null)
                return View(s);
            else
                return RedirectToAction("Index");
        }

        public ActionResult ClearAllData()
        {
            dataRepository.ClearTable();
            blobRepository.ClearContainer();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddEntity(string Name, string LastName, string JMBG, string Index, HttpPostedFileBase file)
        {
            if (dataRepository.Exists(Index))
                return View("Exists");

            var student = new Student(Index)
            {
                Name = Name,
                LastName = LastName,
                JMBG = JMBG,
                Index = Index,
                ThumbnailUrl = blobRepository.InsertBlob(file.InputStream, file.ContentType, Index) // Insert into blob storage and return address
            };

            dataRepository.InsertOrRelace(student);
            queueHelper.AddMessage(Index);

            return RedirectToAction("Index");
        }
    }
}