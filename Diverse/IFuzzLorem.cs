using System;
using System.Collections.Generic;

namespace Diverse
{
    /// <summary>
    /// Fuzz texts in Latin (see. Lorem Ipsum... https://www.lipsum.com/).
    /// </summary>
    public interface IFuzzLorem
    {
        /// <summary>
        /// Generates a random letter.
        /// </summary>
        /// <returns>The generated letter.</returns>
        char GenerateLetter();

        /// <summary>
        /// Generates random latin words.
        /// </summary>
        /// <param name="number">(optional) Number of words to generate.</param>
        /// <returns>The generated latin words.</returns>
        IEnumerable<string> GenerateWords(int? number);

        /// <summary>
        /// Generate a sentence in latin.
        /// </summary>
        /// <param name="nbOfWords">(optional) Number of words for this sentence.</param>
        /// <returns>The generated sentence in latin.</returns>
        string GenerateSentence(int? nbOfWords = null);

        /// <summary>
        /// Generates a paragraph in latin.
        /// </summary>
        /// <param name="nbOfSentences">(optional) Number of sentences for this paragraph.</param>
        /// <returns>The generated paragraph in latin.</returns>
        string GenerateParagraph(int? nbOfSentences = null);

        /// <summary>
        /// Generates a collection of paragraphs. 
        /// </summary>
        /// <param name="nbOfParagraphs">(optional) Number of paragraphs to generate.</param>
        /// <returns>The collection of paragraphs.</returns>
        IEnumerable<string> GenerateParagraphs(int? nbOfParagraphs = null);

        /// <summary>
        /// Generates a text in latin with a specified number of paragraphs.
        /// </summary>
        /// <param name="nbOfParagraphs">(optional) Number of paragraphs to generate.</param>
        /// <returns>The generated text in latin.</returns>
        string GenerateText(int? nbOfParagraphs = null);
    }
}