using System;
using System.Threading;

namespace ChessGame
{
    class Program
    {
        //board elements
       static string[,] ChessPieces = new string[8, 8]{ { "BR", "BN", "BB", "BQ", "BK", "BB", "BN", "BR" },
                                                       { "Bp", "Bp", "Bp", "Bp", "Bp", "Bp", "Bp", "Bp" },
                                                        {".",   ".",  ".",  ".",  ".",  ".",  ".",  "."},
                                                        {".",   ".",  ".",  ".",  ".",  ".",  ".",  "."},
                                                        {".",   ".",  ".",  ".",  ".",  ".",  ".",  "."},
                                                        {".",   ".",  ".",  ".",  ".",  ".",  ".",  "."},
                                                        { "Wp", "Wp", "Wp", "Wp", "Wp", "Wp", "Wp", "Wp" },
                                                        { "WR", "WN", "WB", "WQ", "WK", "WB", "WN", "WR" } };

        static string[,] Playablechess = new string[8, 8];
        static bool whitechecked = false;
        static bool blackchecked = false;
        static string[,] chesscheckw = new string[8, 8];
        static string[,] chesscheckb = new string[8, 8];
        static bool white = true;
        //for castling 
        static int count_WK = 0, count_WR1 = 0, count_WR2 = 0;
        static int count_BK = 0, count_BR1 = 0, count_BR2 = 0;
        static int getx = 0, gety = 0;
        static int cursorx=6, cursory=5;
        static string[,] castling_check = new string[8, 8];
        static void Main()
        {
            ConsoleKeyInfo cki;
            string value = "";
            bool white = true;

            string mode;
            do
            {
                Console.WriteLine("Please choose the game mode that you want to play:\n  1)Normal mode \n 2)Easy mode");
                mode = Console.ReadLine();
            } while (mode != "1" && mode != "2" );
            Console.Clear();

            GameBoard();
            refresh();


            while (true)
            {
                Console.SetCursorPosition(cursorx, cursory);
                if (Console.KeyAvailable)
                {       // true: there is a key in keyboard buffer

                    cki = Console.ReadKey(true);  // true: do not write character 

                    if (cki.Key == ConsoleKey.RightArrow && cursorx < 40)
                    {   // key and boundary control

                        cursorx += 5;
                    }
                    if (cki.Key == ConsoleKey.LeftArrow && cursorx > 10)
                    {
                        cursorx -= 5;
                    }
                    if (cki.Key == ConsoleKey.UpArrow && cursory > 6)
                    {
                        cursory -= 2;
                    }
                    if (cki.Key == ConsoleKey.DownArrow && cursory < 19)
                    {
                        cursory += 2;
                    }
                    if (((cursorx) % 2 == 0 && ((cursory) / 2) % 2 == 0) || ((cursorx) % 2 == 1 && ((cursory) / 2) % 2 == 1))
                        Console.BackgroundColor = ConsoleColor.White;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;

                    if (cki.Key == ConsoleKey.Spacebar)
                    {
                        Array.Clear(Playablechess, 0, 64);
                        Console.SetCursorPosition(25, 25);
                        value = ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5].Substring(1);
                        Console.WriteLine(value);

                        //seçilen taşa göre fonksiyon çağırma
                        if (value == "p") pawn((cursory - 5) / 2, (cursorx - 6) / 5);
                        if (value == "N") horse((cursory - 5) / 2, (cursorx - 6) / 5);
                        if (value == "K") king((cursory - 5) / 2, (cursorx - 6) / 5);
                        if (value == "Q") queen((cursory - 5) / 2, (cursorx - 6) / 5);
                        if (value == "B") bishop((cursory - 5) / 2, (cursorx - 6) / 5);
                        if (value == "R") rook((cursory - 5) / 2, (cursorx - 6) / 5);
                        if (value == "") continue;
                        getx = ((cursory - 5) / 2);
                        gety = ((cursorx - 6) / 5);

                        //colored green playable spaces if the mode is easy mode

                        if (mode == "2")
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                for (int j = 0; j < 8; j++)
                                {
                                    if (Playablechess[i, j] == "-")
                                    {
                                        Console.SetCursorPosition(j * 5 + 6, i * 2 + 5);
                                        Console.BackgroundColor = ConsoleColor.Green;
                                        Console.WriteLine("   ");
                                    }
                                    else
                                    {
                                        if (((j * 5 + 6) % 2 == 0 && ((i * 2 + 5) / 2) % 2 == 0) || ((j * 5 + 6) % 2 == 1 && ((i * 2 + 5) / 2) % 2 == 1))
                                        {
                                            Console.SetCursorPosition(j * 5 + 6, i * 2 + 5);
                                            Console.BackgroundColor = ConsoleColor.White;
                                            Console.WriteLine("   ");
                                        }
                                        else
                                        {
                                            Console.SetCursorPosition(j * 5 + 6, i * 2 + 5);
                                            Console.BackgroundColor = ConsoleColor.Black;
                                            Console.WriteLine("   ");
                                        }
                                    }
                                }
                            }
                        }
                        refresh();
                    }
                    //special movements

