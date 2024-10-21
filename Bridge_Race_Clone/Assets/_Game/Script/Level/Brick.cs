using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : ColorObject
{
    [HideInInspector] public Stage stage;

    /// <summary>
    /// Delete brick when character touch
    /// </summary>
    public void OnDespawn()
    {
        stage.RemoveBrick(this);
    }
}
