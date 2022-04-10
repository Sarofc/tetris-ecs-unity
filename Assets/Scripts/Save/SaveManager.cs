using Saro;
using Saro.SaveSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris.Save
{
    public sealed class SaveManager : IService
    {
        public static SaveManager Current => Main.Resolve<SaveManager>();

        private ISaveFile m_SaveFile;
        private string m_SaveFilePath = Application.persistentDataPath + "/" + k_FileName;
        private const string k_FileName = "tetris.conf";

        public T GetSaveData<T>() where T : class, ISaveData
        {
            var type = typeof(T);
            return GetSaveData(type) as T;
        }

        public ISaveData GetSaveData(Type type)
        {
            for (int i = 0; i < m_SaveFile.SaveDatas.Count; i++)
            {
                var saveData = m_SaveFile.SaveDatas[i];
                if (saveData.GetType() == type)
                {
                    return saveData;
                }
            }

            return default;
        }

        public void Save()
        {
            m_SaveFile.Save();
        }

        public void Load()
        {
            if (!m_SaveFile.HasSaveFile())
            {
                var saveDatas = new List<ISaveData>
                {
                    new GameSettings(),
                };

                foreach (var item in saveDatas)
                {
                    m_SaveFile.AddSaveData(item);
                }

                Log.INFO("Save", "null save file. init");
            }
            else
            {
                m_SaveFile.Load();
                Log.INFO("Save", "load save: " + m_SaveFile.FilePath);
            }
        }

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
    }
}
