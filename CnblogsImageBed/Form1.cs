using System;
using System.Windows.Forms;
using System.Net;
using System.Web.Script.Serialization; //需要添加对System.Web.Extensions添加dll引用

namespace CnblogsImageBed
{
    public partial class Form1 : Form
    {
        //全局变量保存cookie信息
        public static CookieContainer cc = new CookieContainer();

        public Form1()
        {
            InitializeComponent();

            //设置窗体样式
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            DoLoginCnblogs();
        }

        /// <summary>
        /// 登录博客园的方法
        /// </summary>
        protected void DoLoginCnblogs()
        {
            string userName = this.txtUserName.Text.Trim();
            string password = this.txtPwd.Text.Trim();

            Login login = new Login();

            string html = login.LoginCnblogs(userName, password);

            cc = login.cc;

            if (html.IndexOf("<title>首页 - 我的园子 - 博客园</title>") >= 0)
            {
                this.pnlUpload.Visible = true;
                this.pnlLogin.Visible = false;
                this.pnlUpload.Location = new System.Drawing.Point(10, 10);
            }
            else
            {
                MessageBox.Show("登陆失败", this.Text);
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

            string url = "http://upload.cnblogs.com/imageuploader/processupload?host=&qqfile=11.jpg";
            string filePath = this.txtImagePath.Text.Trim();

            string html = HttpHelper.HttpPost(url, filePath, cc);

            //将post得到的response值反序列化为Image实体对象
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ImageEntity ie = jss.Deserialize<ImageEntity>(html);

            if (ie.Success == true)
            {
                this.txtImageUrl.Text = ie.Message;
                this.txtImageUrl.Focus();
                this.txtImageUrl.SelectAll();
            }
            else
            {
                this.txtImageUrl.Text = "图片上传失败!";
            }


        }
    }
}
