using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Saro.Table;

namespace Saro.Localization
{
    internal class LocalizationDataProviderExcel : ILocalizationDataProvider
    {
        public async UniTask<bool> LoadAsync(ELanguage language, Dictionary<int, string> map)
        {
            switch (language)
            {
                case ELanguage.None:
                    break;
                case ELanguage.ZH:
                {
                    var csv = CsvLanguageZh.Get();
                    var result = await csv.LoadAsync();
                    if (!result) return false;
                    foreach (var item in csv.GetTable())
                    {
                        var val = item.Value;
                        map.Add(val.key, val.txt);
                    }
                    csv.Unload();
                }
                    break;
                case ELanguage.EN:
                {
                    var csv = CsvLanguageEn.Get();
                    var result = await csv.LoadAsync();
                    if (!result) return false;
                    foreach (var item in csv.GetTable())
                    {
                        var val = item.Value;
                        map.Add(val.key, val.txt);
                    }
                    csv.Unload();
                }
                    break;
                default:
                    throw new NotImplementedException($"{language} is not impl");
            }

            return true;
        }

        void ILocalizationDataProvider.Load(ELanguage language, Dictionary<int, string> map)
        {
            switch (language)
            {
                case ELanguage.None:
                    break;
                case ELanguage.ZH:
                {
                    var csv = CsvLanguageZh.Get();
                    csv.Load();
                    foreach (var item in csv.GetTable())
                    {
                        var val = item.Value;
                        map.Add(val.key, val.txt);
                    }
                    csv.Unload();
                }
                    break;
                case ELanguage.EN:
                {
                    var csv = CsvLanguageEn.Get();
                    csv.Load();
                    foreach (var item in csv.GetTable())
                    {
                        var val = item.Value;
                        map.Add(val.key, val.txt);
                    }
                    csv.Unload();
                }
                    break;
                default:
                    throw new NotImplementedException($"{language} is not impl");
            }
        }
    }
}