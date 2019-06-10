using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
namespace OpenAward
{
    class Program
    {
        static int LastRunTime = 0;

        /// <summary>
        /// 线程函数
        /// </summary>
        static void TimeLine()
        {
            while (true)
            {
                int sleepTime = 1000;
                if(LastRunTime == 0)
                {
                    sleepTime = DayLessTime - 3600 * 2 - 1800;
                }
                else
                {
                    sleepTime = DayLessTime + 3600 * 21 + 1800;
                }
                //Console.Write();
                Console.WriteLine("下次将于"+sleepTime+"秒后执行");
                Thread.Sleep(sleepTime*1000);
                DayAction();
            }
        }

        /// <summary>
        /// 每周日21:30分执行动作
        /// </summary>
        public static void DayAction()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                Console.WriteLine("准备开奖");
                RunWebRequest();
            }
            else
            {
                Console.WriteLine("当天不是周日");
            }
        }

        /// <summary>
        /// 当天的剩余时间
        /// </summary>
        public static int DayLessTime
        {
            get
            {
                return 86400 - TimeStamp % 86400;
            }
        }
        /// <summary> 
        /// 获取时间戳 
        /// </summary> 
        /// <returns></returns> 
        public static int TimeStamp
        {
            get
            {
                TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                return Convert.ToInt32(ts.TotalSeconds);
            }
        }

        static void RunWebRequest()
        {
            string strURL = "http://localhost?aw=anums";
            if (File.Exists(System.Environment.CurrentDirectory + "/url.txt"))
            {
                strURL = File.ReadAllText(System.Environment.CurrentDirectory + "/url.txt");
            }

            // 创建一个HTTP请求
            string responseText = "";
            try
            {
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
                responseText = myreader.ReadToEnd();
                myreader.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine(responseText);
            DateTime current = DateTime.Now;
            string FileName = current.Year + "_" + current.Month + "_" + current.Day + "-" + Math.Ceiling(current.TimeOfDay.TotalMilliseconds);


            File.WriteAllText(System.Environment.CurrentDirectory + "/" + FileName + ".txt", responseText);
        }

        static void Main(string[] args)
        {
            Thread thread = new Thread(TimeLine);
            thread.Start();
            return; 
        }
    }
}
