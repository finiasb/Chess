using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chess
{
    public class Pawn : Piece
    {
        public Pawn(PieceColor color, PieceType type, Point position) : base(color, PieceType.Pawn, position) { }

        public override List<Point> GetPossibleMoves(Board board)
        {
            List<Point> result = new List<Point>();
            int forward = (this.Color == PieceColor.White) ? 1 : -1;
            int nextY = Position.Y + forward;

            Piece[,] grid = board.Grid;

            if (IsOnBoard(Position.X, nextY) && grid[Position.X, nextY] == null)
            {
                result.Add(new Point(Position.X, nextY));

                int startRow = (this.Color == PieceColor.White) ? 1 : 6;
                int doubleNextY = Position.Y + (2 * forward);
                if (Position.Y == startRow && grid[Position.X, doubleNextY] == null)
                {
                    result.Add(new Point(Position.X, doubleNextY));
                }
            }

            int[] diagX = { Position.X - 1, Position.X + 1 };
            foreach (int nx in diagX)
            {
                if (IsOnBoard(nx, nextY))
                {
                    Piece piesaTarget = grid[nx, nextY];
                    if (piesaTarget != null && piesaTarget.Color != this.Color)
                    {
                        result.Add(new Point(nx, nextY));
                    }
                }
            }
            if (board.EnPassantTarget != null)
            {
                Point target = board.EnPassantTarget.Value;
                int direction = (this.Color == PieceColor.White) ? 1 : -1;
                if (Math.Abs(target.X - this.Position.X) == 1 && target.Y == this.Position.Y + direction)
                {
                    result.Add(target);
                }
            }

            return result;
        }
    }
}
