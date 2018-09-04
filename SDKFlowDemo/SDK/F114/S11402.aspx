﻿<%@ Page Title="子线程" Language="C#" MasterPageFile="~/SDKFlowDemo/SDK/Site.Master" AutoEventWireup="true" CodeBehind="S11402.aspx.cs" Inherits="CCFlow.SDKFlowDemo.SDK.F114.S11402" %>

<%@ Register src="../../../WF/SDKComponents/DocMainAth.ascx" tagname="DocMainAth" tagprefix="uc1" %>
<%@ Register src="../../../WF/SDKComponents/DocMultiAth.ascx" tagname="DocMultiAth" tagprefix="uc2" %>
<%@ Register src="../../../WF/SDKComponents/Toolbar.ascx" tagname="Toolbar" tagprefix="uc3" %>

<%@ Register src="../../../WF/SDKComponents/SubFlowDtl.ascx" tagname="SubFlowDtl" tagprefix="uc4" %>

<%@ Register src="../../../WF/SDKComponents/FrmCheck.ascx" tagname="FrmCheck" tagprefix="uc5" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<!-- 加载EUI需要的文件. -->
<script src="/WF/Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
<script src="/WF/Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
<script src="/WF/Scripts/locale/easyui-lang-zh_CN.js" type="text/javascript"></script>
<link href="/WF/Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
<link href="/WF/Scripts/easyUI/themes/default/easyui.css" rel="stylesheet"  type="text/css" />

<script type="text/javascript">
    // 锁定所有的控件.
    var doType = "";
    var paras = "";
    function LockPage() {
        if (doType != "Send")
            return;
        return;

        var arrObj = document.all;
        for (var i = 0; i < arrObj.length; i++) {
            if (typeof arrObj[i].type == "undefined")
                continue;

            if (arrObj[i].type == "button")
                arrObj[i].disabled = 'disabled';

            if (arrObj[i].type == "text")
                arrObj[i].disabled = 'disabled';
        }
    }
    function ShowMsg(title, msg) {
        $('#Msg').dialog(
        {
            title: title,
            modal: true,
            width: 500,
            height: 300,
            content: msg,
            buttons: [{ text: '关闭', handler: function () {
                $('#Msg').dialog("close");
            }
            }]
        }
        );
    }

    function Send() {
        if (confirm("您确定要发送吗？") == false)
            return;
        doType = "Send";
        LoadFrm();
        //表单提交.
        $('#form1').submit();
        LockPage();
    }
    function Save() {
        doType = "Save";
        LoadFrm();
        //表单提交.
        $('#form1').submit();
    }

    // 使用form提交数据.
    function LoadFrm() {
        GetParas();
        $('#form1').form({
            url: "/SDKFlowDemo/SDK/F114/Serv.ashx?DoType=" + doType + paras,
            data: paras,
            onSubmit: function (param) {
            },
            success: function (data) {
                ShowMsg('发送成功', data);
            }
        });
    }
    function GetParas() {
        paras = "";
        //获取其他参数
        var sHref = window.location.href;
        var args = sHref.split("?");
        var retval = "";
        if (args[0] != sHref) /*参数不为空*/
        {
            var str = args[1];
            args = str.split("&");
            for (var i = 0; i < args.length; i++) {
                str = args[i];
                var arg = str.split("=");
                if (arg.length <= 1)
                    continue;
                //不包含就添加
                if (paras.indexOf('&' + arg[0]) == -1) {
                    paras += "&" + arg[0] + "=" + arg[1];
                }
            }
        }
    }
 </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="Msg" />
子线程-销售人员处理任务
<table  height="100%" cellspacing="1" cellpadding="0" width="800px" align="center"
    border="0">
<tr>
<td>
 <uc3:Toolbar ID="Toolbar1" runat="server" />

<fieldset>
<legend>单附件-组件</legend>
 <uc1:DocMainAth ID="DocMainAth1" runat="server" />
</fieldset>


<fieldset>
<legend>多附件-组件</legend>
    <uc2:DocMultiAth ID="DocMultiAth1" runat="server" />
</fieldset>

<fieldset>
<legend>子流程组件</legend>
    <uc4:SubFlowDtl ID="SubFlowDtl1" runat="server" />

    <fieldset>
    <legend>调用子流程115</legend>
    <% string workid=this.Request.QueryString["WorkID"]; %>
    <% string pFID=this.Request.QueryString["FID"]; %>
    <a href="/WF/MyFlow.aspx?PFlowNo=114&PNodeID=11402&PWorkID=<%=workid %>&PFID=<%=pFID %>&FK_Flow=115&FK_Node=11501" target="_blank" >发起子流程</a>
    </fieldset>

</fieldset>


<fieldset>
<legend>审核组件</legend>
<uc5:FrmCheck ID="FrmCheck1" runat="server" />
</fieldset>


</td>
</tr>
 
 </table>

</asp:Content>
