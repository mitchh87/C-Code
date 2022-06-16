<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Online/MasterPages/SiteMaster.Master" CodeBehind="evalEleves.aspx.cs" Inherits="Dars.Web.Online.Eval.evalEleves" Culture="auto" meta:resourcekey="PageResource1" UICulture="auto" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="eval" TagName="evalDet" Src="~/Online/Eval/evalDetailCtrl.ascx" %>
<%@ Register TagPrefix="eval" TagName="evalComment" Src="~/Online/Eval/evalEvaluationComments.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">
            function onlnkModifieClicked(id, note) {
                var ajaxManager = $find("<%= RadAjaxManager.GetCurrent(Page).ClientID %>");
                var cwd = $find("<%=openwindow.ClientID %>");
                cwd.setUrl("evalElevesModif.aspx?id=" + id + "&note=" + note);
                cwd.Show();
                return false;
            }

            function wndClientClose(sender, args) {
                var arg = args.get_argument();
                if (arg) {
                    var ajaxManager = $find("<%= RadAjaxManager.GetCurrent(Page).ClientID %>");
                    ajaxManager.ajaxRequest(arg);
                }
            }
        </script>
    </telerik:RadCodeBlock>
    <telerik:RadWindow runat="server" ID="openwindow" Width="650" Height="400" Behaviors="Maximize, close"
        VisibleOnPageLoad="false" Modal="true" OnClientClose="wndClientClose" Style="z-index: 9999">
    </telerik:RadWindow>
    <telerik:RadAjaxLoadingPanel ID="loadPanel" runat="server" Skin="Default"></telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxPanel ID="ajaxPanel" runat="server" LoadingPanelID="loadPanel" HorizontalAlign="NotSet">
        <table runat="server" id="tblToolBar">
            <tr id="ToolBar" runat="server">
                <td style="padding:0px 5px 0px 0px">
                    <asp:LinkButton ID="lnkBackToList" runat="server" class="btn btn-white" OnClick="lnkBackToList_Clicked">
                        <i class="material-icons">arrow_back</i>
                        <i></i><asp:Label runat="server" Text="Back" ID="lblBack" meta:resourcekey="lblBackResource1" />
                    </asp:LinkButton>
                </td>
                <td style="padding:0px 5px 0px 0px">
                    <asp:LinkButton ID="lnkModify" runat="server" class="btn btn-primary" OnClick="lnkModify_Clicked">
                        <i></i><asp:Label runat="server" Text="Modify" ID="Label2" meta:resourcekey="lnkModifieResource1" />
                    </asp:LinkButton>
                </td>
                <td style="padding:0px 5px 0px 0px">
                    <asp:LinkButton ID="lnkDetail" runat="server" class="btn btn-warning" OnClick="lnkDetail_Clicked"
                        Visible='<%# ((Session["ProfIsPrincipal"].ToString().ToLower()=="true")||(Session["TypeISIS"].ToString().ToLower()=="admin")) %>'>
                        <i></i><asp:Label ID="Label3" runat="server" Text="Detail" meta:resourcekey="lnkDetailResource1" />
                    </asp:LinkButton>
                </td>
                <td style="padding:0px 5px 0px 0px">
                    <asp:LinkButton ID="lnkQuiz" runat="server" class="btn btn-primary" OnClick="lnkQuiz_Click">
                        <i class="material-icons">cloud</i>
                        <asp:Label ID="Label4" runat="server" Text="Quiz" />
                    </asp:LinkButton>
                </td>
                <td style="padding:0px 5px 0px 0px">
                    <asp:HyperLink ID="LnkAdminEdit" runat="server" class="btn btn-primary">
                        <i class="material-icons">edit</i>
                        <asp:Label runat="server" ID="lblAdminEdit" Text="Edit" meta:resourcekey="lblAdminEditResource1" />
                    </asp:HyperLink>
                </td>
            </tr>
        </table>
        <eval:evalDet runat="server" ID="EvalDetail" />
        <table id="table1" runat="server">
            <tr>
                <td style="padding-left:20px;">
                    <asp:RadioButton ID="chkByGrade" runat="server" Text="Show by grade" AutoPostBack="true"
                        OnCheckedChanged="rbByGrade_CheckedChanged" GroupName="grp1" Checked="true" />
                </td>
                <td style="padding-left:20px;">
                    <asp:RadioButton ID="chkByLetter" runat="server" Text="Show by letter" AutoPostBack="true"
                        OnCheckedChanged="rbByGrade_CheckedChanged" GroupName="grp1" />
                </td>
            </tr>
        </table>
        <br />
        <div class="simplebar-content">
            <div class="container-fluid">
                <div class="row">
                    <div class="col-md-7">
                        <table>
                            <tr>
                                <td>
                                    <telerik:RadGrid ID="MainGrid" runat="server" AutoGenerateColumns="False" DataSourceID="odsEleveEval"
                                        GridLines="None" EnableAJAX="true" CellSpacing="0" CellPadding="0"
                                        OnDataBound="MainGrid_DataBound" OnItemCommand="MainGrid_ItemCommand">
                                        <MasterTableView AllowCustomPaging="True" AllowPaging="True" DataSourceID="odsEleveEval"
                                            EditMode="InPlace" GridLines="None" PageSize="50">
                                            <CommandItemSettings ExportToPdfText="Export to PDF" />
                                            <RowIndicatorColumn FilterControlAltText="Filter RowIndicator column"></RowIndicatorColumn>
                                            <ExpandCollapseColumn FilterControlAltText="Filter ExpandColumn column"></ExpandCollapseColumn>
                                            <Columns>
                                                
                                                <telerik:GridBoundColumn DataField="RecNo" FilterControlAltText="Filter Matiere column" HeaderText="No." UniqueName="RecNo" />
                                                <telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn column" HeaderText="Elève"
                                                    meta:resourceKey="GridTemplateColumnResource1" UniqueName="TemplateColumn">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblLongDisplayName" runat="server" Text='<%# Eval("LongDisplayName") %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn DataField="OldNote" HeaderText="" SortExpression="" meta:resourceKey="GridTemplateColumnResource111">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblOldNote" runat="server" Text='<%# Eval("OldNote") %>' />
                                                        <asp:HiddenField ID="HidOldNote_Lettre" runat="server" Value='<%# Bind("OldNote_Lettre") %>' />
                                                        <asp:Label ID="lblProgresPositif" runat="server" Visible='<%# Eval("Note") != null && Eval("OldNote") != null && (Convert.ToDecimal(Eval("Note")) - Convert.ToDecimal(Eval("OldNote")) >= 0) %>'
                                                            CssClass="label label-pill label-success m-b-05" Text='<%# String.Format("+{0}", Convert.ToDecimal(Eval("Note")) - Convert.ToDecimal(Eval("OldNote"))) %>' />
                                                        
                                                        <asp:Label ID="lblProgresNegatif" runat="server" Visible='<%# Eval("Note") != null && Eval("OldNote") != null && (Convert.ToDecimal(Eval("Note")) - Convert.ToDecimal(Eval("OldNote")) < 0) %>'
                                                            CssClass="label label-pill label-danger m-b-05" Text='<%# Convert.ToDecimal(Eval("Note")) - Convert.ToDecimal(Eval("OldNote")) %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn FilterControlAltText="Filter TemplateColumn1 column"
                                                    HeaderText="Note" meta:resourceKey="GridTemplateColumnResource2" UniqueName="TemplateColumn1">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="HidID_EEval" runat="server" Value='<%# Bind("ID_EEval") %>' />
                                                        <asp:HiddenField ID="HidId_Eleve" runat="server" Value='<%# Bind("ID_Eleve") %>' />
                                                        <asp:HiddenField ID="HiddColor" runat="server" Value='<%# Eval("Color") %>' />
                                                        <asp:HiddenField ID="HiddLettre" runat="server" Value='<%# Bind("Lettre") %>' />
                                                        <asp:HiddenField ID="HidOld_LibelleSousColonne" runat="server" Value='<%# Eval("Old_LibelleSousColonne") %>' />
                                                        <asp:HiddenField ID="HidSaved" runat="server" />
                                                        <asp:TextBox ID="TxtNote" runat="server" Font-Bold="True" Text='<%# Bind("Note") %>' Width="50%" />
                                                        <asp:LinkButton ID="lnkResults" runat="server" class="btn btn-primary-outline btn-circle active"
                                                            Visible='<%# Convert.ToInt32(Eval("IsQuiz")) == 1 %>'
                                                            CommandName="ShowResult" CommandArgument='<%# Eval("Id_Eleve") %>'>
                                                            <i class="material-icons" style="color:white">visibility</i>
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn DataField="CurrentNote" HeaderText="" SortExpression="" meta:resourceKey="GridTemplateColumnResource222">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="HiddCurrentNote_Lettre" runat="server" Value='<%# Bind("CurrentNote_Lettre") %>' />
                                                        <asp:Label ID="lblCurrentNote" runat="server" Text='<%# Eval("CurrentNote") %>' />
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                            <EditFormSettings>
                                                <EditColumn FilterControlAltText="Filter EditCommandColumn column"></EditColumn>
                                            </EditFormSettings>
                                            <PagerStyle AlwaysVisible="True" />
                                        </MasterTableView>
                                        <PagerStyle ShowPagerText="False" AlwaysVisible="True" FirstPageImageUrl="~/Styles/Images/first.png"
                                            LastPageImageUrl="~/Styles/Images/last.png" NextPageImageUrl="~/Styles/Images/next.png"
                                            PrevPageImageUrl="~/Styles/Images/previous.png" PageButtonCount="3" />
                                        <FilterMenu EnableImageSprites="False">
                                        </FilterMenu>
                                        <HeaderContextMenu CssClass="GridContextMenu GridContextMenu_WebBlue">
                                        </HeaderContextMenu>
                                    </telerik:RadGrid>
                                </td>
                                <td style="vertical-align: top;padding-left:10px" runat="server" id="tdExcel">
                                    <img src="../../images/filetypeicon/excel.png" />
                                    <br />
                                    Copy paste
                                    <br />
                                    from Excel
                                    <br />
                                    <asp:TextBox runat="server" ID="TxtExcel" TextMode="MultiLine" Height="450px" Width="80px" meta:resourcekey="TxtExcelResource1" />
                                    <br />
                                    <asp:Button runat="server" ID="BtnImport" OnClick="BtnImport_Click" Text="Importer" class="btn btn-success" meta:resourcekey="BtnImportResource1" />
                                </td>
                            </tr>
                        </table>
                        <asp:Button ID="BtnSave" runat="server" OnClick="BtnSave_Click" Text="Save"
                            class="btn btn-primary" meta:resourcekey="BtnSaveResource1" />
                        <asp:LinkButton ID="BtnSubmit" Text="Submit" runat="server" class="btn btn-icon btn-primary"
                            OnClick="BtnSubmit_Click" meta:resourcekey="BtnSubmitResource1" />
                        <asp:LinkButton ID="BtnValidate" runat="server" class="btn btn-icon btn-success glyphicons ok_2" OnClick="BtnValidate_Click">
                            <i></i><asp:Label runat="server" Text="Validate" ID="Label1" meta:resourcekey="LblValidateResource1" />
                        </asp:LinkButton>
                        <asp:LinkButton ID="BtnApprove" runat="server" class="btn btn-icon btn-success glyphicons ok_2" OnClick="BtnApprove_Click">
                            <i></i><asp:Label runat="server" Text="Approve" ID="LblApprove" meta:resourcekey="LblApproveResource1" />
                        </asp:LinkButton>
                        <asp:LinkButton ID="BtnReject" runat="server" class="btn btn-icon btn-warning glyphicons unshare" OnClick="BtnReject_Click">
                            <i></i><asp:Label runat="server" Text="Reject" ID="LblReject" meta:resourcekey="LblRejectResource1" />
                        </asp:LinkButton>
                        <br /><br />
                        <eval:evalComment runat="server" ID="lstevalComment" />
                    </div>
                    <div class="col-md-5">
                        <telerik:RadHtmlChart runat="server" ID="RadHtmlChart1" DataSourceID="odsEvaluationDashboard" Skin="Office2007">
                            <PlotArea>
                                <Appearance>
                                    <FillStyle BackgroundColor="White"></FillStyle>
                                </Appearance>
                                <Series>
                                    <telerik:PieSeries DataFieldY="Evaluation_Grade" NameField="Note" ExplodeField="CurrentTerm_Grade">
                                        <LabelsAppearance Position="OutsideEnd" DataFormatString="{0} %"></LabelsAppearance>
                                        <TooltipsAppearance>
                                            <ClientTemplate>Grade: #=dataItem.Note# </br> Percentage: #=dataItem.Evaluation_Grade# %</ClientTemplate>
                                        </TooltipsAppearance>
                                    </telerik:PieSeries>
                                </Series>
                            </PlotArea>
                        </telerik:RadHtmlChart>
                    </div>
                </div>
            </div>
        </div>
        <telerik:RadHtmlChart runat="server" ID="RadHtmlChart2" Skin="Bootstrap" DataSourceID="odsEvaluationDashboardAll">
            <PlotArea>
                <Series>
                    <telerik:ColumnSeries DataFieldY="Evaluation_Grade" Name="Current Evaluation" meta:resourcekey="lblEvaluation_GradeResource1"></telerik:ColumnSeries>
                    <telerik:ColumnSeries DataFieldY="CurrentTerm_Grade" Name="Current Term(Final Grade)" meta:resourcekey="lblCurrentTerm_GradeResource1" ></telerik:ColumnSeries>
                    <telerik:ColumnSeries DataFieldY="PreviousTerm_Grade" Name="Previous Term(Final Grade)" meta:resourcekey="lblPreviousTerm_GradeResource1"></telerik:ColumnSeries>
                </Series>
                <XAxis DataLabelsField="Note">
                    <MinorGridLines Visible="false"></MinorGridLines>
                    <MajorGridLines Visible="false"></MajorGridLines>
                </XAxis>
                <YAxis Step="1">
                    <MinorGridLines Visible="false"></MinorGridLines>
                </YAxis>
            </PlotArea>
            <Legend>
                <Appearance Position="Bottom"><TextStyle FontSize="18" /></Appearance>
            </Legend>
        </telerik:RadHtmlChart>
  
        <asp:ObjectDataSource ID="odsEleveEval" runat="server" TypeName="Dars.BLL.Online.Front.Comm.FrontComm" SelectMethod="Get_EleveEval">
            <SelectParameters>
                <asp:QueryStringParameter Name="Id_Evaluation" QueryStringField="eval" Type="Int32" />
                <asp:QueryStringParameter Name="Id_Classe" QueryStringField="classe" Type="Int32" />
                <asp:SessionParameter Name="Id_College" SessionField="Id_College" Type="Int32" />
                <asp:QueryStringParameter Name="uType" QueryStringField="type" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>

        <asp:ObjectDataSource ID="odsEvaluationDashboard" runat="server" TypeName="Dars.BLL.Online.Front.Comm.FrontComm" SelectMethod="GetEnt_EvaluationDashboard">
            <SelectParameters>
                <asp:SessionParameter Name="Id_College" SessionField="Id_College" Type="Int32" />
                <asp:QueryStringParameter Name="Id_Evaluation" QueryStringField="eval" Type="Int32" />
                <asp:Parameter Name="ShowPrevious" DefaultValue="False" Type="String" />
                <asp:QueryStringParameter Name="uType" QueryStringField="type" Type="String" />
                
            </SelectParameters>
        </asp:ObjectDataSource>

        <asp:ObjectDataSource ID="odsEvaluationDashboardAll" runat="server" TypeName="Dars.BLL.Online.Front.Comm.FrontComm" SelectMethod="GetEnt_EvaluationDashboard">
            <SelectParameters>
                <asp:SessionParameter Name="Id_College" SessionField="Id_College" Type="Int32" />
                <asp:QueryStringParameter Name="Id_Evaluation" QueryStringField="eval" Type="Int32" />
                <asp:Parameter Name="ShowPrevious" DefaultValue="True" Type="String" />
                <asp:QueryStringParameter Name="uType" QueryStringField="type" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </telerik:RadAjaxPanel>
</asp:Content>
