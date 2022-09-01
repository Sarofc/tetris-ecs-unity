

using Cysharp.Threading.Tasks.CompilerServices;
using Cysharp.Threading.Tasks;
using System.Runtime.CompilerServices;
using UnityEngine;

public partial class RefTypes 
{
    void RefAsyncMethod_UniTask()
    {
        IAsyncStateMachine stateMachine = new MyStateMachine();

        UniTask.Awaiter aw0 = default;
        var c0 = new AsyncUniTaskMethodBuilder();
        c0.Start(ref stateMachine);
        c0.AwaitUnsafeOnCompleted(ref aw0, ref stateMachine);
        c0.SetException(null);
        c0.SetResult();

        var c1 = new AsyncUniTaskMethodBuilder();
        c1.Start(ref stateMachine);
        c1.AwaitUnsafeOnCompleted(ref aw0, ref stateMachine);
        c1.SetException(null);
        c1.SetResult();

        UniTask<bool>.Awaiter aw2 = default;
        var c2 = new AsyncUniTaskMethodBuilder<bool>();
        c2.Start(ref stateMachine);
        c2.AwaitUnsafeOnCompleted(ref aw2, ref stateMachine);
        c2.SetException(null);
        c2.SetResult(default);

        UniTask<int>.Awaiter aw3 = default;
        var c3 = new AsyncUniTaskMethodBuilder<int>();
        c3.Start(ref stateMachine);
        c3.AwaitUnsafeOnCompleted(ref aw3, ref stateMachine);
        c3.SetException(null);
        c3.SetResult(default);

        UniTask<long>.Awaiter aw4 = default;
        var c4 = new AsyncUniTaskMethodBuilder<long>();
        c4.Start(ref stateMachine);
        c4.AwaitUnsafeOnCompleted(ref aw4, ref stateMachine);
        c4.SetException(null);

        UniTask<float>.Awaiter aw5 = default;
        var c5 = new AsyncUniTaskMethodBuilder<float>();
        c5.Start(ref stateMachine);
        c5.AwaitUnsafeOnCompleted(ref aw5, ref stateMachine);
        c5.SetException(null);
        c5.SetResult(default);

        UniTask<double>.Awaiter aw6 = default;
        var c6 = new AsyncUniTaskMethodBuilder<double>();
        c6.Start(ref stateMachine);
        c6.AwaitUnsafeOnCompleted(ref aw6, ref stateMachine);
        c6.SetException(null);
        c6.SetResult(default);

        UniTask<object>.Awaiter aw7 = default;
        var c7 = new AsyncUniTaskMethodBuilder<object>();
        //var c7 = AsyncUniTaskMethodBuilder<object>.Create();
        c7.Start(ref stateMachine);
        c7.AwaitUnsafeOnCompleted(ref aw7, ref stateMachine);
        c7.SetException(null);
        c7.SetResult(default);

        UniTask<IntEnum>.Awaiter aw8 = default;
        var c8 = new AsyncUniTaskMethodBuilder<IntEnum>();
        c8.Start(ref stateMachine);
        c8.AwaitUnsafeOnCompleted(ref aw8, ref stateMachine);
        c8.SetException(null);
        c8.SetResult(default);

        var c9 = new AsyncUniTaskMethodBuilder();
        var b = AsyncUniTaskMethodBuilder.Create();
        c9.Start(ref stateMachine);
        c9.AwaitUnsafeOnCompleted(ref aw0, ref stateMachine);
        c9.SetException(null);
        c9.SetResult();
        Debug.Log(b);

        //UniTask<Saro.UI.IWindow>.Awaiter aw10 = default;
        //var c10 = new AsyncUniTaskMethodBuilder<Saro.UI.IWindow>();
        //c10.Start(ref stateMachine);
        //c10.AwaitUnsafeOnCompleted(ref aw10, ref stateMachine);
        //c10.SetException(null);
        //c10.SetResult(default);

        var builder = new AsyncUniTaskMethodBuilder();
        IAsyncStateMachine asm = default;
        builder.Start(ref asm);
    }

}
