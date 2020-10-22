using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Web;

namespace ShanDianCha
{
    public partial class Form1 : Form
    {
        static string ADDID = "yourid";
        static string SECKEY = "yourseckey";

        static int SEARCHOFFSET = 7;
      
        public Form1()
        {
            InitializeComponent();
            //textBox1.Focus();
        }
        private static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }

        private string CheckLanguage(string str)
        {
            if((str[0]>='a' && str[0]<='z') || (str[0]>='A' && str[0]<='Z'))
            {
                return "en";
            }
            else
            {
                return "zh";
            }
        }

        private string ToAnother(string str)
        {
            if(str=="en")
            {
                return "zh";
            }
            else
            {
                return "en";
            }
        }

       
        private string Search(string str)
        {
            // 原文
            string q = str;
            // 源语言
            string from = CheckLanguage(str);
            // 目标语言
            string to = ToAnother(from);
            // 改成您的APP ID
            string appId = ADDID;
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            // 改成您的密钥
            string secretKey = SECKEY;
            string sign = EncryptString(appId + q + salt + secretKey);
            string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
            url += "q=" + HttpUtility.UrlEncode(q);
            url += "&from=" + from;
            url += "&to=" + to;
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = 6000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;

           
        }

        private string EnToZh(string str)
        {
            string[] temp = str.Split(new string[] { "\\u" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < temp.Length; i++)
                temp[i] = ((char)Convert.ToInt32(temp[i], 16)).ToString();
            string res = string.Join("", temp);
            return res;
        }
        private string Handle(string str)
        {
            //{"from":"zh","to":"en","trans_result":[{"src":"\u4f60\u597d","dst":"Hello"}]}
            //{"from":"en","to":"zh","trans_result":[{"src":"hello","dst":"\u4f60\u597d"}]}
            int dstindex = str.IndexOf("\"dst\"");
            int length = str.Length;
            int reslen = length - dstindex - SEARCHOFFSET - 4;
            string res = str.Substring(dstindex + SEARCHOFFSET, reslen);
            if((res[0]>='a' && res[0]<='z') || (res[0]>='A' && res[0]<='Z'))
            {
                return res;
            }
            else
            {
                return EnToZh(res);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string res = Search(textBox1.Text);
            res = Handle(res);
            textBox2.Text = res;
        }

        private void Search_keyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                string res = Search(textBox1.Text);
                res = Handle(res);
                textBox2.Text = res;
            }
        }
    }
}
