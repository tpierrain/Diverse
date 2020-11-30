
using System.Collections.Generic;

namespace Diverse
{
    public static class Adjectives
    {
        /// <summary>
        /// Gets a dictionary of all the adjective per <see cref="Feeling"/>.
        /// </summary>
        public static IDictionary<Feeling, string[]> PerFeeling { get; } = new Dictionary<Feeling, string[]>
        {
            {
                Feeling.Positive,
                new[]
                {
                    "admiring", "adoring", "affectionate", "amazing", "awesome", "beautiful", "blissful", "bold", "brave","charming",
                    "clever", "cool", "compassionate", "competent", "confident", "determined", "dreamy", "eager", "ecstatic", "elastic",
                    "elated", "elegant", "eloquent", "epic", "exciting", "fervent", "festive", "flamboyant", "focused", "friendly",
                    "funny", "gallant", "gifted", "goofy", "gracious", "great", "happy", "hopeful", "infallible", "inspiring", "interesting", 
                    "intelligent", "jolly", "jovial", "keen", "kind", "laughing", "loving", "lucid", "magical", "mystifying", "modest", "musing",
                    "nice", "nifty", "objective", "optimistic", "peaceful", "pensive", "practical", "priceless", "relaxed", "reverent", "romantic",
                    "serene", "sharp", "sweet", "tender", "vibrant", "vigilant", "vigorous", "wizardly", "wonderful", "xenodochial", "youthful","zen"
                }
            },
            {
                Feeling.Negative,
                new[]
                {
                    "agitated", "angry", "boring", "busy", "condescending", "cranky", "crazy", "dazzling", "distracted", "frosty", "hardcore",
                    "hungry", "naughty", "nervous", "nostalgic", "pedantic", "quirky", "quizzical", "sad", "silly", "sleepy", "stoic", "strange", 
                    "stupefied", "suspicious", "thirsty", "trusting", "unruffled", "upbeat", "zealous"
                }
            }
        };
    }
}