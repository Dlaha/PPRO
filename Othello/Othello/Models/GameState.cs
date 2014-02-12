using System;
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
        
        public GameState Previous { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        [Required]
        public FieldColor[,] Board { get; set; }
        [Required]
        public bool Invalid { get; set; }

        #region White Player

        [ForeignKey("WhitePlayer")]
        [Required]
        public int idWhitePlayer { get; set; }
        [ForeignKey("idWhitePlayer")]
        public virtual Player WhitePlayer { get; set; }

        #endregion

        #region Black Player

        [ForeignKey("BlackPlayer")]
        [Required]
        public int idBlackPlayer { get; set; }
        [ForeignKey("idBlackPlayer")]
        public virtual Player BlackPlayer { get; set; }

        #endregion

        private GameState()
        {
            Invalid = false;
            TimeStamp = DateTime.UtcNow;
            // default board
            Board = new FieldColor[Judge.boardDimension, Judge.boardDimension];
            for (int x = 0; x < Judge.boardDimension; x++)
                for (int y = 0; y < Judge.boardDimension; y++)
                    Board[x, y] = FieldColor.Empty;
            // game default state
            Board[3, 3] = FieldColor.White; Board[4, 3] = FieldColor.Black;
            Board[3, 4] = FieldColor.Black; Board[4, 4] = FieldColor.White;

            Judge.PossibleMoves(Board,FieldColor.White);
        }

        public GameState(Player whitePlayer, Player blackPlayer):this()
        {
            this.WhitePlayer = whitePlayer;
            this.BlackPlayer = blackPlayer;
            
        }

        public GameState NextTurn()
        {
            return new GameState(WhitePlayer, BlackPlayer) { Board = this.Board };
        }
    }
}