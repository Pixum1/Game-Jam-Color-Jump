using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class SolidBlock : MonoBehaviour, IHasColor
{
    public ScriptableColor ActiveColor { get; private set; }

    private BoxCollider2D col;
    private SpriteRenderer sprRend;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        sprRend = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ChangeColor(ColorID.Red);
    }

    public void Init(ColorID _color)
    {
        ChangeColor(_color);
    }
    public void ToggleCollider()
    {
        col.enabled = !col.enabled;
    }

    public void ChangeColor(ColorID _color)
    {
        ActiveColor = ColorManager.Instance.GetColorByID(_color);
        sprRend.color = ActiveColor.Color;
    }

    public void SetToNextColorInCycle()
    {
        ChangeColor(ColorManager.Instance.GetNextColor(ActiveColor.ColorID));
    }
}
