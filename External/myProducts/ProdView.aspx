<%@ Page Title="" Language="C#" MasterPageFile="~/External/MasterPage.Master" AutoEventWireup="true" CodeBehind="ProdView.aspx.cs" Inherits="External.myProducts.ProdView" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">產品明細</h5>
                    <ol class="breadcrumb">
                        <li><a href="<%=PageUrl %>">產品資料</a></li>
                        <li class="active">產品明細</li>
                    </ol>
                </div>
            </div>
        </div>
    </div>
    <!-- Sub Header End -->
    <!-- Body Content Start -->
    <div class="container">
        <!-- 浮動按鈕選單 Start -->
        <div class="fixed-action-btn" style="bottom: 45px; right: 24px;">
            <a class="btn-floating btn-large red">
                <i class="material-icons">menu</i>
            </a>
            <ul>
                <li><a class="btn-floating blue" title="編輯"><i class="material-icons">create</i></a></li>
                <li><a class="btn-floating green" title="返回列表" href="<%=Application["Web_Url"] %>myProducts/ProdList.aspx"><i class="material-icons">list</i></a></li>
                <li><a class="btn-floating grey" title="置頂" href="#top"><i class="material-icons">keyboard_arrow_up</i></a></li>
            </ul>
        </div>
        <!-- 浮動按鈕選單 End -->
        <asp:PlaceHolder ID="ph_Message" runat="server">
            <div class="card-panel red darken-1">
                <i class="material-icons flow-text white-text">error_outline</i>
                <span class="flow-text white-text">找不到相關資料</span>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="ph_Data" runat="server">
            <div class="row">
                <div class="col s12 m6">
                    <div class="card">
                        <div class="card-image">
                            <img src="http://api.prokits.com.tw/myProd/<%=Req_DataID %>/" alt="產品圖" class="materialboxed" data-caption="<%=Req_DataID %>" />
                        </div>
                        <div class="card-content">
                            <div class="card-title">
                                <div class="grey-text text-darken-4">
                                    <asp:Literal ID="lt_ModelName" runat="server"></asp:Literal>
                                </div>
                                <div class="green-text text-darken-4">
                                    <asp:Literal ID="lt_ModelNo" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col s12 m6">
                    <ul class="collection with-header">
                        <li class="collection-header cyan darken-1">
                            <h5 class="white-text">基本資料</h5>
                        </li>
                        <li class="collection-item">
                            <span class="title grey-text text-darken-1">英文品名</span>
                            <p>
                                <asp:Literal ID="lt_NameEN" runat="server"></asp:Literal>
                            </p>
                        </li>
                        <li class="collection-item">
                            <span class="title grey-text text-darken-1">簡中品名</span>
                            <p>
                                <asp:Literal ID="lt_NameCN" runat="server"></asp:Literal>
                            </p>
                        </li>

                        <li class="collection-item">
                            <span class="title grey-text text-darken-1">上市日期</span>
                            <p>
                                <asp:Literal ID="lt_OpenDate" runat="server"></asp:Literal>
                            </p>
                        </li>
                        <li class="collection-item">
                            <span class="title grey-text text-darken-1">停售日期</span>
                            <p>
                                <asp:Literal ID="lt_CloseDate" runat="server"></asp:Literal>
                            </p>
                        </li>
                        <li class="collection-item">
                            <span class="title grey-text text-darken-1">目錄</span>
                            <p>
                                <asp:Literal ID="lt_Vol" runat="server"></asp:Literal>
                            </p>
                        </li>
                        <li class="collection-item">
                            <span class="title grey-text text-darken-1">頁次</span>
                            <p>
                                <asp:Literal ID="lt_Page" runat="server"></asp:Literal>
                            </p>
                        </li>
                    </ul>
                </div>
            </div>
            <%--<div class="row">
            <div class="col s12 m6">
                <ul class="collection with-header">
                    <li class="collection-header cyan darken-1">
                        <h5 class="white-text">產品明細</h5>
                    </li>
                    <li class="collection-item">
                        <span class="title grey-text text-darken-1">英文品名</span>
                        <p>
                            7Pcs Folding Type Hex Key Set
                        </p>
                    </li>
                    <li class="collection-item">
                        <span class="title grey-text text-darken-1">簡中品名</span>
                        <p>
                            7Pcs Folding Type Hex Key Set
                        </p>
                    </li>


                    <li class="collection-item">
                        <span class="title grey-text text-darken-1">上市日期</span>
                        <p>
                            7Pcs Folding Type Hex Key Set
                        </p>
                    </li>
                    <li class="collection-item">
                        <span class="title grey-text text-darken-1">停售日期</span>
                        <p>
                            2009-05-18
                        </p>
                    </li>
                    <li class="collection-item">
                        <span class="title grey-text text-darken-1">目錄</span>
                        <p>
                            5
                        </p>
                    </li>
                    <li class="collection-item">
                        <span class="title grey-text text-darken-1">頁次</span>
                        <p>
                            21
                        </p>
                    </li>

                </ul>
            </div>
        </div>--%>
        </asp:PlaceHolder>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="server">
</asp:Content>
