#define v1
//#define v1_patch

using System.Collections;
using UnityEngine;

namespace Tetris.Tests
{
    public class TestIFix : MonoBehaviour
    {
#if v1
        private void Start()
        {
            Foo();
        }

        private void Foo()
        {
            Debug.LogError("foo");
        }
#endif

#if v1_patch
        [IFix.Patch]
        private void Start()
        {
            Foo();

            Bar();
        }

        [IFix.Patch]
        private void Foo()
        {
            Debug.LogError("fix foo");
        }

        [IFix.Interpret]
        private void Bar()
        {
            Debug.LogError("interpret bar");
        }
#endif
    }
}