using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Web;
using BP.WF.Template;
using BP.WF;
using BP.En;
using BP.Sys;

namespace BP.Web.WF
{
    /// <summary>
    /// Do 的摘要说明。
    /// </summary>
    public partial class Do : PageBase
    {
        public string ActionType
        {
            get
            {
                string s = this.Request.QueryString["ActionType"];
                if (s == null)
                    s = this.Request.QueryString["DoType"];

                if (string.IsNullOrEmpty(s) && this.Request.QueryString["SID"] != null)
                    s = "Track";
                return s;
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string RefNo
        {
            get
            {
                return this.Request.QueryString["RefNo"];
            }
        }
        public string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.Request.QueryString["FK_Emp"];
            }
        }
        public string PageID
        {
            get
            {
                return this.Request.QueryString["PageID"];
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int NodeID
        {
            get
            {
                string s = this.Request.QueryString["NodeID"];
                if (s == null || s == "")
                    s = this.Request.QueryString["FK_Node"];
                return int.Parse(s);
            }
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Response.AddHeader("P3P", "CP=CAO PSA OUR");
            Response.AddHeader("Cache-Control", "no-store");
            Response.AddHeader("Expires", "0");
            Response.AddHeader("Pragma", "no-cache");
            string url = this.Request.RawUrl;
            if (url.Contains("DTT=") == false)
            {
                //this.Response.Redirect(url + "&DTT=" + DateTime.Now.ToString("mmDDhhmmss"), true);
                //return;
            }

            try
            {
                switch (this.ActionType)
                {
                    case "Focus": //把任务放入任务池.
                        BP.WF.Dev2Interface.Flow_Focus(Int64.Parse(this.Request.QueryString["WorkID"]));
                        this.WinClose("ss");
                        break;
                    case "PutOne": //把任务放入任务池.
                          Int64 workid42 = Int64.Parse(this.Request.QueryString["WorkID"]);
                          BP.WF.Dev2Interface.Node_TaskPoolPutOne(workid42);
                        this.WinClose("ss");
                        break;
                    case "DoAppTask": // 申请任务.
                        Int64 workid2 = Int64.Parse(this.Request.QueryString["WorkID"]);
                        BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(workid2);
                        this.WinClose("ss");
                        return;
                    case "DoOpenCC":
                        string fk_flow1 = this.Request.QueryString["FK_Flow"];
                        string fk_node1 = this.Request.QueryString["FK_Node"];
                        string workid1 = this.Request.QueryString["WorkID"];
                        string fid1 = this.Request.QueryString["FID"];
                        string Sta = this.Request.QueryString["Sta"];
                        if (Sta == "0")
                        {
                            BP.WF.Template.CCList cc1 = new BP.WF.Template.CCList();
                            cc1.MyPK = this.Request.QueryString["MyPK"];
                            cc1.Retrieve();
                            cc1.HisSta = CCSta.Read;
                            cc1.Update();
                        }
                        this.Response.Redirect("./WorkOpt/OneWork/Track.aspx?FK_Flow=" + fk_flow1 + "&FK_Node=" + fk_node1 + "&WorkID=" + workid1 + "&FID=" + fid1, false);
                        return;
                    case "DelCC": //删除抄送.
                        CCList cc = new CCList();
                        cc.MyPK = this.MyPK;
                        cc.Retrieve();
                        cc.HisSta = CCSta.Del;
                        cc.Update();
                        this.WinClose();
                        break;
                    case "DelSubFlow": //删除进程。
                        try
                        {
                            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID,"手工删除");
                            this.WinClose();
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                        }
                        break;
                    case "DownBill":
                        Bill b = new Bill(this.MyPK);
                        b.DoOpen();
                        break;
                    case "DelDtl":
                        GEDtls dtls = new GEDtls(this.EnsName);
                        GEDtl dtl = (GEDtl)dtls.GetNewEntity;
                        dtl.OID = this.RefOID;
                        if (dtl.RetrieveFromDBSources() == 0)
                        {
                            this.WinClose();
                            break;
                        }
                        FrmEvents fes = new FrmEvents(this.EnsName); //获得事件.

                        // 处理删除前事件.
                        try
                        {
                            fes.DoEventNode(BP.WF.XML.EventListDtlList.DtlItemDelBefore, dtl);
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                            break;
                        }
                        dtl.Delete();

                        // 处理删除后事件.
                        try
                        {
                            fes.DoEventNode(BP.WF.XML.EventListDtlList.DtlItemDelAfter, dtl);
                        }
                        catch (Exception ex)
                        {
                            this.WinCloseWithMsg(ex.Message);
                            break;
                        }
                        this.WinClose();
                        break;
                    case "EmpDoUp":
                        BP.WF.Port.WFEmp ep = new BP.WF.Port.WFEmp(this.RefNo);
                        ep.DoUp();

                        BP.WF.Port.WFEmps emps111 = new BP.WF.Port.WFEmps();
                      //  emps111.RemoveCash();
                        emps111.RetrieveAll();
                        this.WinClose();
                        break;
                    case "EmpDoDown":
                        BP.WF.Port.WFEmp ep1 = new BP.WF.Port.WFEmp(this.RefNo);
                        ep1.DoDown();

                        BP.WF.Port.WFEmps emps11441 = new BP.WF.Port.WFEmps();
                      //  emps11441.RemoveCash();
                        emps11441.RetrieveAll();
                        this.WinClose();
                        break;

                    case "Track": //通过一个串来打开一个工作.
                         string mySid = this.Request.QueryString["SID"];
                         string[] mystrs = mySid.Split('_');

                         Int64  myWorkID = int.Parse(mystrs[1]);
                         string fk_emp = mystrs[0];
                         int fk_node = int.Parse(mystrs[2]);
                         Node mynd = new Node();
                         mynd.NodeID = fk_node;
                         mynd.RetrieveFromDBSources();

                         string fk_flow = mynd.FK_Flow;
                         string myurl = "./WorkOpt/OneWork/Track.aspx?FK_Node=" + mynd.NodeID + "&WorkID=" + myWorkID + "&FK_Flow=" + fk_flow;
                         Web.WebUser.SignInOfGener( new BP.Port.Emp(fk_emp), true);
                         this.Response.Write("<script> window.location.href='" + myurl + "'</script> *^_^*  <br><br>正在进入系统请稍后，如果长时间没有反应，请<a href='" + myurl + "'>点这里进入。</a>");
                        return;
                    case "OF": //通过一个串来打开一个工作.
                        string sid = this.Request.QueryString["SID"];
                        string[] strs = sid.Split('_');
                        GenerWorkerList wl = new GenerWorkerList();
                        int i = wl.Retrieve(GenerWorkerListAttr.FK_Emp, strs[0],
                            GenerWorkerListAttr.WorkID, strs[1],
                            GenerWorkerListAttr.FK_Node, strs[2]);
                        if (i == 0)
                        {
                            this.Response.Write("<h2>提示</h2>此工作已经被别人处理或者此流程已删除。");
                            return;
                        }

                        BP.Port.Emp empOF = new BP.Port.Emp(wl.FK_Emp);
                        Web.WebUser.SignInOfGener(empOF, true);
                        string u = "MyFlow.aspx?FK_Flow=" + wl.FK_Flow + "&WorkID=" + wl.WorkID+"&FK_Node="+wl.FK_Node+"&FID="+wl.FID;
                        this.Response.Write("<script> window.location.href='" + u + "'</script> *^_^*  <br><br>正在进入系统请稍后，如果长时间没有反应，请<a href='" + u + "'>点这里进入。</a>");
                        return;
                    case "ExitAuth":
                        BP.Port.Emp emp = new BP.Port.Emp(this.FK_Emp);
                        //首先退出，再进行登录
                        BP.Web.WebUser.Exit();
                        BP.Web.WebUser.SignInOfGenerLang(emp, WebUser.SysLang);
                        this.WinClose();
                        return;
                    case "LogAs":
                        BP.WF.Port.WFEmp wfemp = new BP.WF.Port.WFEmp(this.FK_Emp);
                        if (wfemp.AuthorIsOK == false)
                        {
                            this.WinCloseWithMsg("授权失败");
                            return;
                        }
                        BP.Port.Emp emp1 = new BP.Port.Emp(this.FK_Emp);
                        BP.Web.WebUser.SignInOfGener(emp1, WebUser.SysLang, WebUser.No, true, false);
                        this.WinClose();
                        return;
                    case "TakeBack": // 取消授权。
                        BP.WF.Port.WFEmp myau = new BP.WF.Port.WFEmp(WebUser.No);
                        BP.DA.Log.DefaultLogWriteLineInfo("取消授权:" + WebUser.No + "取消了对(" + myau.Author + ")的授权。");
                        myau.Author = "";
                        myau.AuthorWay = 0;
                        myau.Update();
                        this.WinClose();
                        return;
                    case "AutoTo": // 执行授权。
                        BP.WF.Port.WFEmp au = new BP.WF.Port.WFEmp();
                        au.No = WebUser.No;
                        au.RetrieveFromDBSources();
                        au.AuthorDate = BP.DA.DataType.CurrentData;
                        au.Author = this.FK_Emp;
                        au.AuthorWay = 1;
                        au.Save();
                        BP.DA.Log.DefaultLogWriteLineInfo("执行授权:" + WebUser.No + "执行了对(" + au.Author + ")的授权。");
                        this.WinClose();
                        return;
                    case "UnSend": //执行撤消发送。
                        this.Response.Redirect("./WorkOpt/UnSend.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow, false);
                        return;
                    case "SetBillState":
                        break;
                    case "WorkRpt":
                        Bill bk1 = new Bill(this.Request.QueryString["OID"]);
                        Node nd = new Node(bk1.FK_Node);
                        this.Response.Redirect("WFRpt.aspx?WorkID=" + bk1.WorkID + "&FID=" + bk1.FID + "&FK_Flow=" + nd.FK_Flow + "&NodeId=" + bk1.FK_Node, false);
                        //this.WinOpen();
                        //this.WinClose();
                        break;
                    case "PrintBill":
                        //Bill bk2 = new Bill(this.Request.QueryString["OID"]);
                        //Node nd2 = new Node(bk2.FK_Node);
                        //this.Response.Redirect("NodeRefFunc.aspx?NodeId=" + bk2.FK_Node + "&FlowNo=" + nd2.FK_Flow + "&NodeRefFuncOID=" + bk2.FK_NodeRefFunc + "&WorkFlowID=" + bk2.WorkID);
                        ////this.WinClose();
                        break;
                    //删除流程中第一个节点的数据，包括待办工作
                    case "DeleteFlow":
                        string fk_flowDel = this.Request.QueryString["FK_Flow"];
                        Int64 workid = Int64.Parse(this.Request.QueryString["WorkID"]);
                        //调用DoDeleteWorkFlowByReal方法
                        WorkFlow wf = new WorkFlow(new Flow(fk_flowDel), workid);
                        wf.DoDeleteWorkFlowByReal(true);
                      //  Glo.ToMsg("流程删除成功");
                        BP.WF.Glo.ToMsg("流程删除成功");

             


                        //this.ToWFMsgPage("流程删除成功");
                        break;
                    default:
                        throw new Exception("ActionType error" + this.ActionType);
                }
            }
            catch (Exception ex)
            {
                this.ToErrorPage("执行其间如下异常：<BR>" + ex.Message);
            }
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion
    }
}
