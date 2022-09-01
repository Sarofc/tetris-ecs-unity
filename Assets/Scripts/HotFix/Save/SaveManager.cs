using System;
using System.Collections.Generic;
using Saro;
using Saro.SaveSystem;
using UnityEngine;

namespace Tetris.Save
{
    public sealed class SaveManager : IService
    {
        private const string k_FileName = "tetris.conf";

        private ISaveFile m_SaveFile;
        private readonly string m_SaveFilePath = Application.persistentDataPath + "/" + k_FileName;
        public static SaveManager Current => Main.Resolve<SaveManager>();

        void IService.Awake()
        {
            m_SaveFile = new DefaultSaveFile(m_SaveFilePath, new JsonSaveDataProvider());
        }

        void IService.Update()
        {
        }

        void IService.Dispose()
        {
        }

        public T GetSaveData<T>() where T : class, ISaveData
        {
            var type = typeof(T);
            return GetSaveData(type) as T;
        }

        public ISaveData GetSaveData(Type type)
        {
            for (var i = 0; i < m_SaveFile.SaveDatas.Count; i++)
            {
                var saveData = m_SaveFile.SaveDatas[i];
                if (saveData.GetType() == type) return saveData;
            }

            return default;
        }

        public void Save()
        {
            Log.INFO("Save", "SaveManager::Save");

            m_SaveFile.Save();
        }

        public void Load()
        {
            if (!m_SaveFile.HasSaveFile())
            {
                var saveDatas = new List<ISaveData>
                {
                    new GameSettings()
                };

                foreach (var item in saveDatas) m_SaveFile.AddSaveData(item);

                Log.INFO("Save", "null save file. init");
            }
            else
            {
                m_SaveFile.Load();
                Log.INFO("Save", "load save: " + m_SaveFile.FilePath);
            }
        }
    }
}