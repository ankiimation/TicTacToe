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
using TicTacToe_Client;

namespace TicTacToeServer
{
    public partial class Form1 : Form
    {
        System.Timers.Timer aTimer = new System.Timers.Timer();
        static int PORT;
        static int TIME = 20;
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

            int time = Convert.ToInt32(labelTime.Text);
            time--;

            labelTime.Invoke((MethodInvoker)delegate () { labelTime.Text = time.ToString(); });
            if (time == 0)
            {

                aTimer.Stop();

                //  endGame = true;
                //  flowLayoutPanel1.Invoke((MethodInvoker)delegate() { newDesk(); });
            }



        }
        //Tao socket
        TcpClient client = new TcpClient();
        // Ket noi den server
        Stream stream;

        private void Form1_Load(object sender, EventArgs e)
        {

            // refresh();
            bool joinSuccessfully = false;

            do
            {
                JoinRoomForm joinRoomForm = new JoinRoomForm();
                if (joinRoomForm.ShowDialog() == DialogResult.OK)
                {
                    PORT = joinRoomForm.PORT;
                    try
                    {
                        client.Connect("127.0.0.1", PORT);
                        joinSuccessfully = true;

                    }
                    catch (Exception e1)
                    {
                        joinSuccessfully = false;
                        MessageBox.Show("Không thể tham gia bàn!\n + Bàn đang chơi\n + Bàn không tồn tại", "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                }
            } while (!joinSuccessfully);

            Thread thread = new Thread(delegate ()
            {


                // 1. connect

                stream = client.GetStream();

                Console.WriteLine("Đã kết nối");

                //GET DESK SIZE
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
                string receivedString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                TicTacToeController.DESK_SIZE = Convert.ToInt32(receivedString);
                groupBox2.Invoke((MethodInvoker)delegate ()
                {
                    labelDeskSize.Text = TicTacToeController.DESK_SIZE.ToString();
                    labelPort.Text = PORT.ToString();
                });
                //NEW GAME
                flowLayoutPanel1.Invoke((MethodInvoker)delegate () { newDesk(); });
                while (true)
                {


                    buffer = new byte[client.ReceiveBufferSize];
                    bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);
                    receivedString = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (receivedString.Contains("!hit"))
                    {
                        receivedString = receivedString.Replace("!hit", "");

                        Console.WriteLine(receivedString);

                        string[] rs = receivedString.Split(' ');
                        int i = Convert.ToInt32(rs[0]);
                        int j = Convert.ToInt32(rs[1]);
                        int playerNumber = Convert.ToInt32(rs[2]);

                        TicTacToeController.hit(i, j, playerNumber);
                        isMoved = false;
                        txtStatus.Invoke((MethodInvoker)delegate ()
                        {
                            txtStatus.Text = "ĐẾN LƯỢT BẠN";
                        });
                        if (aTimer.Enabled == false)
                            aTimer.Start();
                        labelTime.Invoke((MethodInvoker)delegate () { labelTime.Text = TIME.ToString(); });
                        flowLayoutPanel1.Invoke((MethodInvoker)delegate () { refresh(); });
                    }


                    else if (receivedString.Equals("!endgame"))
                    {
                        if (isMoved)
                        {
                            TicTacToeController.winnerNumber = 1;
                            txtStatus.Invoke((MethodInvoker)delegate () { txtStatus.Text = "HẾT GIỜ! BẠN ĐÃ THAWSNG!"; });
                        }
                        else
                        {
                            TicTacToeController.winnerNumber = 2;
                            txtStatus.Invoke((MethodInvoker)delegate () { txtStatus.Text = "HẾT GIỜ! BẠN ĐÃ THUA!"; });
                        }
                        this.Invoke((MethodInvoker)delegate () { this.Enabled = false; });

                        endGame = true;
                        this.Invoke((MethodInvoker)delegate () { this.Enabled = false; });
                        flowLayoutPanel1.Invoke((MethodInvoker)delegate () { refresh(); });
                        labelTime.Invoke((MethodInvoker)delegate () { labelTime.Text = TIME.ToString(); });

                    }
                    else if (receivedString.Equals("!newgame"))
                    {
                        newGame = true;
                        this.Invoke((MethodInvoker)delegate () { this.Enabled = false; });
                        endGame = true;
                        flowLayoutPanel1.Invoke((MethodInvoker)delegate () { refresh(); });
                        labelTime.Invoke((MethodInvoker)delegate () { labelTime.Text = TIME.ToString(); });
                    }

                    else if (receivedString.Equals("!2win"))
                    {

                        txtStatus.Invoke((MethodInvoker)delegate () { txtStatus.Text = "BẠN ĐÃ THUA!"; });
                        this.Invoke((MethodInvoker)delegate () { this.Enabled = false; });


                        endGame = true;
                        flowLayoutPanel1.Invoke((MethodInvoker)delegate () { refresh(); });
                        labelTime.Invoke((MethodInvoker)delegate () { labelTime.Text = TIME.ToString(); });

                    }

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
        private void newDesk()
        {
            isMoved = false;
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Text = "CHIẾN THÔI!";
            });
            flowLayoutPanel1.Controls.Clear();
            for (int i = 0; i < TicTacToeController.DESK_SIZE; i++)
            {
                for (int j = 0; j < TicTacToeController.DESK_SIZE; j++)
                {
                    Button temp = new Button();


                    //  temp.BackColor = Color.White;





                    temp.Width =30;
                    temp.Height = temp.Width;

                    temp.Tag = new int[2] { i, j };
                    temp.Click += Temp_Click;
                    flowLayoutPanel1.Controls.Add(temp);

                }
            }
            flowLayoutPanel1.Enabled = true;
        }

        bool endGame = false;
        bool newGame = false;
        private void refresh()
        {
           
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

            if (!TicTacToeController.canMove() || endGame || TicTacToeController.winnerNumber != 0)
            {



                TicTacToeController.newGame();
                endGame = false;

                aTimer.Stop();



                if (newGame)
                {
                    this.Invoke((MethodInvoker)delegate () { this.Enabled = true; });
                    newGame = false;
                    newDesk();
                }


                // return;

            }
        }

        bool isMoved = false;
        private void Temp_Click(object sender, EventArgs e)
        {
            int[] point = (int[])((Control)sender).Tag;


            send("!hit" + point[0] + " " + point[1] + " " + 1);




            TicTacToeController.hit(point[0], point[1], 1);
            isMoved = true;
            txtStatus.Invoke((MethodInvoker)delegate ()
            {
                txtStatus.Text = "ĐẾN LƯỢT ĐỐI THỦ";
            });

            if (TicTacToeController.winnerNumber == 1)
            {
                send("!1win");
                txtStatus.Invoke((MethodInvoker)delegate () { txtStatus.Text = "BẠN ĐÃ THẮNG!"; });
            }



            if (aTimer.Enabled == false)
                aTimer.Start();
            labelTime.Invoke((MethodInvoker)delegate () { labelTime.Text = TIME.ToString(); });
            refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
        private void send(string textToSend)
        {

            byte[] bytesToSend = Encoding.UTF8.GetBytes(textToSend);
            stream.Write(bytesToSend, 0, bytesToSend.Length);
        }
    }
}
