using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : ColorObject
{
    [HideInInspector] public Stage stage;

    public void OnDespawn()
    {
        stage.RemoveBrick(this);
    }
}
