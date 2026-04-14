using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Pawn : Piece
    {
        public Pawn(PieceColor color, PieceType type, Point position) : base(color, PieceType.Pawn, position) { }

        public override List<Point> GetPossibleMoves(Piece[,] board)
        {
            List<Point> result = new List<Point>();
            int forward = (this.Color == PieceColor.White) ? 1 : -1;
            int nextY = Position.Y + forward;

            if (IsOnBoard(Position.X, nextY) && board[Position.X, nextY] == null)
            {
                result.Add(new Point(Position.X, nextY));

                int startRow = (this.Color == PieceColor.White) ? 1 : 6;
                int doubleNextY = Position.Y + (2 * forward);
                if (Position.Y == startRow && board[Position.X, doubleNextY] == null)
                {
                    result.Add(new Point(Position.X, doubleNextY));
                }
            }

            int[] diagX = { Position.X - 1, Position.X + 1 };
            foreach (int nx in diagX)
            {
                if (IsOnBoard(nx, nextY))
                {
                    Piece piesaTarget = board[nx, nextY];
                    if (piesaTarget != null && piesaTarget.Color != this.Color)
                    {
                        result.Add(new Point(nx, nextY));
                    }
                }
            }
            return result;
        }
    }
}