                    for (int i = 0; i < 8; i++)
                    {
                        for (int k = 0; k < 8; k++)
                        {

                            if (ChessPieces[7, k].Substring(0, 1) == "B" && ChessPieces[7, k].Substring(1) == "p")   // black promotion
                            {
                                promotion();
                            }
                            if (ChessPieces[0, k].Substring(0, 1) == "W" && ChessPieces[0, k].Substring(1) == "p")   // white promotion
                            {
                                promotion();
                            }
                        }
                    }

                    if (cki.Key == ConsoleKey.T)
                    {
                        string last = ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5];
                        if (white == true)//beyaz
                        {

                            if (white == true && ChessPieces[getx, gety].Substring(0, 1) == "W" && Playablechess[(cursory - 5) / 2, (cursorx - 6) / 5] == "-")
                            {

                                take((cursory - 5) / 2, (cursorx - 6) / 5);
                                //for short castling
                                if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5].Substring(1) == "K" && Math.Abs(gety - ((cursorx - 6) / 5)) == 2)
                                {
                                    ChessPieces[7, 7] = ".";
                                    ChessPieces[7, 5] = "WR";

                                }
                                //long castling
                                else if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5].Substring(1) == "K" && (Math.Abs(gety - ((cursorx - 6) / 5)) == 3))
                                {
                                    ChessPieces[7, 0] = ".";
                                    ChessPieces[7, 3] = "WR";
                                }
                                //En-passant
                                else if (getx == 3 && last == "." && Math.Abs(gety - (cursorx - 6) / 5) == 1) ChessPieces[(cursory - 5) / 2 + 1, (cursorx - 6) / 5] = ".";

                                refresh();
                                //to checking the can make castling
                                if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5] == "WK") count_WK++;
                                else if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5] == "WR")
                                {
                                    if (gety == 0) count_WR1++;
                                    else if (gety == 7) count_WR2++;
                                }

                                if (whitechecked == false)
                                {
                                    white = false;
                                }
                            }
                            //for castling check
                            Array.Clear(Playablechess, 0, 64);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int k = 0; k < 8; k++)
                                {
                                    if (ChessPieces[k, i].Substring(0, 1) == "B")
                                    {
                                        string value1 = ChessPieces[k, i].Substring(1);
                                        ///seçilen taşa göre fonksiyon çağırma

                                        if (value1 == "N") horse(k, i);
                                        if (value1 == "K") king(k, i);
                                        if (value1 == "Q") queen(k, i);
                                        if (value1 == "B") bishop(k, i);
                                        if (value1 == "R") rook(k, i);
                                    }
                                }
                            }
                            Array.Clear(castling_check, 0, 64);
                            Array.Copy(Playablechess, castling_check, 64);
                        }
                        if (white == false)//siyah
                        {

                            if (white == false && ChessPieces[getx, gety].Substring(0, 1) == "B" && Playablechess[(cursory - 5) / 2, (cursorx - 6) / 5] == "-")
                            {
                                take((cursory - 5) / 2, (cursorx - 6) / 5);
                                //for short castling
                                if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5].Substring(1) == "K" && Math.Abs(gety - (cursorx - 6) / 5) == 2)
                                {
                                    ChessPieces[0, 7] = ".";
                                    ChessPieces[0, 5] = "BR";
                                }
                                //long castling
                                else if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5].Substring(1) == "K" && (Math.Abs(gety - (cursorx - 6) / 5) == 3))
                                {
                                    ChessPieces[0, 0] = ".";
                                    ChessPieces[0, 3] = "BR";

                                }
                                //En-passant
                                else if (getx == 4 && last == "." && Math.Abs(gety - (cursorx - 6) / 5) == 1) ChessPieces[(cursory - 5) / 2 - 1, (cursorx - 6) / 5] = ".";

                                refresh();
                                //to checking the can make castling
                                if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5] == "BK") count_WK++;
                                else if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5] == "BR")
                                {
                                    if (gety == 0) count_WR1++;
                                    else if (gety == 7) count_WR2++;
                                }
                                if (blackchecked == false)
                                {
                                    white = true;
                                }
                            }

                            //castling can make?
                            Array.Clear(Playablechess, 0, 64);
                            for (int i = 0; i < 8; i++)
                            {
                                for (int k = 0; k < 8; k++)
                                {
                                    if (ChessPieces[k, i].Substring(0, 1) == "W")
                                    {
                                        string value1 = ChessPieces[k, i].Substring(1);
                                        ///seçilen taşa göre fonksiyon çağırma

                                        if (value1 == "N") horse(k, i);
                                        if (value1 == "K") king(k, i);
                                        if (value1 == "Q") queen(k, i);
                                        if (value1 == "B") bishop(k, i);
                                        if (value1 == "R") rook(k, i);
                                    }
                                }
                            }

                            Array.Clear(castling_check, 0, 64);
                            Array.Copy(Playablechess, castling_check, 64);
                        }


                        value = "";
                    }

                    Console.SetCursorPosition(cursorx, cursory);
                    if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5].Substring(0, 1) == "B")
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                    if (ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5].Substring(0, 1) == "W")
                        Console.ForegroundColor = ConsoleColor.DarkBlue;

                    Console.WriteLine(ChessPieces[(cursory - 5) / 2, (cursorx - 6) / 5].Substring(1));
                    Console.ForegroundColor = ConsoleColor.White;
                }



                Thread.Sleep(50);
            }

        }

        static void GameBoard()
        {
            //basic visual design of chess board
            for (int i = 4; i < 20; i += 2)
            {
                for (int j = 4; j < 44; j += 5)
                {
                    Console.SetCursorPosition(j, i);
                    if ((j % 2 == 0 && (i / 2) % 2 == 0) || (j % 2 == 1 && (i / 2) % 2 == 1))
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write("     ");
                        Console.SetCursorPosition(j, i + 1);
                        Console.Write("     ");
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.Write("     ");
                        Console.SetCursorPosition(j, i + 1);
                        Console.Write("     ");

                    }
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;

            //print numbers and letters of the sides of board
            for (int i = 1; i < 9; i++)
            {
                Console.SetCursorPosition(2, i * 2 + 3);
                Console.WriteLine(9 - i);
                Console.SetCursorPosition(i * 5 + 1, 3);
                Console.WriteLine(Convert.ToChar(i + 96));
            }

        }


        static void refresh()
        {
            //refreshing the board elements
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Console.SetCursorPosition(j * 5 + 6, i * 2 + 5);
                    if (((j * 5 + 6) % 2 == 0 && ((i * 2 + 5) / 2) % 2 == 0) || ((j * 5 + 6) % 2 == 1 && ((i * 2 + 5) / 2) % 2 == 1))
                        Console.BackgroundColor = ConsoleColor.White;
                    else
                        Console.BackgroundColor = ConsoleColor.Black;
                    if (ChessPieces[i, j].Substring(0, 1) == "B")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(ChessPieces[i, j].Substring(1));
                    }
                    if (ChessPieces[i, j].Substring(0, 1) == "W")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.Write(ChessPieces[i, j].Substring(1));
                    }
                    if (ChessPieces[i, j] == ".") Console.Write(" ");


                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
            if (whitechecked == true) { Console.SetCursorPosition(46, 5); Console.WriteLine("white checked"); }
            if (blackchecked == true) { Console.SetCursorPosition(46, 6); Console.WriteLine("black checked"); }
            if (whitechecked == false) { Console.SetCursorPosition(46, 5); Console.WriteLine("             "); }
            if (blackchecked == false) { Console.SetCursorPosition(46, 6); Console.WriteLine("             "); }
            Console.ForegroundColor = ConsoleColor.White;
        }


       static void rook(int x, int y) //movement of rook and check 
        {
            int z = y;
            // down
            for (int i = x + 1; i <= 7; i++)
            {
                if (ChessPieces[i, z] == "." || ChessPieces[i, z].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[i, z] = "-";
                if (ChessPieces[i, z] == "WK" && ChessPieces[x, y].Substring(0, 1) == "B") { whitechecked = true; break; }
                if (ChessPieces[i, z] == "BK" && ChessPieces[x, y].Substring(0, 1) == "W") { blackchecked = true; break; }
                if (ChessPieces[i, z].Substring(0, 1) == "B" && ChessPieces[x, y].Substring(0, 1) == "W") break;
                if (ChessPieces[i, z].Substring(0, 1) == "W" && ChessPieces[x, y].Substring(0, 1) == "B") break;
                if (ChessPieces[i, z].Substring(0, 1) == ChessPieces[x, y].Substring(0, 1)) break;
            }

            //up
            for (int i = x - 1; i >= 0; i--)
            {
                if (ChessPieces[i, z] == "." || ChessPieces[i, z].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[i, z] = "-";
                if (ChessPieces[i, z] == "WK" && ChessPieces[x, y].Substring(0, 1) == "B") { whitechecked = true; break; }
                if (ChessPieces[i, z] == "BK" && ChessPieces[x, y].Substring(0, 1) == "W") { blackchecked = true; break; }
                if (ChessPieces[i, z].Substring(0, 1) == "B" && ChessPieces[x, y].Substring(0, 1) == "W") break;
                if (ChessPieces[i, z].Substring(0, 1) == "W" && ChessPieces[x, y].Substring(0, 1) == "B") break;
                if (ChessPieces[i, z].Substring(0, 1) == ChessPieces[x, y].Substring(0, 1)) break;
            }

            //left 
            for (int i = z - 1; i >= 0; i--)
            {
                if (ChessPieces[x, i] == "." || ChessPieces[x, i].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x, i] = "-";
                if (ChessPieces[x, i] == "WK" && ChessPieces[x, y].Substring(0, 1) == "B") { whitechecked = true; break; }
                if (ChessPieces[x, i] == "BK" && ChessPieces[x, y].Substring(0, 1) == "W") { blackchecked = true; break; }
                if (ChessPieces[x, i].Substring(0, 1) == "B" && ChessPieces[x, y].Substring(0, 1) == "W") break;
                if (ChessPieces[x, i].Substring(0, 1) == "W" && ChessPieces[x, y].Substring(0, 1) == "B") break;
                if (ChessPieces[x, i].Substring(0, 1) == ChessPieces[x, y].Substring(0, 1)) break;
            }

            //right & up
            for (int i = z + 1; i <= 7; i++)
            {
                if (ChessPieces[x, i] == "." || ChessPieces[x, i].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x, i] = "-";
                if (ChessPieces[x, i] == "WK" && ChessPieces[x, y].Substring(0, 1) == "B") { whitechecked = true; break; }
                if (ChessPieces[x, i] == "BK" && ChessPieces[x, y].Substring(0, 1) == "W") { blackchecked = true; break; }
                if (ChessPieces[x, i].Substring(0, 1) == "B" && ChessPieces[x, y].Substring(0, 1) == "W") break;
                if (ChessPieces[x, i].Substring(0, 1) == "W" && ChessPieces[x, y].Substring(0, 1) == "B") break;
                if (ChessPieces[x, i].Substring(0, 1) == ChessPieces[x, y].Substring(0, 1)) break;
            }
        }
        static void bishop(int x, int y)
        {
            int z = y;
            //right & down
            for (int i = x + 1; i <= 7; i++)
            {
                z++;
                if (z > 7) break;
                if (ChessPieces[i, z] == "." || ChessPieces[i, z].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[i, z] = "-";
                if (ChessPieces[i, z] == "WK" && ChessPieces[x, y].Substring(0, 1) == "B") { whitechecked = true; break; }
                if (ChessPieces[i, z] == "BK" && ChessPieces[x, y].Substring(0, 1) == "W") { blackchecked = true; break; }
                if (ChessPieces[i, z].Substring(0, 1) == "B" && ChessPieces[x, y].Substring(0, 1) == "W") break;
                if (ChessPieces[i, z].Substring(0, 1) == "W" && ChessPieces[x, y].Substring(0, 1) == "B") break;
                if (ChessPieces[i, z].Substring(0, 1) == ChessPieces[x, y].Substring(0, 1)) break;
            }
            z = y;
            //left & down
            for (int i = x + 1; i <= 7; i++)
            {
                z--;
                if (z < 0) break;
                if (ChessPieces[i, z] == "." || ChessPieces[i, z].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[i, z] = "-";
                if (ChessPieces[i, z] == "WK" && ChessPieces[x, y].Substring(0, 1) == "B") { whitechecked = true; break; }
                if (ChessPieces[i, z] == "BK" && ChessPieces[x, y].Substring(0, 1) == "W") { blackchecked = true; break; }
                if (ChessPieces[i, z].Substring(0, 1) == "B" && ChessPieces[x, y].Substring(0, 1) == "W") break;
                if (ChessPieces[i, z].Substring(0, 1) == "W" && ChessPieces[x, y].Substring(0, 1) == "B") break;
                if (ChessPieces[i, z].Substring(0, 1) == ChessPieces[x, y].Substring(0, 1)) break;
            }
            z = y;
            //left & up
            for (int i = x - 1; i >= 0; i--)
            {
                z--;
                if (z < 0) break;
                if (ChessPieces[i, z] == "." || ChessPieces[i, z].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[i, z] = "-";
                if (ChessPieces[i, z] == "WK" && ChessPieces[x, y].Substring(0, 1) == "B") { whitechecked = true; break; }
                if (ChessPieces[i, z] == "BK" && ChessPieces[x, y].Substring(0, 1) == "W") { blackchecked = true; break; }
                if (ChessPieces[i, z].Substring(0, 1) == "B" && ChessPieces[x, y].Substring(0, 1) == "W") break;
                if (ChessPieces[i, z].Substring(0, 1) == "W" && ChessPieces[x, y].Substring(0, 1) == "B") break;
                if (ChessPieces[i, z].Substring(0, 1) == ChessPieces[x, y].Substring(0, 1)) break;
            }
            z = y;
            //right & up
            for (int i = x - 1; i >= 0; i--)
            {
                z++;
                if (z > 7) break;
                if (ChessPieces[i, z] == "." || ChessPieces[i, z].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[i, z] = "-";
                if (ChessPieces[i, z] == "WK" && ChessPieces[x, y].Substring(0, 1) == "B") { whitechecked = true; break; }
                if (ChessPieces[i, z] == "BK" && ChessPieces[x, y].Substring(0, 1) == "W") { blackchecked = true; break; }
                if (ChessPieces[i, z].Substring(0, 1) == "B" && ChessPieces[x, y].Substring(0, 1) == "W") break;
                if (ChessPieces[i, z].Substring(0, 1) == "W" && ChessPieces[x, y].Substring(0, 1) == "B") break;
                if (ChessPieces[i, z].Substring(0, 1) == ChessPieces[x, y].Substring(0, 1)) break;
            }

        }


       static void queen(int x, int y)
        {
            rook(x, y);
            bishop(x, y);

        }
        static void king(int x, int y)
        {
            if (x > 0) if (ChessPieces[x - 1, y] == "." || ChessPieces[x - 1, y].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x - 1, y] = "-";
            if (x > 0 && y > 0) if (ChessPieces[x - 1, y - 1] == "." || ChessPieces[x - 1, y - 1].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x - 1, y - 1] = "-";
            if (x > 0 && y < 7) if (ChessPieces[x - 1, y + 1] == "." || ChessPieces[x - 1, y + 1].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x - 1, y + 1] = "-";
            if (x < 7) if (ChessPieces[x + 1, y] == "." || ChessPieces[x + 1, y].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x + 1, y] = "-";
            if (x < 7 && y > 0) if (ChessPieces[x + 1, y - 1] == "." || ChessPieces[x + 1, y - 1].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x + 1, y - 1] = "-";
            if (x < 7 && y < 7) if (ChessPieces[x + 1, y + 1] == "." || ChessPieces[x + 1, y + 1].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x + 1, y + 1] = "-";
            if (y > 0) if (ChessPieces[x, y - 1] == "." || ChessPieces[x, y - 1].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x, y - 1] = "-";
            if (y < 7) if (ChessPieces[x, y + 1] == "." || ChessPieces[x, y + 1].Substring(0, 1) != ChessPieces[x, y].Substring(0, 1)) Playablechess[x, y + 1] = "-";
            //long castling for white
            if (ChessPieces[x, y].Substring(0, 1) == "W" && count_WK == 0 && count_WR1 == 0 && ChessPieces[7, 1] == "." && ChessPieces[7, 2] == "." && ChessPieces[7, 3] == "." &&
                castling_check[7, 1] != "-" && castling_check[7, 2] != "-" && castling_check[7, 3] != "-" && castling_check[7, 4] != "-") Playablechess[7, 1] = "-";
            //short castling for white
            if (ChessPieces[x, y].Substring(0, 1) == "W" && count_WK == 0 && count_WR2 == 0 && ChessPieces[7, 6] == "." && ChessPieces[7, 5] == "." &&
                castling_check[7, 6] != "-" && castling_check[7, 5] != "-" && castling_check[7, 4] != "-") Playablechess[7, 6] = "-";
            //long castling for black
            if (ChessPieces[x, y].Substring(0, 1) == "B" && count_BK == 0 && count_BR1 == 0 && ChessPieces[0, 1] == "." && ChessPieces[0, 2] == "." && ChessPieces[0, 3] == "." &&
             castling_check[0, 1] != "-" && castling_check[0, 2] != "-" && castling_check[0, 3] != "-" && castling_check[0, 4] != "-") Playablechess[0, 1] = "-";
            //short castling for black
            if (ChessPieces[x, y].Substring(0, 1) == "B" && count_BK == 0 && count_BR2 == 0 && ChessPieces[0, 6] == "." && ChessPieces[0, 5] == "." &&
                castling_check[0, 6] != "-" && castling_check[0, 5] != "-" && castling_check[0, 4] != "-") Playablechess[0, 6] = "-";

        }
       static void horse(int x, int y)
        {

            if (ChessPieces[x, y].Substring(0, 1) == "W")
            {
                if (x >= 2 && y >= 1) if (ChessPieces[x - 2, y - 1] == "BK") blackchecked = true;
                if (x >= 2 && y <= 6) if (ChessPieces[x - 2, y + 1] == "BK") blackchecked = true;
                if (x <= 5 && y >= 1) if (ChessPieces[x + 2, y - 1] == "BK") blackchecked = true;
                if (x <= 5 && y <= 6) if (ChessPieces[x + 2, y + 1] == "BK") blackchecked = true;

                if (x >= 1 && y >= 2) if (ChessPieces[x - 1, y - 2] == "BK") blackchecked = true;
                if (x >= 1 && y <= 5) if (ChessPieces[x - 1, y + 2] == "BK") blackchecked = true;
                if (x <= 6 && y >= 2) if (ChessPieces[x + 1, y - 2] == "BK") blackchecked = true;
                if (x <= 6 && y <= 5) if (ChessPieces[x + 1, y + 2] == "BK") blackchecked = true;

                if (x >= 2 && y >= 1) if (ChessPieces[x - 2, y - 1] == "." || ChessPieces[x - 2, y - 1].Substring(0, 1) == "B") Playablechess[x - 2, y - 1] = "-";
                if (x >= 2 && y <= 6) if (ChessPieces[x - 2, y + 1] == "." || ChessPieces[x - 2, y + 1].Substring(0, 1) == "B") Playablechess[x - 2, y + 1] = "-";
                if (x <= 5 && y >= 1) if (ChessPieces[x + 2, y - 1] == "." || ChessPieces[x + 2, y - 1].Substring(0, 1) == "B") Playablechess[x + 2, y - 1] = "-";
                if (x <= 5 && y <= 6) if (ChessPieces[x + 2, y + 1] == "." || ChessPieces[x + 2, y + 1].Substring(0, 1) == "B") Playablechess[x + 2, y + 1] = "-";

                if (x >= 1 && y >= 2) if (ChessPieces[x - 1, y - 2] == "." || ChessPieces[x - 1, y - 2].Substring(0, 1) == "B") Playablechess[x - 1, y - 2] = "-";
                if (x >= 1 && y <= 5) if (ChessPieces[x - 1, y + 2] == "." || ChessPieces[x - 1, y + 2].Substring(0, 1) == "B") Playablechess[x - 1, y + 2] = "-";
                if (x <= 6 && y >= 2) if (ChessPieces[x + 1, y - 2] == "." || ChessPieces[x + 1, y - 2].Substring(0, 1) == "B") Playablechess[x + 1, y - 2] = "-";
                if (x <= 6 && y <= 5) if (ChessPieces[x + 1, y + 2] == "." || ChessPieces[x + 1, y + 2].Substring(0, 1) == "B") Playablechess[x + 1, y + 2] = "-";
            }
            if (ChessPieces[x, y].Substring(0, 1) == "B")
            {
                if (x >= 2 && y >= 1) if (ChessPieces[x - 2, y - 1] == "WK") whitechecked = true;
                if (x >= 2 && y <= 6) if (ChessPieces[x - 2, y + 1] == "WK") whitechecked = true;
                if (x <= 5 && y >= 1) if (ChessPieces[x + 2, y - 1] == "WK") whitechecked = true;
                if (x <= 5 && y <= 6) if (ChessPieces[x + 2, y + 1] == "WK") whitechecked = true;

                if (x >= 1 && y >= 2) if (ChessPieces[x - 1, y - 2] == "WK") whitechecked = true;
                if (x >= 1 && y <= 5) if (ChessPieces[x - 1, y + 2] == "WK") whitechecked = true;
                if (x <= 6 && y >= 2) if (ChessPieces[x + 1, y - 2] == "WK") whitechecked = true;
                if (x <= 6 && y <= 5) if (ChessPieces[x + 1, y + 2] == "WK") whitechecked = true;

                if (x >= 2 && y >= 1) if (ChessPieces[x - 2, y - 1] == "." || ChessPieces[x - 2, y - 1].Substring(0, 1) == "W") Playablechess[x - 2, y - 1] = "-";
                if (x >= 2 && y <= 6) if (ChessPieces[x - 2, y + 1] == "." || ChessPieces[x - 2, y + 1].Substring(0, 1) == "W") Playablechess[x - 2, y + 1] = "-";
                if (x <= 5 && y >= 1) if (ChessPieces[x + 2, y - 1] == "." || ChessPieces[x + 2, y - 1].Substring(0, 1) == "W") Playablechess[x + 2, y - 1] = "-";
                if (x <= 5 && y <= 6) if (ChessPieces[x + 2, y + 1] == "." || ChessPieces[x + 2, y + 1].Substring(0, 1) == "W") Playablechess[x + 2, y + 1] = "-";

                if (x >= 1 && y >= 2) if (ChessPieces[x - 1, y - 2] == "." || ChessPieces[x - 1, y - 2].Substring(0, 1) == "W") Playablechess[x - 1, y - 2] = "-";
                if (x >= 1 && y <= 5) if (ChessPieces[x - 1, y + 2] == "." || ChessPieces[x - 1, y + 2].Substring(0, 1) == "W") Playablechess[x - 1, y + 2] = "-";
                if (x <= 6 && y >= 2) if (ChessPieces[x + 1, y - 2] == "." || ChessPieces[x + 1, y - 2].Substring(0, 1) == "W") Playablechess[x + 1, y - 2] = "-";
                if (x <= 6 && y <= 5) if (ChessPieces[x + 1, y + 2] == "." || ChessPieces[x + 1, y + 2].Substring(0, 1) == "W") Playablechess[x + 1, y + 2] = "-";
            }


        }
       static void pawn(int x, int y)
        {
            if (ChessPieces[x, y].Substring(0, 1) == "W")
            {
                if (ChessPieces[x - 1, y] == ".")
                {
                    Playablechess[x - 1, y] = "-";
                    if (x == 6 && ChessPieces[x - 2, y] == ".") Playablechess[x - 2, y] = "-";
                }

                //en passant
                if (x == 3)
                {
                    if (y > 0 && ChessPieces[x, y - 1] == "Bp" && ChessPieces[x - 1, y - 1] == ".") Playablechess[x - 1, y - 1] = "-";
                    else if (y < 7 && ChessPieces[x, y + 1] == "Bp" && ChessPieces[x - 1, y + 1] == ".") Playablechess[x - 1, y + 1] = "-";
                }

                if (y != 0) if (ChessPieces[x - 1, y - 1].Substring(0, 1) == "B") Playablechess[x - 1, y - 1] = "-";
                if (y != 7) if (ChessPieces[x - 1, y + 1].Substring(0, 1) == "B") Playablechess[x - 1, y + 1] = "-";
                if (y != 0) if (ChessPieces[x - 1, y - 1].Substring(0, 1) == "BK") blackchecked = true;
                if (y != 7) if (ChessPieces[x - 1, y + 1].Substring(0, 1) == "BK") blackchecked = true;

            }

            if (ChessPieces[x, y].Substring(0, 1) == "B")
            {
                if (ChessPieces[x + 1, y] == ".")
                {
                    Playablechess[x + 1, y] = "-";
                    if (x == 1 && ChessPieces[x + 2, y] == ".") Playablechess[x + 2, y] = "-";
                }
                //en passant
                if (x == 4 && y > 0 && ChessPieces[4, y - 1] == "Wp" && ChessPieces[x + 1, y - 1] == ".") Playablechess[x + 1, y - 1] = "-";
                if (x == 4 && y < 7 && ChessPieces[4, y + 1] == "Wp" && ChessPieces[x + 1, y + 1] == ".") Playablechess[x + 1, y + 1] = "-";

                if (y != 0) if (ChessPieces[x + 1, y - 1].Substring(0, 1) == "W") Playablechess[x + 1, y - 1] = "-";
                if (y != 7) if (ChessPieces[x + 1, y + 1].Substring(0, 1) == "W") Playablechess[x + 1, y + 1] = "-";
                if (y != 0) if (ChessPieces[x - 1, y - 1].Substring(0, 1) == "WK") whitechecked = true;
                if (y != 7) if (ChessPieces[x - 1, y + 1].Substring(0, 1) == "WK") whitechecked = true;
            }
        }


       static void promotion()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {

                    if (ChessPieces[0, k].Substring(0, 1) == "W" && ChessPieces[0, k].Substring(1) == "p")
                    {
                        Console.SetCursorPosition(0, 23);
                        Console.WriteLine("1 : Queen");
                        Console.WriteLine("2 : Rook");
                        Console.WriteLine("3 : Bishop");
                        Console.WriteLine("4 : Knight");
                        int pnumber;
                        do
                        {
                            pnumber = Convert.ToInt32(Console.ReadLine());
                        } while (pnumber != 1 && pnumber != 2 && pnumber != 3 && pnumber != 4);
                        if (pnumber == 1) { ChessPieces[0, k] = "WQ"; }
                        if (pnumber == 2) { ChessPieces[0, k] = "WR"; }
                        if (pnumber == 3) { ChessPieces[0, k] = "WB"; }
                        if (pnumber == 4) { ChessPieces[0, k] = "WN"; }
                    }

                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {

                    if (ChessPieces[7, k].Substring(0, 1) == "B" && ChessPieces[7, k].Substring(1) == "p")
                    {
                        Console.SetCursorPosition(0, 23);
                        Console.WriteLine("1 : Queen");
                        Console.WriteLine("2 : Rook");
                        Console.WriteLine("3 : Bishop");
                        Console.WriteLine("4 : Knight");
                        int pnumber;
                        do
                        {
                            pnumber = Convert.ToInt32(Console.ReadLine());
                        } while (pnumber != 1 && pnumber != 2 && pnumber != 3 && pnumber != 4);
                        if (pnumber == 1) { ChessPieces[7, k] = "BQ"; }
                        if (pnumber == 2) { ChessPieces[7, k] = "BR"; }
                        if (pnumber == 3) { ChessPieces[7, k] = "BB"; }
                        if (pnumber == 4) { ChessPieces[7, k] = "BN"; }
                    }

                }
            }
        }


        static void kingcantmove()
        {
            string value1;
            int blackkingx, blackkingy, whitekingx, whitekingy;
            if (true)
            {
                blackchecked = false;
                Array.Clear(Playablechess, 0, 64);
                for (int i = 0; i < 8; i++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (ChessPieces[i, k].Substring(0, 1) == "W")//beyaz taşlatın hareket ettiği yerler
                        {
                            value1 = ChessPieces[i, k].Substring(1);
                            ///seçilen taşa göre fonksiyon çağırma

                            if (value1 == "N") horse(i, k);
                            Array.Clear(Playablechess, 0, 64);
                            if (value1 == "K") king(i, k);
                            Array.Clear(Playablechess, 0, 64);
                            if (value1 == "Q") queen(i, k);
                            Array.Clear(Playablechess, 0, 64);
                            if (value1 == "B") bishop(i, k);
                            Array.Clear(Playablechess, 0, 64);
                            if (value1 == "R") rook(i, k);
                            Array.Copy(Playablechess, chesscheckw, 64);

                            Array.Clear(Playablechess, 0, 64);
                        }
                    }
                }

            }
            if (true)
            {
                whitechecked = false;
                Array.Clear(Playablechess, 0, 64);
                for (int i = 0; i < 8; i++)
                {
                    for (int k = 0; k < 8; k++)
                    {
                        if (ChessPieces[i, k].Substring(0, 1) == "B")//siyah taşlatın hareket ettiği yerler
                        {
                            value1 = ChessPieces[i, k].Substring(1);
                            ///seçilen taşa göre fonksiyon çağırma

                            if (value1 == "N") horse(i, k);
                            Array.Clear(Playablechess, 0, 64);
                            if (value1 == "K") king(i, k);
                            Array.Clear(Playablechess, 0, 64);
                            if (value1 == "Q") queen(i, k);
                            Array.Clear(Playablechess, 0, 64);
                            if (value1 == "B") bishop(i, k);
                            Array.Clear(Playablechess, 0, 64);
                            if (value1 == "R") rook(i, k);
                            Array.Copy(Playablechess, chesscheckb, 64);
                            Array.Clear(Playablechess, 0, 64);
                        }
                    }
                }
            }
        }


      static  void take(int x, int y)
        {
            if (Playablechess[x, y] == "-")
            {
                //checkcontrol();


                ChessPieces[x, y] = ChessPieces[getx, gety];
                ChessPieces[getx, gety] = ".";
                kingcantmove();
                if (white == true && whitechecked == true)
                {
                    ChessPieces[getx, gety] = ChessPieces[x, y];
                    ChessPieces[x, y] = ".";
                    white = true;
                }
                if (white == false && blackchecked == true)
                {
                    ChessPieces[getx, gety] = ChessPieces[x, y];
                    ChessPieces[x, y] = ".";
                }
            }

            refresh();
        }

    }
}
