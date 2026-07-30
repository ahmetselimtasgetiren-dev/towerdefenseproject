using System;
using System.IO;
using UnityEngine;

namespace TowerDefenseIncremental
{
    public static class SaveSystem
    {
        private static string FilePath => Path.Combine(
            Application.persistentDataPath,
            "tower-defense-save.json");

        public static MetaSaveData LoadMeta()
        {
            if (!File.Exists(FilePath))
                return new MetaSaveData();

            try
            {
                return JsonUtility.FromJson<MetaSaveData>(File.ReadAllText(FilePath)) ??
                       new MetaSaveData();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
                return new MetaSaveData();
            }
        }

        public static void SaveMeta(MetaSaveData data)
        {
            try
            {
                File.WriteAllText(FilePath, JsonUtility.ToJson(data, true));
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }

    [Serializable]
    public sealed class MetaSaveData
    {
        public int cores;
        public SkillNodeSaveData[] skillNodes = Array.Empty<SkillNodeSaveData>();
    }

    [Serializable]
    public sealed class SkillNodeSaveData
    {
        public string id;
        public int level;
    }
}
