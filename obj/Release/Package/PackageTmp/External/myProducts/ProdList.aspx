<%@ Page Title="" Language="C#" MasterPageFile="~/External/MasterPage.Master" AutoEventWireup="true" CodeBehind="ProdList.aspx.cs" Inherits="External.myProducts.ProdList" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Sub Header Start -->
    <div class="grey lighten-3">
        <div class="container">
            <div class="row">
                <div class="col s12 m12 l12">
                    <h5 class="breadcrumbs-title">產品列表</h5>
                    <ol class="breadcrumb">
                        <li><a href="<%=PageUrl %>">產品資料</a></li>
                        <li class="active">產品列表</li>
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
                <li><a class="btn-floating blue" title="新增"><i class="material-icons">create</i></a></li>
                <li><a class="btn-floating green search-trigger" title="資料篩選" href="#search-modal"><i class="material-icons">search</i></a></li>
            </ul>
        </div>
        <!-- 浮動按鈕選單 End -->

        <!-- // Search Modal Start // -->
        <div id="search-modal" class="modal bottom-sheet">
            <div class="modal-content">
                <h5>資料篩選</h5>
                <div class="row">
                    <div class="input-field col s12">
                        <asp:TextBox ID="tb_Keyword" runat="server" placeholder="輸入關鍵字:品號,品名" autocomplete="off"></asp:TextBox>
                        <label for="MainContent_tb_Keyword">關鍵字查詢</label>
                    </div>
                </div>
                <div class="row">
                    <div class="col s12 right-align">
                        <a id="trigger-Search" class="btn waves-effect waves-light green"><i class="material-icons">search</i></a>
                        <asp:Button ID="btn_Search" runat="server" Text="Search" CssClass="btn btn-large waves-effect waves-light blue" OnClick="btn_Search_Click" Style="display: none;" />
                    </div>
                </div>
            </div>
        </div>
        <!-- // Search Modal End // -->

        <div class="section">
            <div class="row">
                <div class="col s12">
                    <asp:ListView ID="lvDataList" runat="server" ItemPlaceholderID="ph_Items">
                        <LayoutTemplate>
                            <table class="bordered highlight">
                                <thead>
                                    <tr>
                                        <th>品號</th>
                                        <th>品名</th>
                                        <th class="center-align">Vol</th>
                                        <th class="center-align">Page</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:PlaceHolder ID="ph_Items" runat="server" />
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td class="right-align" colspan="4">
                                            <asp:Literal ID="lt_Pager" runat="server"></asp:Literal>
                                        </td>
                                    </tr>
                                </tfoot>
                            </table>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><a href="<%=Application["Web_Url"] %>myProducts/ProdView.aspx?DataID=<%#Server.UrlEncode(Eval("ModelNo").ToString()) %>"><%#Eval("ModelNo") %></a></td>
                                <td>
                                    <%#Eval("Name_TW") %>
                                </td>
                                <td class="center-align">
                                    <%#Eval("Vol") %>
                                </td>
                                <td class="center-align">
                                    <%#Eval("Page") %>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="card-panel red darken-1">
                                <i class="material-icons flow-text white-text">error_outline</i>
                                <span class="flow-text white-text">找不到產品資料</span>
                            </div>
                        </EmptyDataTemplate>

                    </asp:ListView>
                </div>

            </div>
        </div>
    </div>
    <!-- Body Content End -->
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        $(function () {
            //偵測Keypress(Enter) - Search
            $("#MainContent_tb_Keyword").keypress(function (e) {
                code = (e.keyCode ? e.keyCode : e.which);
                if (code == 13) {
                    $("#MainContent_btn_Search").trigger("click");

                    e.preventDefault();
                }
            });

            //觸發查詢
            $("#trigger-Search").click(function () {
                $("#MainContent_btn_Search").trigger("click");
            });

            //Search Modal
            $(".search-trigger").leanModal();

        });

    </script>
</asp:Content>
