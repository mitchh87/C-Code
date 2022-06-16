<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ComposeSurveyQuestions.aspx.cs" Inherits="Dars.Web.Online.Mail.ComposeSurveyQuestions"MasterPageFile="~/Online/MasterPages/SiteMaster.Master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server" class="loader">
    <asp:Panel ID="PnlMain" runat="server">
        <div style="padding-top:1.25rem;padding-bottom:1.25rem">
            <div style="float:left">
                <asp:LinkButton ID="lnkBackk" runat="server" class="btn btn-white" OnClick="lnkBack_Click">
                    <i class="material-icons">arrow_back</i>
                </asp:LinkButton>
                <asp:LinkButton ID="ContinueButton" runat="server" class="btn btn-primary"
                    OnClick="ContinueButton_Click" Text="Continuer" meta:resourcekey="AddButtonResource1">     
                </asp:LinkButton>
            </div>
            <div class="card pull-xs-right">
                <asp:HyperLink runat="server" ID="lnkAdd" Text="Add Question" CssClass="btn btn-info-outline active" />
            </div>
        </div>        
        <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="Repeater1_ItemCommand">
            <ItemTemplate>
                <div class="card-group">
                    <div class="card" style="width:5%;text-align:center" runat="server" visible='<%# (Eval("isDone").ToString()=="0") %>'>
                        <asp:LinkButton ID="btnUp" runat="server" Enabled='<%# Convert.ToInt32(Eval("QuestionRank")) > 1 %>'
                            CommandName="up" CommandArgument='<%# Eval("QuestionRank") %>' style="margin-top:5px;"
                            class='<%# String.Format("label label-primary {0}", Convert.ToInt32(Eval("QuestionRank")) > 1 ? "" : "btn-disabled") %>'>
                            <i class="material-icons" style="color:white">keyboard_arrow_up</i>    
                        </asp:LinkButton>
                        <asp:LinkButton ID="btnDown" runat="server" Enabled='<%# Convert.ToInt32(Eval("QuestionRank")) < Convert.ToInt32(Eval("MaxQuestionRank")) %>'
                            CommandName="down" CommandArgument='<%# String.Format("{0}|{1}", Eval("QuestionRank"), Eval("MaxQuestionRank")) %>' style="margin-top:5px"
                            class='<%# String.Format("label label-primary {0}", Convert.ToInt32(Eval("QuestionRank")) < Convert.ToInt32(Eval("MaxQuestionRank")) ? "" : "btn-disabled") %>'>
                            <i class="material-icons" style="color:white">keyboard_arrow_down</i>    
                        </asp:LinkButton>
                    </div>
                    <%# Eval("ContentHTML") %>
                    <div class="card" style="width:5%;text-align:center">
                        <a class="btn btn-primary-outline btn-circle active" runat="server" visible='<%# (Eval("isDone").ToString()=="0") %>'
                            href='<%# String.Format("/Online/Mail/AddQuestion.aspx?mode=edit&q={0}&utype={1}&mail={2}&classe={3}&type={4}&from={5}&classematiere={6}&section={7}",
                                Eval("QuestionID"), Request["utype"], Request["mail"], Request["classe"],
                            Request["type"], Request["from"], Request["classematiere"], Request["section"]) %>'>
                            <i class="material-icons">edit</i>
                        </a>
                        <br />
                        <asp:LinkButton ID="btnDelete" runat="server" CommandName="deleteQuestion" CommandArgument='<%# Eval("QuestionID") %>'
                            class="btn btn-danger-outline btn-circle" visible='<%# (Eval("isDone").ToString()=="0") %>'>
                            <i class="material-icons">delete</i>    
                        </asp:LinkButton>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
</asp:Content>