using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Vector2 offset;

    public Vector2 GetOffset()
    {
        return offset;
    }
}
