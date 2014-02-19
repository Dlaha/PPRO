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

        public bool ImPlaying { get { return GameState.ActivePlayerInstance == Player; } }

        public PlayerGame(Player player, GameState gameState)
        {
            this.GameState = gameState;
            this.Player = player;
        }

    }
}