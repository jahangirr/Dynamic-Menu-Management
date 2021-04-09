using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InteriorDesign.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PagedList;
using InteriorDesign.Repository;

using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.Web.Security;
using Newtonsoft.Json;

namespace InteriorDesign.Controllers
{
    public class RoleBasedSignaturesController : Controller
    {

        FileStoreCreateFolderName fileStoreCreateFolderName = new FileStoreCreateFolderName();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RoleBasedSignatures
        public ActionResult Index(string RoleName, int? page)
        {

           // RoleName
            ViewBag.RoleName = RoleName;
            var rbs = (from r in db.RoleBasedSignatures select r);

            if( string.IsNullOrEmpty( RoleName)== false)
            {
                rbs = rbs.Where(w => w.RoleName.Trim() == RoleName.Trim());
            }


            List<SelectListItem> selectionItemsList = new List<SelectListItem>();
            selectionItemsList.Add(new SelectListItem() { Text = "--Select-- ", Value = "100" });
            selectionItemsList.Add(new SelectListItem() { Text = "RoleName ", Value = "0" });
            ViewBag.selectionItems = new SelectList(selectionItemsList, "Value", "Text");

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(rbs.OrderBy(o => o.RoleName).ToPagedList(pageNumber, pageSize));
        }

        // GET: RoleBasedSignatures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleBasedSignature roleBasedSignature = db.RoleBasedSignatures.Find(id);
            if (roleBasedSignature == null)
            {
                return HttpNotFound();
            }
            return View(roleBasedSignature);
        }

        // GET: RoleBasedSignatures/Create
        public ActionResult Create()
        {
            List<SelectListItem> RoleList = new List<SelectListItem>();
            var Roles = db.Roles.Where(w => w.Name != "Admin").ToList().Distinct().OrderBy(o => o.Name);
            foreach (var role in Roles)
            {
                RoleList.Add(new SelectListItem() { Text = role.Name.ToString(), Value = role.Name.ToString() });
            }
            ViewBag.RoleData = new SelectList(RoleList, "Value", "Text");
            return View();
        }

        // POST: RoleBasedSignatures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RoleBasedSignature roleBasedSignature, FormCollection collection, HttpPostedFileBase image)
        {
            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canCreate = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("RoleBasedSignatures") && rm.RoleId == roleId select rm.CanCreate).FirstOrDefault();

            if (!canCreate)
            {
                return RedirectToAction("Index");
            }


            var roleBasedSignatueUpdate = db.RoleBasedSignatures.Where(w => w.RoleName == roleBasedSignature.RoleName).ToList();
            foreach(var rbsu in roleBasedSignatueUpdate)
            {
                rbsu.IsAcive = false;
                db.Entry(rbsu).State = EntityState.Modified;
                db.SaveChanges();
            }




            if (image != null)
            {
                //attach the uploaded image to the object before saving to Database
                roleBasedSignature.SignatureMimeType = image.ContentLength;
                roleBasedSignature.SignatureData = new byte[image.ContentLength];
                image.InputStream.Read(roleBasedSignature.SignatureData, 0, image.ContentLength);

                // To Get the Max Id
                int countrbs = db.RoleBasedSignatures.Count();
                int maxId = 0;
                if (countrbs > 0)
                {
                    maxId = db.RoleBasedSignatures.Max(m => m.Id) + 1;
                }
                else
                {
                    maxId = 1;
                }

                string FileWhole = image.FileName;
                string[] SplitFileWhole = FileWhole.Split('\\');

                int LastPartOfFile = SplitFileWhole.Count() - 1;
                //Save image to file
                var filename = maxId.ToString() + "_" + roleBasedSignature.RoleName + "_" + SplitFileWhole[LastPartOfFile];
                roleBasedSignature.FileAddressInfo = filename;

                var filePathOriginal = Server.MapPath("~/Content/Uploads/Originals");
                var filePathThumbnail = Server.MapPath("~/Content/Uploads/Thumbnails");
                string savedFileName = Path.Combine(filePathOriginal, filename);
                string UploadedfileExtension = Path.GetExtension(image.FileName);
                if (System.IO.File.Exists(savedFileName))
                {
                    System.IO.File.Delete(savedFileName);
                }
                image.SaveAs(savedFileName);

                // back up into external drive
                try
                {

                    var ExDrive = db.TestCallls.Select(s => s.value).FirstOrDefault();
                    string externalLocationAddress = ExDrive + fileStoreCreateFolderName.getFolderName();

                    bool exists = System.IO.Directory.Exists(externalLocationAddress);
                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(externalLocationAddress);
                    }
                    string externalFileName = Path.Combine(externalLocationAddress, filename);
                    bool fileExist = System.IO.File.Exists(externalFileName);
                    if (fileExist)
                    {
                        Random r = new Random();
                        // Later to be indetified by the sequence which got start in between 900000 to 900100
                        string ExistExternalFileName = Path.Combine(externalLocationAddress, r.Next(900000, 900100).ToString() + "_" + filename);
                        image.SaveAs(ExistExternalFileName);
                    }
                    else
                    {
                        string ExistExternalFileName = Path.Combine(externalLocationAddress, filename);
                        image.SaveAs(ExistExternalFileName);
                    }
                }
                catch (Exception ex)
                {

                }
                // back up into external drive


