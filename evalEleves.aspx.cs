using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Dars.BLL.Online.Front.Comm;
using System.Collections.Generic;
using Dars.BLL.Online.Utils;
using Dars.BLL.Online.Front.DAL;
using System.Drawing;

namespace Dars.Web.Online.Eval
{
    public partial class evalEleves : ISISPage
    {
        public bool EvaluationCommentsVisibility = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((this.Session["Id_UserType"].ToString() == "3") || (this.Session["Id_UserType"].ToString() == "4")
                || ((this.Request["utype"] == "principal") && (Session["ProfIsPrincipal"].ToString() != "true"))
                || ((this.Request["utype"] == "coordinator") && (Session["ProfIsCoordinator"].ToString() != "true"))
                || ((this.Request["utype"] == "prof") && (Session["ProfIsProf"].ToString() != "true"))
                || ((this.Request["utype"] == "admin") && (this.Session["Id_UserType"].ToString() != "1")))
                Response.Redirect("~/Error.aspx?err=access&s=" + this.Session["Id_Session"]);

            RadAjaxManager manager = RadAjaxManager.GetCurrent(Page);
            RadAjaxLoadingPanel pnlLoad = (this.Page.Master.FindControl("pnlLoading") as RadAjaxLoadingPanel);
            manager.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(manager_AjaxRequest);

            FrontComm frntComm = new FrontComm();
            Ent_EvaluationDetailResult eval;
            int status = 0;
            int userid = 0;
            bool noteFinale = false;
            bool AjoutEval = false;
            bool isQuiz = false;
            DateTime DateEcheance = DateTime.Now.AddDays(1);

            try
            {
                eval = frntComm.Get_EvaluationDetail(Convert.ToInt32(Request["eval"]), Convert.ToInt32(Session["Id_College"]), this.Session["APPLANG"].ToString());
                status = eval.Status.Value;
                userid = eval.Id_User.Value;
                noteFinale = eval.NoteFinale;
                AjoutEval = eval.AjoutEval;
                isQuiz = eval.IsQuiz;

                if (eval.DateEcheance != null)
                    DateEcheance = eval.DateEcheance.Value;

                lnkQuiz.Visible = eval.IsQuiz;
            }
            catch (Exception ex)
            {
                Response.Redirect("~/Error.aspx?err=invalid&s=" + this.Session["Id_Session"]);
            }

            bool validVisit = false;

            //The user is the teacher who created the evaluation
            if (userid == Convert.ToInt32(Session["Id_User"].ToString()) && this.Request["utype"] == "prof")
            {
                validVisit = true;
                EvaluationCommentsVisibility = true;

                if (status < 2)
                    lnkModify.Visible = false;

                if (status < 2 && AjoutEval && (!isQuiz || (isQuiz && DateEcheance < DateTime.Now)))
                {
                    ChangeMainGridLayout(true);
                    BtnSave.Visible = true;
                    BtnSubmit.Visible = true;
                    BtnApprove.Visible = false;
                    BtnReject.Visible = false;
                    BtnValidate.Visible = false;
                }
                else
                {
                    validVisit = true;
                    setProfLayout();
                }
            }
            //The user is NOT the teacher who created the evaluation
            else if (this.Request["utype"] == "prof")
            {
                validVisit = true;
                EvaluationCommentsVisibility = false;
                setProfLayout();
            }
            else if (this.Request["utype"] == "principal")
            {
                int userPrincipal = frntComm.Get_UserIsPrincipal(Convert.ToInt32(Session["Id_User"]),
                    Convert.ToInt32(Request["eval"]), Convert.ToInt32(Session["Id_College"]));

                //read only principal will see the same layout as coordinator
                if (userPrincipal == 1)
                {
                    validVisit = true;
                    EvaluationCommentsVisibility = false;
                    setPrincipalReadOnlyLayout();
                }
                //Full access principal will be allowed to edit notes even if they are submitted and even if he is the teacher who added the grades
                else if (userPrincipal == 2)
                {
                    validVisit = true;
                    EvaluationCommentsVisibility = true;
                    setPrincipalLayout();
                }
            }
            else if (this.Request["utype"] == "coordinator")
            {
                int userCoordinator = frntComm.Get_UserIsCoordinator(Convert.ToInt32(Session["Id_User"]),
                    Convert.ToInt32(Request["eval"]), Convert.ToInt32(Session["Id_College"]));

                if (userCoordinator == 1)
                {
                    validVisit = true;
                    EvaluationCommentsVisibility = false;
                    setCoordinatorReadOnlyLayout();
                }
                if (userCoordinator == 2)
                {
                    validVisit = true;
                    EvaluationCommentsVisibility = true;
                    setCoordinatorLayout();
                }
            }

