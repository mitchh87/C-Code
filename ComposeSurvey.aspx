<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ComposeSurvey.aspx.cs" Inherits="Dars.Web.Online.Mail.ComposeSurvey" MasterPageFile="~/Online/MasterPages/SiteMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register TagPrefix="drs" TagName="Files" Src="~/Online/Mail/MailFiles.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server" class="loader">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script language="javascript" type="text/javascript">
            var fileList = null;

            function fileUploaded(sender, args) {
                var name = args.get_fileName();
                if (fileList == null) {
                    fileList = document.getElementById("exFileList");

                    fileList.style.display = "block";
                }
            }

            $(document).ready(function () {
                var LANG = '<%=Session["APPLANG"].ToString() %>';
                LoadClasses(LANG);
            });

            function LoadClasses(LANG) {
                var dsClasses = new kendo.data.DataSource({
                    schema: {
                        data: "d",
                        model: {
                            id: "Id_Classe",
                            fields: {
                                Id_Classe: { validation: { required: true } },
                                classe: { validation: { required: true } }
                            }
                        }
                    },
                    transport: {
                        read: {
                            url: "ComposeSurvey.aspx/Get_ListeClasse",
                            contentType: "application/json; charset=utf-8",
                            data: {
                                idUser: '<%=Session["Id_User"].ToString() %>',
                                idCollege: '<%=Session["Id_College"].ToString() %>',                   
                                annee: '<%=Session["Annee"].ToString() %>'
                            },
                            dataType: "json",
                            type: "POST"
                        },

                        parameterMap: function (data, operation) {
                            data = $.extend({ sort: null, filter: null }, data);
                            return JSON.stringify(data);
                        }
                    }
                });

                $("#multiselectClasses").kendoMultiSelect({
                    dataTextField: "classe",
                    dataValueField: "Id_Classe",
                    autoClose: false,
                    dataSource: dsClasses,
                    close: onChangeClasses,
                    placeholder: LANG == "EN" ? "  Classe(s) ...  " : LANG == "FR" ? "  Classe(s) ...  " : "  ... (الصف(وف  ",
                    filter: "contains",
                    change: onChangeClasses,
                });

                var multiselectClasses = $("#multiselectClasses").data("kendoMultiSelect");

                if (document.getElementById('ctl00_MainContent_hidClasses')) {
                    multiselectClasses.value(document.getElementById('<%= hidClasses.ClientID %>').value.split(","));
                    multiselectClasses.input.blur();
                    multiselectClasses.list.width(300);
                }
            }

            function onChangeClasses() {
                if ($("#multiselectClasses").data("kendoMultiSelect").value() == '')
                    document.getElementById('<%= hidClasses.ClientID %>').value = "-999";
                else
                    document.getElementById('<%= hidClasses.ClientID %>').value = $("#multiselectClasses").data("kendoMultiSelect").value();
            }
        </script>

        <style>
            .k-button {
                text-align: left;
            }

            /* width */
            .scrollable::-webkit-scrollbar {
                width: 15px;
            }

            /* Track */
            .scrollable::-webkit-scrollbar-track {
                box-shadow: inset 0 0 5px grey;
                border-radius: 10px;
            }
 
            /* Handle */
            .scrollable::-webkit-scrollbar-thumb {
                background: #039BE5;
                border-radius: 10px;
            }

            /* Handle on hover */
            .scrollable::-webkit-scrollbar-thumb:hover {
                background: #0487c4;
            }

            .rcbInner{
                font-size: 12px;
            }

            .rcbItem {
                font-size: 12px;
            }

            .rcbHovered {
                font-size: 12px;
            }
        </style>
    </telerik:RadCodeBlock>
    <asp:Panel ID="PnlMain" runat="server">
        <br />
        <asp:LinkButton ID="lnkBackk" runat="server" class="btn btn-white" OnClick="lnkBack_Click">
            <i class="material-icons">arrow_back</i>
        </asp:LinkButton>
        <asp:LinkButton ID="ContinueButton" runat="server" class="btn btn-primary" CausesValidation="true" ValidationGroup="Grp1"
            OnClick="ContinueButton_Click" Text="Continuer" meta:resourcekey="AddButtonResource1">     
        </asp:LinkButton>
        <asp:ValidationSummary ID="valSummary" runat="server" ValidationGroup="Grp1" />
        <asp:HiddenField ID="hidTread" runat="server" />
        <br />
        <br />
        <div class="tab-pane active box-generic">
            <div class="row">
                <div class="col-md-6">
                    <div class="col-md-12">
                        <asp:Label ID="lblSubject" runat="server" class="col-sm-1 form-control-label"
                            Width="150px" Text="Sujet : " meta:resourcekey="lblSubjectResource1" />
                        <div class="col-sm-7">
                            <asp:TextBox ID="txtSubject" runat="server" class="form-control" autocomplete="off" />
                        </div>
                        <div class="col-sm-1">
                            <asp:RequiredFieldValidator ID="rv1" runat="server" ControlToValidate="txtSubject" InitialValue=""
                                ValidationGroup="Grp1" Text="*" ErrorMessage="Subject is Required" Display="Dynamic" ForeColor="Red" />
                        </div>
                    </div>
                    <div class="col-md-12" style="padding-top: 10px">
                        <asp:Label ID="LblDate" runat="server" class="col-sm-2 form-control-label" Text="Date : " Width="150px" />
                        <div class="col-sm-7">
                            <telerik:RadDateTimePicker ID="rdpDate" runat="server" RenderMode="Auto" Culture="en-US" Width="100%"
                                TimeView-TimeFormat="t" DateInput-DateFormat="dd/MM/yyyy h:mm tt" DateInput-DisplayDateFormat="dd/MM/yyyy h:mm tt">
                                <TimeView Skin="Default" ShowHeader="False" Interval="00:30:00" Columns="6"></TimeView>
                            </telerik:RadDateTimePicker>                            
                        </div>
                        <div class="col-sm-1">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="rdpDate" InitialValue=""
                                ValidationGroup="Grp1" Text="*" ErrorMessage="Date is Required" Display="Dynamic" ForeColor="Red" />
                        </div>
                    </div>
                    <div class="col-md-12" style="padding-top: 10px">
                        <asp:Label ID="lblEndTime" runat="server" class="col-sm-2 form-control-label"
                            Width="150px" Text="Date fin : " meta:resourcekey="lblEndTimeResource1" />
                        <div class="col-sm-7">
                            <telerik:RadDateTimePicker ID="rdtEndTime" runat="server" RenderMode="Auto" Culture="en-US" Width="100%"
                                TimeView-TimeFormat="t" DateInput-DateFormat="dd/MM/yyyy h:mm tt" DateInput-DisplayDateFormat="dd/MM/yyyy h:mm tt">
                                <TimeView Skin="Default" ShowHeader="False" Interval="00:30:00" Columns="6"></TimeView>
                            </telerik:RadDateTimePicker>                            
                        </div>
                        <div class="col-sm-1">
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="rdtEndTime" InitialValue=""
                                ValidationGroup="Grp1" Text="*" ErrorMessage="End date is Required" Display="Dynamic" ForeColor="Red" />
                        </div>
                    </div>
                    <div class="col-md-12">
                        <asp:CheckBox ID="ChkAllowReview" class="col-sm-4 form-control-label" runat="server"
                            Text="révision autorisée" meta:resourcekey="ChkAllowReviewResource1" />
                    </div>
                </div>
                <div class="col-md-6 scrollable" style="max-height:400px;overflow-y:auto;">
                    <div class="col-md-12">
                        <div id="divClasses" runat="server" class="col-sm-12" style="padding-bottom:10px">
                            <asp:HiddenField ID="hidClasses" runat="server" Value="-999" />
                            <select id="multiselectClasses" style="width:88%;float:left"></select>
                            <div style="float:left;padding-left:10px">
                                <asp:LinkButton ID="btnRefresh" runat="server" class="btn btn-sm btn-success" OnClick="btnRefresh_Click" Text="Ok" />
                            </div>
                        </div>
                        <div class="col-sm-12">
                            <asp:ListView ID="lstClasseMatiere" runat="server" OnDataBound="lstClasseMatiere_DataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td><asp:CheckBox ID="chkSelected" runat="server" /></td>
                                        <td>
                                            <asp:HiddenField ID="HidValue" runat="server" Value='<%# Bind("Value") %>' />
                                            <small><%# Eval("Display") %></small>
                                        </td>
                                        <td>
                                            <telerik:RadComboBox ID="cmbSections" runat="server" RenderMode="Auto"
                                                DataTextField="Display" DataValueField="Value" MaxHeight="250px" Width="80%" />
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <EmptyDataTemplate>
                                    <table id="Table1" runat="server" class="table table-bordered table-vertical-center table-striped">
                                        <tr runat="server">
                                            <td runat="server">
                                                <asp:Label ID="lblEmpty" runat="server" Text="Aucune matière trouvée." meta:resourcekey="lblEmpty1" />
                                            </td>
                                        </tr>
                                    </table>
                                </EmptyDataTemplate>
                                <LayoutTemplate>
                                    <table class="table table-bordered table-primary table-condensed">
                                        <thead>
                                            <tr>
                                                <th style="width:5%"></th>
                                                <th style="width:55%">Matière</th>
                                                <th style="width:40%">Section</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr id="itemPlaceholder" runat="server"></tr>
                                        </tbody>
                                    </table>
                                </LayoutTemplate>
                            </asp:ListView>
                        </div>
                    </div>
                </div>
            </div>
            <drs:Files runat="server" ID="CtrlFiles" />
            <div class="jumbotron">
                <div class="row">
                    <div class="col-md-12">
                        <asp:Label ID="lblAttachment" runat="server" Text="Attachments" meta:resourcekey="lblAttachmentResource1" />
                        <telerik:RadAsyncUpload runat="server" ID="AsyncUpload1"
                            MultipleFileSelection="Automatic" OnFileUploaded="AsyncUpload1_FileUploaded" />

                    </div>
                </div>
            </div>
            <div>
                <telerik:RadEditor ID="txtBody" runat="server" Width="100%" Style="height: 500px" EnableResize="false"
                    EditModes="Design,Html" RenderMode="Auto" DialogsCssFile="~/RadControls/Editor/Style.css"
                    ConfigFile="~/RadControls/Editor/ConfigFile.xml" ToolsFile="~/RadControls/Editor/ToolsFile.xml">
                    <Content></Content>
                    <DocumentManager EnableAsyncUpload="true" MaxUploadFileSize="10485760" />
                    <ImageManager EnableAsyncUpload="true" MaxUploadFileSize="10485760" />
                </telerik:RadEditor>
            </div>
        </div>
    </asp:Panel>
</asp:Content>