using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using NDde.Client;

namespace DDEClient
{
    class Program
    {
        static float T1;
        static float T2;
        static TextWriter writer;
        static void Main(string[] args)
        {
            

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            FileStream fs = File.Create(DateTime.Now.ToString("HH_mm_ss") + ".csv");
            writer = new StreamWriter(fs);
            writer.WriteLine("\"TIME\";\"T1\";\"T2\"");
            TimerCallback timerCallback = new TimerCallback(onTick);
            Timer timer = new Timer(timerCallback, null, 0, 2000);
            using (DdeClient client = new DdeClient("CoDeSys", @"D:\Project\discreteOut.pro"))
                try
                {
                    client.Disconnected += Client_Disconnected;
                    client.Connect();
                    //var s = client.Request("myitem1", 60000);
                    client.Advise += OnAdvise;
                    client.StartAdvise("T1", 1, true, 600);
                    client.StartAdvise("T2", 1, true, 600);
                    Console.ReadLine();

                }
                catch (NDde.DdeException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    timer.Dispose();
                    if (client.IsConnected == true) client.Disconnect();
                    writer.Close();
                    fs.Close();
                    
                }

            /*}
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }*/
            
            
        }

        private static void Client_Disconnected(object sender, DdeDisconnectedEventArgs args)
        {
            Console.WriteLine("IsServerInitiated=" + args.IsServerInitiated.ToString() + " " +
                "IsDisposed=" + args.IsDisposed.ToString());
        }

        static void OnAdvise(object sender,DdeAdviseEventArgs e)
        {
            if (e.Item == "T1") T1 = float.Parse(e.Text);
            if (e.Item == "T2") T2 = float.Parse(e.Text);
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs r)
        {
            Console.WriteLine("Exit");
        }

        static void onTick(object obj)
        {
            string str = $"\"{DateTime.Now.ToLongTimeString()}\";\"{T1}\";\"{T2}\"";

            Console.WriteLine(str);
            writer.WriteLine(str);
            
        }
    }
}