            else if (this.Request["utype"] == "admin")
            {
                EvaluationCommentsVisibility = true;
                if (status >= 2)
                {
                    validVisit = true;
                    setAdminLayout();
                }
                else
                {
                    validVisit = true;
                    (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = true;
                    ChangeMainGridLayout(true);
                    BtnApprove.Visible = false;
                    BtnReject.Visible = false;
                    BtnSave.Visible = true;
                    BtnSubmit.Visible = false;
                    lnkModify.Visible = false;
                    BtnValidate.Visible = false;
                }
            }
            else if (noteFinale)
            {
                ChangeMainGridLayout(false);
                BtnImport.Visible = false;
                tdExcel.Visible = false;
                BtnSave.Visible = false;
                BtnSubmit.Visible = false;
                lnkModify.Visible = false;
                BtnApprove.Visible = false;
                BtnReject.Visible = false;
                (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = false;
            }

            if (!IsPostBack)
            {
                FrontComm comm = new FrontComm();
                Ent_EvaluationDetailResult det = comm.Get_EvaluationDetail(Convert.ToInt32(this.Request["eval"]),
                    Convert.ToInt32(this.Session["Id_College"]), this.Session["APPLANG"].ToString());

                if (det.DisplayType == "" || det.DisplayType == null)
                    table1.Visible = false;
                else
                    table1.Visible = true;

                if (this.Session["Id_UserType"].ToString() == "1")
                {
                    LnkAdminEdit.Visible = true;
                    LnkAdminEdit.NavigateUrl = "~/Online/eval/evalEditMatiere.aspx?eval=" + Request["eval"]
                        + "&classe=" + Request["classe"] + "&utype=" + Request["utype"];
                }
                else
                    LnkAdminEdit.Visible = false;
            }

            if (!validVisit)
                Response.Redirect("~/Error.aspx?err=access&s=" + this.Session["Id_Session"]);
        }

        private void setProfLayout()
        {
            //this method (setProfLayout) is called only when the status is submitted (>2) and the prof who created the evaluation is not a principal or coordinator
            (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = false;
            ChangeMainGridLayout(false);
            BtnImport.Visible = false;
            tdExcel.Visible = false;
            BtnApprove.Visible = false;
            BtnReject.Visible = false;
            BtnSave.Visible = false;
            BtnSubmit.Visible = false;
            BtnValidate.Visible = false;
        }

        private void setCoordinatorReadOnlyLayout()
        {
            (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = false;
            ChangeMainGridLayout(false);
            BtnImport.Visible = false;
            tdExcel.Visible = false;
            BtnApprove.Visible = false;
            BtnReject.Visible = false;
            BtnSave.Visible = false;
            BtnSubmit.Visible = false;
            BtnValidate.Visible = false;
            lnkModify.Visible = false;
        }

        private void setCoordinatorLayout()
        {
            FrontComm frntComm = new FrontComm();
            Ent_EvaluationDetailResult eval;
            int status = 0;
            int approvalStatus = 0;
            bool CoordSave = true;
            bool CoordValidate = true;
            bool CoordReject = true;

            try
            {
                eval = frntComm.Get_EvaluationDetail(Convert.ToInt32(Request["eval"]), Convert.ToInt32(Session["Id_College"]), this.Session["APPLANG"].ToString());
                status = eval.Status.Value;
                approvalStatus = eval.ApprovalStatus.Value;
                CoordSave = eval.CoordSave;
                CoordValidate = eval.CoordValidate;
                CoordReject = eval.CoordReject;

                (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = (CoordSave == true);
                ChangeMainGridLayout(CoordSave);
                BtnImport.Visible = (CoordSave == true);
                tdExcel.Visible = (CoordSave == true);
                BtnApprove.Visible = false;
                BtnReject.Visible = (CoordReject == true);
                BtnSave.Visible = (CoordSave == true);
                BtnSubmit.Visible = false;
                BtnValidate.Visible = (CoordValidate == true);
                lnkModify.Visible = ((CoordSave == true) && (status == 2 || status == 3));
            }
            catch (Exception)
            {
                Response.Redirect("~/Error.aspx?err=invalid&s=" + this.Session["Id_Session"]);
            }
        }

        private void setPrincipalReadOnlyLayout()
        {
            //this method (setCoordinatorLayout) is called only when the status is submitted (>2)
            (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = false;
            ChangeMainGridLayout(false);
            BtnImport.Visible = false;
            tdExcel.Visible = false;
            BtnApprove.Visible = false;
            BtnReject.Visible = false;
            BtnSave.Visible = false;
            BtnSubmit.Visible = false;
            BtnValidate.Visible = false;
            lnkModify.Visible = false;
        }

        private void setPrincipalLayout()
        {
            FrontComm frntComm = new FrontComm();
            Ent_EvaluationDetailResult eval;
            int status = 0;
            int approvalStatus = 0;
            bool PrincSave = true;
            bool PrincApprove = true;
            bool PrincReject = true;

            try
            {
                eval = frntComm.Get_EvaluationDetail(Convert.ToInt32(Request["eval"]), Convert.ToInt32(Session["Id_College"]), this.Session["APPLANG"].ToString());
                status = eval.Status.Value;
                approvalStatus = eval.ApprovalStatus.Value;
                PrincSave = eval.PrincSave;
                PrincApprove = eval.PrincApprove;
                PrincReject = eval.PrincReject;

                (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = (PrincSave == true);
                ChangeMainGridLayout(PrincSave);
                BtnApprove.Visible = (PrincApprove == true);
                BtnReject.Visible = (PrincReject == true);
                BtnSave.Visible = (PrincSave == true);
                BtnSubmit.Visible = false;
                BtnValidate.Visible = false;
                lnkModify.Visible = ((PrincSave == true) && (status == 2 || status == 3));
            }
            catch (Exception)
            {
                Response.Redirect("~/Error.aspx?err=invalid&s=" + this.Session["Id_Session"]);
            }
        }

        private void setAdminLayout()
        {
            (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = true;
            ChangeMainGridLayout(true);
            BtnApprove.Visible = true;
            BtnReject.Visible = true;
            BtnSave.Visible = true;
            BtnSubmit.Visible = false;
            BtnValidate.Visible = false;
            lnkModify.Visible = false;
        }

        private void ChangeMainGridLayout(bool status)
        {
            if (status == false)
            {
                MainGrid.DataBind();
                foreach (GridDataItem it in MainGrid.Items)
                {
                    (this.MainGrid.Items[it.ItemIndex].FindControl("TxtNote") as TextBox).Enabled = false;
                    (this.MainGrid.Items[it.ItemIndex].FindControl("TxtNote") as TextBox).ForeColor = Color.Gray;
                }
            }
        }

        private void ShowMessage(string MsgType, string MsgText)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "starScript", "showNotif('" + MsgText + "','" + MsgType.ToLower() + "');", true);
        }

        private decimal GetNote(int ItemIndex)
        {
            if ((this.MainGrid.Items[ItemIndex].FindControl("TxtNote") as TextBox).Text == "")
                return -5;

            decimal Note, parsedValue;
            decimal NoteSur = Convert.ToDecimal((EvalDetail.FindControl("frmEvaluation").FindControl("NoteSurtxt") as TextBox).Text);

            try
            {
                string DisplayType = (this.EvalDetail.FindControl("frmEvaluation").FindControl("HidDisplayType") as HiddenField).Value;

                if (DisplayType != "" && !decimal.TryParse((this.MainGrid.Items[ItemIndex].FindControl("TxtNote") as TextBox).Text, out parsedValue))
                {
                    FrontComm comm = new FrontComm();
                    List<Not_LettreNote> not = comm.GetNot_LettreNoteByFormat(Convert.ToInt32(this.Session["Id_College"]),
                        DisplayType, (this.MainGrid.Items[ItemIndex].FindControl("TxtNote") as TextBox).Text);

                    if (not.Count == 0)
                    {
                        this.MainGrid.Items[ItemIndex].BackColor = Color.Red;
                        ShowMessage("error", GetGlobalResourceObject("MessagesPage", "InvalidEntry").ToString());
                        return -1;
                    }
                    else
                    {
                        Note = not[0].GradeTo.Value * NoteSur / 20;
                    }
                }
                else
                    Note = Convert.ToDecimal((this.MainGrid.Items[ItemIndex].FindControl("TxtNote") as TextBox).Text);
            }
            catch (Exception)
            {
                this.MainGrid.Items[ItemIndex].BackColor = Color.Red;
                ShowMessage("error", GetGlobalResourceObject("MessagesPage", "InvalidEntry").ToString());
                return -1;
            }

            if (Note > NoteSur)
            {
                this.MainGrid.Items[ItemIndex].BackColor = Color.Red;
                ShowMessage("error", GetGlobalResourceObject("MessagesPage", "ExceededMax").ToString() + " " + Note.ToString() + " > " + NoteSur.ToString());

                return -1;
            }
            return Note;
        }

        private bool CommitNotes()
        {
            int nbErrors = 0;
            int Id_EleveEval, Id_Eleve, Id_College, Id_Evaluation, SessID;
            decimal Note;
            Id_College = Convert.ToInt32(Session["Id_College"].ToString());
            SessID = Convert.ToInt32(Session["Id_Session"].ToString());
            Id_Evaluation = Convert.ToInt32(Request["eval"]);
            FrontComm frntComm = new FrontComm();

            foreach (Telerik.Web.UI.GridDataItem it in MainGrid.Items)
            {
                this.MainGrid.Items[it.ItemIndex].BackColor = Color.Transparent;

                Id_EleveEval = Convert.ToInt32((this.MainGrid.Items[it.ItemIndex].FindControl("HidID_EEval") as HiddenField).Value);
                Id_Eleve = Convert.ToInt32((this.MainGrid.Items[it.ItemIndex].FindControl("HidID_Eleve") as HiddenField).Value);

                Note = GetNote(it.ItemIndex);

                if (Note == -1)
                {

                }
                else
                {
                    frntComm.CommitEleveNote(Id_EleveEval, Id_Eleve, Id_College, Id_Evaluation, Note, SessID);
                    (this.MainGrid.Items[it.ItemIndex].FindControl("HidSaved") as HiddenField).Value = "saved";
                }
            }
            int IdClasseMatiere = Convert.ToInt32((this.EvalDetail.FindControl("frmEvaluation").FindControl("HidId_Classe_Matiere") as HiddenField).Value);
            int NumColonne = Convert.ToInt32((this.EvalDetail.FindControl("frmEvaluation").FindControl("HidNumColonne") as HiddenField).Value);

            ExecResult res = frntComm.CalcMoyenneClasse(Id_Evaluation, Id_College);
            frntComm.CalcStandardDeviation(Id_Evaluation, Id_College);

            if (res.RetValue == -1)
            {
                ShowMessage("error", GetGlobalResourceObject("MessagesPage", "ErrorCalculatingAvg").ToString());
            }

            if (nbErrors == 0)
                return true;
            else
                return false;
        }

        protected void BtnImport_Click(object sender, EventArgs e)
        {
            FrontComm frntComm = new FrontComm();
            if (this.MainGrid.Items.Count > 0)
            {
                List<string> lst = new List<string>();
                lst = TxtExcel.Text.Split('\n').ToList();
                decimal NoteEleve = 0;
                string result = "";
                int Id_EleveEval, Id_Eleve, Id_College, Id_Evaluation, SessID;
                Id_College = Convert.ToInt32(Session["Id_College"].ToString());
                SessID = Convert.ToInt32(Session["Id_Session"].ToString());
                Id_Evaluation = Convert.ToInt32(Request["eval"]);
                decimal NoteSur = Convert.ToDecimal((EvalDetail.FindControl("frmEvaluation").FindControl("NoteSurtxt") as TextBox).Text);
                string DisplayType = (this.EvalDetail.FindControl("frmEvaluation").FindControl("HidDisplayType") as HiddenField).Value;

                for (int i = 0; i < lst.Count; i++)
                {
                    if (this.MainGrid.Items.Count < i)
                        return;
                    try
                    {
                        NoteEleve = System.Convert.ToDecimal(lst[i]);
                        if (NoteEleve > NoteSur)
                        {
                            result = result + lst[i] + "-> invalid";
                        }
                        else
                        {
                            Id_EleveEval = Convert.ToInt32((this.MainGrid.Items[i].FindControl("HidID_EEval") as HiddenField).Value);
                            Id_Eleve = Convert.ToInt32((this.MainGrid.Items[i].FindControl("HidID_Eleve") as HiddenField).Value);
                            frntComm.CommitEleveNote(Id_EleveEval, Id_Eleve, Id_College, Id_Evaluation, NoteEleve, SessID);
                            (this.MainGrid.Items[i].FindControl("HidSaved") as HiddenField).Value = "saved";
                            result = result + lst[i] + "-> ok";
                        }
                    }
                    catch (Exception)
                    {
                        result = result + lst[i] + "-> Not Valid";
                    }
                }
                TxtExcel.Text = result;
                ShowMessage("success", GetGlobalResourceObject("MessagesPage", "Imported").ToString());
                int IdClasseMatiere = Convert.ToInt32((this.EvalDetail.FindControl("frmEvaluation").FindControl("HidId_Classe_Matiere") as HiddenField).Value);
                int NumColonne = Convert.ToInt32((this.EvalDetail.FindControl("frmEvaluation").FindControl("HidNumColonne") as HiddenField).Value);

                ExecResult res = frntComm.CalcMoyenneClasse(Id_Evaluation, Id_College);
                frntComm.CalcStandardDeviation(Id_Evaluation, Id_College);

                if (res.RetValue == -1)
                {
                    ShowMessage("error", GetGlobalResourceObject("MessagesPage", "ErrorCalculatingAvg").ToString());
                }

                MainGrid.DataBind();
            }
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                decimal NoteSur = Convert.ToDecimal((EvalDetail.FindControl("frmEvaluation").FindControl("NoteSurtxt") as TextBox).Text);
            }
            catch (Exception)
            {
                (EvalDetail.FindControl("frmEvaluation").FindControl("NoteSurtxt") as TextBox).BackColor = Color.Red;
                ShowMessage("error", GetGlobalResourceObject("MessagesPage", "Error").ToString());
                return;
            }

            try
            {
                decimal Coeff = Convert.ToDecimal((EvalDetail.FindControl("frmEvaluation").FindControl("CoeffLabel") as TextBox).Text);
            }
            catch (Exception)
            {
                (EvalDetail.FindControl("frmEvaluation").FindControl("CoeffLabel") as TextBox).BackColor = Color.Red;
                ShowMessage("error", GetGlobalResourceObject("MessagesPage", "Error").ToString());
                return;
            }

            (this.EvalDetail.FindControl("frmEvaluation") as FormView).UpdateItem(true);
            (this.EvalDetail.FindControl("frmEvaluation") as FormView).ChangeMode(FormViewMode.Edit);

            int idEval = Convert.ToInt32(((this.EvalDetail.FindControl("frmEvaluation") as FormView).FindControl("HidID") as HiddenField).Value);
            int status = Convert.ToInt32(((this.EvalDetail.FindControl("frmEvaluation") as FormView).FindControl("HidStatus") as HiddenField).Value);
            bool isQuiz = Convert.ToBoolean(((this.EvalDetail.FindControl("frmEvaluation") as FormView).FindControl("HidIsQuiz") as HiddenField).Value);

            if (status <= 1)
            {
                FrontComm frntComm = new FrontComm();
                frntComm.Update_Ent_EvaluationStatus(Convert.ToInt32(Session["Id_User"]), 1, 1, idEval,
                    Convert.ToInt32(Session["Id_College"]), Session["APPLANG"].ToString(), "SAVED", true, 4);
            }              

            CommitNotes();
            ShowMessage("success", GetGlobalResourceObject("MessagesPage", "Saved").ToString());

            MainGrid.DataBind();

            RadHtmlChart1.DataBind();
            RadHtmlChart2.DataBind();
            ((EvalDetail.FindControl("lstEvalProgres") as ListView)).DataBind();
            ((EvalDetail.FindControl("BarChart") as RadHtmlChart)).DataBind();
        }

        protected void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (CheckGrades())
            {
                (this.EvalDetail.FindControl("frmEvaluation") as FormView).UpdateItem(true);
                (this.EvalDetail.FindControl("frmEvaluation") as FormView).ChangeMode(FormViewMode.Edit);
                if (CommitNotes())
                {
                    int idEval = Convert.ToInt32(((this.EvalDetail.FindControl("frmEvaluation") as FormView).FindControl("HidID") as HiddenField).Value);
                    FrontComm frntComm = new FrontComm();
                    frntComm.Update_Ent_EvaluationStatus(Convert.ToInt32(Session["Id_User"]), 2, 2, idEval,
                        Convert.ToInt32(Session["Id_College"]), Session["APPLANG"].ToString(), "SUBMITED", true, 4);
                    (this.EvalDetail.FindControl("frmEvaluation") as FormView).Enabled = false;
                    ShowMessage("success", GetGlobalResourceObject("MessagesPage", "Submitted").ToString());
                    setProfLayout();
                }
                ShowMessage("error", GetGlobalResourceObject("MessagesPage", "NotSubmitted").ToString());
            }
            else
            {
                ShowMessage("error", GetGlobalResourceObject("MessagesPage", "FailedSubmitted").ToString());
            }
        }

        protected void BtnReject_Click(object sender, EventArgs e)
        {
            int idEval = Convert.ToInt32(Request["eval"]);
            FrontComm frntComm = new FrontComm();

            if (this.Request["utype"] == "coordinator")
                frntComm.Update_Ent_EvaluationStatus(Convert.ToInt32(Session["Id_User"]), 1, 5, idEval,
                    Convert.ToInt32(Session["Id_College"]), Session["APPLANG"].ToString(), "REJECTEDCOORD", true, 4);
            else
                frntComm.Update_Ent_EvaluationStatus(Convert.ToInt32(Session["Id_User"]), 1, 5, idEval,
                    Convert.ToInt32(Session["Id_College"]), Session["APPLANG"].ToString(), "REJECTEDPRINC", true, 4);

            ShowMessage("success", GetGlobalResourceObject("MessagesPage", "Rejected").ToString());

            if (this.Request["utype"] == "coordinator")
                setCoordinatorLayout();
            if (this.Request["utype"] == "principal")
                setPrincipalLayout();
            if (this.Request["utype"] == "admin")
                setAdminLayout();
        }

        protected void BtnValidate_Click(object sender, EventArgs e)
        {
            int idEval = Convert.ToInt32(Request["eval"]);
            FrontComm frntComm = new FrontComm();
            frntComm.ValidateEvaluation(Convert.ToInt32(Session["Id_User"]), 4, idEval,
                Convert.ToInt32(Session["Id_College"]), Session["APPLANG"].ToString(), true, 4);
            ShowMessage("success", GetGlobalResourceObject("MessagesPage", "Validated").ToString());
            setCoordinatorLayout();
        }

        protected void BtnApprove_Click(object sender, EventArgs e)
        {
            int idEval = Convert.ToInt32(Request["eval"]);
            FrontComm frntComm = new FrontComm();

            frntComm.Update_Ent_EvaluationStatus(Convert.ToInt32(Session["Id_User"]), 3, 3, idEval,
                Convert.ToInt32(Session["Id_College"]), Session["APPLANG"].ToString(), "APPROVED", true, 4);

            ShowMessage("success", GetGlobalResourceObject("MessagesPage", "Approved").ToString());

            if (this.Request["utype"] == "principal")
                setPrincipalLayout();
            if (this.Request["utype"] == "admin")
                setAdminLayout();
        }

        private bool CheckGrades()
        {
            int count = 0;
            foreach (Telerik.Web.UI.GridDataItem it in MainGrid.Items)
                if (((this.MainGrid.Items[it.ItemIndex].FindControl("TxtNote") as TextBox).Text == "") || ((this.MainGrid.Items[it.ItemIndex].FindControl("TxtNote") as TextBox).Text == "-5"))
                    count++;

            if (count == MainGrid.Items.Count)
                return false;
            else
                return true;
        }

        protected void manager_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            if (e.Argument == "Refresh")
                ScriptManager.RegisterStartupScript(this, this.GetType(), "starScript", "showNotif('" + GetGlobalResourceObject("MessagesPage", "RequestSent").ToString() + "','success');", true);
        }

        protected void lnkModify_Clicked(object sender, EventArgs e)
        {
            Response.Redirect("evalElevesModif.aspx?eval=" + this.Request["eval"] + "&classe=" + this.Request["classe"] + "&utype=" + this.Request["utype"] + "&request=evaluation");
        }

        protected void lnkDetail_Clicked(object sender, EventArgs e)
        {
            string Id_Matiere = (EvalDetail.FindControl("frmEvaluation").FindControl("HidId_Matiere") as HiddenField).Value;
            string Colonne = (EvalDetail.FindControl("frmEvaluation").FindControl("cmbLibelleColonne") as DropDownList).SelectedValue;
            string Id_Classe_Matiere = (EvalDetail.FindControl("frmEvaluation").FindControl("HidId_Classe_Matiere") as HiddenField).Value;

            Response.Redirect("evalTeacher.aspx?utype=" + this.Request["utype"] + "&eval=" + this.Request["eval"]
                + "&classe=" + this.Request["classe"] + "&ClasseMatiere=" + Id_Classe_Matiere + "&colonne=" + Colonne + "&req=eval");
        }

        protected void MainGrid_DataBound(object sender, EventArgs e)
        {
            foreach (GridDataItem item in MainGrid.MasterTableView.Items)
            {
                string Color = (item.FindControl("HiddColor") as HiddenField).Value;
                (item.FindControl("TxtNote") as TextBox).ForeColor = System.Drawing.ColorTranslator.FromHtml(Color);

                if (chkByGrade.Checked)
                {

                }
                else if (chkByLetter.Checked)
                {
                    (item.FindControl("lblOldNote") as Label).Text = (item.FindControl("HidOldNote_Lettre") as HiddenField).Value;
                    (item.FindControl("lblProgresPositif") as Label).Visible = false;
                    (item.FindControl("lblProgresNegatif") as Label).Visible = false;
                    (item.FindControl("TxtNote") as TextBox).Text = (item.FindControl("HiddLettre") as HiddenField).Value;
                    (item.FindControl("lblCurrentNote") as Label).Text = (item.FindControl("HiddCurrentNote_Lettre") as HiddenField).Value;
                }
            }

        }

        protected void lnkBackToList_Clicked(object sender, EventArgs e)
        {
            if (this.Request["type"] == "message")
                Response.Redirect("~/Online/profHome.aspx&s=" + this.Session["Id_Session"]);

            if (this.Request.QueryString["req"] != null)
            {
                if (this.Request["req"] == "notebook")
                {
                    string Id_Matiere = (EvalDetail.FindControl("frmEvaluation").FindControl("HidId_Matiere") as HiddenField).Value;
                    string Colonne = (EvalDetail.FindControl("frmEvaluation").FindControl("cmbLibelleColonne") as DropDownList).SelectedValue;
                    string Id_Classe_Matiere = (EvalDetail.FindControl("frmEvaluation").FindControl("HidId_Classe_Matiere") as HiddenField).Value;

                    Response.Redirect("evalTeacher.aspx?utype=" + this.Request["utype"] + "&eval=" + this.Request["eval"] + "&classe=" + this.Request["classe"] + "&ClasseMatiere=" + Id_Classe_Matiere + "&colonne=" + Colonne);
                }
                else if (this.Request["req"] == "dashboard")
                    Response.Redirect("evalTeacherDashboard.aspx?s=" + this.Session["Id_Session"]);
                else if (this.Request["req"] == "timeline")
                    Response.Redirect("~/Online/Timeline/profDashboard.aspx?utype=" + this.Request["utype"] + "&classematiere="
                        + this.Request["classematiere"].ToString() + "&s=" + this.Session["Id_Session"]);
                else if (this.Request["req"] == "quiz")
                    Response.Redirect("/Online/QuestionBank/QuestionListResult.aspx?exam="
                        + this.Request["eval"] + " &req=eval&utype=" + this.Request["utype"]);
            }
            else
            {
                if (this.Request["utype"] == "principal")
                {
                    if (this.Session["EvaluationURL"] != null)
                    {
                        string EvaluationURL = this.Session["EvaluationURL"].ToString();
                        if (EvaluationURL != "")
                            Response.Redirect(EvaluationURL);
                        else
                            Response.Redirect("~/Online/Eval/evalListPrincipal.aspx?utype=principal&s=" + this.Session["Id_Session"]);
                    }
                    else
                        Response.Redirect("~/Online/Eval/evalListPrincipal.aspx?utype=principal&s=" + this.Session["Id_Session"]);
                }

                if (this.Request["utype"] == "coordinator")
                {
                    if (this.Session["EvaluationURL"] != null)
                    {
                        string EvaluationURL = this.Session["EvaluationURL"].ToString();
                        if (EvaluationURL != "")
                            Response.Redirect(EvaluationURL);
                        else
                            Response.Redirect("~/Online/Eval/evalListCoordinator.aspx?utype=coordinator&s=" + this.Session["Id_Session"]);
                    }
                    else
                        Response.Redirect("~/Online/Eval/evalListCoordinator.aspx?utype=coordinator&s=" + this.Session["Id_Session"]);
                }

                if (this.Request["utype"] == "prof")
                {
                    if (this.Session["EvaluationURL"] != null)
                    {
                        string EvaluationURL = this.Session["EvaluationURL"].ToString();
                        if (EvaluationURL != "")
                            Response.Redirect(EvaluationURL);
                        else
                            Response.Redirect("~/Online/Eval/evalListProf.aspx?utype=prof&s=" + this.Session["Id_Session"]);
                    }
                    else
                        Response.Redirect("~/Online/Eval/evalListProf.aspx?utype=prof&s=" + this.Session["Id_Session"]);
                }
                else if (this.Request["utype"] == "admin")
                {
                    if (this.Session["EvaluationURL"] != null)
                    {
                        string EvaluationURL = this.Session["EvaluationURL"].ToString();
                        if (EvaluationURL != "")
                            Response.Redirect(EvaluationURL);
                        else
                            Response.Redirect("~/Online/Eval/evalList.aspx?utype=admin&s=" + this.Session["Id_Session"]);
                    }
                    else
                        Response.Redirect("~/Online/Eval/evalList.aspx?utype=admin&s=" + this.Session["Id_Session"]);
                }
            }
        }

        protected void rbByGrade_CheckedChanged(object sender, EventArgs e)
        {
            MainGrid.DataBind();
        }

        public bool EvaluationCommentsAccess(bool Visibility)
        {
            return Visibility;
        }

        protected void lnkQuiz_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Online/QuestionBank/QuestionListResult.aspx?exam=" + this.Request["eval"] + "&req=evaldet&utype=" + this.Request["utype"]);
        }

        protected void MainGrid_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "showresult")
            {
                string studentId = e.CommandArgument.ToString();
                this.Session["Id_QuizResult"] = this.Request["eval"].ToString();

                Response.Redirect("~/Online/QuestionBank/PreviewResultStudent.aspx?student=" + studentId + "&utype="
                    + this.Request["utype"] + "&classe=" + this.Request["classe"] + "&req=" + this.Request["req"]);
            }
           
        }
    }
}
