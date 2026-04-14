using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public abstract class Piece
    {
        public PieceColor Color { get; set; }
        public PieceType Type { get; set; }
        public Point Position { get; set; }
        public Piece(PieceColor color, PieceType type, Point position)
        {
            Color = color;
            Type = type;
            Position = position;
        }
        public abstract List<Point> GetPossibleMoves(Piece[,] board);
        protected bool IsOnBoard(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }
    }
}
