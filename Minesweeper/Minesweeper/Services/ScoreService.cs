using Minesweeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace Minesweeper.Services
{
    public class ScoreService
    {
        private readonly string _fileName = "highscore.json";

        public void Save(GameScore score)
        {
            string json = JsonSerializer.Serialize(score);
            File.WriteAllText(_fileName, json);
        }

        public GameScore Load()
        {
            if (!File.Exists(_fileName)) return null;
            string json = File.ReadAllText(_fileName);
            return JsonSerializer.Deserialize<GameScore>(json);
        }
    }
}
