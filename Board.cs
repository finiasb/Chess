using System;
using System.Collections.Generic;
using System.Drawing;

namespace Chess
{
    public class Board
    {
        public Piece[,] Grid { get; private set; }
        public PieceColor CurrentTurn { get; private set; }
        public Point? EnPassantTarget { get; set; } = null;

        public Board()
        {
            Grid = new Piece[8, 8];
            CurrentTurn = PieceColor.White;
        }
        public void LoadFen(string fen)
        {
            Array.Clear(Grid, 0, Grid.Length);

            string[] parts = fen.Split(' ');
            string positions = parts[0];
            CurrentTurn = (parts[1] == "w") ? PieceColor.White : PieceColor.Black;

            int row = 7;
            int col = 0;

            foreach (char c in positions)
            {
                if (c == '/')
                {
                    row--;
                    col = 0;
                }
                else if (char.IsDigit(c))
                {
                    col += (int)char.GetNumericValue(c);
                }
                else
                {
                    PieceColor color = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                    Point pos = new Point(col, row);

                    Grid[col, row] = CreatePiece(c, color, pos);
                    col++;
                }
            }
        }

        private Piece CreatePiece(char c, PieceColor color, Point pos)
        {
            switch (char.ToLower(c))
            {
                case 'p': return new Pawn(color, PieceType.Pawn, pos);
                case 'n': return new Knight(color, PieceType.Knight, pos);
                case 'b': return new Bishop(color, PieceType.Bishop, pos);
                case 'r': return new Rook(color, PieceType.Rook, pos);
                case 'q': return new Queen(color, PieceType.Queen, pos);
                case 'k': return new King(color, PieceType.King, pos);
                default: return null;
            }
        }
        public King GetKing(PieceColor color, Piece[,] board)
        {
            foreach (var piece in board)
            {
                if (piece is King && piece.Color == color)
                {
                    return (King)piece;
                }
            }
            return null;
        }
        public void MovePiece(Point from, Point to)
        {
            Piece p = Grid[from.X, from.Y];
            EnPassantTarget = null;

            if (p == null) return;

            if (p is Pawn && Math.Abs(from.Y - to.Y) == 2)
            {
                EnPassantTarget = new Point(from.X, (from.Y + to.Y) / 2);
            }

            p.Position = to;
            Grid[to.X, to.Y] = p;
            Grid[from.X, from.Y] = null;

            if (p is Pawn)
            {
                if ((p.Color == PieceColor.White && to.Y == 7) || (p.Color == PieceColor.Black && to.Y == 0))
                {
                    Grid[to.X, to.Y] = new Queen(p.Color, PieceType.Queen, to);
                }
            }
            CurrentTurn = (CurrentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;

        }
        public bool VerifyIfInCheck(PieceColor color, Point kingPosition)
        {
            foreach (Piece piece in Grid)
            {
                if (piece != null && piece.Color != color)
                {
                    List<Point> moves = piece.GetPossibleMoves(this);
                    foreach (Point p in moves)
                    {
                        if (p.X == kingPosition.X && p.Y == kingPosition.Y)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}