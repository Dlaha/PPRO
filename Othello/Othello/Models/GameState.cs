﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Othello.Models
{

    public class GameState
    {

        [Key]
        public int Id { get; set; }
        [Required]
        public bool ActiveWhitePlayer { get; set; }

        [Required]
        public DateTime TimeStamp { get; set; }
        [Required, StringLength(Judge.boardDimension * Judge.boardDimension, MinimumLength = Judge.boardDimension * Judge.boardDimension)]
        public string BoardRepresentation
        {
            get
            {
                string s = string.Empty;
                for (int x = 0; x < Judge.boardDimension; x++)
                    for (int y = 0; y < Judge.boardDimension; y++)
                        s += Convert.ToString((int)Board[x,y]);
                return s;
            }
            set {
                for (int x = 0; x < Judge.boardDimension; x++)
                    for (int y = 0; y < Judge.boardDimension; y++)
                        Board[x, y] = (FieldColor)Convert.ToInt32(""+value[x * Judge.boardDimension + y]);
            }
        }
        [Required]
        public bool Invalid { get; set; }

        #region White Player

        [Required, ForeignKey("WhitePlayer")]
        public int idWhitePlayer { get; set; }
        [Required,ForeignKey("idWhitePlayer")]
        public virtual Player WhitePlayer { get; set; }

        [NotMapped]
        public int WhiteScore { get { return Judge.FieldsCount(Board, FieldColor.White); } }

        #endregion

        #region Black Player

        [Required, ForeignKey("BlackPlayer")]
        public int idBlackPlayer { get; set; }
        [Required, ForeignKey("idBlackPlayer")]
        public virtual Player BlackPlayer { get; set; }

        [NotMapped]
        public int BlackScore { get { return Judge.FieldsCount(Board, FieldColor.Black); } }

        #endregion

        [NotMapped]
        public FieldColor[,] Board { get; private set; }

        [NotMapped]
        public Player ActivePlayerInstance
        {
            get
            { return ActiveWhitePlayer ? WhitePlayer : BlackPlayer; }
        }

        [NotMapped]
        private FieldColor activePlayerColor
        {
            get
            {
                return (ActiveWhitePlayer ? FieldColor.White : FieldColor.Black);
            }
        }

        [NotMapped]
        public Player WinningPlayer
        {
            get
            {
                int b = BlackScore, w = WhiteScore;
                if (b == w) return null;
                return (b > w ? BlackPlayer : WhitePlayer);
            }
        }

        private GameState()
        {
            Invalid = false;
            TimeStamp = DateTime.UtcNow;
            ActiveWhitePlayer = true; // white player plays first
            // default board
            Board = new FieldColor[Judge.boardDimension, Judge.boardDimension];
            for (int x = 0; x < Judge.boardDimension; x++)
                for (int y = 0; y < Judge.boardDimension; y++)
                    Board[x, y] = FieldColor.Empty;
            // game default state
            Board[3, 3] = FieldColor.White; Board[4, 3] = FieldColor.Black;
            Board[3, 4] = FieldColor.Black; Board[4, 4] = FieldColor.White;
            ValidMove(5,3);
        }

        public GameState(Player whitePlayer, Player blackPlayer):this()
        {
            this.WhitePlayer = whitePlayer;
            this.BlackPlayer = blackPlayer;
        }

        public Player PlayerByID(int idPlayer)
        {
            if (WhitePlayer.Id == idPlayer) return WhitePlayer;
            else if (BlackPlayer.Id == idPlayer) return BlackPlayer;
            else return null;
        }

        public bool ValidMove(int x, int y)
        {
            Point p; p.x=x; p.y=y;
            return Judge.IsMovePossible(Board, (ActiveWhitePlayer ? FieldColor.White : FieldColor.Black), p);
        }

        public void NextTurn(int x, int y)
        {
            // move
            Point p; p.x = x; p.y = y;
            FieldColor[,] temp = Board;
            Judge.DoMove(ref temp, activePlayerColor, p);
            temp[x, y] = (ActiveWhitePlayer ? FieldColor.White : FieldColor.Black);
            Board = temp;
            ActiveWhitePlayer = !ActiveWhitePlayer; // change player
            TimeStamp = DateTime.UtcNow;
        }

        public bool IsGameOver()
        {
            return Judge.PossibleMoves(Board, activePlayerColor).Count == 0;
        }


    }
}