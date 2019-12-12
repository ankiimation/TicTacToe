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
using System.Timers;
using System.Windows.Forms;

namespace TicTacToeServer
{
    public partial class Form1 : Form
    {
        System.Timers.Timer aTimer = new System.Timers.Timer();

        static int PORT;
        public Form1()
        {
            aTimer.Elapsed += ATimer_Elapsed;
            // Set the Interval to 1 second.
            aTimer.Interval = 1000;
            aTimer.Stop();
            InitializeComponent();
        }

        private void ATimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (connected)
            {
                int time = Convert.ToInt32(labelTime.Text);
                time--;

                labelTime.Invoke((MethodInvoker)delegate () { labelTime.Text = time.ToString(); });
               
               
                if (time == 0)
                {

                    send("#endgame");
                    ((System.Timers.Timer)sender).Stop();
                        flowLayoutPanel1.Invoke((MethodInvoker)delegate () { refresh(); });
                        
                        labelTime.Invoke((MethodInvoker)delegate () { labelTime.Text = "10"; });
                    
                }
            }

        }

        static IPAddress address = IPAddress.Parse("127.0.0.1");

        static TcpListener listener;
        static NetworkStream stream;


        bool connected = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            //newDesk();
            //refresh();
            CreateRoomForm createRoomForm = new CreateRoomForm();
            if (createRoomForm.ShowDialog() == DialogResult.OK)
            {
                TicTacToeController.DESK_SIZE = createRoomForm.deskSize;
                labelDeskSize.Text = TicTacToeController.DESK_SIZE.ToString();
                PORT = (new Random()).Next(1, 1000);
                listener = new TcpListener(address, PORT);
            }
            //newDesk();
            labelDeskSize.Text = TicTacToeController.DESK_SIZE.ToString();
            labelPort.Text = PORT.ToString();

            Thread thread = new Thread(delegate () {
                listener.Start();

                Console.WriteLine("Server started on " + listener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection...");
                txtStatus.Invoke((MethodInvoker)delegate () {
                    txtStatus.Text = "ĐANG CHỜ ĐỢI 1 ĐỐI THỦ XỨNG TẦM!";
                });

                Socket socket = listener.AcceptSocket();
                Console.WriteLine("Connection received from " + socket.RemoteEndPoint);
                txtStatus.Invoke((MethodInvoker)delegate () {
                    txtStatus.Text = "ĐÃ TÌM THẤY ĐỐI THỦ";
                });
                connected = true;
                listener.Stop();

                flowLayoutPanel1.Invoke((MethodInvoker)delegate () { newDesk(); });
                stream = new NetworkStream(socket);
                send(TicTacToeController.DESK_SIZE.ToString());
                var reader = new StreamReader(stream);
                
                
                while (true)
                {
                    // 2. receive
                  byte[] buffer = new byte[socket.ReceiveBufferSize];
                   int  bytesRead = stream.Read(buffer, 0, socket.ReceiveBufferSize);
                    string receivedString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine(receivedString);
                    string[] rs = receivedString.Split(' ');
                    int i = Convert.ToInt32(rs[0]);
                    int j = Convert.ToInt32(rs[1]);
                    int playerNumber = Convert.ToInt32(rs[2]);



                    TicTacToeController.hit(i, j, playerNumber);
                    isMoved = false;
                    
                    txtStatus.Invoke((MethodInvoker)delegate () {
                        txtStatus.Text = "ĐẾN LƯỢT BẠN";
                    });
                    labelTime.Invoke((MethodInvoker)delegate () {
                        labelTime.Text = "10";
                    });
                    aTimer.Start();
                    flowLayoutPanel1.Invoke((MethodInvoker)delegate () { refresh(); });

                    
                    
                }
                // 4. close
                stream.Close();
                socket.Close();
                listener.Stop();
              
                    
                   


               
                /////////////////////////////////////
                //Dong socket
              
            });

            thread.Start();
          
        }

        private void newDesk()
        {
            isMoved = false;
            txtStatus.Invoke((MethodInvoker)delegate () {
                txtStatus.Text = "CHIẾN THÔI!";
            });
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < TicTacToeController.DESK_SIZE; i++)
            {
                for (int j = 0; j < TicTacToeController.DESK_SIZE; j++)
                {
                    Button temp = new Button();


                    //  temp.BackColor = Color.White;


                  


                    temp.Width = flowLayoutPanel1.Width / (TicTacToeController.DESK_SIZE+10);
                    temp.Height = temp.Width;

                    temp.Tag = new int[2] { i, j };
                    temp.Click += Temp_Click;
                    flowLayoutPanel1.Controls.Add(temp);

                }
            }
        }

        private void refresh()
        {
           
                    if (!TicTacToeController.canMove() || labelTime.Text.Equals("0"))
                    {
                        TicTacToeController.newGame();
                        aTimer.Stop();
                         newDesk();

                     // return;
                    }
            if (isMoved)
            {
                flowLayoutPanel1.Enabled = false;

            }
            else
            {
                flowLayoutPanel1.Enabled = true;
            }

            for (int i = 0; i < TicTacToeController.DESK_SIZE; i++)
            {
                for (int j = 0; j < TicTacToeController.DESK_SIZE; j++)
                {
                    foreach (Control control in flowLayoutPanel1.Controls)
                    {
                        Button temp = (Button)control;

                        if (((int[])temp.Tag)[0] == i && ((int[])temp.Tag)[1] == j)
                        {
                            if (TicTacToeController.desk[i, j] != 0)
                            {

                                temp.Enabled = false;

                            }
                          


                            if (TicTacToeController.desk[i, j] == 1)
                            {

                                temp.BackColor = Color.Red;
                                temp.Text = "X";

                            }
                            if (TicTacToeController.desk[i, j] == 2)
                            {

                                temp.BackColor = Color.Green;

                                temp.Text = "O";

                            }
                        }

                    }
                }
            }

        }


        bool isMoved = false;
        private void Temp_Click(object sender, EventArgs e)
        {
            try
            {
                int[] point = (int[])((Control)sender).Tag;
            TicTacToeController.hit(point[0], point[1], 2);



            //SEND
            
                send( point[0] + " " + point[1] + " " + 2);
             
            


            isMoved = true;
                txtStatus.Invoke((MethodInvoker)delegate () {
                    txtStatus.Text = "ĐẾN LƯỢT ĐỐI THỦ";
                });
                labelTime.Invoke((MethodInvoker)delegate () {
                    labelTime.Text = "10";
                });
                if (aTimer.Enabled==false)
                    aTimer.Start();
                refresh();
            }
            catch (Exception e1)
            {
                MessageBox.Show("Đánh với ma à?", "Lỗi!", MessageBoxButtons.OK,MessageBoxIcon.Error);

                connected = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(Environment.ExitCode);
        }
        private void send(string textToSend)
        {
           
            byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
            stream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        private void labelTime_Click(object sender, EventArgs e)
        {

        }
    }
}
