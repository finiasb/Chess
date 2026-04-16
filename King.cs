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

        public override List<Point> GetPossibleMoves(Board board)
        {
            List<Point> moves = new List<Point>();
            Piece[,] grid = board.Grid;

            int[] dx = { 0, 0, 1, -1, 1, 1, -1, -1 };
            int[] dy = { 1, -1, 0, 0, 1, -1, 1, -1 };

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

        public bool CanSmallCastle(Board board)
        {
            int row = this.Position.Y;
            int rookCol = 7;

            Piece rook = board.Grid[rookCol, row];

            if (this.HasMoved || rook == null || !(rook is Rook) || rook.HasMoved)
                return false;

            if (board.Grid[5, row] != null || board.Grid[6, row] != null)
                return false;

            PieceColor opponentColor = (this.Color == PieceColor.White) ? PieceColor.Black : PieceColor.White;

            if (board.VerifyIfInCheck(this.Color, new Point(4, row)) || board.VerifyIfInCheck(this.Color, new Point(5, row)) || board.VerifyIfInCheck(this.Color, new Point(6, row)))
            {
                return false;
            }

            return true;
        }
        public bool CanLongCastle(Board board)
        {
            int row = this.Position.Y;
            int rookCol = 0;

            Piece rook = board.Grid[rookCol, row];

            if (this.HasMoved || rook == null || !(rook is Rook) || rook.HasMoved)
                return false;

            if (board.Grid[1, row] != null || board.Grid[2, row] != null || board.Grid[3, row] != null)
                return false;

            PieceColor opponentColor = (this.Color == PieceColor.White) ? PieceColor.Black : PieceColor.White;

            if (board.VerifyIfInCheck(this.Color, new Point(2, row)) || board.VerifyIfInCheck(this.Color, new Point(3, row)) || board.VerifyIfInCheck(this.Color, new Point(4, row)))
            {
                return false;
            }

            return true;
        }
    }
}