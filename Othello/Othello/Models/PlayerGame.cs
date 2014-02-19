using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Othello.Models
{
    /// <summary>
    /// Representation of game from player point of view
    /// </summary>
    public class PlayerGame
    {

        public GameState GameState { get; private set; }
        public Player Player { get; private set; }

        public bool MyTurn { get { return GameState.ActivePlayerInstance == Player; } }

        public Player Opponent { get { return (GameState.BlackPlayer == Player ? GameState.WhitePlayer : GameState.BlackPlayer); } }

        public PlayerGame(Player player, GameState gameState)
        {
            this.GameState = gameState;
            this.Player = player;
            if (GameState.WhitePlayer != Player && GameState.BlackPlayer != Player)
                throw new Exception(this.ToString()+"Player doesn't play the game");
        }

    }
}