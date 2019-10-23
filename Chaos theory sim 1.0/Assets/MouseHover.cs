using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHover : MonoBehaviour
{

    void MouseOver()
    {
        ChaosSim.instance.canCreatePoints = false;
        print("ASD");
    }
    void MouseExit()
    {
        ChaosSim.instance.canCreatePoints = true;
    }
}
