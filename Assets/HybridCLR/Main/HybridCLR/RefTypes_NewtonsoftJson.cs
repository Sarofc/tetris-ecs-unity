
using Newtonsoft.Json.Utilities;

public partial class RefTypes
{
    void RefNewtonsoftJson()
    {
        AotHelper.EnsureList<int>();
        AotHelper.EnsureList<long>();
        AotHelper.EnsureList<float>();
        AotHelper.EnsureList<double>();
        AotHelper.EnsureList<string>();
        AotHelper.EnsureDictionary<int, int>();
        AotHelper.EnsureDictionary<int, string>();
    }
}
