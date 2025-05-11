using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NameList { public List<string> names; }

public static class NamePool
{
    private static Queue<string> pool;

    public static string GetUniqueName()
    {
        if (pool == null || pool.Count == 0) LoadAndShuffle();
        return pool.Dequeue();
    }

    private static void LoadAndShuffle()
    {
        TextAsset ta = Resources.Load<TextAsset>("planet_names");
        if (!ta) { pool = new Queue<string>(); pool.Enqueue("Unnamed"); return; }

        NameList nl = JsonUtility.FromJson<NameList>(ta.text);
        List<string> list = new List<string>(nl.names);

        // Fisher–Yates shuffle
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        pool = new Queue<string>(list);
    }
}
