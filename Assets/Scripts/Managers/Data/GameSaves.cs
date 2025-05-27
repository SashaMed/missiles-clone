using System;
using System.Collections.Generic;
using System.Linq;
using GameData.SavesData;
using SavesData;
using UnityEngine;

namespace GameData
{
    public class GameSaves : DataSaves<GameSavesData>
    {
        public static void LoadAndCheck()
        {
            Load();
            CheckToVersionTransit();

            //Data.Coins ??= new();
            //Data.Progress ??= new();
        }

        private static void CheckToVersionTransit()
        {
            if ((int)Data.version < (int)GameSavesData.CurrentVersion)
            {
                // < UPDATE DATA LOGIC HERE >

                Data.version = GameSavesData.CurrentVersion;
            }
        }

        public static void ChangeAndSave(Action<GameSavesData> action)
        {
            action?.Invoke(Data);

            Save();
        }

    }

    namespace SavesData
    {
        public enum SavesVersion
        {
            initial,
        }

        [Serializable]
        public class GameSavesData
        {
            public const SavesVersion CurrentVersion = SavesVersion.initial;
            public SavesVersion version = CurrentVersion;

            public long TimestampBase => timestamp;

            private long timestamp = 0;

            public GameSavesData()
            {
                timestamp = DateTime.Now.Ticks;
            }

        }

    }
}
