using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, AxisState.IInputAxisProvider
{
    [HideInInspector]
    public InputAction horizontal;
    [HideInInspector]
    public InputAction Vertical;
    public bool IsAimAssist;
    public Vector2 AimAssistOffset;

    public float GetAxisValue(int axis)
    {
        
            if (!IsAimAssist) 
            switch (axis)
            {
                case 0: return horizontal.ReadValue<Vector2>().x;
                case 1: return horizontal.ReadValue<Vector2>().y;
                case 2: return Vertical.ReadValue<float>();
            }
            else
            {
                return AimAsist(AimAssistOffset, axis);
            }
        return 0;
    }

    public float AimAsist(Vector2 offset,int axis) 
    {
        switch (axis)
        {
            case 0: return horizontal.ReadValue<Vector2>().x  + offset.x;
            case 1: return horizontal.ReadValue<Vector2>().y + offset.y; 
            case 2: return Vertical.ReadValue<float>();
        }
        return 0;
    }



}
