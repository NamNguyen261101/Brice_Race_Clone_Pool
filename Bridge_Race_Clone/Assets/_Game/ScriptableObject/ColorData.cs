using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorData", order = 1)]
public class ColorData : ScriptableObject
{
    [SerializeField] Material[] colorMats;

    public Material GetColorMat(ColorType colorType)
    {
        return colorMats[(int)colorType];
    }
}
