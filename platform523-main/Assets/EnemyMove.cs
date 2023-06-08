using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        Invoke("Think", 2);
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    private void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove,rigid.velocity.y);
        //���� �� üũ
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove, rigid.velocity.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1,2); //-1 ~ 1����
        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime); //����Լ�

        anim.SetInteger("walkSpeed", nextMove);
        //���� �ٲٱ�(0�϶��� ���� �ٲ� �ʿ䰡 ���� ���ǹ� ���)
        if(nextMove != 0)
        {
            spriteRenderer.flipX = (nextMove == 1);
        }
    }
    void Turn()
    {
        nextMove = nextMove * (-1);
        spriteRenderer.flipX = (nextMove == 1);

        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        //��������Ʈ ���İ�
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //��������Ʈ ���� Flip
        spriteRenderer.flipX = true;
        //�ݶ��̴� disable
        capsuleCollider.enabled = false;   //��Ȱ��ȭ
        //die effect jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        //destroy
        Invoke("DeActive", 5);
    }
    void DeActive()
    {
        gameObject.SetActive(false);
    }

}
