using System.Collections;
using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Chess Player (dummy class for testing purpose).
    /// </summary>
    internal class ChessPlayer : Player
    {
        /// <summary>
        /// Chess level (important to be an enum here (for testing purpose).
        /// </summary>
        public ChessLevel ChessLevel { get; }

        public ChessPlayer FavoriteOpponent { get;  }

        public ChessClub CurrentClub { get; }

        public IEnumerable<ChessClub> FormerClubs { get;  }

        public ChessPlayer(string firstName, string lastName, int age, ChessLevel chessLevel, ChessPlayer favoriteOpponent = null, ChessClub currentClub = null, IEnumerable<ChessClub> formerClubs = null) : base(firstName, lastName, age)
        {
            ChessLevel = chessLevel;
            FavoriteOpponent = favoriteOpponent;
            CurrentClub = currentClub;
            FormerClubs = formerClubs;
        }
    }

    internal class ChessClub
    {
        public string Name { get; }
        public Country Country { get; }

        public ChessClub(string name, Country country)
        {
            Name = name;
            Country = country;
        }
    }

    internal enum Country
    {
        Russia,
        France,
        Usa,
        Canada,
        Senegal,
        Ukraine
    }
}