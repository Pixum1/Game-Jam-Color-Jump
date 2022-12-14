using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorID
{
    None = -1,
    Red = 0,
    Green = 1,
    Blue = 2,
    Yellow = 3,
}

[CreateAssetMenu(fileName = "New Color", menuName = "Scriptable Color")]
public class ScriptableColor : ScriptableObject
{
    [SerializeField] private ColorID colorID;
    [SerializeField] private Color color;

    public ColorID ColorID { get { return colorID; } }
    public Color Color { get { return color; } }
}
