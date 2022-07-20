using System;
using System.IO;
using BayatGames.SaveGameFree;
using UnityEngine;

namespace Overflow.Model.Save
{
    public static class SaveHandler
    {
        private const string Password = "Client_Setting";
        private const bool IsEncryption = true;

        public static void Save<T>(string saveKey, T saveData)
        {
            SaveGame.Encode = IsEncryption;
            SaveGame.Save(saveKey, saveData, Password);
        }

        public static T Load<T>(string saveKey)
        {
            return SaveGame.Load<T>(saveKey, IsEncryption, Password);
        }

        public static bool ExitSaveData(string saveKey)
        {
            return SaveGame.Exists(saveKey);
        }

        public static void DeleteSaveData()
        {
            var directoryInfo = new DirectoryInfo(Application.persistentDataPath);
            var saveFiles = directoryInfo.GetFiles();
            foreach (var file in saveFiles)
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}");
                }
            }
        }
    }
}