using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Monsterfall_01
{
    public class HighScores
    {
        private static string _fileName = "scores.xml"; // Since we don't give a path, this'll be saved in the "bin" folder

        public List<GameStats> Highscores { get; private set; }

        public List<GameStats> Scores { get; private set; }

        public HighScores()
          : this(new List<GameStats>())
        {

        }

        public HighScores(List<GameStats> scores)
        {
            Scores = scores;

            UpdateHighscores();
        }

        public void Add(GameStats score)
        {
            Scores.Add(score);

            Scores = Scores.OrderByDescending(c => c.score).ToList(); // Orders the list so that the higher scores are first

            UpdateHighscores();
        }

        public static HighScores Load()
        {
            // If there isn't a file to load - create a new instance of "ScoreManager"
            if (!File.Exists(_fileName))
                return new HighScores();

            // Otherwise we load the file

            using (var reader = new StreamReader(new FileStream(_fileName, FileMode.Open)))
            {
                var serilizer = new XmlSerializer(typeof(List<GameStats>));

                var stats = (List<GameStats>)serilizer.Deserialize(reader);

                return new HighScores(stats);
            }
        }

        //public void UpdateHighscores()
        //{
        //    Highscores = Scores.Take(5).ToList(); // Takes the first 5 elements
        //}

        public static void Save(HighScores scoreManager)
        {
            // Overrides the file if it alreadt exists
            using (var writer = new StreamWriter(new FileStream(_fileName, FileMode.Create)))
            {
                var serilizer = new XmlSerializer(typeof(List<GameStats>));

                serilizer.Serialize(writer, scoreManager.Scores);
            }
        }
    }
}
