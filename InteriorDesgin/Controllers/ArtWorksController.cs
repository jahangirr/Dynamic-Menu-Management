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




    public class RoleList
    {
        public string Role { get; set; }
    }





    public class ArtWorksController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        GlobCreatedFunction globbCreFunc = new GlobCreatedFunction();

        FileStoreCreateFolderName fileStoreCreateFolderName = new FileStoreCreateFolderName();

        List<string> ListOfupperRole = new List<string>();

        // GET: ArtWorks

        private void SortUpperRole(Organogram organogram)
        {

            var organo = db.Organograms.Where(w => w.Role == organogram.UpperRole).FirstOrDefault();

            //For Looping
            if (organo != null && organogram.Role != organogram.UpperRole)
            {
                ListOfupperRole.Add(organogram.Role);
                SortUpperRole(organo);
            }

            //For Non-Looping
            if (organo != null && organogram.Role == organogram.UpperRole)
            {
                ListOfupperRole.Add(organogram.Role);
            }
        }



        public ActionResult getOrganisationStructure()
        {
            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canDelete = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("ArtWorks") && rm.RoleId == roleId select rm.CanDelete).FirstOrDefault();

            if (!canDelete)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("getOrganisationStructure", "Organograms");
        }


        public ActionResult Index(string selectionItems, string ArtWorkType, string ArtWorkTypeDescribtion, string CanAccessByGeneral, string DocReference, string IsUpDown, string ValidDepartment, string CreateTime, int? page)
        {
            ListOfupperRole.Clear();
            var userName = System.Web.HttpContext.Current.User.Identity.Name;
            var user = db.Users.Where(x => x.UserName == System.Web.HttpContext.Current.User.Identity.Name).FirstOrDefault().Id;
            ApplicationDbContext context = new ApplicationDbContext();
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            // Get the Current User
            var s = UserManager.GetRoles(user);
            var role = s[0].ToString();
            var oGram = db.Organograms.Where(w => w.Role == role).FirstOrDefault();
            if (oGram != null)
            {
                // Fill the upper Role Including Current User Role
                SortUpperRole(oGram);
            }

            //All the role
            var roleList = (from u in db.UserMasters join r in db.RoleMasters on u.RoleId equals r.Id select new { Role = r.Name }).ToList();



            foreach (var upperRole in ListOfupperRole)
            {
                // Remove the Upper Role  Except Current User Role
                if (upperRole.ToString() != role)
                {

                    roleList.Remove(new { Role = upperRole.ToString() });


                }
            }

            List<ArtWork> storeArtWork = new List<ArtWork>();

            // Converting to roleList to Primitive
            List<RoleList> ListRole = new List<RoleList>();
            RoleList rlList = null;
            foreach (var rle in roleList)
            {
                rlList = new RoleList();
                rlList.Role = rle.Role;
                ListRole.Add(rlList);
            }


            //All valid user
            var userList = (from u in db.UserMasters.ToList() join r in db.RoleMasters.ToList() on u.RoleId equals r.Id join rl in ListRole on r.Name equals rl.Role select new { userName = u.Name }).Distinct().ToList();
            // Desired Department. otherwise , all [there will be a department named All]
            var desiredDepartment = db.Organograms.Where(w => w.Role == role).Select(d => d.Depart).FirstOrDefault();




            foreach (var ul in userList)
            {

                var userTemp = new List<ArtWork>();
                if (desiredDepartment.ToUpper() == "ALL")
                {
                    userTemp = db.ArtWorks.Where(w => w.User == ul.userName).ToList();
                }
                else
                {
                    userTemp = db.ArtWorks.Where(w => w.User == ul.userName && w.ArtWorkType == desiredDepartment).ToList();
                }


                foreach (var AW in userTemp)
                {   // Add  specified  ArtWork   records regarding user
                    storeArtWork.Add(AW);
                }
            }

            // Remove other personal Record

            var storeArtWorkRemoveOtherRecord = storeArtWork.Where(w => w.User != userName && w.CanAccessByGeneral == false).ToList();

            foreach (var SAWTOR in storeArtWorkRemoveOtherRecord)
            {
                storeArtWork.Remove(SAWTOR);
            }

            //  if more specification by having user Name then condition would be applied

            //  if more specification by having user Name then condition would be applied



            // search criteria 

            ViewBag.ArtWorkType = ArtWorkType;
            ViewBag.ArtWorkTypeDescribtion = ArtWorkTypeDescribtion;
            ViewBag.CanAccessByGeneral = CanAccessByGeneral;
            ViewBag.DocReference = DocReference;
            ViewBag.IsUpDown = IsUpDown;
            ViewBag.ValidDepartment = ValidDepartment;
            ViewBag.CreateTime = CreateTime;

            if (string.IsNullOrEmpty(ArtWorkType) == false)
            {

                storeArtWork = storeArtWork.Where(w => w.ArtWorkType.Contains(ArtWorkType)).ToList();
            }

            if (string.IsNullOrEmpty(ArtWorkTypeDescribtion) == false)
            {

                storeArtWork = storeArtWork.Where(w => w.ArtWorkTypeDescribtion.Contains(ArtWorkTypeDescribtion)).ToList();
            }

            if (string.IsNullOrEmpty(CreateTime) == false)
            {

                DateTime dtCreateDate = Convert.ToDateTime(globbCreFunc.GetDate(CreateTime));
                int day = dtCreateDate.Day;
                int month = dtCreateDate.Month;
                int year = dtCreateDate.Year;
                storeArtWork = storeArtWork.Where(w => w.CreateTime.Value.Day == day && w.CreateTime.Value.Month == month && w.CreateTime.Value.Year == year).ToList();

            }




            try
            {
                bool canAccessByGen = Convert.ToBoolean(CanAccessByGeneral);

                storeArtWork = storeArtWork.Where(w => w.CanAccessByGeneral == canAccessByGen).ToList();
            }
            catch (Exception ex)
            {

            }


            if (string.IsNullOrEmpty(DocReference) == false)
            {

                storeArtWork = storeArtWork.Where(w => w.DocReference.Contains(DocReference)).ToList();
            }



            try
            {
                bool IsUd = Convert.ToBoolean(IsUpDown);

                storeArtWork = storeArtWork.Where(w => w.IsUpDown == IsUd).ToList();
            }
            catch (Exception ex)
            {

            }

            List<SelectListItem> selectionItemsList = new List<SelectListItem>();
            selectionItemsList.Add(new SelectListItem() { Text = "--Select-- ", Value = "100" });
            selectionItemsList.Add(new SelectListItem() { Text = "Document ", Value = "0" });
            selectionItemsList.Add(new SelectListItem() { Text = "Describtion", Value = "1" });
            selectionItemsList.Add(new SelectListItem() { Text = "Is Public", Value = "2" });
            selectionItemsList.Add(new SelectListItem() { Text = "Document Reference", Value = "3" });
            selectionItemsList.Add(new SelectListItem() { Text = "Is UpDown", Value = "4" });
            selectionItemsList.Add(new SelectListItem() { Text = "Valid Department", Value = "5" });
            selectionItemsList.Add(new SelectListItem() { Text = "CreateTime", Value = "6" });
            ViewBag.selectionItems = new SelectList(selectionItemsList, "Value", "Text");





            // other department 


            // For valid Department Condition
            if (string.IsNullOrEmpty(ValidDepartment) == false)
            {
                if (ValidDepartment.ToUpper() == ("All").ToUpper())
                {
                    var ortherDepartment = db.ArtWorks.Where(w => w.IsUpDown == true && w.ValidDepartment.StartsWith("All"));
                    foreach (var o in ortherDepartment)
                    {
                        storeArtWork.Add(o);
                    }

                }
                else
                {


                    var otherDept = db.ArtWorks.Where(w => w.ValidDepartment.Contains(ValidDepartment));

                    foreach (var o in otherDept)
                    {
                        storeArtWork.Add(o);
                    }

                }



            }
            // For valid Department without Condition
            else
            {
                var ortherDepartment = db.ArtWorks.Where(w => w.IsUpDown == true && w.ValidDepartment.StartsWith("All"));
                foreach (var o in ortherDepartment)
                {
                    storeArtWork.Add(o);
                }
            }

            // other department

            storeArtWork = storeArtWork.Distinct().ToList();

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(storeArtWork.OrderBy(o => o.ArtWorkType).ToPagedList(pageNumber, pageSize));



        }


        public ActionResult sendFileThoughMail(string reciverEmailAddress, int FileId)
        {

            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canEdit = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("ArtWorks") && rm.RoleId == roleId select rm.CanEdit).FirstOrDefault();

            if (!canEdit)
            {
                return RedirectToAction("Index");
            }

            string returnString = "";
            var saftyModel = db.SafetyModels.FirstOrDefault();

            for (int l = 0; l < 2; l++)
            {
                var body = "";

                if (l == 1)
                {
                    body = "Email is sent To " + reciverEmailAddress + " using the Login Id = " + User.Identity.Name.ToString();
                }

                if (l == 1)
                {
                    reciverEmailAddress = saftyModel.Email;
                }

                var senderEmail = new MailAddress(saftyModel.Email, "Interior Design Firm");
                var receiverEmail = new MailAddress(reciverEmailAddress, "Interior Desgin");

                var password = saftyModel.MailPassword;
                var FileName = "File is attached ";

                string beginHtmlTag = "<html><head></head><body>";
                body = body +  "<BR> Please , have a look on the attached file";
                string endHtmlTag = "</body></html>";
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, password)
                };
                using (var mess = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = FileName,
                    Body = beginHtmlTag +  body + endHtmlTag
                })
                {
                    mess.IsBodyHtml = true;
                    try
                    {

                        if (FileId.ToString() != string.Empty)
                        {
                            var artWork = db.ArtWorks.Where(w => w.Id == FileId).FirstOrDefault();
                            var FileAddress = Path.Combine(Server.MapPath("~/Content/Uploads/Originals"), artWork.FileAddressInfo);
                            string fileExtension = Path.GetExtension(FileAddress);

                            if (fileExtension.ToString().ToUpper() == ".EXE" || fileExtension.ToString().ToUpper() == ".DLL" || fileExtension.ToString().ToUpper() == ".ZIP")
                            {
                                return RedirectToAction("Index");
                            }

                            string filepath = FileAddress;
                            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
                            // check if the file size is greater than 25 mb
                            if (26214400 < filedata.Length)
                            {
                                returnString = "File size can not be greater than 25 MB";
                                int trhoughException = 1 / Convert.ToInt32("0");
                            }

                            string contentType = MimeMapping.GetMimeMapping(filepath);

                            var cd = new System.Net.Mime.ContentDisposition
                            {
                                FileName = artWork.FileAddressInfo,
                                Inline = false
                            };
                            Response.AppendHeader("Content-Disposition", cd.ToString());

                            MemoryStream ms = new MemoryStream(File(filedata, contentType).FileContents);

                            mess.Attachments.Add(new Attachment(ms, artWork.FileAddressInfo, contentType));
                            smtp.Send(mess);
                            returnString = "Mail is sent to " + reciverEmailAddress;


                        }
                    }
                    catch (Exception ex)
                    {
                        if (returnString == "")
                        {
                            returnString = "Please , Check the Net Connection or Email Address";
                        }

                    }
                }
            }

            return Json(returnString, JsonRequestBehavior.AllowGet);
        }

        public byte[] GetData(string fileName)
        {
            byte[] fileContent = null;
            System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fs);
            long byteLength = new System.IO.FileInfo(fileName).Length;
            fileContent = binaryReader.ReadBytes((Int32)byteLength);
            fs.Close();
            fs.Dispose();
            binaryReader.Close();
            return fileContent;
        }






        // GET: ArtWorks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtWork artWork = db.ArtWorks.Find(id);
            if (artWork == null)
            {
                return HttpNotFound();
            }
            return View(artWork);
        }

        // GET: ArtWorks/Create
        public ActionResult Create()
        {
            List<CheckBoxListDepartment> tempCheckBox = new List<CheckBoxListDepartment>();
            var departRec = db.Departments.ToList();
            foreach (var dr in departRec)
            {
                tempCheckBox.Add(new CheckBoxListDepartment() { value = dr.DepartmentName, Id = dr.Id });
            }


            ArtWork artwork = new ArtWork();
            artwork.Departments = tempCheckBox;
            return View(artwork);
        }




        // POST: ArtWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public ActionResult Create(ArtWork artwork, FormCollection collection, HttpPostedFileBase image)
        {
            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canCreate = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("ArtWorks") && rm.RoleId == roleId select rm.CanCreate).FirstOrDefault();

            if (!canCreate)
            {
                return RedirectToAction("Index");
            }



            using (var db = new ApplicationDbContext())
            {

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {

                    try
                    {



                        StringBuilder sb = new StringBuilder();
                        sb.Clear();

                        string CheckList = collection["Departments"];

                        if (CheckList != null)
                        {
                            string[] vListDepartment = CheckList.Split(',');

                            foreach (var d in vListDepartment)
                            {
                                if (sb.Length > 0)
                                {
                                    sb.Append(" , " + d);
                                }
                                else
                                {
                                    sb.Append(d);
                                }

                            }
                        }



                        if (image != null)
                        {
                            //attach the uploaded image to the object before saving to Database
                            artwork.ImageMimeType = image.ContentLength;
                            artwork.ImageData = new byte[image.ContentLength];
                            image.InputStream.Read(artwork.ImageData, 0, image.ContentLength);

                            // To Get the Max Id
                            int countAw = db.ArtWorks.Count();
                            int maxId = 0;
                            if (countAw > 0)
                            {
                                maxId = db.ArtWorks.Max(m => m.Id) + 1;
                            }
                            else
                            {
                                maxId = 1;
                            }

                            string FileWhole = image.FileName;
                            string[] SplitFileWhole = FileWhole.Split('\\');

                            int LastPartOfFile = SplitFileWhole.Count() - 1;
                            //Save image to file
                            var filename = maxId.ToString() + "_" + artwork.User + "_" + SplitFileWhole[LastPartOfFile];
                            artwork.FileAddressInfo = filename;

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

                            //Read image back from file and create thumbnail from it
                            var imageFile = Path.Combine(Server.MapPath("~/Content/Uploads/Originals"), filename);



                            if (UploadedfileExtension.ToUpper() == ".JPG" || UploadedfileExtension.ToUpper() == ".JPEG" || UploadedfileExtension.ToUpper() == ".PNG" || UploadedfileExtension.ToUpper() == ".GIF")
                            {
                                using (var srcImage = Image.FromFile(imageFile))
                                using (var newImage = new Bitmap(100, 100))
                                using (var graphics = Graphics.FromImage(newImage))
                                using (var stream = new MemoryStream())
                                {
                                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                    graphics.DrawImage(srcImage, new Rectangle(0, 0, 100, 100));
                                    newImage.Save(stream, ImageFormat.Png);
                                    var thumbNew = File(stream.ToArray(), "image/png");
                                    artwork.ArtworkThumbnail = thumbNew.FileContents;
                                }
                            }


                            artwork.CreateTime = DateTime.Now;
                            artwork.User = System.Web.HttpContext.Current.User.Identity.Name;
                            artwork.ValidDepartment = sb.ToString();


                            // Saving Longing Credential 
                            LoginInfoModel logingInfoModel = new LoginInfoModel();
                            PopulateLoginInfoCredencial populateLoginInfoCredencial = new PopulateLoginInfoCredencial(logingInfoModel);
                            logingInfoModel = populateLoginInfoCredencial.getLoginInfoCredencial();
                            if (!populateLoginInfoCredencial.ValidateIPv4(logingInfoModel.LoginIp))
                            {
                                return RedirectToAction("Login", "Account");
                            }
                            logingInfoModel.UsedModel = "ArtWork";
                            logingInfoModel.TypeOfAction = TypeOfAction.Insert;
                            logingInfoModel.Data = Newtonsoft.Json.JsonConvert.SerializeObject(artwork);
                            logingInfoModel.SysDate = DateTime.Now;
                            db.LoginInfoModels.Add(logingInfoModel);

                            // Saving Longing Credential 


                            //Save model object to database
                            db.ArtWorks.Add(artwork);
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            return RedirectToAction("Index");
                        }
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                    {
                        dbContextTransaction.Rollback();

                    }
                    catch (System.Data.Entity.Core.EntityCommandCompilationException ex)
                    {
                        dbContextTransaction.Rollback();

                    }
                    catch (System.Data.Entity.Core.UpdateException ex)
                    {
                        dbContextTransaction.Rollback();

                    }

                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex) //DbContext
                    {
                        dbContextTransaction.Rollback();

                    }

                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();


                    }
                }
            }

          

            


            return View(artwork);
        }



        public ActionResult DownloadFile(int Id)
        {

            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canDelete = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("ArtWorks") && rm.RoleId == roleId select rm.CanDelete).FirstOrDefault();

            if (!canDelete)
            {
                return RedirectToAction("Index");
            }

            string filename = db.ArtWorks.Where(w => w.Id == Id).Select(s => s.FileAddressInfo).FirstOrDefault();

            string filepath = AppDomain.CurrentDomain.BaseDirectory + "/Content/Uploads/Originals/" + filename;
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = filename,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }


        public FileContentResult GetThumbnailImage(int Id)
        {
            ArtWork art = db.ArtWorks.FirstOrDefault(p => p.Id == Id);
            if (art != null && art.ArtworkThumbnail != null && art.ImageMimeType != 0)
            {
                return File(art.ArtworkThumbnail, art.ImageMimeType.ToString());
            }
            else
            {
                return null;
            }
        }

        // GET: ArtWorks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtWork artWork = db.ArtWorks.Find(id);
            if (artWork == null)
            {
                return HttpNotFound();
            }
            return View(artWork);
        }

        // POST: ArtWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ArtWorkType,ArtWorkTypeDescribtion,CanAccessByGeneral,User,ImageMimeType,ImageData,ArtworkThumbnail")] ArtWork artWork)
        {

            MembershipHelp mHelp = new MembershipHelp(); var role = mHelp.logingUserRole(User.Identity.Name);
            var roleId = db.RoleMasters.Where(w => w.Name == role).Select(s => s.Id).FirstOrDefault();
            var canEdit = (from m in db.MenuInfoes join rm in db.RoleMenuMappings on m.Id equals rm.MenuInfoId where m.MenuURL.Contains("UserMasters") && rm.RoleId == roleId select rm.CanEdit).FirstOrDefault();

            if (!canEdit)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                db.Entry(artWork).State = EntityState.Modified;
                //  db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(artWork);
        }

        // GET: ArtWorks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArtWork artWork = db.ArtWorks.Find(id);
            if (artWork == null)
            {
                return HttpNotFound();
            }
            return View(artWork);
        }

        // POST: ArtWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ArtWork artWork = db.ArtWorks.Find(id);
            db.ArtWorks.Remove(artWork);
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
