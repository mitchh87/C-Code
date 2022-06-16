using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Dars.BLL.Online.Front.Comm;
using Dars.BLL.Online.Front.DAL;
using System.Collections.Generic;
using Telerik.Web.UI;
using Dars.BLL.Online.Utils;
using Dars.BLL.Online.Admin.Comm;
using System.Web.Services;
using System.Drawing;

namespace Dars.Web.Online.Eval
{
    public partial class evalDetailCtrl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request["mode"] == "ins")
                {
                    frmEvaluation.ChangeMode(FormViewMode.Insert);
                    BarChart.Visible = false;
                }
                else
                {
                    frmEvaluation.ChangeMode(FormViewMode.Edit);
                    tblToolBar.Visible = false;
                    BarChart.Visible = true;
                }                
            }
        }

        private void SaveEval()
        {
            if (Request["mode"] == "ins")
            {
                FrontComm comm = new FrontComm();

                int Id_College = Convert.ToInt32(Session["Id_College"].ToString());
                int Id_Classe_Matiere = Convert.ToInt32((this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue);
                int Id_Type = Convert.ToInt32((this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).SelectedValue);
                int Id_User = Convert.ToInt32(Session["Id_User"].ToString());
                decimal NoteSur = Convert.ToDecimal((this.frmEvaluation.FindControl("TxtNoteSur") as TextBox).Text);

                decimal Coeff = 1;
                try
                {
                    if ((this.frmEvaluation.FindControl("TxtCoeff") as TextBox).Text != "")
                        Coeff = Convert.ToDecimal((this.frmEvaluation.FindControl("TxtCoeff") as TextBox).Text);
                }
                catch (Exception)
                {
                    Coeff = 1;
                }

                int NumColonne = Convert.ToInt32((this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).SelectedValue);
                string Remarques = (this.frmEvaluation.FindControl("RemarquesTextBox") as TextBox).Text;
                int SessID = Convert.ToInt32(Session["Id_Session"].ToString());
                DateTime DateEval = (this.frmEvaluation.FindControl("rdpDateEval") as RadDateTimePicker).SelectedDate.Value;
                bool IsQuiz = (this.frmEvaluation.FindControl("chkIsQuiz") as CheckBox).Checked;
                string ExamLocation = (this.frmEvaluation.FindControl("txtExamLocation") as TextBox).Text;
                bool ShuffleQuestions = (this.frmEvaluation.FindControl("chkShuffleQuestions") as CheckBox).Checked;
                bool AllowReview = (this.frmEvaluation.FindControl("chkAllowReview") as CheckBox).Checked;
                int Id_Section = Convert.ToInt32((this.frmEvaluation.FindControl("cmbCourseSections") as DropDownList).SelectedValue);

                int? NbQuestionsPerPage = null, TotalDuration = null;
                DateTime? DateEcheance = null;

                if ((this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).DbValue != null)
                    NbQuestionsPerPage = Convert.ToInt32((this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).DbValue);

                if((this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).DbValue != null)
                    TotalDuration = Convert.ToInt32((this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).DbValue);

                if ((this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).SelectedDate != null)
                    DateEcheance = (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).SelectedDate;

                int NbErrors = 0;

                if(IsQuiz)
                {
                    if(TotalDuration == null)
                    {
                        (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).BorderWidth = Unit.Pixel(1);
                        (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).BorderColor = Color.Red;
                        NbErrors += 1;
                    }
                    else
                        (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).BorderWidth = Unit.Pixel(0);

                    if(NbQuestionsPerPage == null)
                    {
                        (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).BorderWidth = Unit.Pixel(1);
                        (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).BorderColor = Color.Red;
                        NbErrors += 1;
                    }
                    else
                        (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).BorderWidth = Unit.Pixel(0);

                    if (DateEcheance == null)
                    {
                        (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).BorderWidth = Unit.Pixel(1);
                        (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).BorderColor = Color.Red;
                        NbErrors += 1;
                    }
                    else
                    {
                        (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).BorderWidth = Unit.Pixel(0);

                        if(DateEcheance < DateEval.AddMinutes(Convert.ToDouble(TotalDuration)))
                        {
                            (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).BorderWidth = Unit.Pixel(1);
                            (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).BorderColor = Color.Red;
                            
                            ((evalDetail)this.Page).ShowMessage("error", GetGlobalResourceObject("MessagesPage", "SubmissionDateError").ToString());
                            NbErrors += 1;
                        }
                    }
                }

                if (NbErrors == 0)
                {
                    ExecResult res = comm.InsertEnt_Evaluation(Id_College, Id_Classe_Matiere, Id_Type, Id_User,
                        NoteSur, Coeff, NumColonne, Remarques, SessID, DateEval, Id_User, IsQuiz, TotalDuration,
                        ExamLocation, NbQuestionsPerPage, ShuffleQuestions, AllowReview, DateEcheance, Id_Section);

                    if (res.RetValue == -1)
                        ((evalDetail)this.Page).ShowMessage("error", GetGlobalResourceObject("MessagesPage", "AlreadyAdded").ToString());
                    else if (res.RetValue == -2)
                        ((evalDetail)this.Page).ShowMessage("error", GetGlobalResourceObject("MessagesPage", "BonusGradeMsg").ToString());
                    else
                    {
                        if (IsQuiz)
                            Response.Redirect("~/Online/QuestionBank/AddQuestion.aspx?mode=ins&exam=" + res.RetValue.ToString()
                                + "&s=" + this.Session["Id_Session"] + "&utype=" + this.Request["utype"].ToString());
                        else
                            Response.Redirect("~/Online/Eval/evalEleves.aspx?utype=prof&eval=" + res.RetValue.ToString()
                                + "&classe=" + res.Message + "&s=" + this.Session["Id_Session"]);
                    }
                }
            }
            else
                frmEvaluation.UpdateItem(true);
        }

        private void FillColonnes(int Id_Classe_Matiere)
        {
            if (this.frmEvaluation.CurrentMode != FormViewMode.ReadOnly)
            {
                bool AjoutEval = true;

                if (this.frmEvaluation.CurrentMode == FormViewMode.Edit)
                {
                    if ((this.frmEvaluation.FindControl("HidAjoutEval") as HiddenField).Value.ToLower() == "false")
                    {
                        AjoutEval = false;//so the dropdown will contain that value
                        (this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).Enabled = false;//enabled if in insert mode or in edit mode and the selected column is allowed
                    }
                }
                FrontComm frntComm = new FrontComm();

                List<Ent_LibelleNumColonneResult> lstLibelleNumColonne = frntComm.GetEnt_LibelleNumColonne(Convert.ToInt32(Session["Id_College"]),
                    0, "", Convert.ToInt32(Session["Annee"]), null, Id_Classe_Matiere, 0, AjoutEval, false, false, false);

                (this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).DataSource = lstLibelleNumColonne;
                (this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).DataBind();
            }
        }

        private void FillSections(int Id_Classe_Matiere)
        {
            if (this.frmEvaluation.CurrentMode != FormViewMode.ReadOnly)
            {
                FrontComm frntComm = new FrontComm();

                List<EntSP_CourseSectionsResult> lstSections = frntComm.GetCourseSection(Convert.ToInt32(Session["Id_College"]), Id_Classe_Matiere,
                    Convert.ToInt32(this.Session["Id_User"]), Request["utype"].ToString(), 0, Convert.ToInt32(Session["Annee"]), 1, 0);

                (this.frmEvaluation.FindControl("cmbCourseSections") as DropDownList).DataSource = lstSections;
                (this.frmEvaluation.FindControl("cmbCourseSections") as DropDownList).DataBind();
            }

            if (this.Request.QueryString["section"] != null || this.frmEvaluation.CurrentMode != FormViewMode.Insert)
                (this.frmEvaluation.FindControl("cmbCourseSections") as DropDownList).Enabled = false;

            if (this.Request.QueryString["section"] != null)
            {
                foreach (ListItem itm in (this.frmEvaluation.FindControl("cmbCourseSections") as DropDownList).Items)
                {
                    if (itm.Value == this.Request["section"])
                    {
                        (this.frmEvaluation.FindControl("cmbCourseSections") as DropDownList).SelectedValue = this.Request["section"];
                        return;
                    }
                }
            }
        }

        protected void cmbClasseMatiere_DataBound(object sender, EventArgs e)
        {
            if (this.Request.QueryString["classematiere"] != null)
            {
                string classematiere = this.Request["classematiere"].ToString();

                (this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue = this.Request["classematiere"].ToString();
                (this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).Enabled = false;

                FillColonnes(Convert.ToInt32(classematiere));
            }

            if ((this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue != "" && (this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue != null)
            {
                FrontComm frntComm = new FrontComm();

                int Id_College = Convert.ToInt32(Session["Id_College"]);
                int Annee = Convert.ToInt32(Session["Annee"]);
                int Id_Classe_Matiere = Convert.ToInt32((this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue);

                List<EntSp_EvaluationTypeResult> lst = frntComm.Get_EvaluationList(Id_College, Annee,
                    0, 0, "", Id_Classe_Matiere, "evaluationinsert", Request["utype"].ToString());

                (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).DataSource = lst;
                (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).DataBind();
            }
        }

        protected void cmbClasseMatiere_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Id_College = Convert.ToInt32(Session["Id_College"]);
            int Annee = Convert.ToInt32(Session["Annee"]);
            int Id_Classe_Matiere = Convert.ToInt32((this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue);

            FillColonnes(Id_Classe_Matiere);
            FillSections(Id_Classe_Matiere);

            FrontComm frntComm = new FrontComm();

            List<EntSp_EvaluationTypeResult> lst = frntComm.Get_EvaluationList(Id_College, Annee,
                0, 0, "", Id_Classe_Matiere, "evaluationinsert", Request["utype"].ToString());

            (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).DataSource = lst;
            (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).DataBind();
        }

        protected void frmEvaluation_ItemInserting(object sender, FormViewInsertEventArgs e)
        {
            this.odsEvaluation.InsertParameters["Id_College"].DefaultValue = Session["Id_College"].ToString();
            this.odsEvaluation.InsertParameters["Id_User"].DefaultValue = Session["Id_User"].ToString();
            this.odsEvaluation.InsertParameters["Id_Classe_Matiere"].DefaultValue = (this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue;
            this.odsEvaluation.InsertParameters["NumColonne"].DefaultValue = (this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).SelectedValue;
            this.odsEvaluation.InsertParameters["SessID"].DefaultValue = Session["Id_Session"].ToString();
            this.odsEvaluation.InsertParameters["Id_Type"].DefaultValue = (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).SelectedValue;
        }

        protected void frmEvaluation_DataBound(object sender, EventArgs e)
        {
            if (this.frmEvaluation.CurrentMode != FormViewMode.Insert)
            {
                FillColonnes(Convert.ToInt32((this.frmEvaluation.FindControl("HidId_Classe_Matiere") as HiddenField).Value));
                FillSections(Convert.ToInt32((this.frmEvaluation.FindControl("HidId_Classe_Matiere") as HiddenField).Value));

                if (this.frmEvaluation.CurrentMode == FormViewMode.Edit)
                {
                    (this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).SelectedValue = (this.frmEvaluation.FindControl("HidNumColonne") as HiddenField).Value;
                    (this.frmEvaluation.FindControl("cmbCourseSections") as DropDownList).SelectedValue = (this.frmEvaluation.FindControl("HidId_Section") as HiddenField).Value;
                }

                odsTypeEval.SelectParameters["Id_Classe_Matiere"].DefaultValue = (this.frmEvaluation.FindControl("HidId_Classe_Matiere") as HiddenField).Value;
                odsTypeEval.DataBind();
            }

            if (this.frmEvaluation.CurrentMode == FormViewMode.Edit)
            {
                if (Session["TypeISIS"].ToString() == "ADMIN")
                    (this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).Enabled = true;
                else
                    (this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).Enabled = false;

                int Res = 0;
                (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).DataBind();

                foreach (ListItem item in (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).Items)
                {
                    if (item.Value == (this.frmEvaluation.FindControl("HidOldType") as HiddenField).Value)
                    {
                        (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).BackColor = System.Drawing.Color.White;
                        (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).SelectedValue = (this.frmEvaluation.FindControl("HidOldType") as HiddenField).Value;
                        Res++;
                    }
                }
                if (Res == 0)
                {
                    string EvaluationType = (this.frmEvaluation.FindControl("HidEvaluationType") as HiddenField).Value;
                    string EvaluationType_Value = (this.frmEvaluation.FindControl("HidOldType") as HiddenField).Value;
                    (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).Items.Add(new ListItem(EvaluationType, EvaluationType_Value));
                    (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).SelectedValue = EvaluationType_Value;
                    (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).BackColor = System.Drawing.ColorTranslator.FromHtml("#EBEBE4");
                    (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).Enabled = false;
                }

                FrontComm comm = new FrontComm();

                int ID = Convert.ToInt32((frmEvaluation.FindControl("cmbTypeEval") as DropDownList).SelectedValue);
                List<EntSp_EvaluationTypeResult> type = comm.Get_EvaluationList(Convert.ToInt32(this.Session["Id_College"]),
                    Convert.ToInt32(this.Session["Annee"]), ID, 0, "", 0, "typebyid", "");

                if (type.Count > 0)
                {
                    if (type[0].Poids == 0)
                        (this.frmEvaluation.FindControl("CoeffLabel") as TextBox).Enabled = true;
                    else
                        (this.frmEvaluation.FindControl("CoeffLabel") as TextBox).Enabled = false;
                }
            }
            else if (this.frmEvaluation.CurrentMode == FormViewMode.Insert)
            {
                (this.frmEvaluation.FindControl("rdpDateEval") as RadDateTimePicker).SelectedDate = DateTime.Now;
                (this.frmEvaluation.FindControl("TxtCoeff") as TextBox).Text = "1";

                FrontComm frntComm = new FrontComm();
                List<Ent_UserClasseMatiereResult> lstUserClasseMatiere = frntComm.Get_UserClasseMatiere(Convert.ToInt32(Session["Id_User"]),
                    Convert.ToInt32(Session["Id_College"]), 0, Convert.ToInt32(Session["Annee"]), Request["utype"], "eval", false);

                (this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).DataSource = lstUserClasseMatiere;
                (this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).DataBind();

                if (lstUserClasseMatiere.Count > 0)
                {
                    FillColonnes(Convert.ToInt32((this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue));
                    FillSections(Convert.ToInt32((this.frmEvaluation.FindControl("cmbClasseMatiere") as DropDownList).SelectedValue));
                }

                if (this.Request.QueryString["type"] != null)
                {
                    if (this.Request.QueryString["type"] == "quiz")
                    {
                        (this.frmEvaluation.FindControl("chkIsQuiz") as CheckBox).Checked = true;

                        (this.frmEvaluation.FindControl("lblTotalDuration") as Label).Visible = true;
                        (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).Visible = true;
                        (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).DbValue = "30";

                        (this.frmEvaluation.FindControl("lblExamLocation") as Label).Visible = true;
                        (this.frmEvaluation.FindControl("txtExamLocation") as TextBox).Visible = true;

                        (this.frmEvaluation.FindControl("lblNbQuestionsPerPage") as Label).Visible = true;
                        (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).Visible = true;
                        (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).DbValue = "10";

                        (this.frmEvaluation.FindControl("lblDateEcheance") as Label).Visible = true;
                        (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).Visible = true;

                        (this.frmEvaluation.FindControl("lblShuffleQuestions") as Label).Visible = true;
                        (this.frmEvaluation.FindControl("chkShuffleQuestions") as CheckBox).Visible = true;
                        (this.frmEvaluation.FindControl("chkShuffleQuestions") as CheckBox).Checked = true;

                        (this.frmEvaluation.FindControl("lblAllowReview") as Label).Visible = true;
                        (this.frmEvaluation.FindControl("chkAllowReview") as CheckBox).Visible = true;
                        (this.frmEvaluation.FindControl("chkAllowReview") as CheckBox).Checked = false;
                    }
                    else
                    {
                        (this.frmEvaluation.FindControl("chkIsQuiz") as CheckBox).Checked = false;

                        (this.frmEvaluation.FindControl("lblTotalDuration") as Label).Visible = false;
                        (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).Visible = false;
                        (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).DbValue = null;

                        (this.frmEvaluation.FindControl("lblExamLocation") as Label).Visible = false;
                        (this.frmEvaluation.FindControl("txtExamLocation") as TextBox).Visible = false;

                        (this.frmEvaluation.FindControl("lblNbQuestionsPerPage") as Label).Visible = false;
                        (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).Visible = false;
                        (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).DbValue = null;

                        (this.frmEvaluation.FindControl("lblDateEcheance") as Label).Visible = false;
                        (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).Visible = false;
                        (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).SelectedDate = null;

                        (this.frmEvaluation.FindControl("lblShuffleQuestions") as Label).Visible = false;
                        (this.frmEvaluation.FindControl("chkShuffleQuestions") as CheckBox).Visible = false;
                        (this.frmEvaluation.FindControl("chkShuffleQuestions") as CheckBox).Checked = false;

                        (this.frmEvaluation.FindControl("lblAllowReview") as Label).Visible = false;
                        (this.frmEvaluation.FindControl("chkAllowReview") as CheckBox).Visible = false;
                        (this.frmEvaluation.FindControl("chkAllowReview") as CheckBox).Checked = false;
                    }
                }
                else
                {
                    (this.frmEvaluation.FindControl("chkIsQuiz") as CheckBox).Checked = false;

                    (this.frmEvaluation.FindControl("lblTotalDuration") as Label).Visible = false;
                    (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).Visible = false;
                    (this.frmEvaluation.FindControl("txtTotalDuration") as RadNumericTextBox).DbValue = null;

                    (this.frmEvaluation.FindControl("lblExamLocation") as Label).Visible = false;
                    (this.frmEvaluation.FindControl("txtExamLocation") as TextBox).Visible = false;

                    (this.frmEvaluation.FindControl("lblNbQuestionsPerPage") as Label).Visible = false;
                    (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).Visible = false;
                    (this.frmEvaluation.FindControl("txtNbQuestionsPerPage") as RadNumericTextBox).DbValue = null;

                    (this.frmEvaluation.FindControl("lblDateEcheance") as Label).Visible = false;
                    (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).Visible = false;
                    (this.frmEvaluation.FindControl("rdpDateEcheance") as RadDateTimePicker).SelectedDate = null;

                    (this.frmEvaluation.FindControl("lblShuffleQuestions") as Label).Visible = false;
                    (this.frmEvaluation.FindControl("chkShuffleQuestions") as CheckBox).Visible = false;
                    (this.frmEvaluation.FindControl("chkShuffleQuestions") as CheckBox).Checked = false;

                    (this.frmEvaluation.FindControl("lblAllowReview") as Label).Visible = false;
                    (this.frmEvaluation.FindControl("chkAllowReview") as CheckBox).Visible = false;
                    (this.frmEvaluation.FindControl("chkAllowReview") as CheckBox).Checked = false;
                }
            }
        }

        protected void frmEvaluation_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            this.odsEvaluation.UpdateParameters["Id_College"].DefaultValue = Session["Id_College"].ToString();
            this.odsEvaluation.UpdateParameters["SessID"].DefaultValue = Session["Id_Session"].ToString();
            this.odsEvaluation.UpdateParameters["NumColonne"].DefaultValue = (this.frmEvaluation.FindControl("cmbLibelleColonne") as DropDownList).SelectedValue;
            this.odsEvaluation.UpdateParameters["Old_IdType"].DefaultValue = (this.frmEvaluation.FindControl("HidOldType") as HiddenField).Value;
            this.odsEvaluation.UpdateParameters["Old_NumColonne"].DefaultValue = (this.frmEvaluation.FindControl("HidNumColonne") as HiddenField).Value;
            this.odsEvaluation.UpdateParameters["Id_Type"].DefaultValue = (this.frmEvaluation.FindControl("cmbTypeEval") as DropDownList).SelectedValue;
        }

        protected void lnkSave_Click(object sender, EventArgs e)
        {
            SaveEval();
        }

        protected void lnkBackToList_Click(object sender, EventArgs e)
        {
            if (this.Session["Id_UserType"].ToString() == "1")
                Response.Redirect("~/Online/Eval/evalList.aspx?utype=admin&s=" + this.Session["Id_Session"]);
            else
            {
                if (this.Request.QueryString["classematiere"] != null)
                    Response.Redirect("~/Online/Timeline/profDashboard.aspx?utype=" + this.Request["utype"]
                    + "&classematiere=" + this.Request["classematiere"].ToString() + "&s=" + this.Session["Id_Session"]);
                else
                {
                    if (this.Request.QueryString["req"] == null)
                        Response.Redirect("~/Online/Eval/evalListProf.aspx?utype=" + Request["utype"] + "&s=" + this.Session["Id_Session"]);
                    else
                        Response.Redirect("~/Online/Eval/evalTeacherDashboard.aspx?s=" + this.Session["Id_Session"]);
                }
            }
        }
    }
}