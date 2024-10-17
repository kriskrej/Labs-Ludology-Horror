using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Globalization;

public static class Extensions {

    public static int WordCount(this String str) {
        return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    public static Vector3 SetX(this Vector3 v, float x) {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 SetY(this Vector3 v, float y) {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 SetZ(this Vector3 v, float z) {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 SetX(ref Vector3 v, float x) {
        v = new Vector3(x, v.y, v.z);
        return v;
    }

    public static Vector3 SetY(ref Vector3 v, float y) {
        v = new Vector3(v.x, y, v.z);
        return v;
    }

    public static Vector3 SetZ(ref Vector3 v, float z) {
        v = new Vector3(v.x, v.y, z);
        return v;
    }

    /// <summary>
    /// Changes transform's parent to new one, preserving objects scale
    /// </summary>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    public static void ReparentTo(this Transform child, Transform parent) {
        var oldscale = child.localScale;
        child.parent = parent;
        child.localScale = oldscale;
    }

    /// <summary>
    /// Returns list of transforms children. It is safe do Destroy objects from this list
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static Transform[] GetChildren(this Transform parent) {
        var kids = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++) {
            kids[i] = parent.GetChild(i);
        }
        return kids;
    }

    public static void DestroyChildren(this Transform parent) {
        foreach (var t in parent.GetChildren()) {
            Component.Destroy(t.gameObject);
        }
    }

    public static IEnumerable<Transform> GetFamily(this Transform parent) {
        yield return parent;
        foreach (Transform t in parent) {
            foreach (Transform t1 in t.GetFamily()) {
                yield return t1;
            }
        }
    }

    public static T GetComponentInAncestors<T>(this Transform t) where T : Component {
        var p = t.parent;
        while (p != null) {
            T c = p.GetComponent<T>();
            if (c) {
                return c;
            }
            p = p.parent;
        }
        return null;
    }

    public static Dictionary<T1, T2> AddRange<T1, T2>(this Dictionary<T1, T2> dict, T1[] keys, T2[] values) {
        if (keys.Length != values.Length) {
            throw new ArgumentException("keys and values must have same length");
        }
        for (int i = 0; i < keys.Length; i++) {
            dict.Add(keys[i], values[i]);
        }
        return dict;
    }

    public static T2 GetValueOrNull<T1, T2>(this Dictionary<T1, T2> dict, T1 key) where T2 : class {
        T2 ret;
        dict.TryGetValue(key, out ret);
        return ret;

    }

    public static bool HasIndex<T>(this List<T> list, int index) {
        return index >= 0 && index < list.Count;
    }

    public static T GetOrDefault<T>(this List<T> list, int index, T defaultValue) {
        return list.HasIndex(index) ? list[index] : defaultValue;
    }

    public static void MoveFloatTowards(this Animator target, string name, float to, float step) {
        var old = target.GetFloat(name);
        var val = Mathf.MoveTowards(old, to, step);
        target.SetFloat(name, val);
    }

    public static T PickRandom<T>(this List<T> list) {
        if (list.Count == 0) {
            throw new ArgumentException("List cannot be empty");
        }
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T PickRandom<T>(this IEnumerable<T> enumerable) {
        if (!enumerable.Any()) {
            throw new ArgumentException("Enumerable cannot be empty");
        }
        return enumerable.ElementAt(UnityEngine.Random.Range(0, enumerable.Count()));
    }

    public static string[] Split(this string s, string delimiter) {
        return s.Split(new string[] { delimiter }, System.StringSplitOptions.None);
    }

    public static bool IsVisibleFrom(this Renderer renderer, Camera camera) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    public static string GetScenePath(this Transform current) {
        if (current.parent == null) {
            return "/" + current.name;
        }
        return current.parent.GetScenePath() + "/" + current.name;
    }

    public static bool NotEmpty(this string str) {
        return !string.IsNullOrEmpty(str);
    }

    public static string Format(this string str, params object[] args) {
        return string.Format(str, args);
    }

    public static bool ContainsIgnoreCase(this string source, string toCheck) {
        var sourceLowerCase = source.ToLower();
        var toCheckLowerCase = toCheck.ToLower();
        return sourceLowerCase.Contains(toCheckLowerCase);
    }

    public static RectTransform GetRectTransform(this Component c) {
        return c.transform as RectTransform;
    }


    /// <summary>
    /// get rect transform, and then get it's rect, and then transform it to world space
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static Rect GetWorldRect(this Component c) {
        var rect = c.GetRectTransform().rect;
        var pos = c.transform.TransformPoint(rect.position);
        var size = c.transform.TransformVector(rect.size);
        return new Rect(pos, size);
    }

    public static T GetComponentInParentEvenHidden<T>(this GameObject gameObject) where T : UnityEngine.Object {
        Transform parent = gameObject.transform;
        T componet;
        while (true) {
            if (parent == null)
                break;
            componet = parent.GetComponent<T>();
            if (componet != null)
                return componet;
            else
                parent = parent.parent;
        }
        return null;
    }

    /// <summary>
    ///     Instantiate an object and add it to the specified parent.
    /// </summary>
    public static GameObject AddChild(this GameObject parent, GameObject prefab) {
        var go = GameObject.Instantiate(prefab);
        //#if UNITY_EDITOR
        //        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
        //#endif
        if (go != null && parent != null) {
            var t = go.transform;
            t.SetParent(parent.transform, false);
            go.layer = parent.layer;
        }
        return go;
    }

    public static T AddChild<T>(this GameObject parent, GameObject prefab) where T : Component {
        var gameObject = AddChild(parent, prefab);
        return gameObject.GetComponent<T>();
    }

    public static T AddChild<T>(this GameObject parent) where T : Component {
        var go = new GameObject("game object", typeof(T));
        if (go != null && parent != null) {
            var t = go.transform;
            t.SetParent(parent.transform, false);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            go.layer = parent.layer;
        }
        return go.GetComponent<T>();
    }

    public static Transform GetOrMakeChild(this Transform p, string desiredChildName) {
        var ch = p.Find(desiredChildName);
        if (!ch) {
            ch = new GameObject(desiredChildName).transform;
            ch.SetParent(p, false);
        }
        return ch;
    }

    public static Transform GetOrMakeChild(this Transform t, string desiredChildName, int childIndex) {
        var ch = t.GetOrMakeChild(desiredChildName);
        ch.SetSiblingIndex(childIndex);
        return ch;
    }

    public static void EnsureChildDoesntExistIfEmpty(this Transform t, string undesiredChildName) {
        var ch = t.Find(undesiredChildName);
        if (ch && ch.childCount == 0) {
            if (Application.isPlaying)
                GameObject.Destroy(ch.gameObject);
            else
                GameObject.DestroyImmediate(ch.gameObject);
        }
    }

    public static Transform GetRootAncestor(this Transform tr) {
        return tr.parent ? GetRootAncestor(tr.parent) : tr;
    }

    public static bool IsNull(this System.Object systemObject) {
        return systemObject == null || systemObject.Equals(null);
    }

    public static bool IsNotNull(this System.Object systemObject) {
        return systemObject.IsNull() == false;
    }




    public static void SetLayerRecursively(this GameObject obj, int layer) {
        obj.layer = layer;
        foreach (Transform child in obj.transform) {
            if (!child.gameObject.layer.Equals(LayerMask.NameToLayer("Tasks")))
                child.gameObject.SetLayerRecursively(layer);
        }
    }

    // V: commenting out because this is not used and I feel I might want to consult this without digging in git history
    //
    //public static bool CanExecute<T>(this Action<T> action) {
    //    if (action.IsNull())
    //        return false;

    //    var methodReflectedType = action.Method.ReflectedType;
    //    var isMethodReflectedTypeStatic = methodReflectedType.IsStatic(); //IsStatic<T>(methodReflectedType);
    //    if (action.Target == null && !isMethodReflectedTypeStatic)
    //        Debug.LogError("Null target and method is not static: " + methodReflectedType
    //                       + " - make that method static or think about not deleting object");
    //    return isMethodReflectedTypeStatic || action.Target.IsNotNull();
    //}

    public static bool IsStatic(this Type method) {
        return method.GetProperties(BindingFlags.Static) != null;
    }
}

public static class ActionExtensions {
    public static void PerformAfterCoroutine<T>(this Action action, int frames = 1) where T : YieldInstruction, new() {
        CoroutineBehaviour.StartCoroutine<T>(action, frames);
    }
}

public static class MonoBehaviourExtensions {
    /// <summary>
    /// Performs an Action after a YieldInstruction. 
    /// </summary>
    public static void StartCoroutine<T>(this MonoBehaviour monoBehaviour, Action action, int frames = 1) where T : YieldInstruction, new() {
        monoBehaviour.StartCoroutine(Coroutine<T>(action, frames));
    }

    static IEnumerator Coroutine<T>(Action action, int frames = 1) where T : YieldInstruction, new() {
        for (int i = 0; i < frames; i++) {
            yield return new T();
        }
        action();
    }
}

public class CoroutineBehaviour : MonoBehaviour {
    static MonoBehaviour Instance = new GameObject { hideFlags = HideFlags.HideAndDontSave }.AddComponent<CoroutineBehaviour>();

    public static void StartCoroutine<T>(Action action, int frames) where T : YieldInstruction, new() {
        Instance.StartCoroutine<T>(action,frames);
    }
}