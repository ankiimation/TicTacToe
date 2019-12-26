
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
        public static int winnerNumber = 0;


        static TicTacToeController()
        {

            newGame();
        }

        public static void newGame()
        {
            winnerNumber = 0;
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
            updateWinnerNumber(i, j, playerNumber);
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

        public static void updateWinnerNumber(int x, int y, int playerNumber)
        {


            int demNgang = 0;
            int demDoc = 0;
            int demCheo = 0;


            // CHECK DOC
            int toaDoX = x;
            while (toaDoX - 1 > 0 && desk[toaDoX - 1, y] == playerNumber)
            {
                demDoc++;
                Console.WriteLine("DEM DOC: " + demNgang);
                toaDoX--;
            }

            toaDoX = x;
            while (toaDoX + 1 < DESK_SIZE && desk[toaDoX + 1, y] == playerNumber)
            {
                demDoc++;
                Console.WriteLine("DEM DOC: " + demNgang);
                toaDoX++;
            }

            // CHECK NGANG
            int toaDoY = y;
            while (toaDoY - 1 > 0 && desk[x, toaDoY - 1] == playerNumber)
            {
                demNgang++;
                Console.WriteLine("DEM NGANG: " + demNgang);
                toaDoY--;
            }

            toaDoY = y;
            while (toaDoY + 1 < DESK_SIZE && desk[x, toaDoY + 1] == playerNumber)
            {
                demNgang++;
                Console.WriteLine("DEM NGANG: " + demNgang);
                toaDoY++;
            }

            


            if (demCheo >= 4 || demDoc >= 4 || demNgang >= 4)
            {
                winnerNumber = playerNumber;
                Console.WriteLine(playerNumber + " WINNNNNNNNNNNNNNNN!");


            }

        }


    }
}
