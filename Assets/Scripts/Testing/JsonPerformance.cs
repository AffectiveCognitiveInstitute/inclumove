// <copyright file=JsonPerformance.cs/>
// <copyright>
//   Copyright (c) 2018, Affective & Cognitive Institute
//   
//   Permission is hereby granted, free of charge, to any person obtaining a copy of this software andassociated documentation files
//   (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
//   merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
//   furnished to do so, subject to the following conditions:
//   
//   The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//   
//   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//   OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//   LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR
//   IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
// </copyright>
// <license>MIT License</license>
// <main contributors>
//   Moritz Umfahrer
// </main contributors>
// <co-contributors/>
// <patent information/>
// <date>07/12/2018 05:59</date>

#if UNITY_EDITOR

using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class EmptyClass
{
}

[Serializable]
public class StringOnly
{
    public string String;
}

[Serializable]
public class ArrayOnly
{
    public int[] IntArray;
}

[Serializable]
public class A
{
    public B B;
}

[Serializable]
public class B
{
    public C C;
}

[Serializable]
public class C
{
    public D D;
}

[Serializable]
public class D
{
    public E E;
}

[Serializable]
public class E
{
    public F F;
}

[Serializable]
public class F
{
    public G G;
}

[Serializable]
public class G
{
    public H H;
}

[Serializable]
public class H
{
    public I I;
}

[Serializable]
public class I
{
    public int Val;
}

internal class JsonPerformance : MonoBehaviour
{
    private string report = "";

    private void Start()
    {
        EmptyClass emptyClass = new EmptyClass();
        StringOnly smallString = new StringOnly {String = "hey"};
        StringOnly largeString = new StringOnly {String = new string('*', 100000)};
        ArrayOnly smallArray = new ArrayOnly {IntArray = new[] {1, 2, 3}};
        ArrayOnly largeArray = new ArrayOnly {IntArray = new int[1000]};
        A shallowNestedObject = new A {B = new B {C = new C()}};
        A deepNestedObject = new A
        {
            B = new B
            {
                C = new C
                {
                    D = new D
                    {
                        E = new E
                        {
                            F = new F
                            {
                                G = new G
                                {
                                    H = new H
                                    {
                                        I = new I {Val = 123}
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        Test("Empty Class", emptyClass, 100000);
        Test("Small String", smallString, 10000);
        Test("Large String", largeString, 100);
        Test("Small Array", smallArray, 100000);
        Test("Large Array", largeArray, 100);
        Test("Shallow Nested Object", shallowNestedObject, 10000);
        Test("Deep Nested Object", deepNestedObject, 10000);
    }

    private void Test<T>(string label, T obj, int reps)
        where T : class
    {
        // Warm up reflection
        JsonUtility.ToJson(obj);
        JsonConvert.SerializeObject(obj);

        Stopwatch stopwatch = new Stopwatch();
        string unityJson = null;
        string jsonDotNetJson = null;
        T unityObj = null;
        T jsonDotNetObj = null;

        stopwatch.Start();
        for (int i = 0; i < reps; ++i) unityJson = JsonUtility.ToJson(obj);
        long unitySerializeTime = stopwatch.ElapsedMilliseconds;

        stopwatch.Reset();

        stopwatch.Start();
        for (int i = 0; i < reps; ++i) jsonDotNetJson = JsonConvert.SerializeObject(obj);
        long jsonDotNetSerializeTime = stopwatch.ElapsedMilliseconds;

        stopwatch.Reset();
        stopwatch.Start();
        for (int i = 0; i < reps; ++i) unityObj = JsonUtility.FromJson<T>(unityJson);
        long unityDeserializeTime = stopwatch.ElapsedMilliseconds;

        stopwatch.Reset();

        stopwatch.Start();
        for (int i = 0; i < reps; ++i) jsonDotNetObj = JsonConvert.DeserializeObject<T>(jsonDotNetJson);
        long jsonDotNetDeserializeTime = stopwatch.ElapsedMilliseconds;

        int unitySize = Encoding.UTF8.GetBytes(unityJson).Length;
        int jsonDotNetSize = Encoding.UTF8.GetBytes(jsonDotNetJson).Length;
        string msg = string.Format(
            "{0} ({1} reps)\n" +
            "Library,Size,SerializeTime,Deserialize Time\n" +
            "Unity,{2},{3},{4}\n" +
            "Json.NET,{5},{6},{7}\n\n",
            label, reps,
            unitySize, unitySerializeTime, unityDeserializeTime,
            jsonDotNetSize, jsonDotNetSerializeTime, jsonDotNetDeserializeTime
        );
        report += msg;
    }

    private void OnGUI()
    {
        GUI.TextArea(new Rect(0, 0, Screen.width, Screen.height), report);
    }
}

#endif