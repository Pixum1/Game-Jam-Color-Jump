using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasColor
{
    public void SetColor(ColorID _color);
    public void SetToNextColorInCycle();
    public void SetToPreviousColorInCycle();
}
