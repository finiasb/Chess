using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Queen : Piece
    {
        public Queen(PieceColor color, PieceType type, Point position) : base(color, PieceType.Queen, position) { }
        public override List<Point> GetPossibleMoves(Piece[,] board)
        {
            var moves = new Rook(Color, PieceType.Rook, Position).GetPossibleMoves(board);
            moves.AddRange(new Bishop(Color, PieceType.Bishop, Position).GetPossibleMoves(board));
            return moves;
        }
    }
}
