using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    [SerializeField] private ColorContainer container;

    [SerializeField] private Image upperWheel;
    [SerializeField] private Image rightWheel;
    [SerializeField] private Image lowerWheel;
    [SerializeField] private Image leftWheel;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        upperWheel.color = GetColorByID(ColorID.Red).Color;
        rightWheel.color = GetColorByID(ColorID.Green).Color;
        lowerWheel.color = GetColorByID(ColorID.Blue).Color;
        leftWheel.color = GetColorByID(ColorID.Yellow).Color;
    }

    public ScriptableColor GetColorByID(ColorID _id)
    {
        for (int i = 0; i < container.Colors.Length; i++)
        {
            if (container.Colors[i].ColorID == _id)
                return container.Colors[i];
        }

        // ColorID not found in container
        return null;
    }
    public ColorID GetNextColorInCycle(ColorID _id)
    {
        for (int i = 0; i < container.Colors.Length; i++)
        {
            // end of cycle reached
            if ((int)_id >= container.Colors.Length - 1) return 0;

            if (container.Colors[i].ColorID == _id + 1)
                return _id + 1;
        }

        return ColorID.None;
    }
    public ColorID GetPreviousColorInCycle(ColorID _id)
    {
        for (int i = 0; i < container.Colors.Length; i++)
        {
            if ((int)_id <= 0) 
                return (ColorID)(container.Colors.Length - 1);

            if (container.Colors[i].ColorID == _id - 1)
                return _id - 1;
        }

        return ColorID.None;
    }
    public ColorID GetRandomColor()
    {
        return (ColorID)Random.Range(0, container.Colors.Length);
    }
}
