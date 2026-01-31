using System;
using UnityEngine;

[CreateAssetMenu(menuName = "GJ/Mask Model Library")]
public class MaskModelLibrary : ScriptableObject
{
    [Serializable]
    public class Entry
    {
        public string code;
        public GameObject prefab;
    }

    [SerializeField] private Entry[] entries;

    public GameObject GetPrefab(string code)
    {
        if (string.IsNullOrWhiteSpace(code) || entries == null)
        {
            return null;
        }

        for (int i = 0; i < entries.Length; i++)
        {
            Entry entry = entries[i];
            if (entry == null || entry.prefab == null)
            {
                continue;
            }

            if (string.Equals(entry.code, code, StringComparison.OrdinalIgnoreCase))
            {
                return entry.prefab;
            }
        }

        return null;
    }
}
