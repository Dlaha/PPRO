using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Othello.Models
{
    public struct Point{
        public int x,y;
    }

    public enum FieldColor
    {
        Empty, Black, White
    }
    
    public static class Judge
    {
        public const int boardDimension = 8;

        static private bool ValidDirection(FieldColor[,] board, FieldColor activePlayer, Point startPos, Point direction)
        {
            return false;
        }

        static public List<Point> PossibleMoves(FieldColor[,] board, FieldColor activePlayer)
        {
            List<Point> res = new List<Point>();
            for (int x = 0; x < boardDimension; x++)
                for (int y = 0; y < boardDimension; y++)
                {
                    Point p; p.x = x; p.y = y;
                    if (board[p.x,p.y]!=FieldColor.Empty || res.Contains(p)) continue;
                    for (int i = 0; i < 9; i++)
                    {
                        Point d;
                        d.x = ((i/3) % 3) - 1;
                        d.y = (i % 3) - 1;
                        if (ValidDirection(board, activePlayer, p, d))
                        {
                            res.Add(p);
                            break;
                        }
                    }
                }
            return res;
        }
    }

}