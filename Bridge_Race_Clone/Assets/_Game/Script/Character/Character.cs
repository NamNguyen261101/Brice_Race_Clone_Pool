using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : ColorObject
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask stairLayer;

    private List<PlayerBrick> playerBricks = new List<PlayerBrick>();
    [SerializeField] private PlayerBrick playerBrickPrefab;
    [SerializeField] private Transform brickHolder;
    [SerializeField] protected Transform skin;
    public Animator anim;
    private string currentAnim;

    [HideInInspector] public Stage stage;


    public int BrickCount => playerBricks.Count;

    public override void OnInit()
    {
        ClearBrick();
        skin.rotation = Quaternion.identity;
    }

    //check diem tiep theo xem co phai la ground khong
    //+ tra ve vi tri next do
    //- tra ve vi tri hien tai
    public Vector3 CheckGround(Vector3 nextPoint)
    {
        RaycastHit hit;

        if (Physics.Raycast(nextPoint, Vector3.down, out hit, 2f, groundLayer))
        {
            return hit.point + Vector3.up * 1.1f;
        }

        return TF.position;
    }

    public void AddBrick()
    {
        PlayerBrick playerBrick = Instantiate(playerBrickPrefab, brickHolder);
        playerBrick.ChangeColor(colorType);
        playerBrick.TF.localPosition = Vector3.up * 0.25f * playerBricks.Count;
        playerBricks.Add(playerBrick);
    }

    public void RemoveBrick()
    {
        if (playerBricks.Count > 0)
        {
            PlayerBrick playerBrick = playerBricks[playerBricks.Count - 1];
            playerBricks.RemoveAt(playerBricks.Count - 1);
            Destroy(playerBrick.gameObject);
        }
    }

    public void ClearBrick()
    {
        for (int i = 0; i < playerBricks.Count; i++)
        {
            Destroy(playerBricks[i]);
        }

        playerBricks.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_BRICK))
        {
            Brick brick = Cache.GetBrick(other);

            if (brick.colorType == colorType)
            {
                brick.OnDespawn();
                AddBrick();
                Destroy(brick.gameObject);
            }
        }
    }



    public bool CanMove(Vector3 nextPoint)
    {
        //check mau stair
        //k cung mau -> fill
        //het gach + k cung mau + huong di len

        bool isCanMove = true;
        RaycastHit hit;

        if (Physics.Raycast(nextPoint, Vector3.down, out hit, 2f, stairLayer))
        {
            Stair stair = Cache.GetStair(hit.collider);

            if (stair.colorType != colorType && playerBricks.Count > 0)
            {
                stair.ChangeColor(colorType);
                RemoveBrick();
                stage.NewBrick(colorType);
            }

            if (stair.colorType != colorType && playerBricks.Count == 0 && skin.forward.z > 0)
            {
                isCanMove = false;
            }
        }

        return isCanMove;
    }

    public void ChangeAnim(string animName)
    {
        if (currentAnim != animName)
        {
            anim.ResetTrigger(currentAnim);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
        }
    }
}
