<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="evalDetailCtrl.ascx.cs" Inherits="Dars.Web.Online.Eval.evalDetailCtrl" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function cmbTypeEvalChanged(cmbTypeEval_Control) {
        var Id_College = '<%=Session["Id_College"]%>';
        var ddl = document.getElementById(cmbTypeEval_Control);
        var Id_Evaluation = ddl.options[ddl.selectedIndex].value;

        var res = PageMethods.Get_EvaluationTypeByID(Id_College, Id_Evaluation, onComplete);
    }

    function onComplete(res) {
        if (res == 0)
            document.getElementById("ctl00_MainContent_EvalDetail_frmEvaluation_CoeffLabel").disabled = '';
        else
            document.getElementById("ctl00_MainContent_EvalDetail_frmEvaluation_CoeffLabel").disabled = 'true';
    }

    function ChangeQuiz(isQuiz_control) {
        var isQuiz = document.getElementById(isQuiz_control).checked;

        var TotalDuration_control = $find(isQuiz_control.replace("chkIsQuiz", "txtTotalDuration"));
        var ExamLocation_control = document.getElementById(isQuiz_control.replace("chkIsQuiz", "txtExamLocation"));
        var NbQuestionsPerPage_control = $find(isQuiz_control.replace("chkIsQuiz", "txtNbQuestionsPerPage"));
        var ShuffleQuestions_control = document.getElementById(isQuiz_control.replace("chkIsQuiz", "chkShuffleQuestions"));
        var AllowReview_control = document.getElementById(isQuiz_control.replace("chkIsQuiz", "chkAllowReview"));

        if (isQuiz) {
            TotalDuration_control.enable();
            ExamLocation_control.disabled = '';
            NbQuestionsPerPage_control.enable();
            ShuffleQuestions_control.disabled = '';
            AllowReview_control.disabled = '';
        }
        else {
            TotalDuration_control.disable();
            TotalDuration_control.clear();
            ExamLocation_control.disabled = 'true';
            ExamLocation_control.value = '';
            NbQuestionsPerPage_control.disable();
            NbQuestionsPerPage_control.clear();
            ShuffleQuestions_control.disabled = 'true';
            ShuffleQuestions_control.checked = false;
            AllowReview_control.checked = false;
        }
    }
</script>

<table runat="server" id="tblToolBar">
    <tr id="ToolBar" runat="server">
        <td>
            <asp:LinkButton ID="lnkBackToList" runat="server" OnClick="lnkBackToList_Click"
                CausesValidation="false" class="btn btn-icon btn-default glyphicons circle_arrow_left">
                <i></i><asp:Label runat="server" Text="Liste" ID="LblListe" meta:resourcekey="LblListeResource1" />
            </asp:LinkButton>
        </td>
        <td>
            <asp:LinkButton ID="lnkSave" runat="server" class="btn btn-primary"
                CausesValidation="true" OnClick="lnkSave_Click" ValidationGroup="defGrp">
                <i></i><asp:Label runat="server" ID="lblSave" Text="Sauvegarde" meta:resourcekey="lblSaveResource1" />
            </asp:LinkButton>
        </td>
    </tr>
