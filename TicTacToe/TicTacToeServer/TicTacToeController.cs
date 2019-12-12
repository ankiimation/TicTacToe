
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToeServer
{
    static class TicTacToeController
    {
        public static int DESK_SIZE = 20;
        public static int[,] desk = new int[DESK_SIZE, DESK_SIZE];


        static TicTacToeController()
        {

            newGame();
        }

        public static void newGame()
        {
            for (int i = 0; i < DESK_SIZE; i++)
            {
                for (int j = 0; j < DESK_SIZE; j++)
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
            for (int i = 0; i < DESK_SIZE; i++)
            {
                for (int j = 0; j < DESK_SIZE; j++)
                {
                    if (desk[i, j] == 0)
                        return true;
                }
            }
            return false;
        }



    }
}
