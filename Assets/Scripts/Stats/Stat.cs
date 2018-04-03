using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

    [SerializeField]
    private int minValue;
    [SerializeField]
    private int maxValue;

    private List<int> modifiers = new List<int>();

    public void InitValue(int min, int max)
    {
        minValue = min;
        maxValue = max;
    }

    public int GetValue()
    {
        int totalValue = maxValue;
        modifiers.ForEach(x => totalValue += x);
        return Random.Range(minValue, totalValue);
    }

    public void AddModifier(int modifier)
    {
        if (modifier != 0)
            modifiers.Add(modifier);
    }

    public void RemoveModifier(int modifier)
    {
        if (modifier != 0)
            modifiers.Remove(modifier);
    }
}
