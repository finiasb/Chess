using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
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
        ChessBot blackBot;
        List<Point> AllowedMovingToDraw = new List<Point>();
        public ChessTable()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 800);
            blackBot = new ChessBot(myGame);
            string startPosition = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            LoadGame(startPosition);
        }
        private async void TriggerBotMove()
        {
            var move = blackBot.GetBestMove(GetLegalMoves);

            if (move.HasValue)
            {
                selectedPiece = move.Value.Piece;
                ExecuteMove(move.Value.Target);
            }
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
                if (myGame.VerifyIfInCheck(color, king.Position))
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
        void PieceMouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            if (pic == null || e.Button != MouseButtons.Left) return;
            int x = pic.Location.X / size;
            int y = 7 - (pic.Location.Y / size);
            Point clickLocation = new Point(x, y);

            if (selectedPiece != null && AllowedMovingToDraw.Contains(clickLocation))
            {
                ExecuteMove(clickLocation);
                return;
            }

            Piece p = pic.Tag as Piece;
            if (p != null && p.Color == myGame.CurrentTurn)
            {
                selectedPiece = p;
                AllowedMovingToDraw = GetLegalMoves(p);

                this.Invalidate();

                pic.DoDragDrop(pic, DragDropEffects.Move);

                AllowedMovingToDraw.Clear();
                selectedPiece = null;
                this.Invalidate();
            }
        }

        List<Point> GetLegalMoves(Piece piece)
        {
            List<Point> pseudoMoves = piece.GetPossibleMoves(myGame);
            List<Point> legalMoves = new List<Point>();

            Point originalPos = piece.Position;
            Piece[,] board = myGame.Grid;

            foreach (Point targetPos in pseudoMoves)
            {
                Piece targetPieceBackup = board[targetPos.X, targetPos.Y];
                board[targetPos.X, targetPos.Y] = piece;
                board[originalPos.X, originalPos.Y] = null;
                piece.Position = targetPos;
                King king = myGame.GetKing(piece.Color, myGame.Grid);

                if (!myGame.VerifyIfInCheck(piece.Color, king.Position))
                {
                    legalMoves.Add(targetPos);
                }

                board[originalPos.X, originalPos.Y] = piece;
                board[targetPos.X, targetPos.Y] = targetPieceBackup;
                piece.Position = originalPos;
            }

            if (piece is King kingPiece)
            {
                if (kingPiece.CanSmallCastle(myGame))
                {
                    int targetY = (kingPiece.Color == PieceColor.White) ? 0 : 7;
                    legalMoves.Add(new Point(6, targetY));
                }
                if (kingPiece.CanLongCastle(myGame))
                {
                    int targetY = (kingPiece.Color == PieceColor.White) ? 0 : 7;
                    legalMoves.Add(new Point(2, targetY));
                }
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
            if (selectedPiece.Type == PieceType.Rook)
                selectedPiece.HasMoved = true;
            if (selectedPiece.Type == PieceType.King)
                selectedPiece.HasMoved = true;


            if (selectedPiece is Pawn && target == myGame.EnPassantTarget)
            {
                int capturedPawnY = oldPosition.Y;
                Point enemyPawnPos = new Point(target.X, capturedPawnY);

                myGame.Grid[enemyPawnPos.X, enemyPawnPos.Y] = null;

                foreach (Control c in this.Controls)
                {
                    if (c is PictureBox pb && pb.Location.X == enemyPawnPos.X * size && pb.Location.Y == (7 - enemyPawnPos.Y) * size)
                    {
                        this.Controls.Remove(pb);
                        pb.Dispose();
                        break;
                    }
                }
            }
            if (selectedPiece.Type == PieceType.King && oldPosition.X == 4 && target.X == 6)
            {
                myGame.MovePiece(oldPosition, target);

                Point rookOldPos = new Point(7, oldPosition.Y);
                Point rookNewPos = new Point(5, oldPosition.Y);
                myGame.MovePiece(rookOldPos, rookNewPos);

                foreach (Control c in this.Controls)
                {
                    if (c is PictureBox pb && pb.Tag is Piece p && p.Type == PieceType.Rook && p.Position == rookNewPos)
                    {
                        pb.Location = new Point(rookNewPos.X * size, (7 - rookNewPos.Y) * size);
                        break;
                    }
                }
            }
            else if (selectedPiece.Type == PieceType.King && oldPosition.X == 4 && target.X == 2)
            {
                myGame.MovePiece(oldPosition, target);

                Point rookOldPos = new Point(0, oldPosition.Y);
                Point rookNewPos = new Point(3, oldPosition.Y);
                myGame.MovePiece(rookOldPos, rookNewPos);

                foreach (Control c in this.Controls)
                {
                    if (c is PictureBox pb && pb.Tag is Piece p && p.Type == PieceType.Rook && p.Position == rookNewPos)
                    {
                        pb.Location = new Point(rookNewPos.X * size, (7 - rookNewPos.Y) * size);
                        break;
                    }
                }
            }
            else
            {
                myGame.MovePiece(oldPosition, target);
            }
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

                if (myGame.VerifyIfInCheck(myGame.CurrentTurn, king.Position))
                {
                    MessageBox.Show($"Check Mate! Player {(myGame.CurrentTurn == PieceColor.White ? "Black" : "White")} won.");
                }
                else
                {
                    MessageBox.Show("Tie");
                }
            }
            if (myGame.CurrentTurn == PieceColor.Black)
            {
                TriggerBotMove();
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
        private void Control_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(PictureBox)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void Control_DragDrop(object sender, DragEventArgs e)
        {
            Point clientPoint = this.PointToClient(new Point(e.X, e.Y));

            int x = clientPoint.X / size;
            int y = 7 - (clientPoint.Y / size);
            Point target = new Point(x, y);

            if (selectedPiece != null && AllowedMovingToDraw.Contains(target))
            {
                ExecuteMove(target);
            }

            AllowedMovingToDraw.Clear();
            selectedPiece = null;
            this.Invalidate();
        }
        void createPictureBoxPiece(Piece piece)
        {
            PictureBox pic = new PictureBox();
            pic.Size = new Size(size, size);
            pic.Location = new Point(piece.Position.X * size, (7 - piece.Position.Y) * size);
            pic.SizeMode = PictureBoxSizeMode.StretchImage;
            pic.BackColor = Color.Transparent;
            pic.AllowDrop = true;
            pic.Tag = piece;
            string numeResursa = piece.Type.ToString() + (piece.Color == PieceColor.White ? "W" : "B");
            pic.Image = (Image)Properties.Resources.ResourceManager.GetObject(numeResursa);

            pic.MouseDown += PieceMouseDown;
            pic.DragEnter += Control_DragEnter;
            pic.DragDrop += Control_DragDrop;
            this.Controls.Add(pic);
            pic.BringToFront();
        }
    }
}