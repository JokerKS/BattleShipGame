using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipGame.Date
{
    class Actor
    {
        protected byte size_board;

        protected static byte[,] numberANDsize_ship_szablon;
        protected byte[,] numberANDsize_ship;

        protected List<Ship> list_of_ships;

        protected int[,] array_of_shots;
        protected int[,] array_of_private_field;

        protected Actor(byte size)
        {
            size_board = size;

            array_of_shots = new int[size, size];
            array_of_private_field = new int[size, size];

            list_of_ships = new List<Ship>();
        }

        protected Actor(byte size, byte[,] array)
        {
            size_board = size;

            array_of_shots = new int[size, size];
            array_of_private_field = new int[size, size];

            list_of_ships = new List<Ship>();
            numberANDsize_ship_szablon = array;

            CopyArray(numberANDsize_ship_szablon, out numberANDsize_ship);
        }

        public byte[,] GetNumberANDsize_ship()
        {
            return numberANDsize_ship;
        }
        protected static void CopyArray(byte[,] masforcopy, out byte[,] copymas)
        {
            copymas = new byte[masforcopy.GetLength(0), masforcopy.GetLength(1)];
            for (int i = 0; i < masforcopy.GetLength(0); i++)
            {
                for (int j = 0; j < masforcopy.GetLength(1); j++)
                {
                    copymas[i, j] = masforcopy[i, j];
                }
            }
        }

        //функція, яка повертає -1 - якщо ранений корабель
        //-2 - якщо знищено кораблік
        //2 - якщо не попав
        //0 - якщо даний постріл вже був
        public int Course(int coorX, int coorY)
        {
            int status = 0;
            int cell_content = array_of_private_field[coorX, coorY];
            if (cell_content == 1)
            {
                int index = FindShip(coorX, coorY);
                if (index != -1)
                {
                    if (list_of_ships[index].ChangeStatus(coorX, coorY))
                        status = - 2;
                    else status = - 1;
                }
                array_of_private_field[coorX, coorY] = -1;
            }
            else if (cell_content == 0)
            {
                array_of_private_field[coorX, coorY] = 2;
                status = 2;
            }

            return status;
        }

        public void DrawAllMiss(Graphics f, int size_cell)
        {
            for (int i = 0; i < size_board; i++)
            {
                for (int j = 0; j < size_board; j++)
                {
                    if (array_of_shots[i, j] == 2)
                        DrawMiss(f,i,j,size_cell);
                }
            }
        }

        private void DrawMiss(Graphics f, int x, int y, int size_cell)
        {
            int coorX = y * size_cell + size_cell * 2 + 4;
            int coorY = x * size_cell + size_cell * 2 + 4;
            f.FillEllipse(new SolidBrush(Color.Yellow), coorX, coorY, size_cell - 8, size_cell - 8);
        }

        public void DrawAllShip(Graphics f, int size_cell)
        {
            foreach (var item in list_of_ships)
            {
                item.DrawingShip(f, size_cell);
            }
        }

        public void DrawAllHiddenShip(Graphics f, int size_cell)
        {
            foreach (var item in list_of_ships)
            {
                item.DrawingHiddenShip(f, size_cell);
            }
        }

        protected int FindShip(int x, int y)
        {
            for (int i = 0; i < list_of_ships.Count; i++)
            {
                if (list_of_ships[i].Type)
                {
                    if (list_of_ships[i].VariableCoor1 <= y && list_of_ships[i].VariableCoor2 >= y 
                        && list_of_ships[i].ConstantCoor == x)
                    {
                        return i;
                    }
                }
                else
                {
                    if (list_of_ships[i].VariableCoor1 <= x && list_of_ships[i].VariableCoor2 >= x 
                        && list_of_ships[i].ConstantCoor == y)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void AddShip()
        {
            
        }

        //Функція для автоматичної генерації кораблів генерації 
        public void AutoGenerateShip()
        {
            /*byte[,] mas;
            if (czy_user) mas = tmp_il_u;
            else mas = tmp_il;*/
            Random rnd = new Random();
            int coorX, coorY, type;

            //цикл, який ходить по всіх кораблях
            for (int i = 0; i < numberANDsize_ship.GetLength(0);)
            {
                int amount = numberANDsize_ship[i, 1];
                if (amount == 0)
                {
                    i++;
                    continue;
                }
                //цикл, який ходить по всіх кількостях
                for (int k = 0, 
                    ship_size = numberANDsize_ship[i, 0]; k < amount; k++)
                {
                    //якщо тип дорівнює 0, то це горизонтальний кораблік, інакше - вертикальний
                    type = rnd.Next(0, 2);
                    //генеруємо координати x i y на ігровій досці
                    coorX = rnd.Next(0, size_board);
                    coorY = rnd.Next(0, size_board);

                    //
                    if (CreateShip(coorX, coorY, (type==0? true :false), ship_size) == true)
                        if (--(numberANDsize_ship[i, 1]) == 0) i++;
                }
            }
        }
        //Функція, яка формує з 2 координат, типу і розміру кораблік
        private bool CreateShip(int coorX, int coorY, bool type, int size)
        {
            int x1, x2, y1, y2;
            if (type)
            {
                y1 = coorY - size + 1;
                y2 = coorY;
                for (int i = 0; i < size; i++)
                {
                    if (y1 >= 0 && y2 <= size_board - 1)
                    {
                        if (Check(y1, y2, coorX, type))
                            return true;
                    }
                    y1++;
                    y2++;
                }
                if (size == 1)
                {
                    if (Check(coorY, coorY, coorX, type))
                        return true;
                }
            }
            else
            {
                x1 = coorX - size + 1;
                x2 = coorX;
                for (int i = 0; i < size; i++)
                {
                    if (x1 >= 0 && x2 <= size_board - 1)
                    {
                        if (Check(x1, x2, coorY, type))
                            return true;
                    }
                    x1++;
                    x2++;
                }
                if (size == 1)
                {
                    if (Check(coorX, coorX, coorY, type))
                        return true;
                }
            }
            return false;
        }

        //формує область для перевірки на вставлення
        public bool Check(int vc1, int vc2, int cons, bool typ)
        {
            bool canAdd = false;
            int x1, x2, y1, y2;

            if (typ)
            {
                if (vc1 == 0)
                {
                    y1 = vc1;
                    y2 = vc2 + 1;
                }
                else if (vc2 == size_board - 1)
                {
                    y1 = vc1 - 1;
                    y2 = vc2;
                }
                else
                {
                    y1 = vc1 - 1;
                    y2 = vc2 + 1;
                }
                if (cons == 0)
                {
                    x1 = cons;
                    x2 = cons + 1;
                }
                else if (cons == size_board - 1)
                {
                    x1 = cons - 1;
                    x2 = cons;
                }
                else
                {
                    x1 = cons - 1;
                    x2 = cons + 1;
                }

                if(CanInsert(x1, x2, y1, y2))
                {
                    for (int i = vc1; i <= vc2; i++)
                        array_of_private_field[cons, i] = 1;
                    list_of_ships.Add(new Ship(vc1,vc2,cons));
                    canAdd = true;
                }
            }
            else
            {
                if (vc1 == 0)
                {
                    x1 = vc1;
                    x2 = vc2 + 1;
                }
                else if (vc2 == size_board - 1)
                {
                    x1 = vc1 - 1;
                    x2 = vc2;
                }
                else
                {
                    x1 = vc1 - 1;
                    x2 = vc2 + 1;
                }
                if (cons == 0)
                {
                    y1 = cons;
                    y2 = cons + 1;
                }
                else if (cons == size_board - 1)
                {
                    y1 = cons - 1;
                    y2 = cons;
                }
                else
                {
                    y1 = cons - 1;
                    y2 = cons + 1;
                }

                if (CanInsert(x1, x2, y1, y2))
                {
                    for (int i = vc1; i <= vc2; i++)
                        array_of_private_field[i, cons] = 1;
                    list_of_ships.Add(new Ship(vc1, vc2, cons, false));
                    canAdd = true;
                }
            }
            return canAdd;
        }

        //перевірка чи можна добавити на дане місце кораблік
        //x1,x2,y1,y2 - координати місця, яке займе корабель
        private bool CanInsert(int x1, int x2, int y1, int y2)
        {
            bool canInsert = true;
            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    if (array_of_private_field[i, j] != 0)
                    {
                        canInsert = false;
                    }
                }
            }
            return canInsert;
        }

        public bool AllKilled()
        {
            if (Ship.AllKilled(list_of_ships))
                return true;
            else return false;
        }
    }
}
