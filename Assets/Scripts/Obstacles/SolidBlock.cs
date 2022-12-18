using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D), typeof(SpriteRenderer))]
public class SolidBlock : MonoBehaviour, IHasColor
{
    public ScriptableColor ActiveColor { get; private set; }

    private EdgeCollider2D col;
    private SpriteRenderer sprRend;

    public Action<SolidBlock> e_BlockDestroyed;

    public void Init(ColorID _color)
    {
        col = GetComponent<EdgeCollider2D>();
        sprRend = GetComponent<SpriteRenderer>();

        //transform.localScale = new Vector3(Random.Range(1f, 4f), .25f);

        SetColor(_color);
    }
    public void DeactivateBlock()
    {
        col.enabled = false;
        sprRend.color = new Color(sprRend.color.r, sprRend.color.g, sprRend.color.b, .25f);

    }
    public void ActivateBlock()
    {
        col.enabled = true;
        sprRend.color = new Color(sprRend.color.r, sprRend.color.g, sprRend.color.b, 1);
    }

    public void SetColor(ColorID _color)
    {
        ActiveColor = ColorManager.Instance.GetColorByID(_color);
        sprRend.color = ActiveColor.Color;
    }

    public void SetToNextColorInCycle()
    {
        SetColor(ColorManager.Instance.GetNextColorInCycle(ActiveColor.ColorID));
    }

    public void SetToPreviousColorInCycle()
    {
        SetColor(ColorManager.Instance.GetPreviousColorInCycle(ActiveColor.ColorID));

    }

    public void DestroyColorObject()
    {
        e_BlockDestroyed?.Invoke(this);

        GameManager.Instance.IncreaseScore(100);
    }
}
