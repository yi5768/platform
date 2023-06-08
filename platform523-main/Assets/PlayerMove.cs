using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    AudioSource audioSource;

    public GameManager gameManager;
    Rigidbody2D rigid;
    public float maxSpeed;
    public float jumpPower;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D capsuleCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.freezeRotation = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }
    //즉각적인 키 입력, 키보드에 손을 뗐을때

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();

    }

    private void Update()
    {
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //방향전환 flip
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = (Input.GetAxisRaw("Horizontal")) == -1;
        }        
        
        //애니메이션
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
        {
            anim.SetBool("isWalk", false);
        } else
        {
            anim.SetBool("isWalk", true);
        }

        //점프
        if (Input.GetButtonDown("Jump"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJump", true);
            PlaySound("JUMP");
        }
    }


    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1))
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        }
        if(rigid.velocity.y < 0)
        {
            // 레이 캐스트   시작위치        방향       색(초록)   R,G,B
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("platform"));

            //빔에 맞았는지
            if (rayHit.collider != null)
            {   //플레이어의 절반크기 만큼이여야 바닥에 닿은 것
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("isJump", false);
                    Debug.Log(rayHit.collider.name);
                }
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            PlaySound("ITEM");
            //점수 얻기
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");
            if (isBronze)
            {
                gameManager.stagePoint += 50;
            }
            else if (isSilver)
            {
                gameManager.stagePoint += 100;
            }
            else if (isGold)
            {
                gameManager.stagePoint += 300;
            }

            //아이템 비활성화
            collision.gameObject.SetActive(false);
        }
        else if(collision.gameObject.tag == "Finish")
        {
            PlaySound("FINISH");
            //다음 스테이지 이동
            gameManager.NextStage();

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        { //Attack
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else //Damaged
                OnDamaged(collision.transform.position);
        }
    }

    void OnAttack(Transform enemy)
    {
        //Point
        PlaySound("DAMAGED");
        gameManager.stagePoint += 100;

        //리액션 Force
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //enemy die
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged(); //몬스터 입장에서는 공격받는 것
    }

    void OnDamaged(Vector2 targetPos)
    {   //health down
        PlaySound("DAMAGED");
        gameManager.HealthDown();   //몬스터와 만났을 때 체력 깍기
        gameObject.layer = 11;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);
        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        PlaySound("DIE");
        //스프라이트 알파값
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //스프라이트 반전 Flip
        spriteRenderer.flipX = true;
        //콜라이더 disable
        capsuleCollider.enabled = false;   //비활성화
        //die effect jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void velocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}