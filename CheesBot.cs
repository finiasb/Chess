using System;
using System.Collections.Generic;
using System.Drawing;

namespace Chess
{
    public class ChessBot
    {
        private Board _game;
        private const int SearchDepth = 3;

        const int pawnValue = 100;
        const int knightValue = 300;
        const int bishopValue = 300;
        const int rookValue = 500;
        const int queenValue = 900;
        const int checkmateScore = 100_000;

        public ChessBot(Board game)
        {
            _game = game;
        }

        public static int Evaluate(Piece[,] board)
        {
            int score = 0;
            foreach (Piece piece in board)
            {
                if (piece == null) continue;
                int value = PieceValue(piece.Type);
                score += piece.Color == PieceColor.White ? value : -value;
            }
            return score;
        }

        private static int PieceValue(PieceType type)
        {
            switch (type)
            {
                case PieceType.Pawn: return pawnValue;
                case PieceType.Knight: return knightValue;
                case PieceType.Bishop: return bishopValue;
                case PieceType.Rook: return rookValue;
                case PieceType.Queen: return queenValue;
                default: return 0;
            }
        }

        private int Minimax(int depth, bool maximizing, int alpha, int beta, Func<Piece, List<Point>> getLegalMoves)
        {
            if (depth == 0)
                return Evaluate(_game.Grid);

            PieceColor color = maximizing ? PieceColor.White : PieceColor.Black;

            var moves = new List<(Piece piece, Point target)>();
            foreach (Piece p in _game.Grid)
            {
                if (p != null && p.Color == color)
                    foreach (Point t in getLegalMoves(p))
                        moves.Add((p, t));
            }

            if (moves.Count == 0)
            {
                King king = _game.GetKing(color, _game.Grid);
                if (_game.VerifyIfInCheck(color, king.Position))
                    return maximizing ? -checkmateScore : checkmateScore;
                return 0; 
            }

            if (maximizing)
            {
                int best = int.MinValue;
                foreach (var (piece, target) in moves)
                {
                    MakeMove(piece, target, out Point from, out Piece captured);
                    int score = Minimax(depth - 1, false, alpha, beta, getLegalMoves);
                    UndoMove(piece, from, target, captured);

                    if (score > best) best = score;
                    if (best > alpha) alpha = best;
                    if (beta <= alpha) break;
                }
                return best;
            }
            else
            {
                int best = int.MaxValue;
                foreach (var (piece, target) in moves)
                {
                    MakeMove(piece, target, out Point from, out Piece captured);
                    int score = Minimax(depth - 1, true, alpha, beta, getLegalMoves);
                    UndoMove(piece, from, target, captured);

                    if (score < best) best = score;
                    if (best < beta) beta = best;
                    if (beta <= alpha) break;
                }
                return best;
            }
        }

        private void MakeMove(Piece piece, Point target, out Point from, out Piece captured)
        {
            from = piece.Position;
            captured = _game.Grid[target.X, target.Y];

            _game.Grid[target.X, target.Y] = piece;
            _game.Grid[from.X, from.Y] = null;
            piece.Position = target;
        }

        private void UndoMove(Piece piece, Point from, Point target, Piece captured)
        {
            _game.Grid[from.X, from.Y] = piece;
            _game.Grid[target.X, target.Y] = captured;
            piece.Position = from;
        }

        public (Piece Piece, Point Target)? GetBestMove(Func<Piece, List<Point>> getLegalMovesDelegate)
        {
            var moves = new List<(Piece piece, Point target)>();
            foreach (Piece p in _game.Grid)
            {
                if (p != null && p.Color == PieceColor.Black)
                    foreach (Point t in getLegalMovesDelegate(p))
                        moves.Add((p, t));
            }

            if (moves.Count == 0) return null;

            int bestScore = int.MaxValue; 
            (Piece, Point)? bestMove = null;

            foreach (var (piece, target) in moves)
            {
                MakeMove(piece, target, out Point from, out Piece captured);
                int score = Minimax(SearchDepth - 1, true, int.MinValue, int.MaxValue, getLegalMovesDelegate);
                UndoMove(piece, from, target, captured);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = (piece, target);
                }
            }

            return bestMove;
        }

        /*int SearchAllCaptures(int alpha, int beta)
        {
            int evaluation = Evaluate(_game.Grid);
            if (evaluation >= beta)
                return beta;

            if(alpha < evaluation)
                alpha = evaluation;

            List<>
        }*/
    }
}