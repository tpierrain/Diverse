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

        private static readonly string[] _words = new[] { "alias", "consequatur", "aut", "perferendis", "sit", "voluptatem",
        "accusantium", "doloremque", "aperiam", "eaque","ipsa", "quae", "ab",
        "illo", "inventore", "veritatis", "et", "quasi", "architecto",
        "beatae", "vitae", "dicta", "sunt", "explicabo", "aspernatur", "aut",
        "odit", "aut", "fugit", "sed", "quia", "consequuntur", "magni",
        "dolores", "eos", "qui", "ratione", "voluptatem", "sequi", "nesciunt",
        "neque", "dolorem", "ipsum", "quia", "dolor", "sit", "amet",
        "consectetur", "adipisci", "velit", "sed", "quia", "non", "numquam",
        "eius", "modi", "tempora", "incidunt", "ut", "labore", "et", "dolore",
        "magnam", "aliquam", "quaerat", "voluptatem", "ut", "enim", "ad",
        "minima", "veniam", "quis", "nostrum", "exercitationem", "ullam",
        "corporis", "nemo", "enim", "ipsam", "voluptatem", "quia", "voluptas",
        "sit", "suscipit", "laboriosam", "nisi", "ut", "aliquid", "ex", "ea",
        "commodi", "consequatur", "quis", "autem", "vel", "eum", "iure",
        "reprehenderit", "qui", "in", "ea", "voluptate", "velit", "esse",
        "quam", "nihil", "molestiae", "et", "iusto", "odio", "dignissimos",
        "ducimus", "qui", "blanditiis", "praesentium", "laudantium", "totam",
        "rem", "voluptatum", "deleniti", "atque", "corrupti", "quos",
        "dolores", "et", "quas", "molestias", "excepturi", "sint",
        "occaecati", "cupiditate", "non", "provident", "sed", "ut",
        "perspiciatis", "unde", "omnis", "iste", "natus", "error",
        "similique", "sunt", "in", "culpa", "qui", "officia", "deserunt",
        "mollitia", "animi", "id", "est", "laborum", "et", "dolorum", "fuga",
        "et", "harum", "quidem", "rerum", "facilis", "est", "et", "expedita",
        "distinctio", "nam", "libero", "tempore", "cum", "soluta", "nobis",
        "est", "eligendi", "optio", "cumque", "nihil", "impedit", "quo",
        "porro", "quisquam", "est", "qui", "minus", "id", "quod", "maxime",
        "placeat", "facere", "possimus", "omnis", "voluptas", "assumenda",
        "est", "omnis", "dolor", "repellendus", "temporibus", "autem",
        "quibusdam", "et", "aut", "consequatur", "vel", "illum", "qui",
        "dolorem", "eum", "fugiat", "quo", "voluptas", "nulla", "pariatur",
        "at", "vero", "eos", "et", "accusamus", "officiis", "debitis", "aut",
        "rerum", "necessitatibus", "saepe", "eveniet", "ut", "et",
        "voluptates", "repudiandae", "sint", "et", "molestiae", "non",
        "recusandae", "itaque", "earum", "rerum", "hic", "tenetur", "a",
        "sapiente", "delectus", "ut", "aut", "reiciendis", "voluptatibus",
        "maiores", "doloribus", "asperiores", "repellat"};

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
                var word = _fuzzer.PickOneFrom(_words);
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

            if(nbOfWords <= 1)
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
                var nbWords = _fuzzer.GenerateInteger(1, 10);
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
    }
}