using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasColor
{
    public void ChangeColor(ColorID _color);
    public void SetToNextColorInCycle();
}
