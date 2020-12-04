using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HttpSender;
using Newtonsoft.Json;
using System.Threading;

namespace WindowsFormsApplication1
{
    class OrderInfos
    {
        public string[] codes { get; set; }
        public string[] names { get; set; }
    }

    class VisionCodes
    {
        public string[] codes { get; set; }
    }



    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // 服务端确认命令，收到“hello”说明服务端存在
        private void hi_Commond()
        {
            string Response = Sender.Get("http://127.0.0.1:8080/hi");
            if (Response == "hello")
            {
                //服务端存在
            }
        }

        // 订单开始命令, 收到该命令后，服务端开始固定间隔拍照
        // 参数(可选)：将订单中的条烟编码和条烟名称按json发送（格式 { "codes" : ["xxxx", "xxxxx"]， "names" : ["xxxx", "xxxxx"]}）。参考下例。
        // 当不传上述参数时，请传递空字符串。Sender.Post("http://127.0.0.1:8080/start", "")
        private void start_Commond()
        {
            OrderInfos info = new OrderInfos();
            string[] codes = { "00001", "00002" };
            info.codes = codes;
            string[] names = { "10000", "20000" };
            info.names = names;
            string Response = Sender.Post("http://127.0.0.1:8080/start", JsonConvert.SerializeObject(info));
        }

        //订单结束命令，收到该命令后，服务端结束拍照，但还可能有最后的任务没有处理完成
        private void stop_Commond()
        {
            string Response = Sender.Get("http://127.0.0.1:8080/stop");
        }

        //获取结果命令， 在发送stop命令后延迟100ms发送。获取订单的结果
        // 返回值 error， 说明服务端未处理完成，需要稍后再次发送
        // 返回值 empty， 说明服务端未检测到烟
        // 返回值 json字符串(格式： { "codes" : ["xxxx", "xxxxx"]})，返回当前订单的所有条烟编码，可参考下列方法解析到VisionCodes对象
        // 注意条烟编码中可能存在"unknow", 说明该条烟置信程度偏低，不能确定是什么烟。
        private void result_Commond()
        {
            string Response = Sender.Get("http://127.0.0.1:8080/result");
            label1.Text = Response;

            if (Response == "error")
            {
                System.Console.WriteLine("result_Commond :error");
                return;
            }
            if (Response == "empty")
            {
                System.Console.WriteLine("result_Commond :empty");
                return;
            }

            VisionCodes visioncodes = JsonConvert.DeserializeObject<VisionCodes>(Response);
            for (int i = 0; i < visioncodes.codes.Count<string>(); i++)
            {
                System.Console.WriteLine(visioncodes.codes[i]);
            }
        }

        //服务器退出命令，收到该命令后，服务端退出。
        private void end_Commond()
        {
            string Response = Sender.Get("http://127.0.0.1:8080/end");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            start_Commond();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            stop_Commond();
            Thread.Sleep(100);
            result_Commond();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

    }
}
