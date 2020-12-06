namespace Diverse.Tests.Utils
{
    /// <summary>
    /// Dummy class for testing purpose (for Property-based Fuzzing).
    /// What matters here is to have one empty (private) constructor only.
    /// </summary>
    public class TypeWithPrivateEmptyConstructorOnly
    {
        private string _modifiableSecret;
        public string ModifiableSecret // Property Fuzzable since it has a Setter
        {
            get => _modifiableSecret;
            set => _modifiableSecret = value;
        }

        private string _consultableSecret = null;
        public string ConsultableSecret => _consultableSecret;

        public string Name { get;  }
        public int FavoriteNumber { get; private set; } // Property Fuzzable since it has a Setter

        private TypeWithPrivateEmptyConstructorOnly()
        {
        }
    }
}