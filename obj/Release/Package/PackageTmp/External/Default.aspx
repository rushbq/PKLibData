<%@ Page Title="" Language="C#" MasterPageFile="~/External/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="External.Default" %>

<asp:Content ID="myCss" ContentPlaceHolderID="CssContent" runat="Server">
</asp:Content>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <div class="row">
            <div class="col s6 section">
                <div class="card hoverable">
                    <div class="card-content">
                        <span class="card-title">暢聯BBC</span>
                        <p>Prototype</p>
                    </div>
                    <div class="card-action">
                        <a href="http://demo.prokits.com.tw/Prototype/1_0__excel.html" target="_blank">View</a>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <div class="col s12 section">
                <div class="card hoverable">
                    <div class="card-content">
                        <span class="card-title">測試區</span>
                        <p class="truncate">我跟你說喲，如果如果如果，這裡的字如果太長會被自動截斷變成點點點喔~~~~</p>
                    </div>
                    <div class="card-action">
                        <div>
                            <a class="waves-effect waves-light btn" onclick="showToast('我是Toast文字喔~~~~~')">Toast!</a>
                            <asp:LinkButton ID="btn_showOK" runat="server" CssClass="waves-effect waves-light blue btn" OnClick="btn_showOK_Click">Modal-OK</asp:LinkButton>
                            <asp:LinkButton ID="btn_showFail" runat="server" CssClass="waves-effect waves-light red btn" OnClick="btn_showFail_Click">Modal-Fail</asp:LinkButton>
                            <a class="waves-effect waves-light green btn" onclick="openBlock()">BlockScreen</a>
                        </div>
                        <div>
                            <asp:Button ID="btn_doSave" runat="server" Text="觸發鈕" OnClick="btn_doSave_Click" Style="display: none" />
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <!-- Modal Structure -->
        <div>
            <div id="modal-success" class="modal modal-fixed-footer">
                <div class="modal-content">
                    <h4>作業已成功</h4>
                    <p>您的新增作業已完成</p>
                </div>
                <div class="modal-footer">
                    <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat blue-text">OK</a>
                    <a href="#!" class="modal-action modal-close waves-effect waves-red btn-flat">Close</a>
                </div>
            </div>

            <div id="modal-fail" class="modal modal-fixed-footer">
                <div class="modal-content">
                    <h4>作業失敗</h4>
                    <p>您的新增作業已失敗,請ooxx.</p>
                </div>
                <div class="modal-footer">
                    <a href="#!" class="modal-action modal-close waves-effect waves-red btn-flat">Close</a>
                </div>
            </div>

            <div id="modal-block" class="modal">
                <div class="modal-content">
                    <h5>資料處理中, 請稍候...</h5>
                    <div class="section">
                        <div class="progress">
                            <div class="indeterminate"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="myScript" ContentPlaceHolderID="ScriptContent" runat="server">
    <script>
        $(function () {
            //init modal

            //OK
            $('#modal-success').modal({
                dismissible: true, // Modal can be dismissed by clicking outside of the modal
                complete: function () { location.replace('default.aspx'); } // Callback for Modal close
            });

            //Fail
            $('#modal-fail').modal({
                dismissible: true, // Modal can be dismissed by clicking outside of the modal
                complete: function () { location.replace('default.aspx'); } // Callback for Modal close
            });

            //BlockScreen
            $('#modal-block').modal({
                dismissible: false, // Modal can be dismissed by clicking outside of the modal
                opacity: .5, // Opacity of modal background
                in_duration: 300, // Transition in duration
                out_duration: 200, // Transition out duration
                starting_top: '10%', // Starting top style attribute
                ending_top: '10%', // Ending top style attribute
                ready: function (modal, trigger) { // Callback for Modal open. Modal and trigger parameters available.
                    $("#MainContent_btn_doSave").trigger("click");
                }
            });
        });


        //toast
        function showToast(word) {
            Materialize.toast(word, 5000)
        }

        //openBlock
        function openBlock() {
            $('#modal-block').modal('open');
        }
    </script>

    <asp:PlaceHolder runat="server" ID="ph_showOK" Visible="false">
        <script>
            $(function () {
                $('#modal-success').modal('open');
            });
        </script>
    </asp:PlaceHolder>

    <asp:PlaceHolder runat="server" ID="ph_showFail" Visible="false">
        <script>
            $(function () {
                $('#modal-fail').modal('open');
            });
        </script>
    </asp:PlaceHolder>
</asp:Content>
