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
        string piesaActuala;
        List<Point> CercuriDeDesenat = new List<Point>();
        Dictionary<string, Point> map = new Dictionary<string, Point>();
        public ChessTable()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 800);
            string startPos = "8/5k2/3p4/1p1Pp2p/pP2Pp1P/P4P1K/8/8 b - - 99 50";
            IncarcaFEN(startPos);
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
        bool Player = true;
        void PieceClick(object sender, MouseEventArgs e)
        {
            CercuriDeDesenat.Clear();
            PictureBox pic = sender as PictureBox;

            if (pic == null)
                return;

            piesaActuala = pic.Name;
            Point pos = map[piesaActuala];
            if (pic.Name.Contains("KnightW") && Player)
            {
                int[] dx = { 1, 1, -1, -1, 2, 2, -2, -2 };
                int[] dy = { 2, -2, 2, -2, 1, -1, 1, -1 };

                for (int k = 0; k < 8; k++)
                {
                    int nx = pos.X + dx[k];
                    int ny = pos.Y + dy[k];

                    if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point newPos = new Point(nx, ny);
                        if (NuExistaPiesa(newPos))
                            CercuriDeDesenat.Add(newPos);
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("KnightB") && !Player)
            {
                int[] dx = { 1, 1, -1, -1, 2, 2, -2, -2 };
                int[] dy = { 2, -2, 2, -2, 1, -1, 1, -1 };

                for (int k = 0; k < 8; k++)
                {
                    int nx = pos.X + dx[k];
                    int ny = pos.Y + dy[k];

                    if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point newPos = new Point(nx, ny);
                        if (NuExistaPiesa(newPos))
                            CercuriDeDesenat.Add(newPos);
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("KnightW") && Player)
            {
                if (pos.Y + 1 < 8 && NuExistaPiesa(new Point(pos.X, pos.Y + 1)))
                {
                    CercuriDeDesenat.Add(new Point(pos.X, pos.Y + 1));
                    if (pos.Y == 1 && NuExistaPiesa(new Point(pos.X, pos.Y + 2)))
                        CercuriDeDesenat.Add(new Point(pos.X, pos.Y + 2));
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("PawnB") && !Player)
            {
                if (pos.Y - 1 >= 0 && NuExistaPiesa(new Point(pos.X, pos.Y - 1)))
                {
                    CercuriDeDesenat.Add(new Point(pos.X, pos.Y - 1));
                    if (pos.Y == 6 && NuExistaPiesa(new Point(pos.X, pos.Y - 2)))
                        CercuriDeDesenat.Add(new Point(pos.X, pos.Y - 2));
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("RookW") && Player)
            {
                int[] dx = { 0, 0, 1, -1 };
                int[] dy = { 1, -1, 0, 0 };
                for (int d = 0; d < 4; d++)
                {
                    int nx = pos.X + dx[d];
                    int ny = pos.Y + dy[d];

                    while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point nextPos = new Point(nx, ny);

                        if (NuExistaPiesa(nextPos))
                        {
                            CercuriDeDesenat.Add(nextPos);
                        }
                        else
                        {
                            //daca e a adversarului sa fac functie
                            break;
                        }

                        nx += dx[d];
                        ny += dy[d];
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("RookB") && !Player)
            {
                int[] dx = { 0, 0, 1, -1 };
                int[] dy = { 1, -1, 0, 0 };
                for (int d = 0; d < 4; d++)
                {
                    int nx = pos.X + dx[d];
                    int ny = pos.Y + dy[d];

                    while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point nextPos = new Point(nx, ny);

                        if (NuExistaPiesa(nextPos))
                        {
                            CercuriDeDesenat.Add(nextPos);
                        }
                        else
                        {
                            //daca e a adversarului sa fac functie
                            break;
                        }

                        nx += dx[d];
                        ny += dy[d];
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("BishopW") && Player)
            {
                int[] dx = { 1, 1, -1, -1 };
                int[] dy = { 1, -1, 1, -1 };

                for (int d = 0; d < 4; d++)
                {
                    int nx = pos.X + dx[d];
                    int ny = pos.Y + dy[d];

                    while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point nextPos = new Point(nx, ny);
                        if (NuExistaPiesa(nextPos))
                        {
                            CercuriDeDesenat.Add(nextPos);
                        }
                        else
                        {
                            //daca e a adversarului sa fac functie
                            break;
                        }
                        nx += dx[d];
                        ny += dy[d];
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("BishopB") && !Player)
            {
                int[] dx = { 1, 1, -1, -1 };
                int[] dy = { 1, -1, 1, -1 };

                for (int d = 0; d < 4; d++)
                {
                    int nx = pos.X + dx[d];
                    int ny = pos.Y + dy[d];

                    while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point nextPos = new Point(nx, ny);
                        if (NuExistaPiesa(nextPos))
                        {
                            CercuriDeDesenat.Add(nextPos);
                        }
                        else
                        {
                            //daca e a adversarului sa fac functie
                            break;
                        }
                        nx += dx[d];
                        ny += dy[d];
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("QueenW") && Player)
            {
                int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
                int[] dy = { 1, -1, 0, 0, 1, -1, 1, -1 };

                for (int d = 0; d < 8; d++)
                {
                    int nx = pos.X + dx[d];
                    int ny = pos.Y + dy[d];

                    while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point nextPos = new Point(nx, ny);
                        if (NuExistaPiesa(nextPos))
                        {
                            CercuriDeDesenat.Add(nextPos);
                        }
                        else
                        {
                            // functie
                            break;
                        }
                        nx += dx[d];
                        ny += dy[d];
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("QueenB") && !Player)
            {
                int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
                int[] dy = { 1, -1, 0, 0, 1, -1, 1, -1 };

                for (int d = 0; d < 8; d++)
                {
                    int nx = pos.X + dx[d];
                    int ny = pos.Y + dy[d];

                    while (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point nextPos = new Point(nx, ny);
                        if (NuExistaPiesa(nextPos))
                        {
                            CercuriDeDesenat.Add(nextPos);
                        }
                        else
                        {
                            // functie
                            break;
                        }
                        nx += dx[d];
                        ny += dy[d];
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("KingW") && Player)
            {
                int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
                int[] dy = { 1, -1, 0, 0, 1, -1, 1, -1 };

                for (int d = 0; d < 8; d++)
                {
                    int nx = pos.X + dx[d];
                    int ny = pos.Y + dy[d];

                    if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point nextPos = new Point(nx, ny);
                        if (NuExistaPiesa(nextPos))
                        {
                            CercuriDeDesenat.Add(nextPos);
                        }
                    }
                }
                Player = !Player;
            }
            else if (pic.Name.Contains("KingB") && !Player)
            {
                int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
                int[] dy = { 1, -1, 0, 0, 1, -1, 1, -1 };

                for (int d = 0; d < 8; d++)
                {
                    int nx = pos.X + dx[d];
                    int ny = pos.Y + dy[d];

                    if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                    {
                        Point nextPos = new Point(nx, ny);
                        if (NuExistaPiesa(nextPos))
                        {
                            CercuriDeDesenat.Add(nextPos);
                        }
                    }
                }
                Player = !Player;
            }
            this.Invalidate();
        }

        List<Point> AttackingCellsByWhite = new List<Point>();
        List<Point> AttackingCellsByBlack = new List<Point>();

        void GenerateAllAttackingCells()
        {
            Point pos = map["KnightW"];
            int[] dx = { 1, 1, -1, -1, 2, 2, -2, -2 };
            int[] dy = { 2, -2, 2, -2, 1, -1, 1, -1 };

            for (int k = 0; k < 8; k++)
            {
                int nx = pos.X + dx[k];
                int ny = pos.Y + dy[k];

                if (nx >= 0 && nx < 8 && ny >= 0 && ny < 8)
                {
                    Point newPos = new Point(nx, ny);
                    if (NuExistaPiesa(newPos))
                        AttackingCellsByWhite.Add(newPos);
                }
            }
            
        }
        private void ChessTable_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / 100;
            int y = e.Y / 100;
            Point celula = new Point(x, 7 - y);
            if (CercuriDeDesenat.Contains(celula))
            {
                map[piesaActuala] = celula;
                FacutMutare();
            }
        }

        bool NuExistaPiesa(Point p)
        {
            foreach (var pair in map)
            {
                if (pair.Value == p)
                    return false;
            }
            return true;
        }

        void FacutMutare()
        {
            if (string.IsNullOrEmpty(piesaActuala)) return;

            PictureBox pic = this.Controls.Find(piesaActuala, true).FirstOrDefault() as PictureBox;
            if (pic != null)
            {
                Point p = map[piesaActuala];
                pic.Location = new Point(p.X * size, (7 - p.Y) * size);
            }
            CercuriDeDesenat.Clear();
            this.Invalidate();
        }
        void IncarcaFEN(string fen)
        {
            map.Clear();
            CercuriDeDesenat.Clear();

            foreach (PictureBox pic in this.Controls)
                this.Controls.Remove(pic);

            string[] parti = fen.Split(' ');
            string pozitii = parti[0];

            int rand = 7;
            int coloana = 0;

            Dictionary<char, int> counter = new Dictionary<char, int>();

            foreach (char c in pozitii)
            {
                if (c == '/')
                {
                    rand--;
                    coloana = 0;
                }
                else if (char.IsDigit(c))
                {
                    coloana += (int)char.GetNumericValue(c);
                }
                else
                {
                    string numeBaza = GetNumePiesa(c);
                    if (!counter.ContainsKey(c)) counter[c] = 1;

                    string IDUnic = numeBaza + counter[c];
                    Point pos = new Point(coloana, rand);

                    map.Add(IDUnic, pos);

                    CreazaPictureBoxPiesa(IDUnic, pos, numeBaza);

                    counter[c]++;
                    coloana++;
                }
            }
            this.Invalidate();
        }

        string GetNumePiesa(char c)
        {
            bool isWhite = char.IsUpper(c);
            char tip = char.ToLower(c);
            string culoare = isWhite ? "W" : "B";

            switch (tip)
            {
                case 'p': return "Pawn" + culoare;
                case 'r': return "Rook" + culoare;
                case 'n': return "Knight" + culoare;
                case 'b': return "Bishop" + culoare;
                case 'q': return "Queen" + culoare;
                case 'k': return "King" + culoare;
                default: return "Unknown";
            }
        }

        void CreazaPictureBoxPiesa(string id, Point pos, string numeBaza)
        {
            PictureBox pic = new PictureBox();
            pic.Name = id;
            pic.Size = new Size(size, size);
            pic.Location = new Point(pos.X * size, (7 - pos.Y) * size);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            pic.BackColor = Color.Transparent;


            switch (numeBaza)
            {
                case "PawnW": pic.Image = Properties.Resources.PawnW; break;
                case "PawnB": pic.Image = Properties.Resources.PawnB; break;

                case "RookW": pic.Image = Properties.Resources.RookW; break;
                case "RookB": pic.Image = Properties.Resources.RookB; break;

                case "KnightW": pic.Image = Properties.Resources.KnightW; break;
                case "KnightB": pic.Image = Properties.Resources.KnightB; break;

                case "BishopW": pic.Image = Properties.Resources.BishopW; break;
                case "BishopB": pic.Image = Properties.Resources.BishopB; break;

                case "QueenW": pic.Image = Properties.Resources.QueenW; break;
                case "QueenB": pic.Image = Properties.Resources.QueenB; break;

                case "KingW": pic.Image = Properties.Resources.KingW; break;
                case "KingB": pic.Image = Properties.Resources.KingB; break;

                default: break;
            }


            pic.MouseDown += PieceClick;
            this.Controls.Add(pic);
            pic.BringToFront();
        }

    }
}