                roleBasedSignature.CreateTime = DateTime.Now;
                


                // Saving Longing Credential 
                LoginInfoModel logingInfoModel = new LoginInfoModel();
                PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                {
                    return RedirectToAction("Login", "Account");
                }
                logingInfoModel.UsedModel = "RoleBasedSignature";
                logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(roleBasedSignature);
                logingInfoModel.SysDate = DateTime.Now;
                db.LoginInfoModels.Add(logingInfoModel);
                db.SaveChanges();
                // Saving Longing Credential 
                //Save model object to database
                db.RoleBasedSignatures.Add(roleBasedSignature);
                db.SaveChanges();


                return RedirectToAction("Index");

            }
            return View(roleBasedSignature);
        }

        // GET: RoleBasedSignatures/Edit/5



        public FileContentResult GetDigitalSignature(int Id , int Width ,  int Height )
        {

            RoleBasedSignature roleBasedSignature = db.RoleBasedSignatures.Where(w => w.Id == Id).FirstOrDefault();
            //Read image back from file and create thumbnail from it
            var imageFile = Path.Combine(Server.MapPath("~/Content/Uploads/Originals"), roleBasedSignature.FileAddressInfo);

            try
            {


                using (var srcImage = Image.FromFile(imageFile))
                using (var newImage = new Bitmap(Width, Height))
                using (var graphics = Graphics.FromImage(newImage))
                using (var stream = new MemoryStream())
                {

                    if (roleBasedSignature != null && roleBasedSignature.SignatureData != null && roleBasedSignature.SignatureMimeType != 0)
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphics.DrawImage(srcImage, new Rectangle(0, 0, Width, Height));
                        newImage.Save(stream, ImageFormat.Png);
                        return File(stream.ToArray(), roleBasedSignature.SignatureMimeType.ToString());
                    }
                    else
                    {
                        return null;
                    }

                }
            }catch(Exception ex)
            {
                return null;
            }

          

            
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleBasedSignature roleBasedSignature = db.RoleBasedSignatures.Find(id);
            if (roleBasedSignature == null)
            {
                return HttpNotFound();
            }
            return View(roleBasedSignature);
        }

        // POST: RoleBasedSignatures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,RoleName,SignatureMimeType,SignatureData,FileAddressInfo,IsAcive,CreateTime")] RoleBasedSignature roleBasedSignature)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roleBasedSignature).State = EntityState.Modified;
              //  db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(roleBasedSignature);
        }

        // GET: RoleBasedSignatures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleBasedSignature roleBasedSignature = db.RoleBasedSignatures.Find(id);
            if (roleBasedSignature == null)
            {
                return HttpNotFound();
            }
            return View(roleBasedSignature);
        }

        // POST: RoleBasedSignatures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RoleBasedSignature roleBasedSignature = db.RoleBasedSignatures.Find(id);
            db.RoleBasedSignatures.Remove(roleBasedSignature);
           // db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
