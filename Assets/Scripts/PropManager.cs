using UnityEngine;
using System.Collections;

public class PropManager : MonoBehaviour {

    public Prop[] propsInOrder;

    private int propCount;
    private int currentIndex = 0;

    void Awake()
    {
        propCount = propsInOrder.Length;
        foreach (Prop prop in GetComponentsInChildren<Prop>(true))
        {
            prop.Initialize();
        }
    }

    public void Reset(int numProps)
    {
        for (int i = 0; i < numProps; i++)
        {
            if (currentIndex >= propCount)
            {
                return;
            }

            propsInOrder[currentIndex].TryChangeState();
            ++currentIndex;
        }
    }
}
