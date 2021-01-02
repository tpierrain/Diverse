using System;
using System.Collections.Generic;
using System.Linq;

namespace Diverse.Strings
{
    /// <summary>
    /// Fuzz texts in Latin (see. Lorem Ipsum... https://www.lipsum.com/).
    /// </summary>
    internal class LoremFuzzer : IFuzzLorem
    {
        private readonly IFuzz _fuzzer;

        private static int MinNumberOfWordsInASentence = 2;
        private static readonly char[] Alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();

        /// <summary>
        /// Instantiates a <see cref="LoremFuzzer"/>.
        /// </summary>
        /// <param name="fuzzer">Instance of <see cref="IFuzz"/> to use.</param>
        public LoremFuzzer(IFuzz fuzzer)
        {
            _fuzzer = fuzzer;
        }

        /// <summary>
        /// Generates random latin words.
        /// </summary>
        /// <param name="number">(optional) Number of words to generate.</param>
        /// <returns>The generated latin words.</returns>
        public IEnumerable<string> GenerateWords(int? number)
        {
            number = number ?? 5;

            var result = new List<string>();
            for (var i = 0; i < number.Value; i++)
            {
                var word = _fuzzer.PickOneFrom(Latin.Words);
                result.Add(word);
            }

            return result;
        }

        /// <summary>
        /// Generate a sentence in latin.
        /// </summary>
        /// <param name="nbOfWords">(optional) Number of words for this sentence.</param>
        /// <returns>The generated sentence in latin.</returns>
        public string GenerateSentence(int? nbOfWords = null)
        {
            nbOfWords = nbOfWords ?? 6;

            if(nbOfWords < MinNumberOfWordsInASentence)
            {
                throw new ArgumentOutOfRangeException(nameof(nbOfWords), "A sentence must have more than 1 word.");
            }

            var words = GenerateWords(nbOfWords.Value).ToArray();

            var firstWordCapitalized = words.First().FirstCharToUpper();
            words[0] = firstWordCapitalized;

            var sentence = $"{string.Join(" ", words)}.";

            return sentence;
        }

        /// <summary>
        /// Generate a paragraph in latin.
        /// </summary>
        /// <param name="nbOfSentences">(optional) Number of sentences for this paragraph.</param>
        /// <returns>The generated paragraph in latin.</returns>
        public string GenerateParagraph(int? nbOfSentences = null)
        {
            nbOfSentences = nbOfSentences ?? 5;

            var sentences = new List<string>();

            for (var i = 0; i < nbOfSentences.Value; i++)
            {
                var nbWords = _fuzzer.GenerateInteger(MinNumberOfWordsInASentence, 10);
                var sentence = GenerateSentence(nbWords);
                sentences.Add(sentence);
            }

            var paragraph = string.Join(" ", sentences);

            return paragraph;
        }

        /// <summary>
        /// Generates a collection of paragraphs. 
        /// </summary>
        /// <param name="nbOfParagraphs">(optional) Number of paragraphs to generate.</param>
        /// <returns>The collection of paragraphs.</returns>
        public IEnumerable<string> GenerateParagraphs(int? nbOfParagraphs = null)
        {
            nbOfParagraphs = nbOfParagraphs ?? 3;

            var paragraphs = new List<string>();
            for (var i = 0; i < nbOfParagraphs.Value; i++)
            {
                paragraphs.Add(GenerateParagraph());
            }

            return paragraphs;
        }

        /// <summary>
        /// Generates a text in latin with a specified number of paragraphs.
        /// </summary>
        /// <param name="nbOfParagraphs">(optional) Number of paragraphs to generate.</param>
        /// <returns>The generated text in latin.</returns>
        public string GenerateText(int? nbOfParagraphs = null)
        {
            var paragraphs = GenerateParagraphs(nbOfParagraphs);

            var text = string.Join(Environment.NewLine + Environment.NewLine, paragraphs);

            return text;
        }

        /// <summary>
        /// Generates a random letter.
        /// </summary>
        /// <returns>The generated letter.</returns>
        public char GenerateLetter()
        {
            var alphabet = Alphabet;

            var character = _fuzzer.PickOneFrom<char>(alphabet);

            return character;
        }
    }
}