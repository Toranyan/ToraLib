using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace tora.save
{
    [Serializable]
    public class SaveFile
    {
        public int Version;
        public List<SaveBlock> Blocks = new List<SaveBlock>();
    }

    [Serializable]
    public class SaveBlock
    {
        public string Key;
        public string Json;
    }

    public interface ISaveParticipant
    {
        string SaveKey { get; }
        string CaptureState();
        void RestoreState(string json);
    }

    public static class SaveSystem
    {
        private const int CurrentVersion = 1;

        private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

        private static readonly List<ISaveParticipant> _participants = new List<ISaveParticipant>();

        public static void Register(ISaveParticipant participant)
        {
            if (!_participants.Contains(participant))
            {
                _participants.Add(participant);
            }
        }

        public static void Unregister(ISaveParticipant participant)
        {
            _participants.Remove(participant);
        }

        public static void Save()
        {
            var file = new SaveFile { Version = CurrentVersion };

            foreach (var participant in _participants)
            {
                file.Blocks.Add(new SaveBlock
                {
                    Key = participant.SaveKey,
                    Json = participant.CaptureState()
                });
            }

            File.WriteAllText(SavePath, JsonUtility.ToJson(file));
        }

        public static void Load()
        {
            if (!HasSave())
            {
                return;
            }

            var file = JsonUtility.FromJson<SaveFile>(File.ReadAllText(SavePath));

            var blocksByKey = new Dictionary<string, string>();
            foreach (var block in file.Blocks)
            {
                blocksByKey[block.Key] = block.Json;
            }

            foreach (var participant in _participants)
            {
                blocksByKey.TryGetValue(participant.SaveKey, out var json);
                participant.RestoreState(json);
            }
        }

        public static bool HasSave()
        {
            return File.Exists(SavePath);
        }

        public static void DeleteSave()
        {
            if (HasSave())
            {
                File.Delete(SavePath);
            }
        }
    }
}
