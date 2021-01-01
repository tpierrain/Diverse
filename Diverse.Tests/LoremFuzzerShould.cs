using System;
using NFluent;
using NUnit.Framework;

namespace Diverse.Tests
{
    [TestFixture]
    public class LoremFuzzerShould
    {
        [Test]
        public void GenerateWords()
        {
            var fuzzer = new Fuzzer(1317467515);

            var words = fuzzer.GenerateWords(3);

            Check.That(words).ContainsExactly("sit", "temporibus", "mollitia");
        }

        [Test]
        public void Generate_5_words_by_default()
        {
            var fuzzer = new Fuzzer();

            var words = fuzzer.GenerateWords();

            Check.That(words).HasSize(5);
        }

        [Test]
        public void Generate_a_sentence()
        {
            var fuzzer = new Fuzzer(1981116596);

            var sentence = fuzzer.GenerateSentence(nbOfWords: 4);

            Check.That(sentence).IsEqualTo("Quo cumque odit rerum.");
        }

        [Test]
        public void Throw_when_generating_a_sentence_with_1_word()
        {
            var fuzzer = new Fuzzer();

            Check.ThatCode(() =>
            {
                var sentence = fuzzer.GenerateSentence(nbOfWords: 1);
            }).Throws<ArgumentOutOfRangeException>()
                .WithMessage("A sentence must have more than 1 word. (Parameter 'nbOfWords')");
        }

        [Test]
        public void Generate_a_paragraph()
        {
            var fuzzer = new Fuzzer(1811180938);

            var sentence = fuzzer.GenerateParagraph(nbOfSentences: 5);

            Check.That(sentence).IsEqualTo("Pariatur aut repellendus pariatur. Quisquam quo deleniti consectetur earum mollitia ut quae. Dolor qui qui esse. Et nulla quia iusto omnis. Molestias quia qui consequatur sunt est doloremque iure.");
        }

        [Test]
        public void GenerateParagraphs()
        {
            var fuzzer = new Fuzzer(1811180938);

            var paragraphs = fuzzer.GenerateParagraphs(nbOfParagraphs: 3);

            Check.That(paragraphs).HasSize(3);
        }

        [Test]
        public void Generate_a_text()
        {
            var fuzzer = new Fuzzer(1811180938);

            var sentence = fuzzer.GenerateText(nbOfParagraphs: 3);

            Check.That(sentence).IsEqualTo(@"Pariatur aut repellendus pariatur. Quisquam quo deleniti consectetur earum mollitia ut quae. Dolor qui qui esse. Et nulla quia iusto omnis. Molestias quia qui consequatur sunt est doloremque iure.

Consequatur consequatur est odio ducimus voluptatem quas. Ut ea culpa sint culpa. Nobis. Eaque delectus vitae explicabo totam cum in. Quia.

In voluptatibus rem. Suscipit similique pariatur iusto rerum velit aspernatur. Delectus odit ea. Voluptates beatae sit consequatur molestiae. Quae sit sed.");
        }
    }
}