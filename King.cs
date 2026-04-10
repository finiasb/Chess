using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public class King : Piece
    {
        public King(PieceColor color, PieceType type, Point position) : base(color, PieceType.King, position) { }

        public override List<Point> GetPossibleMoves(Piece[,] board)
        {
            List<Point> moves = new List<Point>();
            int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
            int[] dy = { 1, -1, 0, 0, 1, -1, 1, -1 };

            for (int i = 0; i < 8; i++)
            {
                int nx = Position.X + dx[i];
                int ny = Position.Y + dy[i];

                if (IsOnBoard(nx, ny))
                {
                    if (board[nx, ny] == null || board[nx, ny].Color != this.Color)
                    {
                        moves.Add(new Point(nx, ny));
                    }
                }
            }
            return moves;
        }
    }
}
