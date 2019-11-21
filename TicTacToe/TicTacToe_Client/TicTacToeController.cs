
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeServer
{
    static class TicTacToeController
    {
        public static int[,] desk = new int[3, 3];

        static TicTacToeController()
        {

            newGame();
        }

        public static void newGame()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    desk[i, j] = 0;
                }
            }
        }

        public static void hit(int i, int j, int playerNumber)
        {
            desk[i, j] = playerNumber;
        }

        public static bool canMove()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (desk[i, j] == 0)
                        return true;
                }
            }
            return false;
        }

       

    }
}
