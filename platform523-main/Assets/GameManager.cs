using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int totalPoint; //��������
    public int stagePoint; //�������� ���� ����
    public int stageIndex; //�������� ��ȣ
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIHealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;

    private void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }


    //������ ���������� �����Ѵ�
    public void NextStage()
    {
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + stageIndex + 1;
        }
        else
        {
            //���� Ŭ����
            Time.timeScale = 0;
            //��� UI
            Debug.Log("GAME CLEAR");
            //����� ��ư UI
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "GAME CLEAR";
            RestartBtn.SetActive(true);

        }

        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
            UIHealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            //All Health UI
            UIHealth[0].color = new Color(1, 0, 0, 0.4f);
            //�÷��̾� ���� ����Ʈ
            player.OnDie();
            //��� UI
            Debug.Log("YOU DIE");
            //�ٽý��� ��ư UI
            RestartBtn.SetActive(true);
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {   //�÷��̾� ������
            if (health > 1)
            {
                PlayerReposition();
            }
            HealthDown();
        }   
    }

    //�÷��̾� ������ �Լ� �����
    void PlayerReposition()
    {
        player.transform.position = new Vector3(-4, 3, -1);
        player.velocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

}
