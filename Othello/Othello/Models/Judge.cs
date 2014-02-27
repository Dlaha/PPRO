using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Othello.Models
{
    public struct Point{
        public int x,y;

        public static Point operator +(Point P1, Point P2)
        {
            Point result;
            result.x = P1.x + P2.x;
            result.y = P1.y + P2.y;
            return result;
        }

        public static Point operator -(Point P1, Point P2)
        {
            Point result;
            result.x = P1.x - P2.x;
            result.y = P1.y - P2.y;
            return result;
        }
    }

    public enum FieldColor
    {
        Empty = 0, Black = 1, White = 2
    }
    
    public static class Judge
    {
        public const int boardDimension = 8;

        static public int FieldsCount(FieldColor[,] board, FieldColor color)
        {
            int result = 0;
            for (int x = 0; x < boardDimension; x++)
                for (int y = 0; y < boardDimension; y++)
                    if (board[x, y] == color) result++;
            return result;
        }

        static private int ObtainStonesInDirection(ref FieldColor[,] board, FieldColor activePlayer, Point startPos, Point direction)
        {
            if (direction.x < -1 || direction.x > 1 || direction.y < -1 || direction.y > 1)
                throw new Exception(string.Format("Judge.ValidDirection error - wrong direction (x = {1}, y = {2})", direction.x, direction.y));
            if (board[startPos.x, startPos.y] != FieldColor.Empty) return 0;
            FieldColor oposingPlayer = (activePlayer==FieldColor.Black) ? FieldColor.White : FieldColor.Black;
            int r = 0;
            while (true)
            {
                startPos += direction;
                if (startPos.x < 0 || startPos.x > boardDimension - 1 || startPos.y < 0 || startPos.y > boardDimension - 1) return 0;
                if (board[startPos.x, startPos.y] == FieldColor.Empty) return 0;
                if (board[startPos.x, startPos.y] == oposingPlayer) r++;
                if (board[startPos.x, startPos.y] == activePlayer) break;
            }
            // do move
            for (int i = r; i > 0; i--)
            {
                startPos -= direction;
                board[startPos.x, startPos.y] = activePlayer;
            }
            return r;
        }

        static public bool DoMove(ref FieldColor[,] board, FieldColor activePlayer, Point pos)
        {
            if (board[pos.x, pos.y] != FieldColor.Empty) throw new Exception("Judge.DoMove - Bad start position");
            bool r = false;
            for (int i = 0; i < 9; i++)
            {
                Point d; // direction
                d.x = ((i / 3) % 3) - 1;
                d.y = (i % 3) - 1;
                if (d.x == 0 && d.y == 0) continue;
                r = ObtainStonesInDirection(ref board, activePlayer, pos, d)>0 || r;
            }
            return r;
        }

        static public bool IsMovePossible(FieldColor[,] board, FieldColor activePlayer, Point pos)
        {

            FieldColor[,] temp = board.Clone() as FieldColor[,];
            try
            {
                return DoMove(ref temp, activePlayer, pos);
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        static public List<Point> PossibleMoves(FieldColor[,] board, FieldColor activePlayer)
        {
            List<Point> res = new List<Point>();
            for (int x = 0; x < boardDimension; x++)
                for (int y = 0; y < boardDimension; y++)
                {
                    Point p; p.x = x; p.y = y;
                    if (!res.Contains(p) && IsMovePossible(board,activePlayer,p)) 
                        res.Add(p);
                }
            return res;
        }
    }

}