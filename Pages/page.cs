using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace ELPais.Pages
{
    public class commonMethod
    {
      

        public static Dictionary<string, int> CountRepeatedWords(string text)
        {
            // Split text into words, normalize to lowercase, and remove punctuation
            var words = text.Split(new[] { ' ', '\n', '\r', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(word => word.ToLower());

            // Count word occurrences
            var wordCounts = new Dictionary<string, int>();
            foreach (var word in words)
            {
                if (wordCounts.ContainsKey(word))
                {
                    wordCounts[word]++;
                }
                else
                {
                    wordCounts[word] = 1;
                }
            }

            // Filter and return only repeated words
            return wordCounts.Where(kvp => kvp.Value > 2).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

     

    }
}
