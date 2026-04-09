using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public partial class ChessTable : Form
    {
        int size = 100;
        Dictionary<string, int> map = new Dictionary<string, int>();
        public ChessTable()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 800);
            initializarePiese();

        }
        private void ChessTable_Paint(object sender, PaintEventArgs e)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    bool isWhite = (row + col) % 2 == 0;
                    Color white = Color.FromArgb(238, 214, 176);
                    Color black = Color.FromArgb(184, 135, 99);
                    Brush brush;
                    if (isWhite)
                        brush = new SolidBrush(white);
                    else
                        brush = new SolidBrush(black);

                    e.Graphics.FillRectangle(brush, col * size, row * size, size, size);
                }
            }

            if (CercuriDeDesenat.Count == 0)
                return;
            foreach (int i in CercuriDeDesenat)
            {
                if (i < 0)
                    continue;


                int x = (i - 1) % 8;
                int y = (64 - i) / 8;


                x *= size;
                y *= size;

                if (x > 700 || y > 700)
                    continue;

                Pen pen = new Pen(Color.Blue, 5);
                e.Graphics.DrawEllipse(pen, x + 10, y + 10, 80, 80);
            }
        }
        List<int> CercuriDeDesenat = new List<int>();
        string piesaActuala;
        void PieceClick(object sender, MouseEventArgs e)
        {
            CercuriDeDesenat.Clear();
            PictureBox pic = sender as PictureBox;
            piesaActuala = pic.Name;
            if (pic.Name.Contains("Knight"))
            {
                int numarCelula = map[pic.Name];
                int x = (numarCelula - 1) % 8;
                int y = (numarCelula - 1) / 8;

                int[] dx = { 1, 1, -1, -1, 2, 2, -2, -2 };
                int[] dy = { 2, -2, 2, -2, 1, -1, 1, -1 };

                for (int k = 0; k < 8; k++)
                {
                    int nx = x + dx[k];
                    int ny = y + dy[k];

                    if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        int poz = ny * 8 + nx + 1;
                        if(NuExistaPiesa(poz))
                            CercuriDeDesenat.Add(poz);
                    }
                }
                this.Invalidate();
            }
            if (pic.Name.Contains("PawnW"))
            {
                int numarCelula = map[pic.Name]; 
                if (NuExistaPiesa(numarCelula + 8))
                    CercuriDeDesenat.Add(numarCelula + 8);

                if (numarCelula >= 9 && numarCelula <= 16) 
                {
                    if (NuExistaPiesa(numarCelula + 8) && NuExistaPiesa(numarCelula + 16))
                        CercuriDeDesenat.Add(numarCelula + 16);
                }
                this.Invalidate();
            }
            if (pic.Name.Contains("PawnB"))
            {
                int numarCelula = map[pic.Name];
                if (NuExistaPiesa(numarCelula - 8))
                    CercuriDeDesenat.Add(numarCelula - 8);

                if (numarCelula >= 49 && numarCelula <= 56)
                {
                    if (NuExistaPiesa(numarCelula - 8) && NuExistaPiesa(numarCelula - 16))
                        CercuriDeDesenat.Add(numarCelula - 16);
                }
                this.Invalidate();
            }
        }
        private void ChessTable_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            x /= 100;
            y /= 100;
            int numarCelula = (7 - y) * 8 + x + 1;
            if(CercuriDeDesenat.Contains(numarCelula))
                map[piesaActuala] = numarCelula;

            FacutMutare();
        }

        bool NuExistaPiesa(int poz)
        {
            foreach(var pair in map)
            {
                if (pair.Value == poz)
                    return false;
            }
            return true;
        }
        Point TransformareNrCelulaInPozitieDeDesenat(int poz)
        {

            int x = (poz - 1) % 8;
            int y = (64 - poz) / 8;
            return new Point(x, y);
        }

        void FacutMutare()
        {
            PictureBox pic = this.Controls.Find(piesaActuala, true).FirstOrDefault() as PictureBox;
            Point p = TransformareNrCelulaInPozitieDeDesenat(map[piesaActuala]);
            pic.Location = new Point(p.X * size, p.Y * size);
            CercuriDeDesenat.Clear();
            this.Invalidate();
        }
        void initializarePiese()
        {
            // Knight
            map.Add("KnightB1", 58);
            map.Add("KnightB2", 63);
            map.Add("KnightW1", 2);
            map.Add("KnightW2", 7);

            //Rook
            map.Add("RookB1", 57);
            map.Add("RookB2", 64);
            map.Add("RookW1", 1);
            map.Add("RookW2", 8);
            
            //Bishop
            map.Add("BishopB1", 59);
            map.Add("BishopB2", 62);
            map.Add("BishopW1", 3);
            map.Add("BishopW2", 6);

            //Queen
            map.Add("QueenB", 60);
            map.Add("QueenW", 4);

            //King
            map.Add("KingB", 61);
            map.Add("KingW", 5);

            //PawnB
            map.Add("PawnB1", 49);
            map.Add("PawnB2", 50);
            map.Add("PawnB3", 51);
            map.Add("PawnB4", 52);
            map.Add("PawnB5", 53);
            map.Add("PawnB6", 54);
            map.Add("PawnB7", 55);
            map.Add("PawnB8", 56);

            //PawnW
            map.Add("PawnW1", 9);
            map.Add("PawnW2", 10);
            map.Add("PawnW3", 11);
            map.Add("PawnW4", 12);
            map.Add("PawnW5", 13);
            map.Add("PawnW6", 14);
            map.Add("PawnW7", 15);
            map.Add("PawnW8", 16);
        }
    }
}