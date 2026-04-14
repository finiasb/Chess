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
        Board myGame = new Board(); 
        Piece selectedPiece = null;  
        List<Point> AllowedMovingToDraw = new List<Point>();
        public ChessTable()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 800);

            string startPosition = "r1b1k2r/ppp1q3/5npp/4N3/3P4/2P3P1/PP2PP1P/RNBQKB1R w KQkq - 0 1";
            LoadGame(startPosition);
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

            foreach (PieceColor color in Enum.GetValues(typeof(PieceColor)))
            {
                King king = myGame.GetKing(color, myGame.Grid);
                if (king != null && king.VerifyIfInCheck(myGame.Grid))
                {
                    int drawX = king.Position.X * size;
                    int drawY = (7 - king.Position.Y) * size;

                    e.Graphics.FillEllipse(Brushes.Red, drawX + 5, drawY + 5, size - 10, size - 10);
                }
            }
                
            if (AllowedMovingToDraw.Count == 0) return;

            foreach (Point p in AllowedMovingToDraw)
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
            Point ClickLocation = new Point(x, y);

            if (selectedPiece != null && AllowedMovingToDraw.Contains(ClickLocation))
            {
                ExecuteMove(ClickLocation);
                return;
            }

            Piece p = myGame.Grid[x, y];
            if (p != null && p.Color == myGame.CurrentTurn)
            {
                selectedPiece = p;
                AllowedMovingToDraw = GetLegalMoves(selectedPiece);
            }
            else
            {
                AllowedMovingToDraw.Clear();
                selectedPiece = null;
            }
            this.Invalidate();
        }
        List<Point> GetLegalMoves(Piece piesa)
        {
            List<Point> pseudoMoves = piesa.GetPossibleMoves(myGame.Grid);
            List<Point> legalMoves = new List<Point>();

            Point originalPos = piesa.Position;
            Piece[,] board = myGame.Grid;

            foreach (Point targetPos in pseudoMoves)
            {
                Piece targetPieceBackup = board[targetPos.X, targetPos.Y];
                board[targetPos.X, targetPos.Y] = piesa;
                board[originalPos.X, originalPos.Y] = null;
                piesa.Position = targetPos; 

                King myKing = myGame.GetKing(piesa.Color, board);
                if (myKing != null && !myKing.VerifyIfInCheck(board))
                {
                    legalMoves.Add(targetPos);
                }

                board[originalPos.X, originalPos.Y] = piesa;
                board[targetPos.X, targetPos.Y] = targetPieceBackup;
                piesa.Position = originalPos;
            }
            return legalMoves;
        }
        void ExecuteMove(Point target)
        {
            if (selectedPiece == null) return;

            Point oldPosition = selectedPiece.Position;

            PictureBox pieceToDelete = null;
            foreach (Control c in this.Controls)
            {
                if (c is PictureBox pb && pb.Location.X == target.X * size && pb.Location.Y == (7 - target.Y) * size)
                {
                    if (pb.Tag != selectedPiece)
                    {
                        pieceToDelete = pb;
                        break;
                    }
                }
            }

            if (pieceToDelete != null)
            {
                this.Controls.Remove(pieceToDelete);
                pieceToDelete.Dispose(); 
            }

            myGame.MovePiece(oldPosition, target);

            Piece pieceAfterMove = myGame.Grid[target.X, target.Y];
            foreach (Control c in this.Controls)
            {
                if (c is PictureBox pb && pb.Tag == selectedPiece)
                {
                    pb.Location = new Point(target.X * size, (7 - target.Y) * size);

                    if (pieceAfterMove != selectedPiece)
                    {
                        pb.Tag = pieceAfterMove; 
                        string nameResource = pieceAfterMove.Type.ToString() + (pieceAfterMove.Color == PieceColor.White ? "W" : "B");
                        pb.Image = (Image)Properties.Resources.ResourceManager.GetObject(nameResource);
                    }
                    break;
                }
            }

            foreach (Control c in this.Controls)
            {
                if (c is PictureBox pb && pb.Tag == selectedPiece)
                {
                    pb.Location = new Point(target.X * size, (7 - target.Y) * size);
                    break;
                }
            }

            AllowedMovingToDraw.Clear();
            selectedPiece = null;
            this.Invalidate();
            if (!existLegalMoves(myGame.CurrentTurn))
            {
                King king = myGame.GetKing(myGame.CurrentTurn, myGame.Grid);
                if (king.VerifyIfInCheck(myGame.Grid))
                {
                    MessageBox.Show($"Check Mate! Player {(myGame.CurrentTurn == PieceColor.White ? "Black" : "White")} won.");
                }
                else
                {
                    MessageBox.Show("Tie");
                }
            }
        }
        void ChessTable_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X / size;
            int y = 7 - (e.Y / size);
            Point target = new Point(x, y);

            if (selectedPiece != null && AllowedMovingToDraw.Contains(target))
            {
                ExecuteMove(target);
            }
        }
        void LoadGame(string fen)
        {
            myGame.LoadFen(fen);

            var piecesToDelete = this.Controls.OfType<PictureBox>().ToList();
            foreach (var p in piecesToDelete) this.Controls.Remove(p);

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Piece piece = myGame.Grid[x, y];
                    if (piece != null)
                    {
                        createPictureBoxPiece(piece);
                    }
                }
            }
        }
        bool existLegalMoves(PieceColor color)
        {
            foreach (Piece p in myGame.Grid)
            {
                if (p != null && p.Color == color)
                {
                    List<Point> moves = GetLegalMoves(p);
                    if (moves.Count > 0) return true; 
                }
            }
            return false; 
        }
        void createPictureBoxPiece(Piece piece)
        {
            PictureBox pic = new PictureBox();
            pic.Size = new Size(size, size);
            pic.Location = new Point(piece.Position.X * size, (7 - piece.Position.Y) * size);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            pic.BackColor = Color.Transparent;

            pic.Tag = piece;

            string numeResursa = piece.Type.ToString() + (piece.Color == PieceColor.White ? "W" : "B");
            pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(numeResursa);

            pic.MouseDown += PieceClick;
            this.Controls.Add(pic);
            pic.BringToFront();
        }
    }
}