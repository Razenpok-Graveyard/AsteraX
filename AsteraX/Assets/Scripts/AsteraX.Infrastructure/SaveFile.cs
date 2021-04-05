using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Razensoft.Functional;
using UnityEngine;

namespace AsteraX.Infrastructure
{
    public class SaveFile
    {
        public static readonly string DefaultSaveFilePath = UnityEngine.Application.persistentDataPath + "/AsteraX.save";

        private readonly string _saveFilePath;

        public SaveFile() : this(DefaultSaveFilePath) { }

        public SaveFile(string saveFilePath)
        {
            _saveFilePath = saveFilePath;
        }

        public SaveFileContents GetContents()
        {
            return ReadContents()
                .OnFailure(SaveNewContents)
                .OnFailureCompensate(ReadContents)
                .Value;
        }

        private Result<SaveFileContents> ReadContents()
        {
            return Result.Try(() => File.ReadAllText(_saveFilePath))
                .OnSuccessTry(Convert.FromBase64String)
                .OnSuccessTry(Encoding.UTF8.GetString)
                .OnSuccessTry(JsonUtility.FromJson<SaveFileContents>);
        }

        public void SaveContents(SaveFileContents saveFileContents)
        {
            var json = JsonUtility.ToJson(saveFileContents);
            var bytes = Encoding.UTF8.GetBytes(json);
            var base64 = Convert.ToBase64String(bytes);
            File.WriteAllText(_saveFilePath, base64);
        }

        private void SaveNewContents()
        {
            SaveContents(new SaveFileContents());
        }
    }

    [Serializable]
    public class SaveFileContents
    {
        public int HighScore;
        public List<long> Achievements = new List<long>();
        public List<AchievementProgress> AchievementProgress = new List<AchievementProgress>();
    }

    [Serializable]
    public class AchievementProgress
    {
        public long Id;
        public int Progress;
    }
}