</table>
<br />
<div class="simplebar-content">
    <div class="container-fluid">
        <div class="row">
            <div class="col-md-6">
                <asp:FormView ID="frmEvaluation" runat="server" DataSourceID="odsEvaluation"
                    OnDataBound="frmEvaluation_DataBound" OnItemUpdating="frmEvaluation_ItemUpdating">
                    <EditItemTemplate>
                        <table>
                            <tr>
                                <asp:HiddenField runat="server" ID="HidID" Value='<%# Bind("ID") %>' />
                                <asp:HiddenField runat="server" ID="HidStatus" Value='<%# Eval("Status") %>' />
                                <asp:HiddenField runat="server" ID="HidIsQuiz" Value='<%# Eval("IsQuiz") %>' />
                                <asp:HiddenField runat="server" ID="HidAjoutEval" Value='<%# Eval("AjoutEval") %>' />
                                <asp:HiddenField runat="server" ID="HidId_Matiere" Value='<%# Eval("Id_Matiere") %>' />
                                <asp:HiddenField runat="server" ID="HidDisplayType" Value='<%# Eval("DisplayType") %>' />
                                <asp:HiddenField runat="server" ID="HidIdClasseMatiereUser" Value='<%# Eval("Id_ClasseMatiereUser") %>' />
                                <asp:HiddenField runat="server" ID="HidId_Classe" Value='<%# Bind("Id_Classe") %>' />
                                <asp:HiddenField runat="server" ID="HidId_Classe_Matiere" Value='<%# Bind("Id_Classe_Matiere") %>' />
                                <asp:HiddenField runat="server" ID="HidNumColonne" Value='<%# Eval("NumColonne") %>' />
                                <asp:HiddenField runat="server" ID="HidId_Section" Value='<%# Eval("Id_Section") %>' />
                                <asp:HiddenField runat="server" ID="HidOldType" Value='<%# Eval("Id_Type") %>' />
                                <asp:HiddenField runat="server" ID="HidEvaluationType" Value='<%# Eval("EvaluationType") %>' />
                                <td style="padding: 0px 10px 5px 0px" colspan="2">
                                    <strong>
                                        <asp:Label ID="LblClasseMatiere" runat="server" Visible='<%# Convert.ToInt32(Eval("Id_Classe")) > 0 %>'
                                            Text='<%# String.Format("{0}/{1}-{2} {3}", Eval("Classe"), Eval("Section"), Eval("Abr"), Eval("Matiere")) %>' />
                                        <asp:Label ID="LblClasseMatiere1" runat="server" Visible='<%# Convert.ToInt32(Eval("Id_Classe")) < 0 %>'
                                            Text='<%# String.Format("{0} {1}", Eval("Abr"), Eval("Matiere")) %>' />
                                    </strong>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblLibelle" runat="server" Text="Colonne" meta:resourcekey="LblLibelleResource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:DropDownList ID="cmbLibelleColonne" runat="server" DataTextField="libelle" DataValueField="NumColonne"
                                        MarkFirstMatch="True" meta:resourcekey="cmbLibelleColonneResource3" />
                                    <asp:RequiredFieldValidator ID="RfvcmbLibelleColonne" runat="server" ControlToValidate="cmbLibelleColonne" ForeColor="Red"
                                        ValidationGroup="defGrp" Text="*" ErrorMessage="*" Display="Dynamic" meta:resourcekey="RfvcmbLibelleColonneResource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblNoteSur" runat="server" Text="Note sur" meta:resourcekey="LblNoteSurResource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <b><asp:TextBox ID="NoteSurtxt" runat="server" Text='<%# Bind("NoteSur") %>' Height="30px" /></b>
                                    <telerik:RadInputManager ID="rimNoteSur" runat="server">
                                        <telerik:NumericTextBoxSetting BehaviorID="NumericBehavior3" Type="Number" MinValue="0"
                                            GroupSeparator="" DecimalDigits="2" Validation-IsRequired="true">
                                            <TargetControls>
                                                <telerik:TargetInput ControlID="NoteSurtxt" />
                                            </TargetControls>
                                        </telerik:NumericTextBoxSetting>
                                    </telerik:RadInputManager>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblCoeff" runat="server" Text="Poids" meta:resourcekey="LblCoeffResource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:TextBox ID="CoeffLabel" runat="server" Text='<%# Bind("Coeff") %>' Height="30px" />
                                    <telerik:RadInputManager ID="RadInputManager11" runat="server">
                                        <telerik:NumericTextBoxSetting BehaviorID="NumericBehavior3" Type="Number" MinValue="0"
                                            GroupSeparator="" DecimalDigits="2" Validation-IsRequired="true">
                                            <TargetControls>
                                                <telerik:TargetInput ControlID="CoeffLabel" />
                                            </TargetControls>
                                        </telerik:NumericTextBoxSetting>
                                    </telerik:RadInputManager>
                                    <asp:RequiredFieldValidator ID="RfvCoeffLabel" runat="server" ControlToValidate="CoeffLabel"
                                        ValidationGroup="defGrp" Text="*" ErrorMessage="*" Display="Dynamic" meta:resourcekey="RfvCoeffLabelResource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label7" runat="server" Text="Type" meta:resourcekey="Label7Resource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:DropDownList ID="cmbTypeEval" runat="server" DataSourceID="odsTypeEval"
                                        DataTextField="EvaluationType" DataValueField="ID"
                                        onchange='<%# String.Format("javascript:cmbTypeEvalChanged(\"{0}\");", Container.FindControl("cmbTypeEval").ClientID) %>' />
                                    <asp:RequiredFieldValidator ID="RfvcmbTypeEval" runat="server" ControlToValidate="cmbTypeEval" ForeColor="Red"
                                        ValidationGroup="defGrp" Text="*" ErrorMessage="*" Display="Dynamic" meta:resourcekey="RfvcmbTypeEvalResource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblRemarques" runat="server" Text="Remarques" meta:resourcekey="LblRemarquesResource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:TextBox ID="TxtRemarques" runat="server" Text='<%# Bind("Remarques") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblDateEval" runat="server" Text="Evaluation faite le" meta:resourcekey="LblDateEvalResource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <telerik:RadDateTimePicker ID="rdpDateEval" runat="server" Culture="en-US"
                                        SelectedDate='<%# Bind("DateEval") %>' TimeView-TimeFormat="t" Width="250px"
                                        DateInput-DateFormat="dd/MM/yyyy h:mm tt" DateInput-DisplayDateFormat="dd/MM/yyyy h:mm tt">
                                        <TimeView Skin="Default" ShowHeader="False" Interval="00:30:00" Columns="6"></TimeView>
                                    </telerik:RadDateTimePicker>
                                    <asp:RequiredFieldValidator ID="RfvrdpDateEval" runat="server" ControlToValidate="rdpDateEval" ForeColor="Red"
                                        ValidationGroup="defGrp" Text="*" ErrorMessage="*" Display="Dynamic" meta:resourcekey="RfvrdpDateEvalResource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblSection" runat="server" Text="Section" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:DropDownList ID="cmbCourseSections" runat="server"
                                        DataTextField="SectionName" DataValueField="ID" Width="250px" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblDateSaisie" runat="server" Text="Saisie le" meta:resourcekey="LblDateSaisieResource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="DateSaisieLabel" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy hh:mm}",Eval("DateSaisie")) %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label2" runat="server" Text="Ajoutée par" meta:resourcekey="Label2Resource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label3" runat="server" Text='<%# Eval("DisplayName") %>' meta:resourcekey="Label3Resource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label5" runat="server" Text="Status" meta:resourcekey="Label5Resource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label4" runat="server" Text='<%# Eval("StatusName") %>' meta:resourcekey="Label4Resource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label6" runat="server" Text="Coordinateur statut" meta:resourcekey="CoordiResource" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label10" runat="server" Text='<%# Eval("CoordStatus") %>' />
                                    <asp:Label ID="Label112" runat="server" Text="par" Visible='<%# Convert.ToString(Eval("CoordName")) != "" %>' meta:resourcekey="Label15Resource1" />
                                    <asp:Label ID="Label11" runat="server" Text='<%# Eval("CoordName") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label12" runat="server" Text="Principal statut" meta:resourcekey="DirecteurResource" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label13" runat="server" Text='<%# Eval("princStatus") %>' />
                                    <asp:Label ID="Label15" runat="server" Text="par" Visible='<%# Convert.ToString(Eval("PrincName")) != "" %>' meta:resourcekey="Label15Resource1" />
                                    <asp:Label ID="Label14" runat="server" Text='<%# Eval("PrincName") %>' />
                                </td>
                            </tr>
                            </tr>
                        </table>
                    </EditItemTemplate>
                    <InsertItemTemplate>
                        <table>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label1" runat="server" Text="Classe matière" meta:resourcekey="Label1Resource1" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:DropDownList ID="cmbClasseMatiere" runat="server" AutoPostBack="True"
                                        DataValueField="Id_Classe_Matiere" DataTextField="ClasseMatiere" Width="250px"
                                        OnDataBound="cmbClasseMatiere_DataBound" OnSelectedIndexChanged="cmbClasseMatiere_SelectedIndexChanged" />
                                    <asp:RequiredFieldValidator ID="RfvcmbClasseMatiere" runat="server" ControlToValidate="cmbClasseMatiere" ForeColor="Red"
                                        ValidationGroup="defGrp" Text="*" ErrorMessage="*" Display="Dynamic" meta:resourcekey="RfvcmbClasseMatiereResource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblLibelle" runat="server" Text="Colonne" meta:resourcekey="LblLibelleResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:DropDownList ID="cmbLibelleColonne" runat="server"
                                        DataTextField="libelle" DataValueField="NumColonne" />
                                    <asp:RequiredFieldValidator ID="RfvcmbLibelleColonne" runat="server" ControlToValidate="cmbLibelleColonne" ForeColor="Red"
                                        ValidationGroup="defGrp" Text="*" ErrorMessage="*" Display="Dynamic" meta:resourcekey="RfvcmbLibelleColonneResource2" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblNoteSur" runat="server" Text="Note sur" meta:resourcekey="LblNoteSurResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:TextBox ID="TxtNoteSur" runat="server" Text='<%# Bind("NoteSur") %>' meta:resourcekey="TxtNoteSurResource1" Height="30px" />
                                    <telerik:RadInputManager ID="rimNoteSur" runat="server">
                                        <telerik:NumericTextBoxSetting BehaviorID="NumericBehavior3" Type="Number" MinValue="0"
                                            GroupSeparator="" DecimalDigits="2" Validation-IsRequired="true">
                                            <TargetControls>
                                                <telerik:TargetInput ControlID="TxtNoteSur" />
                                            </TargetControls>
                                        </telerik:NumericTextBoxSetting>
                                    </telerik:RadInputManager>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblCoeff" runat="server" Text="Coeff" meta:resourcekey="LblCoeffResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:TextBox ID="TxtCoeff" runat="server" Text='<%# Bind("Coeff") %>' meta:resourcekey="TxtCoeffResource1" Height="30px"  />
                                    <telerik:RadInputManager ID="rimCoeff" runat="server">
                                        <telerik:NumericTextBoxSetting BehaviorID="NumericBehavior3" Type="Number" MinValue="0"
                                            GroupSeparator="" DecimalDigits="2" Validation-IsRequired="false">
                                            <TargetControls>
                                                <telerik:TargetInput ControlID="TxtCoeff" />
                                            </TargetControls>
                                        </telerik:NumericTextBoxSetting>
                                    </telerik:RadInputManager>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label7" runat="server" Text="Type" meta:resourcekey="Label7Resource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:DropDownList ID="cmbTypeEval" runat="server"
                                        DataTextField="EvaluationType" DataValueField="ID" MarkFirstMatch="True" />
                                    <asp:RequiredFieldValidator ID="rFcmbTypeEval" runat="server" ControlToValidate="cmbTypeEval" ForeColor="Red"
                                        ValidationGroup="defGrp" Text="*" ErrorMessage="*" Display="Dynamic" meta:resourcekey="rFcmbTypeEvalResource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblRemarques" runat="server" Text="Remarques" meta:resourcekey="LblRemarquesResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:TextBox ID="RemarquesTextBox" runat="server" Text='<%# Bind("Remarques") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="LblDateEval" runat="server" Text="Evaluation faite le" meta:resourcekey="LblDateEvalResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <telerik:RadDateTimePicker ID="rdpDateEval" runat="server" Width="250px"
                                        Culture="en-US" SelectedDate='<%# Bind("DateEval") %>' TimeView-TimeFormat="t"
                                        DateInput-DateFormat="dd/MM/yyyy h:mm tt" DateInput-DisplayDateFormat="dd/MM/yyyy h:mm tt">
                                        <TimeView Skin="Default" ShowHeader="False" Interval="00:30:00" Columns="6"></TimeView>
                                    </telerik:RadDateTimePicker>
                                    <asp:RequiredFieldValidator ID="RequiredRfvrdpDateEvalFieldValidator1" runat="server"
                                        ControlToValidate="rdpDateEval" ValidationGroup="defGrp" Text="*" ErrorMessage="*" ForeColor="Red"
                                        Display="Dynamic" meta:resourcekey="RequiredRfvrdpDateEvalFieldValidator1Resource1" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblSection" runat="server" Text="Section" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:DropDownList ID="cmbCourseSections" runat="server"
                                        DataTextField="SectionName" DataValueField="ID" Width="250px" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblQuiz" runat="server" Text="Quiz" meta:resourcekey="lblQuizResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:CheckBox ID="chkIsQuiz" runat="server" Checked='<%# Bind("IsQuiz") %>' Enabled="false" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblTotalDuration" runat="server" Text="Total Duration" meta:resourcekey="lblTotalDurationResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <telerik:RadNumericTextBox ID="txtTotalDuration" runat="server" MinValue="1"
                                        DbValue='<%# Bind("TotalDuration") %>' NumberFormat-DecimalDigits="0" ShowSpinButtons="true" />
                                </td>
                            </tr>                            
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblExamLocation" runat="server" Text="Exam Location" meta:resourcekey="lblExamLocationResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:TextBox ID="txtExamLocation" runat="server" Text='<%# Bind("ExamLocation") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblNbQuestionsPerPage" runat="server" Text="Nb. Questions Per Page" meta:resourcekey="lblNbQuestionsPerPageResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <telerik:RadNumericTextBox ID="txtNbQuestionsPerPage" runat="server" MinValue="1"
                                        DbValue='<%# Bind("NbQuestionsPerPage") %>' NumberFormat-DecimalDigits="0" ShowSpinButtons="true" />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblDateEcheance" runat="server" Text="Date echéance" meta:resourcekey="LblDueDateResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <telerik:RadDateTimePicker ID="rdpDateEcheance" runat="server" Width="250px"
                                        Culture="en-US" SelectedDate='<%# Bind("DateEcheance") %>' TimeView-TimeFormat="t"
                                        DateInput-DateFormat="dd/MM/yyyy h:mm tt" DateInput-DisplayDateFormat="dd/MM/yyyy h:mm tt">
                                        <TimeView Skin="Default" ShowHeader="False" Interval="00:30:00" Columns="6"></TimeView>
                                    </telerik:RadDateTimePicker>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblShuffleQuestions" runat="server" Text="Shuffle Questions" meta:resourcekey="lblShuffleQuestionsResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:CheckBox ID="chkShuffleQuestions" runat="server" Checked='<%# Bind("ShuffleQuestions") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="lblAllowReview" runat="server" Text="Allow Review" meta:resourcekey="lblAllowReviewResource2" />
                                </td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:CheckBox ID="chkAllowReview" runat="server" Checked='<%# Bind("AllowReview") %>' />
                                </td>
                            </tr>
                        </table>
                    </InsertItemTemplate>
                    <ItemTemplate>
                        <table>
                            <tr>
                                <asp:HiddenField runat="server" ID="HidID" Value='<%# Bind("ID") %>' />
                                <asp:HiddenField runat="server" ID="HidStatus" Value='<%# Eval("Status") %>' />
                                <asp:HiddenField runat="server" ID="HidIsQuiz" Value='<%# Eval("IsQuiz") %>' />
                                <asp:HiddenField runat="server" ID="HidCoordSave" Value='<%# Eval("CoordSave") %>' />
                                <asp:HiddenField runat="server" ID="HidCoordValidate" Value='<%# Eval("CoordValidate") %>' />
                                <asp:HiddenField runat="server" ID="HidCoordReject" Value='<%# Eval("CoordReject") %>' />
                                <asp:HiddenField runat="server" ID="HidPrincSave" Value='<%# Eval("PrincSave") %>' />
                                <asp:HiddenField runat="server" ID="HidPrincApprove" Value='<%# Eval("PrincApprove") %>' />
                                <asp:HiddenField runat="server" ID="HidPrincReject" Value='<%# Eval("PrincReject") %>' />
                                <asp:HiddenField runat="server" ID="HidIdClasseMatiereUser" Value='<%# Bind("Id_ClasseMatiereUser") %>' />
                                <td style="padding: 0px 10px 5px 0px" colspan="2">
                                    <strong>
                                        <asp:Label ID="LblClasseMatiere" runat="server" Visible='<%# Convert.ToInt32(Eval("Id_Classe")) > 0 %>'
                                            Text='<%# String.Format("{0}/{1}-{2} {3}", Eval("Classe"), Eval("Section"), Eval("Abr"), Eval("Matiere")) %>' />
                                        <asp:Label ID="LblClasseMatiere1" runat="server" Visible='<%# Convert.ToInt32(Eval("Id_Classe")) < 0 %>'
                                            Text='<%# String.Format("{0} {1}", Eval("Abr"), Eval("Matiere")) %>' />
                                    </strong>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="Label8" runat="server" Text="Type" meta:resourcekey="Label8Resource1" /></td>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="Label9" runat="server" Text='<%# Bind("EvaluationType") %>' meta:resourcekey="Label9Resource1" /></td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="LblNoteSur" runat="server" Text="Note sur" meta:resourcekey="LblNoteSurResource3" /></td>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="NoteSurLabel" runat="server" Text='<%# Bind("NoteSur") %>' meta:resourcekey="NoteSurLabelResource2" /></td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="LblCoeff" runat="server" Text="Coeff" meta:resourcekey="LblCoeffResource3" /></td>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="CoeffLabel" runat="server" Text='<%# Bind("Coeff") %>' meta:resourcekey="CoeffLabelResource2" /></td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="LblLibelle" runat="server" Text="Colonne" meta:resourcekey="LblLibelleResource3" /></td>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="LibelleLabel" runat="server" Text='<%# Bind("Libelle") %>' meta:resourcekey="LibelleLabelResource1" /></td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="LblRemarques" runat="server" Text="Remarques" meta:resourcekey="LblRemarquesResource3" /></td>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="RemarquesLabel" runat="server" Text='<%# Bind("Remarques") %>' meta:resourcekey="RemarquesLabelResource2" /></td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="LblDateEval" runat="server" Text="Evaluation faite le" meta:resourcekey="LblDateEvalResource3" /></td>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="DateEvalLabel" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}",Eval("DateEval")) %>' /></td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="LblDateSaisie" runat="server" Text="Saisie le" meta:resourcekey="LblDateSaisieResource2" /></td>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="DateSaisieLabel" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy hh:mm}",Eval("DateSaisie")) %>' /></td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="Label2" runat="server" Text="Ajoutée par" meta:resourcekey="Label2Resource2" /></td>
                            </tr>
                            <tr>
                                <td colspan="2"><asp:Label ID="Label3" runat="server" Text='<%# Bind("StatusName") %>' meta:resourcekey="Label3Resource2" /></td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="Label6" runat="server" Text="Coordinator status" meta:resourcekey="CoordiResource" /></td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label10" runat="server" Text='<%# Eval("CoordStatus") %>' />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="Label11" runat="server" Text='<%# Eval("CoordName") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="padding: 0px 10px 5px 0px"><asp:Label ID="Label12" runat="server" Text="Principal status" meta:resourcekey="DirecteurResource" /></td>
                                <td style="padding: 0px 10px 5px 0px">
                                    <asp:Label ID="Label13" runat="server" Text='<%# Eval("princStatus") %>' />
                                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Label ID="Label14" runat="server" Text='<%# Eval("PrincName") %>' />
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:LinkButton ID="EditButton" runat="server" CausesValidation="False"
                            CommandName="Edit" Text="Edit" meta:resourcekey="EditButtonResource1" />
                        &nbsp;
                        <asp:LinkButton ID="DeleteButton" runat="server" CausesValidation="False"
                            CommandName="Delete" Text="Delete" meta:resourcekey="DeleteButtonResource1" />
                    </ItemTemplate>
                </asp:FormView>
            </div>
            <div class="col-md-6" style="float: right">
                <asp:ListView ID="lstEvalProgres" runat="server" DataSourceID="odsEvalProgres">
                    <ItemTemplate>
                        <tr class="TableItemRow">
                            <td style="padding: 0px 10px 5px 0px"><asp:Label ID="lblDescType" runat="server" Text='<%# Eval("DescType") %>' /></td>
                            <td style="padding: 0px 10px 5px 0px"><asp:Label ID="lblGrade" runat="server" Text='<%# Eval("Grade") %>' /></td>
                            <td style="padding: 0px 10px 5px 0px"><asp:Label ID="lblBelowGrade" runat="server" Text='<%# Eval("BelowGrade") %>' /></td>
                            <td style="padding: 0px 10px 5px 0px"><asp:Label ID="lblAboveGrade" runat="server" Text='<%# Eval("AboveGrade") %>' /></td>
                        </tr>
                    </ItemTemplate>
                    <LayoutTemplate>
                        <table class="table table-bordered table-primary table-condensed">
                            <thead>
                                <tr>
                                    <th></th>
                                    <th></th>
                                    <th id="Th2" runat="server" style="text-align:center"><asp:Label ID="Label16" runat="server" Text="Below Average" meta:resourcekey="Label16Resource1"></asp:Label></th>
                                    <th id="Th1" runat="server" style="text-align:center"><asp:Label ID="Label17" runat="server" Text="Above Average" meta:resourcekey="Label17Resource1"></asp:Label></th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr id="itemPlaceholder" runat="server"></tr>
                            </tbody>
                        </table>
                    </LayoutTemplate>
                </asp:ListView>
                <telerik:RadHtmlChart ID="BarChart" runat="server" DataSourceID="odsEvalProgresChart" Height="150px">
                    <PlotArea>
                        <Series>
                            <telerik:BarSeries DataFieldY="BelowGrade" Stacked="true">
                                <LabelsAppearance Visible="false"></LabelsAppearance>
                                <Appearance FillStyle-BackgroundColor="Red"></Appearance>
                            </telerik:BarSeries>
                            <telerik:BarSeries DataFieldY="AboveGrade">
                                <LabelsAppearance Visible="false"></LabelsAppearance>
                                <Appearance FillStyle-BackgroundColor="#039BE5"></Appearance>
                            </telerik:BarSeries>
                        </Series>
                        <XAxis DataLabelsField="DescType"></XAxis>
                    </PlotArea>
                </telerik:RadHtmlChart>
            </div>
        </div>
    </div>
