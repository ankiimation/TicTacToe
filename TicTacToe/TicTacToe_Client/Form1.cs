using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicTacToeServer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Tao socket
        TcpClient client = new TcpClient();
        // Ket noi den server
        Stream stream;
        private void Form1_Load(object sender, EventArgs e)
        {
            refresh();

            Thread thread = new Thread(delegate () {
                TcpClient client = new TcpClient();

                // 1. connect
                client.Connect("127.0.0.1", 123);
               stream = client.GetStream();

                Console.WriteLine("Connected to Y2Server.");
                while (true)
                {
                   
                    
                    byte[] buffer = new byte[client.ReceiveBufferSize];
                    int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
                    string receivedString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            
                    Console.WriteLine(receivedString);
                    string[] rs = receivedString.Split(' ');
                    int i = Convert.ToInt32(rs[0]);
                    int j = Convert.ToInt32(rs[1]);
                    int playerNumber = Convert.ToInt32(rs[2]);

                    TicTacToeController.hit(i, j, playerNumber);
                    isMoved = false;
                    flowLayoutPanel1.Invoke((MethodInvoker)delegate () {refresh();});


                }
                // 4. close
                stream.Close();
                client.Close();
                //Client nhan message tu server
                //size = sck.Receive(data);

                //}
                ///////////////////////////////////////

            });
            thread.Start();
       
           
        }

        private void refresh()
        {
            if (!TicTacToeController.canMove())
            {
                TicTacToeController.newGame();
            }
            flowLayoutPanel1.Controls.Clear();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Button temp = new Button();


                    temp.Text = TicTacToeController.desk[i, j].ToString();


                    if (TicTacToeController.desk[i, j] != 0 || isMoved)
                    {
                        temp.Enabled = false;
                    }

                    if (TicTacToeController.desk[i, j] == 1)
                    {
                        temp.BackColor = Color.Red;
                    }
                    if (TicTacToeController.desk[i, j] == 2)
                    {
                        temp.BackColor = Color.Green;
                    }


                    temp.Width = 100;
                    temp.Height = 100;
                    temp.Tag = new int[2] { i, j };

                    temp.Click += Temp_Click;
                    flowLayoutPanel1.Controls.Add(temp);
                }
            }
        }

        bool isMoved = false;
        private void Temp_Click(object sender, EventArgs e)
        {
            int[] point = (int[])((Control)sender).Tag;

           
            string textToSend = point[0] + " " + point[1] + " " + 1;
            byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
            stream.Write(bytesToSend, 0, bytesToSend.Length);


           
            TicTacToeController.hit(point[0], point[1], 1);
            isMoved = true;
            refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
    }
}
