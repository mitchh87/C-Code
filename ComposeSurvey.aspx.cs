using Dars.BLL.Online.Front.Comm;
using Dars.BLL.Online.Front.DAL;
using Dars.BLL.Online.Mail.Comm;
using Dars.BLL.Online.Mail.DAL;
using Dars.BLL.Online.Utils;
using Dars.Web.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Dars.Web.Online.Mail
{
    public partial class ComposeSurvey : ISISPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MailComm comm = new MailComm();
                FrontComm com = new FrontComm();
                List<RfrObject> lst = new List<RfrObject>();

                int Id_Classe_Matiere = 0;

                if (!string.IsNullOrEmpty(this.Request.QueryString["classematiere"]))
                    Id_Classe_Matiere = Convert.ToInt32(this.Request["classematiere"]);

                if (Id_Classe_Matiere == 0)
                {
                    divClasses.Visible = true;

                    foreach (EntSP_MailCoursesResult itm in comm.GetMailCourses(Convert.ToInt32(this.Request["mail"]), Convert.ToInt32(this.Session["Id_College"])))
                    {
                        hidClasses.Value = (hidClasses.Value == "" ? "" : hidClasses.Value + ",") + itm.Id_Class.ToString();
                    }
                }
                else
                    divClasses.Visible = false;

                FillClasseMatiere(Id_Classe_Matiere);

                if (String.IsNullOrEmpty(this.Request["mail"]))
                {
                    int Id_College = Convert.ToInt32(Session["Id_College"].ToString());
                    int Id_User = Convert.ToInt32(Session["Id_User"].ToString());
                    int Id_MailType = Convert.ToInt32(this.Request["type"]);

                    ExecResult res = comm.CreateMail(Id_College, Id_MailType, Id_User, false,
                        rdpDate.SelectedDate, DateTime.Now, "", "", "", true, true, true, false, false);

                    hidTread.Value = res.RetValue.ToString();
                }
                else
                {
                    int Id_College = Convert.ToInt32(Session["Id_College"]);
                    int Id_Mail = Convert.ToInt32(Request["mail"]);
                    int Id_MailType = Convert.ToInt32(this.Request["type"]);
                    int Id_User = Convert.ToInt32(Session["Id_User"]);

                    hidTread.Value = Request["mail"].ToString();

                    List<EntSP_MailByIDResult> mail = comm.GetMailByID(Id_College, Id_Mail, Id_MailType);

                    if (mail.Count > 0)
                    {
                        txtSubject.Text = mail[0].MailSubject;
                        ChkAllowReview.Checked = mail[0].AllowReview;
                        rdtEndTime.SelectedDate = mail[0].DueDate;
                        rdpDate.SelectedDate = mail[0].PublishDate;
                        txtBody.Content = mail[0].Content;
                    }
                }

                string targetFolder = MapPath("~/Uploads/" + this.Session["Id_College"]) + "/RadEditor/" + this.Session["Id_User"] + "/";
                bool exists = Directory.Exists(targetFolder);

                if (!exists)
                    Directory.CreateDirectory(targetFolder);

                string[] FolderPaths = new string[]
                {
                   "/Uploads/" + this.Session["Id_College"] +"/RadEditor/" + this.Session["Id_User"] + "/"
                };

                txtBody.DocumentManager.ViewPaths = FolderPaths;
                txtBody.DocumentManager.UploadPaths = FolderPaths;
                txtBody.DocumentManager.DeletePaths = FolderPaths;

                txtBody.ImageManager.ViewPaths = FolderPaths;
                txtBody.ImageManager.UploadPaths = FolderPaths;
                txtBody.ImageManager.DeletePaths = FolderPaths;
            }
        }

        protected void lstClasseMatiere_DataBound(object sender, EventArgs e)
        {
            FrontComm com = new FrontComm();
            MailComm comm = new MailComm();

            int Id_College = Convert.ToInt32(this.Session["Id_College"]);
            int Id_User = Convert.ToInt32(this.Session["Id_User"]);
            int Annee = Convert.ToInt32(this.Session["Annee"]);
            string uType = this.Request["utype"];

            List<EntSP_MailCoursesResult> tcm = comm.GetMailCourses(Convert.ToInt32(this.Request["mail"]), Id_College);
            List<EntSP_CourseSectionsResult> allSections = com.GetCourseSection(Id_College, 0, Id_User, uType, 0, Annee, 0, 1);
            List<RfrObject> courseSections = new List<RfrObject>();

            foreach (ListViewDataItem itm in lstClasseMatiere.Items)
            {
                int Id_Course = Convert.ToInt32((itm.FindControl("HidValue") as HiddenField).Value);

                // Clear sections on every course
                courseSections.Clear();
                courseSections.Add(new RfrObject("0", ""));

                if (allSections != null)
                {
                    foreach (EntSP_CourseSectionsResult section in allSections.Where(obj => obj.Id_Course == Id_Course).OrderBy(obj => obj.SectionRank))
                        courseSections.Add(new RfrObject(section.ID.ToString(), section.SectionName));
                }

                (itm.FindControl("cmbSections") as RadComboBox).DataSource = courseSections;
                (itm.FindControl("cmbSections") as RadComboBox).DataBind();

                if (!String.IsNullOrEmpty(this.Request["mail"]))
                {
                    if (tcm.Where(obj => obj.Id_Classe_Matiere == Id_Course).Count() > 0)
                        (itm.FindControl("chkSelected") as CheckBox).Checked = true;

                    foreach (RadComboBoxItem item in (itm.FindControl("cmbSections") as RadComboBox).Items)
                    {
                        int Id_Section = Convert.ToInt32(item.Value);

                        if (tcm.Where(obj => obj.Id_Section == Id_Section).Count() > 0)
                            (itm.FindControl("cmbSections") as RadComboBox).SelectedValue = Id_Section.ToString();
                    }
                }
                else
                {
                    string Id_Classe_Matiere = "0", Id_Section = "0";

                    if (!string.IsNullOrEmpty(this.Request.QueryString["classematiere"]))
                        Id_Classe_Matiere = this.Request["classematiere"];

                    if (!string.IsNullOrEmpty(this.Request.QueryString["section"]))
                        Id_Section = this.Request["section"];

                    if (Id_Course == Convert.ToInt32(Id_Classe_Matiere))
                        (itm.FindControl("chkSelected") as CheckBox).Checked = true;

                    foreach (RadComboBoxItem item in (itm.FindControl("cmbSections") as RadComboBox).Items)
                    {
                        if (item.Value == Id_Section)
                            (itm.FindControl("cmbSections") as RadComboBox).SelectedValue = Id_Section;
                    }
                }
            }
        }

        private void ShowMessage(string MsgType, string MsgText)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "starScript", "showNotif('" + MsgText + "','" + MsgType.ToLower() + "');", true);
        }

        protected void AsyncUpload1_FileUploaded(object sender, FileUploadedEventArgs e)
        {
            MailComm Comm = new MailComm();

            string FileName = Regex.Replace(e.File.GetName(), "[^\\w\\._]", "_", RegexOptions.Compiled);

            if (FileName.Length > 100)
                FileName = FileName.Substring(0, 100) + e.File.GetExtension();

            string fileCategory = "";
            string fileIconURL = "file.png";
            string fileType = e.File.GetExtension().Replace(".", "");

            List<Ent_FileTypeIcon> lst = Comm.GetFileTypeIcon(fileType);

            if (lst.Count > 0)
            {
                fileIconURL = lst[0].IconURL;
                fileCategory = lst[0].FileCategory;
            }
            else
                fileCategory = "other";

            // hash and size are calculated based on the original file not the compress one
            int Id_College = Convert.ToInt32(Session["Id_College"]);
            string hash = "";
            decimal size = e.File.ContentLength / 1024;

            using (var md5 = MD5.Create())
            {
                using (var stream = e.File.InputStream)
                {
                    hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                }
            }

            string DBFilePath = Comm.GetFilePathByHashAndSize(Id_College, hash, size);
            string FolderUploaded = "", FolderUploadedToCompress = "";

            //the file is uploaded for the first time
            if (DBFilePath == "")
            {
                Guid g = Guid.NewGuid();
                string targetFolder = "/Uploads/" + this.Session["Id_College"].ToString() + "/mails/" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/";

                bool exists = Directory.Exists(MapPath(targetFolder));

                if (!exists)
                    Directory.CreateDirectory(MapPath(targetFolder));

                if (fileCategory == "image")
                {
                    if (fileType == "png" || fileType == "gif" || fileType == "jpg" || fileType == "jpeg")
                    {
                        FolderUploadedToCompress = targetFolder + g.ToString() + "_original-file" + e.File.GetExtension();
                        FolderUploaded = targetFolder + g.ToString() + e.File.GetExtension();
                    }
                    else // we need to convert the file to jpg so we can scale it
                    {
                        FolderUploadedToCompress = targetFolder + g.ToString() + e.File.GetExtension();
                        FolderUploaded = targetFolder + g.ToString() + e.File.GetExtension();
                    }
                }
                else
                {
                    FolderUploadedToCompress = targetFolder + g.ToString() + e.File.GetExtension();
                    FolderUploaded = targetFolder + g.ToString() + e.File.GetExtension();
                }
            }
            else
            {
                FolderUploaded = DBFilePath;
                FolderUploadedToCompress = DBFilePath;
            }

            ExecResult res = Comm.InsertEnt_MailFiles(Id_College, Convert.ToInt32(hidTread.Value), Convert.ToInt32(hidTread.Value),
                FileName, e.File.GetExtension().Replace(".", ""), fileCategory, fileIconURL, FolderUploaded, hash, size);

            if (DBFilePath == "")
            {
                if (res.RetValue != -1)
                {
                    if (FolderUploaded == FolderUploadedToCompress)
                        e.File.SaveAs(Server.MapPath(FolderUploaded));
                    else
                    {
                        e.File.SaveAs(Server.MapPath(FolderUploadedToCompress));
                        ResizeImage.CompressImage(Server.MapPath(FolderUploadedToCompress), Server.MapPath(FolderUploaded));
                        File.Delete(Server.MapPath(FolderUploadedToCompress));
                    }
                }
                else
                {
                    ShowMessage("error", GetGlobalResourceObject("MessagesPage", "ErrorFileUploaded").ToString());
                }
            }
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            ExecResult res = new ExecResult();
            MailComm comm = new MailComm();

            int Id_College = Convert.ToInt32(Session["Id_College"]);
            int Id_User = Convert.ToInt32(Session["Id_User"].ToString());
            int Id_Mail = Convert.ToInt32(hidTread.Value);
            int Annee = Convert.ToInt32(this.Session["Annee"]);
            string Lang = this.Session["APPLANG"].ToString();

            if (rdtEndTime.SelectedDate == null)
            {
                ShowMessage("error", Lang == "EN" ? "Please select an end time" : Lang == "FR" ? "Veuillez sélectionner une heure de fin" : "يرجى تحديد وقت الانتهاء");
                return;
            }
            else if (rdtEndTime.SelectedDate < rdpDate.SelectedDate)
            {
                ShowMessage("error", Lang == "EN" ? "End time should be after start time" : Lang == "FR" ? "Date fin doit être après la date début" : "وقت البداية يجب ان يسبق وقت الانتهاء ");
                return;
            }

            int nbSelectedCourses = 0;
            string selectedCourses = "";

            foreach (ListViewDataItem itm in lstClasseMatiere.Items)
            {
                if ((itm.FindControl("chkSelected") as CheckBox).Checked)
                {
                    string idCourse = (itm.FindControl("HidValue") as HiddenField).Value;
                    string idSection = (itm.FindControl("cmbSections") as RadComboBox).SelectedValue;

                    nbSelectedCourses += 1;
                    selectedCourses = selectedCourses == "" ? (idCourse + "|" + idSection) : selectedCourses + "," + (idCourse + "|" + idSection);
                }
            }

            List<EntSP_MailCoursesResult> courses = comm.GetMailCourses(Id_Mail, Id_College);

            List<string> selected = selectedCourses.Split(',').ToList();

            foreach (EntSP_MailCoursesResult course in courses)
            {
                if (selected.Where(obj => obj == course.Id_Classe_Matiere.ToString() + "|" + course.Id_Section.ToString()).ToList().Count == 0)
                    comm.DeleteMailClasses(course.ID, course.Id_Course.Value, Convert.ToInt32(Session["Id_College"]), "survey");
            }

            foreach (string s in selectedCourses.Split((",").ToCharArray()[0]))
            {
                if (s != "")
                {
                    string[] strArgs = new string[2];
                    strArgs = s.Split('|');

                    int idCourse = Convert.ToInt32(strArgs[0]);
                    int idSection = Convert.ToInt32(strArgs[1]);

                    if (courses.Where(obj => obj.Id_Classe_Matiere == idCourse && obj.Id_Section == idSection).ToList().Count == 0)
                        comm.AddMailCoursesClasseCourse(Id_Mail, Convert.ToInt32(this.Request["type"].ToString()),
                            Id_College, idCourse, idSection, Convert.ToInt32(Session["Id_User"]));
                }
            }

            bool IsEdited = String.IsNullOrEmpty(Request["mail"]);

            DateTime publishDate, dueDate;

            if (rdpDate.SelectedDate == null)
                publishDate = DateTime.Now;
            else
                publishDate = rdpDate.SelectedDate.Value;

            if (rdtEndTime.SelectedDate == null)
            {
                if (rdpDate.SelectedDate == null)
                    dueDate = DateTime.Now;
                else
                    dueDate = rdpDate.SelectedDate.Value;
            }
            else
                dueDate = rdtEndTime.SelectedDate.Value;

            comm.UpdateMail(ChkAllowReview.Checked, publishDate, dueDate, txtSubject.Text,
                txtBody.Content, txtBody.Text, true, "#000000", false, false, IsEdited,
                Convert.ToInt32(hidTread.Value), Convert.ToInt32(Session["Id_College"]));

            string Id_Classe_Matiere = "0", Id_Section = "0";

            if (!string.IsNullOrEmpty(this.Request.QueryString["classematiere"]))
                Id_Classe_Matiere = this.Request["classematiere"];

            if (!string.IsNullOrEmpty(this.Request.QueryString["section"]))
                Id_Section = this.Request["section"];

            Response.Redirect(String.Format("~/Online/mail/ComposeSurveyQuestions.aspx?utype={0}&mail={1}&classe={2}&type={3}&from={4}&classematiere={5}&section={6}",
                Request["utype"], hidTread.Value, Request["classe"], Request["type"], Request["from"], Id_Classe_Matiere, Id_Section));
        }

        protected void lnkBack_Click(object sender, EventArgs e)
        {
            string Id_Classe_Matiere = "0";

            if (!string.IsNullOrEmpty(this.Request.QueryString["classematiere"]))
                Id_Classe_Matiere = this.Request["classematiere"];

            if (this.Request["from"] == "timeline")
                Response.Redirect(String.Format("~/Online/Timeline/profDashboard.aspx?utype={0}&classematiere={1}", Request["utype"], Id_Classe_Matiere));
            else
            {
                if (Request["mail"] != null)
                    Response.Redirect(String.Format("~/Online/mail/SurveyPreview.aspx?utype={0}&mail={1}&type={2}&from={3}",
                        Request["utype"], Request["mail"], Request["type"], Request["from"]));
                else
                    Response.Redirect("~/Online/mail/mailbox.aspx?utype=" + Request["utype"]);
            }
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            int Id_Classe_Matiere = 0;

            if (!string.IsNullOrEmpty(this.Request.QueryString["classematiere"]))
                Id_Classe_Matiere = Convert.ToInt32(this.Request["classematiere"]);

            FillClasseMatiere(Id_Classe_Matiere);
        }

        private void FillClasseMatiere(int Id_Classe_Matiere)
        {
            FrontComm com = new FrontComm();
            List<RfrObject> lst = new List<RfrObject>();

            if (Id_Classe_Matiere == 0)
            {
                foreach (EntSP_ClasseMatiereUser_ByClassLstResult itm in com.Get_ListClasseMatiereByClassLst(Convert.ToInt32(this.Session["Id_College"]),
                    Convert.ToInt32(this.Session["Annee"]), Convert.ToInt32(this.Session["Id_User"]), this.Request["utype"], hidClasses.Value))
                    lst.Add(new RfrObject(itm.Id_Classe_Matiere.ToString(), itm.ClasseMatiere));
            }
            else
            {
                List<EntSP_ClasseMatiereUser_OtherSectionResult> coursesLst = new List<EntSP_ClasseMatiereUser_OtherSectionResult>();

                if (this.Request["utype"] == "prof")
                {
                    coursesLst = com.GetClasseMatiereUsr_OtherSection(Convert.ToInt32(this.Session["Id_College"]),
                        Convert.ToInt32(this.Session["Annee"]), Id_Classe_Matiere, Convert.ToInt32(this.Session["Id_User"]), "").ToList();
                }
                else
                    coursesLst = com.GetClasseMatiereUsr_OtherSection(Convert.ToInt32(this.Session["Id_College"]),
                        Convert.ToInt32(this.Session["Annee"]), Id_Classe_Matiere, 0, "").ToList();

                foreach (EntSP_ClasseMatiereUser_OtherSectionResult itm in coursesLst)
                    lst.Add(new RfrObject(itm.ID.ToString(), itm.Matiere));
            }

            lstClasseMatiere.DataSource = lst;
            lstClasseMatiere.DataBind();
        }

        [WebMethod]
        public static List<Isc_GetClasseListeUtilisateursResult> Get_ListeClasse(int idUser, int idCollege, int annee)
        {
            FrontComm com = new FrontComm();
            return com.Get_ListeClasseByDivision(idUser, idCollege, annee, 0, "--", "0").ToList();
        }
    }
}