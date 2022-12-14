using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }

    [SerializeField] private ColorContainer container;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
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
    public ColorID GetNextColor(ColorID _id)
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
}
