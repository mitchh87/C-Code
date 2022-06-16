using Dars.BLL.Online.Mail.Comm;
using Dars.BLL.Online.Mail.DAL;
using Dars.BLL.Online.Mail.Model;
using Dars.BLL.Online.Questions.Comm;
using Dars.BLL.Online.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Dars.Web.Online.Mail
{
    public partial class ComposeSurveyQuestions : ISISPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MailComm comm = new MailComm();
                RefreshRepeater(comm);
            }
        }

        public void RefreshRepeater(MailComm comm)
        {
            QuestionUtils utl = new QuestionUtils();

            int isDone = 0;

            List<EntSP_MailRepliesQuestionsResult> lstQuestion = comm.GetMailQuestionsReplies(Convert.ToInt32(this.Session["Id_College"]),
                Convert.ToInt32(this.Request["mail"]), Convert.ToInt32(this.Session["Id_User"]), Convert.ToInt32(this.Session["Id_User"]));

            for (int i = 0; i < lstQuestion.Count; i++)
            {
                if (i == 0)
                    isDone = lstQuestion[i].isDone.Value;

                lstQuestion[i].ContentHTML = utl.ConvertToPreview(lstQuestion[i].QuestionTypeCode,
                    lstQuestion[i].QuestionContent, lstQuestion[i].QuestionID.ToString(), Server.MapPath("~"));
            }

            Repeater1.DataSource = lstQuestion;
            Repeater1.DataBind();

            lnkAdd.Visible = isDone == 0;
            lnkAdd.NavigateUrl = string.Format("/Online/Mail/AddQuestion.aspx?mode=ins&utype={0}&mail={1}&classe={2}&type={3}&from={4}&classematiere={5}&section={6}",
                Request["utype"], Request["mail"], Request["classe"], Request["type"], Request["from"], Request["classematiere"], Request["section"]);
        }

        protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "up")
            {
                MailComm comm = new MailComm();

                int Id_College = Convert.ToInt32(this.Session["Id_College"]);
                int mailId = Convert.ToInt32(Convert.ToInt32(Request["mail"]));
                int qRank = Convert.ToInt32(e.CommandArgument.ToString());

                comm.MailQuestionsSwap(Id_College, mailId, qRank, qRank - 1);
                RefreshRepeater(comm);

            }
            else if (e.CommandName.ToLower() == "down")
            {
                MailComm comm = new MailComm();

                string[] strArgs = new string[2];
                strArgs = e.CommandArgument.ToString().Split('|');

                int Id_College = Convert.ToInt32(this.Session["Id_College"]);
                int mailId = Convert.ToInt32(Convert.ToInt32(Request["mail"]));
                int qRank = Convert.ToInt32(strArgs[0]);
                int maxRank = Convert.ToInt32(strArgs[1]);

                if (qRank < maxRank)
                {
                    comm.MailQuestionsSwap(Id_College, mailId, qRank, qRank + 1);
                    RefreshRepeater(comm);
                }
            }

            else if(e.CommandName.ToLower() == "deletequestion")
            {
                MailComm comm = new MailComm();

                ExecResult res = comm.DeleteEnt_MailQuestions(Convert.ToInt32(this.Session["Id_College"]),
                    Convert.ToInt32(this.Request["mail"]), Guid.Parse(e.CommandArgument.ToString()));

                if (res.RetValue == 0)
                {
                    ShowMessage("success", GetGlobalResourceObject("MessagesPage", "Deleted").ToString());
                    RefreshRepeater(comm);
                }
                else
                    ShowMessage("error", GetGlobalResourceObject("MessagesPage", "CouldNotDelete").ToString());
            }
        }

        protected void lnkBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("~/Online/mail/ComposeSurvey.aspx?utype={0}&mail={1}&classe={2}&type={3}&from={4}&classematiere={5}&section={6}",
                Request["utype"], Request["mail"], Request["classe"], Request["type"], Request["from"], Request["classematiere"], Request["section"]));
        }

        protected void ContinueButton_Click(object sender, EventArgs e)
        {
            if (Repeater1.Items.Count == 0)
            {
                string ErrorMsg = "Please add at least one question";

                if (this.Session["APPLANG"].ToString() == "FR")
                    ErrorMsg = "Veuillez ajouter au moins une question";
                else if (this.Session["APPLANG"].ToString() == "AR")
                    ErrorMsg = "الرجاء إضافة سؤال واحد على الأقل";

                ShowMessage("error", ErrorMsg);
            }
            else
            {
                string Id_Classe_Matiere = "0", Id_Section = "0";

                if (!string.IsNullOrEmpty(this.Request.QueryString["classematiere"]))
                    Id_Classe_Matiere = this.Request["classematiere"];

                if (!string.IsNullOrEmpty(this.Request.QueryString["section"]))
                    Id_Section = this.Request["section"];

                Response.Redirect(String.Format("~/Online/mail/MailSurveyUsers.aspx?utype={0}&mail={1}&classe={2}&type={3}&from={4}&classematiere={5}&section={6}",
                    Request["utype"], Request["mail"], Request["classe"], Request["type"], Request["from"], Id_Classe_Matiere, Id_Section));
            }
        }

        private void ShowMessage(string MsgType, string MsgText)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "starScript", "showNotif('" + MsgText + "','" + MsgType.ToLower() + "');", true);
        }
    }
}