using System.Collections;
using System.Collections.Generic;

namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Chess Player. Dummy class for testing purpose (for Constructor-based Fuzzing).
    /// What matters here is to have at least one non-empty constructor.
    /// </summary>
    internal class ChessPlayerWithPublicConstructor : PlayerWithProtectedConstructor
    {
        /// <summary>
        /// Chess level (important to be an enum here (for testing purpose).
        /// </summary>
        public ChessLevel ChessLevel { get; }

        public ChessPlayerWithPublicConstructor FavoriteOpponent { get;  }

        public ChessClub CurrentClub { get; }

        public IEnumerable<ChessClub> FormerClubs { get;  }

        public ChessPlayerWithPublicConstructor() : base("obsolete", "should not be used", 0)
        {
        }

        public ChessPlayerWithPublicConstructor(string firstName) : base(firstName, "obsolete constructor that should not be used", 0)
        {
        }

        public ChessPlayerWithPublicConstructor(string firstName, string lastName, int age, ChessLevel chessLevel, ChessPlayerWithPublicConstructor favoriteOpponent = null, ChessClub currentClub = null, IEnumerable<ChessClub> formerClubs = null) : base(firstName, lastName, age)
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