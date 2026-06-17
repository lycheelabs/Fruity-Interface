using Newtonsoft.Json;
using UnityEngine;

namespace LycheeLabs.FruityInterface.Tests {

    public static class NewtonsoftCompilationTest {

        [RuntimeInitializeOnLoadMethod]
        private static void TestSerialization () {
            var obj = new { name = "test", value = 42 };
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            var back = JsonConvert.DeserializeAnonymousType(json, obj);

            Debug.Log($"[Newtonsoft] Compilation OK, round-trip: name={back.name} value={back.value}");
        }

    }

}
