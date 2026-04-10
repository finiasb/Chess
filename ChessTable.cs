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
        Board joculMeu = new Board(); 
        Piece piesaSelectata = null;  
        List<Point> CercuriDeDesenat = new List<Point>();

        public ChessTable()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 800);

            string startPos = "r1b1k2r/ppp1q3/5npp/4N3/3P4/2P3P1/PP2PP1P/RNBQKB1R w KQkq - 0 1";
            IncarcaJoc(startPos);
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

            foreach (Point p in CercuriDeDesenat)
            {
                int drawX = p.X * size;
                int drawY = (7 - p.Y) * size;
                int alpha = 100;
                using (Pen pen = new Pen(Color.FromArgb(alpha, Color.LightCyan), 3))
                {
                    using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, Color.LightCyan)))
                    {
                        e.Graphics.DrawEllipse(pen, drawX + 10, drawY + 10, 80, 80);
                        e.Graphics.FillEllipse(b, drawX + 10, drawY + 10, 80, 80);
                    }
                }
            }
        }
        void PieceClick(object sender, MouseEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic == null) return;

            int x = pic.Location.X / size;
            int y = 7 - (pic.Location.Y / size);
            Point locatieClick = new Point(x, y);

            if (piesaSelectata != null && CercuriDeDesenat.Contains(locatieClick))
            {
                ExecutaMutare(locatieClick);
                return;
            }

            Piece p = joculMeu.Grid[x, y];
            if (p != null && p.Color == joculMeu.CurrentTurn)
            {
                piesaSelectata = p;
                CercuriDeDesenat = piesaSelectata.GetPossibleMoves(joculMeu.Grid);
            }
            else
            {
                CercuriDeDesenat.Clear();
                piesaSelectata = null;
            }

            this.Invalidate();
        }
        void ExecutaMutare(Point tinta)
        {
            if (piesaSelectata == null) return;
            Point pozitieVeche = piesaSelectata.Position;

            PictureBox piesaDeSters = null;
            foreach (Control c in this.Controls)
            {
                if (c is PictureBox pb && pb.Location.X == tinta.X * size && pb.Location.Y == (7 - tinta.Y) * size)
                {
                    if (pb.Tag != piesaSelectata)
                    {
                        piesaDeSters = pb;
                        break;
                    }
                }
            }

            if (piesaDeSters != null)
            {
                this.Controls.Remove(piesaDeSters);
                piesaDeSters.Dispose(); 
            }

            joculMeu.MovePiece(pozitieVeche, tinta);

            foreach (Control c in this.Controls)
            {
                if (c is PictureBox pb && pb.Tag == piesaSelectata)
                {
                    pb.Location = new Point(tinta.X * size, (7 - tinta.Y) * size);
                    break;
                }
            }

            CercuriDeDesenat.Clear();
            piesaSelectata = null;
            this.Invalidate();
        }
        private void ChessTable_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / size;
            int y = 7 - (e.Y / size);
            Point tinta = new Point(x, y);

            if (piesaSelectata != null && CercuriDeDesenat.Contains(tinta))
            {
                ExecutaMutare(tinta);
            }
        }
        void IncarcaJoc(string fen)
        {
            joculMeu.IncarcaFEN(fen);

            var deSters = this.Controls.OfType<PictureBox>().ToList();
            foreach (var p in deSters) this.Controls.Remove(p);

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piesa = joculMeu.Grid[x, y];
                    if (piesa != null)
                    {
                        CreazaPictureBoxPiesa(piesa);
                    }
                }
            }
        }

        void CreazaPictureBoxPiesa(Piece piesa)
        {
            PictureBox pic = new PictureBox();
            pic.Size = new Size(size, size);
            pic.Location = new Point(piesa.Position.X * size, (7 - piesa.Position.Y) * size);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            pic.BackColor = Color.Transparent;

            pic.Tag = piesa;

            string numeResursa = piesa.Type.ToString() + (piesa.Color == PieceColor.White ? "W" : "B");
            pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(numeResursa);

            pic.MouseDown += PieceClick;
            this.Controls.Add(pic);
            pic.BringToFront();
        }

        void ActualizeazaInterfataGrafica()
        {
            List<PictureBox> deSters = new List<PictureBox>();

            foreach (Control c in this.Controls)
            {
                if (c is PictureBox pic)
                {
                    Piece p = (Piece)pic.Tag;

                    bool gasita = false;
                    for (int x = 0; x < 8; x++)
                        for (int y = 0; y < 8; y++)
                            if (joculMeu.Grid[x, y] == p) gasita = true;

                    if (!gasita)
                    {
                        deSters.Add(pic);
                    }
                    else
                    {
                        pic.Location = new Point(p.Position.X * size, (7 - p.Position.Y) * size);
                    }
                }
            }

            foreach (var p in deSters) this.Controls.Remove(p);
        }
    }
}