using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Razensoft.Functional;
using UnityEngine;

namespace AsteraX.Infrastructure
{
    public class SaveFileProvider
    {
        public static readonly string DefaultSaveFilePath = Application.persistentDataPath + "/AsteraX.save";

        private readonly string _saveFilePath;

        public SaveFileProvider() : this(DefaultSaveFilePath) { }

        public SaveFileProvider(string saveFilePath)
        {
            _saveFilePath = saveFilePath;
        }

        public SaveFile GetSaveFile()
        {
            return ReadSaveFile()
                .OnFailureCompensate(() => RegenerateSaveFile())
                .Value;
        }

        private Result<SaveFile> ReadSaveFile()
        {
            return Result.Try(() => File.ReadAllText(_saveFilePath))
                .OnSuccessTry(Convert.FromBase64String)
                .OnSuccessTry(Encoding.UTF8.GetString)
                .OnSuccessTry(JsonUtility.FromJson<SaveFile>);
        }

        public void Save(SaveFile saveFile)
        {
            var json = JsonUtility.ToJson(saveFile);
            var bytes = Encoding.UTF8.GetBytes(json);
            var base64 = Convert.ToBase64String(bytes);
            File.WriteAllText(_saveFilePath, base64);
        }

        private SaveFile RegenerateSaveFile()
        {
            var newSaveFile = new SaveFile();
            Save(newSaveFile);
            return newSaveFile;
        }
    }

    [Serializable]
    public class SaveFile
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