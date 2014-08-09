using System;
using System.Windows.Forms;
using System.Net;
using System.Web.Script.Serialization;
using System.Threading; //需要添加对System.Web.Extensions添加dll引用

namespace CnblogsImageBed
{
    public partial class Form1 : Form
    {
        //全局变量保存cookie信息
        public static CookieContainer cc = new CookieContainer();
        string userName = string.Empty;
        string password = string.Empty;
        string homeCnblogsHtml = string.Empty;
        string uploadImgHtml = string.Empty;
        const string HOST = "www.cnblogs.com";
        string imageNameWithExt = string.Empty;
        string localFilePath = string.Empty;

        public Form1()
        {
            InitializeComponent();
            //关闭对文本框跨线程操作的检查
            TextBox.CheckForIllegalCrossThreadCalls = false;

            //设置窗体样式
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            userName = this.txtUserName.Text.Trim();
            password = this.txtPwd.Text.Trim();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("用户名或密码为空", this.Text);
                return;
            }

            Thread thread = new Thread(new ThreadStart(DoLoginCnblogs));
            thread.IsBackground = false;
            thread.Start();


            //防止登录没有返回homeCnblogsHtml时进入判断
            if (!string.IsNullOrEmpty(homeCnblogsHtml))
            {
                if (homeCnblogsHtml.IndexOf("<title>首页 - 我的园子 - 博客园</title>") >= 0 ||
                        homeCnblogsHtml.Contains("编辑个人资料"))
                {
                    pnlUpload.Visible = true;
                    pnlLogin.Visible = false;
                    pnlUpload.Location = new System.Drawing.Point(10, 10);
                }
                else
                {
                    MessageBox.Show("登陆失败", this.Text);
                }
            }
        }

        private void btnChooseImage_Click(object sender, EventArgs e)
        {
            //选择要上传的图片
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files(*.BMP;*.JPG;*.PNG;*.GIF)|*.BMP;*.JPG;*.PNG;*.GIF|All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.txtImagePath.Text = ofd.FileName;
            }
        }

        /// <summary>
        /// 模拟登录博客园
        /// </summary>
        protected void DoLoginCnblogs()
        {
            Login login = new Login();
            homeCnblogsHtml = login.LoginCnblogs(userName, password);
            cc = login.cc;
        }

        protected void DoUploadImg()
        {
            string url = string.Format("http://upload.cnblogs.com/imageuploader/processupload?host={0}&qqfile={1}", HOST, imageNameWithExt);
            uploadImgHtml = HttpHelper.HttpPost(url, localFilePath, cc);   
        }

        /// <summary>
        /// 上传图片事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpload_Click(object sender, EventArgs e)
        {
            //1.拿到本地图片路径
            //2.发起http post请求
            //3.得到response的image url
            //4.赋值给txtImageUrl

            localFilePath = this.txtImagePath.Text.Trim();

            if (string.IsNullOrEmpty(localFilePath))
            {
                MessageBox.Show("请选择图片", this.Text);
                return;
            }

            imageNameWithExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1);

            //Thread thread = new Thread(new ThreadStart(DoUploadImg));
            //thread.IsBackground = true;
            //thread.Start();

            DoUploadImg();

            if (!string.IsNullOrEmpty(uploadImgHtml))
            {
                //将post得到的response值反序列化为Image实体对象
                JavaScriptSerializer jss = new JavaScriptSerializer();
                ImageEntity ie = jss.Deserialize<ImageEntity>(uploadImgHtml);

                if (ie.Success == true)
                {
                    this.txtImageUrl.Text = ie.Message;
                    this.txtImageUrl.Focus();
                    this.txtImageUrl.SelectAll();
                }
                else
                {
                    this.txtImageUrl.Text = "图片上传失败! Error: " + ie.Message;
                } 
            }
        }
    }
}
