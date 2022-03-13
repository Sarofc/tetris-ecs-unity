using Saro.Table;
using System;
using System.Collections.Generic;

namespace Saro.Localization
{
    internal class LocalizationDataProvider_Excel : ILocalizationDataProvider
    {
        void ILocalizationDataProvider.Load(ELanguage language, Dictionary<int, string> map)
        {
            switch (language)
            {
                case ELanguage.None:
                    break;
                case ELanguage.ZH:
                    {
                        var csv = csvLanguage_ZH.Get();
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
                        var csv = csvLanguage_EN.Get();
                        csv.Load();
                        foreach (var item in csv.GetTable())
                        {
                            var val = item.Value;
                            map.Add(val.key, val.txt);
                        }
                        csv.Unload();
                    }
                    break;
                case ELanguage.JA:
                    break;
                default:
                    throw new NotImplementedException($"{language} is not impl");
            }
        }
    }
}
