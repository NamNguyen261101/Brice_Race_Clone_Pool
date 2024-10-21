using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState<Bot>
{
    int targetBrick;
    public void OnEnter(Bot t)
    {
        t.ChangeAnim(Constants.ANIM_RUN);
        targetBrick = Random.Range(2, 7);
        SeekTarget(t);
    }

    public void OnExcute(Bot t)
    {
        if (t.IsDestination)
        {
            if (t.BrickCount >= targetBrick)
            {
                t.ChangeState(new AttackState());
            }
            else
            {
                SeekTarget(t);
            }
        }
    }

    public void OnExit(Bot t)
    {
    }

    private void SeekTarget(Bot t)
    {
        if (t.stage != null)
        {
            Brick brick = t.stage.SeekBrickPoint(t.colorType);

            if (brick == null)
            {
                t.ChangeState(new AttackState());
            }
            else
            {
                t.SetDestination(brick.TF.position);
            }
        }
        else
        {
            t.SetDestination(t.TF.position);
        }
    }

}
