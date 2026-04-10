using System;
using System.Collections.Generic;
using System.Drawing;

namespace Chess
{
    public class Board
    {
        public Piece[,] Grid { get; private set; }
        public PieceColor CurrentTurn { get; private set; }

        public Board()
        {
            Grid = new Piece[8, 8];
            CurrentTurn = PieceColor.White;
        }

        public void IncarcaFEN(string fen)
        {
            Array.Clear(Grid, 0, Grid.Length);

            string[] parti = fen.Split(' ');
            string pozitii = parti[0];
            CurrentTurn = (parti[1] == "w") ? PieceColor.White : PieceColor.Black;

            int rand = 7; 
            int coloana = 0;

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
                    PieceColor color = char.IsUpper(c) ? PieceColor.White : PieceColor.Black;
                    Point pos = new Point(coloana, rand);

                    Grid[coloana, rand] = CreatePiece(c, color, pos);
                    coloana++;
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

        public void MovePiece(Point from, Point to)
        {
            Piece p = Grid[from.X, from.Y];
            if (p != null)
            {
                p.Position = to;
                Grid[to.X, to.Y] = p;
                Grid[from.X, from.Y] = null;

                CurrentTurn = (CurrentTurn == PieceColor.White) ? PieceColor.Black : PieceColor.White;
            }
        }
    }
}