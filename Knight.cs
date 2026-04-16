using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class Knight : Piece
    {
        public Knight(PieceColor color, PieceType type, Point position) : base(color, PieceType.Knight, position) { }

        public override List<Point> GetPossibleMoves(Board board)
        {
            List<Point> moves = new List<Point>();
            Piece[,] grid = board.Grid;

            int[] dx = { 1, 1, -1, -1, 2, 2, -2, -2 };
            int[] dy = { 2, -2, 2, -2, 1, -1, 1, -1 };

            for (int i = 0; i < 8; i++)
            {
                int nx = Position.X + dx[i];
                int ny = Position.Y + dy[i];

                if (IsOnBoard(nx, ny))
                {
                    if (grid[nx, ny] == null || grid[nx, ny].Color != this.Color)
                    {
                        moves.Add(new Point(nx, ny));
                    }
                }
            }
            return moves;
        }
    }
}
