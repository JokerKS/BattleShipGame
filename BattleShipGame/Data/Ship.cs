using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShipGame.Date
{
    sealed class Ship
    {
        private enum Status : byte { Undamaged = 0, Wounded = 1, Killed = 2, Active = 3 };
        private int vc1;
        private int vc2;
        private int cons;
        private bool type;
        private Status status_sh;
        private int[] current_status;
        private static bool is_active = false;

        public int VariableCoor1
        {
            get { return vc1; }
        }
        public int VariableCoor2
        {
            get { return vc2; }
        }
        public int ConstantCoor
        {
            get { return cons; }
        }
        public bool Type
        {
            get { return type; }
        }

        public Ship(int vc1, int vc2, int cons, bool type = true)
        {
            this.vc1 = vc1; this.vc2 = vc2; this.cons = cons;
            this.type = type;
            status_sh = (byte)Status.Undamaged;
            current_status = new int[vc2 - vc1 + 1];
            for (int i = 0; i < current_status.Length; i++)
                current_status[i] = 1;
        }

        public bool ChangeStatus(int x, int y)
        {
            int tmp_i = -1;
            if (type)
            {
                for (int i = vc1; i <= vc2; i++, tmp_i++)
                {
                    if (y == i && cons == x)
                    {
                        current_status[++tmp_i] = -1;
                        break;
                    }
                }
            }
            else
            {
                for (int i = vc1; i <= vc2; i++, tmp_i++)
                {
                    if (x == i && cons == y)
                    {
                        current_status[++tmp_i] = -1;
                        break;
                    }
                }
            }
            status_sh = Status.Wounded;

            if (Kill()) return true;
            else return false;
        }

        private bool Kill()
        {
            bool kill_or_not = true;
            for (int i = 0; i < current_status.Length; i++)
            {
                if (current_status[i] != -1)
                {
                    kill_or_not = false;
                    break;
                }
            }
            if (kill_or_not)
                status_sh = Status.Killed;
            return kill_or_not;
        }

        public static bool AllKilled(List<Ship> sh)
        {
            for (int i = 0; i < sh.Count; i++)
            {
                if (sh[i].status_sh != Status.Killed) return false;
            }
            return true;
        }

        public void DrawingHiddenShip(Graphics gr, int cell)
        {

            int coorX = cell * 2, coorY = cell * 2, height = cell, width = cell;

            if (type)
            {
                coorX += vc1 * cell;
                coorY += cons * cell;
                width *= (vc2 - vc1 + 1);
            }
            else
            {
                coorX += cons * cell;
                coorY += vc1 * cell;
                height *= (vc2 - vc1 + 1);
            }
            Pen pen = new Pen(Color.Green, 2.5F);
            if (status_sh == Status.Killed)
            {
                pen.Color = Color.Red;
                gr.DrawRectangle(pen, coorX, coorY, width, height);
            }
            DrawingStatus(gr, coorX, coorY, cell);
        }

        public void DrawingShip(Graphics gr, int cell)
        {
            Pen pen = new Pen(Color.Green, 2.5F);
            int coorX = cell * 2, coorY = cell * 2, height = cell, width = cell;

            if (type)
            {
                coorX += vc1 * cell;
                coorY += cons * cell;
                width *= (vc2 - vc1 + 1);
            }
            else
            {
                coorX += cons * cell;
                coorY += vc1 * cell;
                height *= (vc2 - vc1 + 1);
            }
            if (status_sh == Status.Killed)
            {
                pen.Color = Color.Red;
                gr.DrawRectangle(pen, coorX, coorY, width, height);
            }
            else if(status_sh == Status.Active)
            {
                gr.DrawRectangle(pen, coorX, coorY, width, height);
            }
            else gr.FillRectangle(new SolidBrush(Color.Green), coorX, coorY, width, height);
            DrawingStatus(gr, coorX, coorY, cell);
        }
        private void DrawingStatus(Graphics gr, int coorX, int coorY, int cell)
        {
            for (int i = 0; i < current_status.Length; i++)
            {
                if (current_status[i] == -1)
                {
                    if (type)
                    {
                        gr.FillRectangle(new SolidBrush(Color.Red), coorX + i * cell + 3, coorY + 3, cell - 6, cell - 6);
                    }
                    else
                    {
                        gr.FillRectangle(new SolidBrush(Color.Red), coorX + 3, coorY + i * cell + 3, cell - 6, cell - 6);
                    }
                }
            }
        }
        public byte Size
        {
            get { return (byte)(vc2 - vc1 + 1); }
        }

        public static void DropActive(Ship[] sh)
        {
            int index = FindActiveShip(sh);
            if (index != -1)
            {
                sh[index].status_sh = Status.Undamaged;
                is_active = !is_active;
            }
        }
        public static int FindActiveShip(Ship[] sh)
        {
            if (!is_active) return -1;
            for (int i = 0; i < sh.Length; i++)
                if (sh[i].status_sh == Status.Active)
                    return i;
            return -1;
        }
        public static int SetActive(List<Ship> ls, Ship[] sh, byte coorX, byte coorY, byte pole, ref bool type, int[,] gamer, byte[,] ilosc)
        {
            if (pole == 1)
            {
                if (!is_active)
                {
                    int index = -1;
                    for (int i = 0; i < ls.Count; i++)
                    {
                        if (ls[i].type)
                        {
                            if (ls[i].vc1 <= coorY && ls[i].vc2 >= coorY && ls[i].cons == coorX)
                            {
                                index = i;
                                break;
                            }
                        }
                        else
                        {
                            if (ls[i].vc1 <= coorX && ls[i].vc2 >= coorX && ls[i].cons == coorY)
                            {
                                index = i;
                                break;
                            }
                        }
                    }
                    if (index != -1)
                    {
                        int size = ls[index].Size;
                        for (int i = 0; i < sh.Length; i++)
                        {
                            if (sh[i].Size == size)
                            {
                                type = ls[index].type;
                                for (int j = 0; j < ilosc.GetLength(0); j++)
                                {
                                    if (ls[index].Size == ilosc[j, 0])
                                    {
                                        if (ilosc[j, 1] >= 0) ilosc[j, 1]++;
                                    }
                                }
                                if (ls[index].type)
                                {
                                    int z = ls[index].vc1;
                                    for (int j = 0; j < ls[index].Size; j++, z++)
                                    {
                                        gamer[ls[index].cons, z] = 0;
                                    }
                                }
                                else
                                {
                                    int z = ls[index].vc1;
                                    for (int j = 0; j < ls[index].Size; j++, z++)
                                    {
                                        gamer[z, ls[index].cons] = 0;
                                    }
                                }
                                ls.Remove(ls[index]);
                                sh[i].status_sh = Status.Active;
                                is_active = true;
                                return 0;
                            }
                        }
                    }
                }
            }
            else if (pole == 2)
            {
                if (!is_active)
                {
                    for (int i = 0; i < sh.Length; i++)
                        if (coorY >= sh[i].vc1 && coorY <= sh[i].vc2 && sh[i].cons == coorX)
                        {
                            for (int k = 0; k < ilosc.GetLength(0); k++)
                            {
                                if (sh[i].Size == ilosc[k, 0] && ilosc[k, 1] > 0)
                                {
                                    sh[i].status_sh = Status.Active;
                                    is_active = !is_active;
                                    break;
                                }
                            }
                        }
                }
                else
                {
                    int index = FindActiveShip(sh);
                    if (coorY >= sh[index].vc1 && coorY <= sh[index].vc2 && sh[index].cons == coorX)
                    {
                        sh[index].status_sh = (int)Status.Undamaged;
                        is_active = !is_active;
                    }
                    else
                    {
                        for (int i = 0; i < sh.Length; i++)
                            if (coorY >= sh[i].vc1 && coorY <= sh[i].vc2 && sh[i].cons == coorX)
                            {
                                sh[index].status_sh = Status.Undamaged;
                                sh[i].status_sh = Status.Active;
                                break;
                            }
                    }
                }
            }
            return -1;
        }
    }
}