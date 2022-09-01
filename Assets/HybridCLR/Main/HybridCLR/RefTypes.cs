
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

[assembly: Preserve]

enum IntEnum : int
{
    A,
    B,
}

public class MyComparer<T> : Comparer<T>
{
    public override int Compare(T x, T y)
    {
        return 0;
    }
}

class MyStateMachine : IAsyncStateMachine
{
    public void MoveNext()
    {
        throw new NotImplementedException();
    }

    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        throw new NotImplementedException();
    }
}

public partial class RefTypes : MonoBehaviour
{
    List<Type> GetTypes()
    {
        return new List<Type>
        {
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GetTypes());
        GameObject.Instantiate<GameObject>(null);
        Instantiate<GameObject>(null, null);
        Instantiate<GameObject>(null, null, false);
        Instantiate<GameObject>(null, new Vector3(), new Quaternion());
        Instantiate<GameObject>(null, new Vector3(), new Quaternion(), null);
    }

    void RefNumerics()
    {
        var a = new System.Numerics.BigInteger();
        a.ToString();
    }

    void RefDelegate()
    {
        // action
        { Action a = () => { }; }
        { Action<int> a = (s) => { }; }
        { Action<uint> a = (s) => { }; }
        { Action<byte> a = (s) => { }; }
        { Action<sbyte> a = (s) => { }; }
        { Action<long> a = (s) => { }; }
        { Action<float> a = (s) => { }; }
        { Action<double> a = (s) => { }; }

        { Action<object> a = (s) => { }; }
        { Action<object, object> a = (s1, s2) => { }; }
        { Action<object, object, object> a = (s1, s2, s3) => { }; }

        // func
        { Func<object> a = () => new object(); }
        { Func<object, object> a = (s) => new object(); }
        { Func<object, object, object> a = (s1, s2) => new object(); }
        { Func<object, object, object, object> a = (s1, s2, s3) => new object(); }
    }

    void RefComparers()
    {
        var a = new object[]
        {
            new MyComparer<int>(),
            new MyComparer<long>(),
            new MyComparer<float>(),
            new MyComparer<double>(),
            new MyComparer<object>(),
        };

        new MyComparer<int>().Compare(default, default);
        new MyComparer<long>().Compare(default, default);
        new MyComparer<float>().Compare(default, default);
        new MyComparer<double>().Compare(default, default);
        new MyComparer<object>().Compare(default, default);

        object b = EqualityComparer<int>.Default;
        b = EqualityComparer<long>.Default;
        b = EqualityComparer<float>.Default;
        b = EqualityComparer<double>.Default;
        b = EqualityComparer<object>.Default;
    }

    void RefNullable()
    {
        // nullable
        object b = null;
        int? a = 5;
        b = a;
        int d = (int?)b ?? 7;
        int e = (int)b;
        a = d;
        b = a;
        b = Enumerable.Range(0, 1).Reverse().Take(1).TakeWhile(x => true).Skip(1).All(x => true);
        b = new WaitForSeconds(1f);
        b = new WaitForSecondsRealtime(1f);
        b = new WaitForFixedUpdate();
        b = new WaitForEndOfFrame();
        b = new WaitWhile(() => true);
        b = new WaitUntil(() => true);
    }

    void RefContainer()
    {
        //int, long,float,double, IntEnum,object
        List<object> b = new List<object>()
        {

        };
    }

    void RefSpan()
    {
        { Span<decimal> s = stackalloc decimal[10]; }
        { Span<int> s = stackalloc int[10]; }
        { Span<uint> s = stackalloc uint[10]; }
        { Span<float> s = stackalloc float[10]; }
        { Span<double> s = stackalloc double[10]; }
        { Span<long> s = stackalloc long[10]; }
        { Span<ulong> s = stackalloc ulong[10]; }
        { Span<short> s = stackalloc short[10]; }
        { Span<ushort> s = stackalloc ushort[10]; }
        { Span<byte> s = stackalloc byte[10]; }
        { Span<sbyte> s = stackalloc sbyte[10]; }

        { Span<Vector3> s = stackalloc Vector3[10]; }
        { Span<Vector3Int> s = stackalloc Vector3Int[10]; }
        { Span<Vector2> s = stackalloc Vector2[10]; }
        { Span<Vector2Int> s = stackalloc Vector2Int[10]; }
        { Span<Matrix4x4> s = stackalloc Matrix4x4[10]; }
        { Span<Rect> s = stackalloc Rect[10]; }
    }

