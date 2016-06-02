using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipGame.Date
{
    class Computer: Actor
    {
        int coordinateX = -1, coordinateY = -1;
        List<int> tmp_ship = new List<int>();
        byte type = 0;

        byte[,] enemyNumberANDsize_ship;
        public Computer(byte size):base(size){}
        public Computer(byte size, byte[,] array) : base(size, array)
        {
            CopyArray(numberANDsize_ship_szablon, out enemyNumberANDsize_ship);
        }

        public void ComputerLogic(out int coorX, out int coorY)
        {
            if (coordinateX == -1 && coordinateY == -1)
            {
                Random rnd = new Random();
                do {
                    do
                    {
                        coorX = rnd.Next(0, size_board);
                        coorY = rnd.Next(0, size_board);
                    }
                    while (array_of_shots[coorX, coorY] != 0);
                }
                while (!It_is_necessary_shoot(coorX, coorY));
            }
            else
            {
                coorX = coordinateX; coorY = coordinateY;
            }
        }

        private bool It_is_necessary_shoot(int coorX, int coorY)
        {
            int ship_size = 0;
            int x1, x2, y1, y2;
            for (int i = 0; i < enemyNumberANDsize_ship.GetLength(0);)
            {
                if (enemyNumberANDsize_ship[i, 1] == 0)
                {
                    i++;
                    continue;
                }
                else ship_size = enemyNumberANDsize_ship[i, 0];

                x1 = coorX - ship_size + 1;
                x2 = coorX;
                y1 = y2 = coorY;
                for (int j = 0; j < ship_size; j++)
                {
                    if (x1 >= 0 && x2 <= size_board - 1)
                    {
                        if (CheckShot(x1, x2, y1, y2)) return true;
                    }
                    x1++;
                    x2++;
                }

                y1 = coorY - ship_size + 1;
                y2 = coorY;
                x1 = x2 = coorX;
                for (int j = 0; j < ship_size; j++)
                {
                    if (y1 >= 0 && y2 <= size_board - 1)
                    {
                        if (CheckShot(x1, x2, y1, y2)) return true;
                    }
                    y1++;
                    y2++;
                }
                break;
            }
            return false;
        }

        private bool CheckShot(int x1, int x2, int y1, int y2)
        {
            if(x1==x2)
            {
                for (int i = y1; i <= y2; i++)
                {
                    if (array_of_shots[x1, i] != 0)
                        return false;
                }
            }
            else if(y1==y2)
            {
                for (int i = x1; i <= x2; i++)
                {
                    if (array_of_shots[i, y1] != 0)
                        return false;
                }
            }
            return true;
        }
        public void Result_OF_Shot(int coorX, int coorY, int status, bool win = false)
        {
            array_of_shots[coorX, coorY] = status;
            if(status==-2)
            {
                coordinateX = -1;
                coordinateY = -1;
                tmp_ship.Add(coorX);
                tmp_ship.Add(coorY);
                for (int i = 0; i < enemyNumberANDsize_ship.GetLength(0); i++)
                {
                    if (enemyNumberANDsize_ship[i, 0] == (int)(tmp_ship.Count / 2))
                    {
                        --enemyNumberANDsize_ship[i, 1];
                        break;
                    }
                }
                if (tmp_ship.Count > 2 && type == 0)
                    WhichType();
                Miss();

                type = 0;
                tmp_ship.Clear();
            }
            else if(status == -1)
            {
                tmp_ship.Add(coorX);
                tmp_ship.Add(coorY);
                if (tmp_ship.Count > 2 && type == 0)
                    WhichType();
                NextCoordinate(coorX, coorY);
            }
            else if(status==2 && coordinateX!=-1 && coordinateY!=-1)
            {
                if(!win)
                    NextCoordinate(tmp_ship[0], tmp_ship[1]);
            }
        }

        private void WhichType()
        {
            if (tmp_ship[0] == tmp_ship[2]) type = 1;
            else if (tmp_ship[1] == tmp_ship[3]) type = 2;
        }

        private void NextCoordinate(int x, int y)
        {
            List<int> ls = new List<int>();
            if (type == 0)
            {
                int x1 = x - 1, x2 = x + 1, y1 = y - 1, y2 = y + 1;
                if (x1 >= 0)
                {
                    if (array_of_shots[x1, y] == 0)
                    {
                        ls.Add(x1); ls.Add(y);
                    }
                }
                if (x2 < size_board)
                {
                    if (array_of_shots[x2, y] == 0)
                    {
                        ls.Add(x2); ls.Add(y);
                    }
                }
                if (y1 >= 0)
                {
                    if (array_of_shots[x, y1] == 0)
                    {
                        ls.Add(x); ls.Add(y1);
                    }
                }
                if (y2 < size_board)
                {
                    if (array_of_shots[x, y2] == 0)
                    {
                        ls.Add(x); ls.Add(y2);
                    }
                }

            }
            else if (type == 1)
            {
                int y1 = Min() - 1, y2 = Max() + 1;
                if (y1 >= 0)
                {
                    if (array_of_shots[x, y1] == 0)
                    {
                        ls.Add(x); ls.Add(y1);
                    }
                }
                if (y2 < size_board)
                {
                    if (array_of_shots[x, y2] == 0)
                    {
                        ls.Add(x); ls.Add(y2);
                    }
                }
            }
            else if (type == 2)
            {
                int x1 = Min() - 1, x2 = Max() + 1;
                if (x1 >= 0)
                {
                    if (array_of_shots[x1, y] == 0)
                    {
                        ls.Add(x1); ls.Add(y);
                    }
                }
                if (x2 < size_board)
                {
                    if (array_of_shots[x2, y] == 0)
                    {
                        ls.Add(x2); ls.Add(y);
                    }
                }
            }
            Random rnd = new Random();
            int liczba = rnd.Next(0, ls.Count);
            if (liczba % 2 == 0)
            {
                coordinateX = ls[liczba]; coordinateY = ls[liczba + 1];
            }
            else
            {
                coordinateX = ls[liczba - 1]; coordinateY = ls[liczba];
            }
        }
        private void Miss()
        {
            int x1 = -1, x2 = -1, y1 = -1, y2 = -1;
            if (type == 0)
            {
                x1 = tmp_ship[0] - 1;
                if (x1 < 0) x1 = tmp_ship[0];
                x2 = tmp_ship[0] + 1;
                if (x2 >= size_board) x2 = tmp_ship[0];
                y1 = tmp_ship[1] - 1;
                if (y1 < 0) y1 = tmp_ship[1];
                y2 = tmp_ship[1] + 1;
                if (y2 >= size_board) y2 = tmp_ship[1];
            }
            else if (type == 1)
            {
                x1 = tmp_ship[0] - 1;
                if (x1 < 0) x1 = tmp_ship[0];
                x2 = tmp_ship[0] + 1;
                if (x2 >= size_board) x2 = tmp_ship[0];
                y1 = Min() - 1;
                if (y1 < 0) y1 = Min();
                y2 = Max() + 1;
                if (y2 >= size_board) y2 = Max();
            }
            else if (type == 2)
            {
                x1 = Min() - 1;
                if (x1 < 0) x1 = Min();
                x2 = Max() + 1;
                if (x2 >= size_board) x2 = Max();
                y1 = tmp_ship[1] - 1;
                if (y1 < 0) y1 = tmp_ship[1];
                y2 = tmp_ship[1] + 1;
                if (y2 >= size_board) y2 = tmp_ship[1];
            }
            if (x1 >= 0 && x2 >= 0 && y1 >= 0 && y2 >= 0)
            {
                for (int i = x1; i <= x2; i++)
                {
                    for (int j = y1; j <= y2; j++)
                    {
                        if (array_of_shots[i, j] == 0)
                            array_of_shots[i, j] = 3;
                    }
                }
            }
        }
        private int Min()
        {
            int min;
            if (type == 2)
            {
                min = tmp_ship[0];
                for (int i = 0; i < tmp_ship.Count; i += 2)
                {
                    if (min > tmp_ship[i]) min = tmp_ship[i];
                }
                return min;
            }
            else if (type == 1)
            {
                min = tmp_ship[1];
                for (int i = 1; i < tmp_ship.Count; i += 2)
                {
                    if (min > tmp_ship[i]) min = tmp_ship[i];
                }
                return min;
            }
            return 0;
        }

        private int Max()
        {
            int max;
            if (type == 2)
            {
                max = tmp_ship[0];
                for (int i = 0; i < tmp_ship.Count; i += 2)
                {
                    if (max < tmp_ship[i]) max = tmp_ship[i];
                }
                return max;
            }
            else if (type == 1)
            {
                max = tmp_ship[1];
                for (int i = 1; i < tmp_ship.Count; i += 2)
                {
                    if (max < tmp_ship[i]) max = tmp_ship[i];
                }
                return max;
            }
            return 0;
        }
    }
}
