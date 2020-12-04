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
        public ChessLevel ChessLevel { get; private set; }

        public ChessPlayer(string firstName, string lastName, int age, ChessLevel chessLevel) : base(firstName, lastName, age)
        {
            ChessLevel = chessLevel;
        }
    }
}