    void RefAsyncMethod_Task()
    {
        var stateMachine = new MyStateMachine();

        TaskAwaiter aw = default;
        var c0 = new AsyncTaskMethodBuilder();
        c0.Start(ref stateMachine);
        c0.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c0.SetException(null);
        c0.SetResult();

        var c1 = new AsyncTaskMethodBuilder();
        c1.Start(ref stateMachine);
        c1.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c1.SetException(null);
        c1.SetResult();

        var c2 = new AsyncTaskMethodBuilder<bool>();
        c2.Start(ref stateMachine);
        c2.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c2.SetException(null);
        c2.SetResult(default);

        var c3 = new AsyncTaskMethodBuilder<int>();
        c3.Start(ref stateMachine);
        c3.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c3.SetException(null);
        c3.SetResult(default);

        var c4 = new AsyncTaskMethodBuilder<long>();
        c4.Start(ref stateMachine);
        c4.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c4.SetException(null);

        var c5 = new AsyncTaskMethodBuilder<float>();
        c5.Start(ref stateMachine);
        c5.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c5.SetException(null);
        c5.SetResult(default);

        var c6 = new AsyncTaskMethodBuilder<double>();
        c6.Start(ref stateMachine);
        c6.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c6.SetException(null);
        c6.SetResult(default);

        var c7 = new AsyncTaskMethodBuilder<object>();
        c7.Start(ref stateMachine);
        c7.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c7.SetException(null);
        c7.SetResult(default);

        var c8 = new AsyncTaskMethodBuilder<IntEnum>();
        c8.Start(ref stateMachine);
        c8.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c8.SetException(null);
        c8.SetResult(default);

        var c9 = new AsyncVoidMethodBuilder();
        var b = AsyncVoidMethodBuilder.Create();
        c9.Start(ref stateMachine);
        c9.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c9.SetException(null);
        c9.SetResult();
        Debug.Log(b);
    }

    void RefAsyncMethod_ValueTask()
    {
        var stateMachine = new MyStateMachine();

        ValueTaskAwaiter aw = default;
        ValueTaskAwaiter<int> aw_int = default;
        ValueTaskAwaiter<float> aw_float = default;

        var c0 = new AsyncValueTaskMethodBuilder();
        c0.Start(ref stateMachine);
        c0.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c0.SetException(null);
        c0.SetResult();

        var c1 = new AsyncValueTaskMethodBuilder();
        c1.Start(ref stateMachine);
        c1.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c1.SetException(null);
        c1.SetResult();

        var c2 = new AsyncValueTaskMethodBuilder<bool>();
        c2.Start(ref stateMachine);
        c2.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c2.SetException(null);
        c2.SetResult(default);

        var c3 = new AsyncValueTaskMethodBuilder<int>();
        c3.Start(ref stateMachine);
        c3.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c3.SetException(null);
        c3.SetResult(default);

        var c4 = new AsyncValueTaskMethodBuilder<long>();
        c4.Start(ref stateMachine);
        c4.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c4.SetException(null);

        var c5 = new AsyncValueTaskMethodBuilder<float>();
        c5.Start(ref stateMachine);
        c5.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c5.SetException(null);
        c5.SetResult(default);

        var c6 = new AsyncValueTaskMethodBuilder<double>();
        c6.Start(ref stateMachine);
        c6.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c6.SetException(null);
        c6.SetResult(default);

        var c7 = new AsyncValueTaskMethodBuilder<object>();
        c7.Start(ref stateMachine);
        c7.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c7.SetException(null);
        c7.SetResult(default);

        var c8 = new AsyncValueTaskMethodBuilder<IntEnum>();
        c8.Start(ref stateMachine);
        c8.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c8.SetException(null);
        c8.SetResult(default);

        var c9 = new AsyncValueTaskMethodBuilder();
        var b = AsyncValueTaskMethodBuilder.Create();
        c9.Start(ref stateMachine);
        c9.AwaitUnsafeOnCompleted(ref aw, ref stateMachine);
        c9.SetException(null);
        c9.SetResult();
        Debug.Log(b);
    }

    class TestTable
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public static async void TestAsync3()
    {
        Debug.Log("async task 1");
        await Task.Delay(10);
        Debug.Log("async task 2");
    }

    public static int Main_1()
    {
        Debug.Log("hello,hybridclr");

        var task = Task.Run(async () =>
        {
            await TestAsync2();
        });

        task.Wait();

        Debug.Log("async task end");
        Debug.Log("async task end2");

        return 0;
    }

    public static async Task TestAsync2()
    {
        Debug.Log("async task 1");
        await Task.Delay(3000);
        Debug.Log("async task 2");
    }

    // Update is called once per frame
    void Update()
    {
        TestAsync();
    }

    public static int TestAsync()
    {
        var t0 = Task.Run(async () =>
        {
            await Task.Delay(10);
        });
        t0.Wait();
        var task = Task.Run(async () =>
        {
            await Task.Delay(10);
            return 100;
        });
        Debug.Log(task.Result);
        return 0;
    }
}
