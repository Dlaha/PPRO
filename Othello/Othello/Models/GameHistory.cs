using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Othello.Models
{
    public class GameHistory
    {

        #region Properties

        #region Primary key

        [Required,ForeignKey("GameState"),Key, Column(Order=0)]
        public int idGameState { get; set; }
        [ForeignKey("idGameState")]
        public virtual GameState GameState { get; set; }

        [Required, Key, Column(Order = 1)]
        public DateTime TimeStamp { get; set; }

        #endregion

        [Required, StringLength(Judge.boardDimension * Judge.boardDimension, MinimumLength = Judge.boardDimension * Judge.boardDimension)]
        public string BoardRepresentation;

        #endregion

    }
}