</div>

<asp:ObjectDataSource ID="odsEvalProgres" runat="server" TypeName="Dars.BLL.Online.Front.Comm.FrontComm" SelectMethod="GetEnt_EvaluationProgres">
    <SelectParameters>
        <asp:SessionParameter Name="Id_College" SessionField="Id_College" Type="Int32" />
        <asp:QueryStringParameter Name="Id_Evaluation" QueryStringField="eval" Type="Int32" />
        <asp:SessionParameter Name="Lang" SessionField="APPLANG" Type="String" />
        <asp:Parameter Name="AvecEcartType" DefaultValue="True" Type="Boolean" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="odsEvalProgresChart" runat="server" TypeName="Dars.BLL.Online.Front.Comm.FrontComm" SelectMethod="GetEnt_EvaluationProgres">
    <SelectParameters>
        <asp:SessionParameter Name="Id_College" SessionField="Id_College" Type="Int32" />
        <asp:QueryStringParameter Name="Id_Evaluation" QueryStringField="eval" Type="Int32" />
        <asp:SessionParameter Name="Lang" SessionField="APPLANG" Type="String" />
        <asp:Parameter Name="AvecEcartType" DefaultValue="False" Type="Boolean" />
    </SelectParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="odsEvaluation" runat="server" TypeName="Dars.BLL.Online.Front.Comm.FrontComm" SelectMethod="Get_EvaluationDetail"
    InsertMethod="InsertEnt_Evaluation" UpdateMethod="UpdateEnt_Evaluation" DeleteMethod="DeleteEnt_Evaluation">
    <DeleteParameters>
        <asp:Parameter Name="ID" Type="Int32" />
        <asp:Parameter Name="Id_College" Type="Int32" />
        <asp:SessionParameter Name="Lang" SessionField="APPLANG" Type="String" />
    </DeleteParameters>
    <UpdateParameters>
        <asp:Parameter Name="Id_Classe" Type="Int32" />
        <asp:Parameter Name="Id_Classe_Matiere" Type="Int32" />
        <asp:Parameter Name="Id_Type" Type="Int32" />
        <asp:Parameter Name="Old_IdType" Type="Int32" />
        <asp:Parameter Name="NoteSur" Type="Decimal" />
        <asp:Parameter Name="Coeff" Type="Decimal" />
        <asp:Parameter Name="NumColonne" Type="Int32" />
        <asp:Parameter Name="Old_NumColonne" Type="Int32" />
        <asp:Parameter Name="Remarques" Type="String" />
        <asp:Parameter Name="SessID" Type="Int32" />
        <asp:Parameter Name="DateEval" Type="DateTime" />
        <asp:Parameter Name="ID" Type="Int32" />
        <asp:Parameter Name="Id_College" Type="Int32" />
    </UpdateParameters>
    <SelectParameters>
        <asp:QueryStringParameter Name="Id_Evaluation" QueryStringField="eval" Type="Int32" />
        <asp:SessionParameter Name="Id_College" SessionField="Id_College" Type="Int32" />
        <asp:SessionParameter Name="Lang" SessionField="APPLANG" Type="String" />
    </SelectParameters>
    <InsertParameters>
        <asp:Parameter Name="Id_College" Type="Int32" />
        <asp:Parameter Name="Id_Classe_Matiere" Type="Int32" />
        <asp:Parameter Name="Id_Type" Type="Int32" />
        <asp:Parameter Name="Id_User" Type="Int32" />
        <asp:Parameter Name="NoteSur" Type="Decimal" />
        <asp:Parameter Name="Coeff" Type="Decimal" />
        <asp:Parameter Name="NumColonne" Type="Int32" />
        <asp:Parameter Name="Remarques" Type="String" />
        <asp:Parameter Name="SessID" Type="Int32" />
        <asp:Parameter Name="DateEval" Type="DateTime" />
        <asp:SessionParameter Name="AddedBy" SessionField="Id_User" Type="Int32" />
        <asp:Parameter Name="Id_Section" Type="Int32" />
    </InsertParameters>
</asp:ObjectDataSource>

<asp:ObjectDataSource ID="odsTypeEval" runat="server" SelectMethod="Get_EvaluationList" TypeName="Dars.BLL.Online.Front.Comm.FrontComm">
    <SelectParameters>
        <asp:SessionParameter Name="Id_College" SessionField="Id_College" Type="Int32" />
        <asp:SessionParameter Name="Annee" SessionField="Annee" Type="Int32" />
        <asp:Parameter Name="Id_EvaluationType" DefaultValue="0" Type="Int32" />
        <asp:Parameter Name="Id_Classe" DefaultValue="0" Type="Int32" />
        <asp:Parameter Name="Classe" Type="String" />
         <asp:Parameter Name="Id_Classe_Matiere" Type="Int32" />
        <asp:Parameter Name="ScreenName" DefaultValue="evaluationinsert" Type="String" />
        <asp:QueryStringParameter Name="utype" QueryStringField="utype" Type="String" />       
    </SelectParameters>
</asp:ObjectDataSource>