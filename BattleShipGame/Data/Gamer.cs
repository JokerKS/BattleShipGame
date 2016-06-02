using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipGame.Date
{
    class Gamer: Actor
    {
        public Gamer(byte size):base(size){}
        public Gamer(byte size, byte[,] array) : base(size, array){}

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public void Result_OF_Shot(int coorX, int coorY, int status)
        {
            array_of_shots[coorX, coorY] = status;
        }

        public void ClearShip()
        {
            list_of_ships.Clear();
            CopyArray(numberANDsize_ship_szablon, out numberANDsize_ship);
            for (int i = 0; i < array_of_private_field.GetLength(0); i++)
            {
                for (int j = 0; j < array_of_private_field.GetLength(1); j++)
                {
                    array_of_private_field[i, j] = 0;
                }
            }
        }

        public int SetActiveShip(Ship[] sh, byte coorX, byte coorY, byte pole, ref bool type)
        {
            return Ship.SetActive(list_of_ships, sh, coorX, coorY, pole, ref type, array_of_private_field, numberANDsize_ship);
        }
    }
}
