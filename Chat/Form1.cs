using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace Chat
{
    public partial class Form1 : MetroForm
    {
        
        public static IPAddress remoteIP;
        public static int localPort;
        public static int remotePort;
        public Thread tRec;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(tRec.IsAlive)
            {
                tRec.Abort();
            }
            Application.Exit();
            System.Environment.Exit(1);
            Process.GetCurrentProcess().Kill();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            localPort = Int32.Parse(metroTextBox1.Text);
            remoteIP = IPAddress.Parse(metroTextBox2.Text);
            remotePort = Int32.Parse(metroTextBox3.Text);

            metroLabel5.Text = "Status: Parsed";
            Thread.Sleep(250);

            if (localPort != 0 && remotePort != 0 && remoteIP != null)
            {
                metroTextBox4.Enabled = true;
                metroButton2.Enabled = true;
            }

            tRec = new Thread(new ThreadStart(Receiver));
            tRec.Start();
            metroLabel5.Text = "Status: Whaiting for message...";
        }
        public void Send(string datagram)
        {
            // Создаем UdpClient
            UdpClient sender = new UdpClient();

            // Создаем endPoint по информации об удаленном хосте
            IPEndPoint endPoint = new IPEndPoint(remoteIP, remotePort);

            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            string lolcalIP = ipHost.AddressList[0].ToString();
            datagram = lolcalIP + ": " + datagram;

            try
            {
                // Преобразуем данные в массив байтов
                byte[] bytes = Encoding.UTF8.GetBytes(datagram);

                // Отправляем данные
                sender.Send(bytes, bytes.Length, endPoint);
                listBox1.Items.Add(datagram);
            }
            catch (Exception ex)
            {
                metroTextBox4.Text = ex.ToString();
            }
            finally
            {
                // Закрыть соединение
                sender.Close();
            }
        }

        public void Receiver()
        {
            // Создаем UdpClient для чтения входящих данных
            UdpClient receivingUdpClient = new UdpClient(localPort);

            IPEndPoint RemoteIpEndPoint = null;

            try
            {
                listBox1.Items.Add(
                   "-----------*******Общий чат*******-----------");

                while (true)
                {
                    // Ожидание дейтаграммы
                    byte[] receiveBytes = receivingUdpClient.Receive(
                       ref RemoteIpEndPoint);

                    // Преобразуем и отображаем данные
                    string returnData = Encoding.UTF8.GetString(receiveBytes);
                    listBox1.Items.Add(" --> " + returnData.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Возникло исключение: " + ex.ToString() + "\n  " + ex.Message);
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Send(metroTextBox4.Text);
            metroLabel5.Text = "Status: Sent";
            Thread.Sleep(300);
            metroLabel5.Text = "Status: Whaiting for message...";
            metroTextBox4.Clear();
        }
    }
}
