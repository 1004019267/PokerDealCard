using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Canvas canvas;
    RectTransform canvasRect;
   
    Transform outCard;//出牌的父节点
   
    public List<Card> playerCard = new List<Card>();//玩家拥有手牌
    public List<GameObject> myCards = new List<GameObject>();//玩家拥有手牌显示
    public List<GameObject> hitOutCards = new List<GameObject>();//准备打出手牌显示 

    Vector2 mousePos2D;//点下鼠标位置
    Vector2 nowMousePos2D;//拖动鼠标位置
    float heigth;//牌的一半长度

    float t;//计时用
    int count = 0;//判断点击次数
 
    CardManager ca;
    /// <summary>
    /// 初始化玩家
    /// </summary>
    void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        canvasRect = canvas.transform as RectTransform;        
        outCard = canvas.transform.Find("Out");    
        ca = GetComponent<CardManager>();
        StartCoroutine(ca.CreatCard(this));
        heigth = myCards[0].GetComponent<RectTransform>().sizeDelta.y / 2;
    }
   

    /// <summary>
    /// 打出牌
    /// </summary>
    void HitCard()
    {
        if (Input.GetMouseButtonDown(1) && hitOutCards.Count != 0)
        {
            for (int i = 0; i < outCard.transform.GetChildCount(); i++)
            {
                Destroy(outCard.transform.GetChild(i).gameObject);
            }
            ca.SetAllCardPos(new Vector3(Screen.width / 2 + Screen.width % 2, outCard.transform.position.y), hitOutCards);
            for (int i = 0; i < hitOutCards.Count; i++)
            {
                hitOutCards[i].transform.SetParent(outCard);
               ca.RemoveMyCards(this,hitOutCards[i]);
            }
            if (myCards.Count != 0)
            {
               ca.SetAllCardPos(ca.midPos, myCards);
            }
            ca.ClearOutHitCard(this);
        }
    }
    /// <summary>
    /// 双击全部取消出牌状态
    /// </summary>
    void TakeBackALLCard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            count++;
            //当第一次点击鼠标，启动计时器
            if (count == 1)
            {
                t = Time.time;
            }
            //当第二次点击鼠标，且时间间隔满足要求时双击鼠标
            if (2 == count && Time.time - t <= 0.2f)
            {
                //Debug.Log(Time.time - t);
                for (int i = 0; i < hitOutCards.Count; i++)
                {
                   ca.SetRectPos(hitOutCards[i], new Vector2(ca.GetRectPos(hitOutCards[i]).x, -150));
                }
               ca.ClearOutHitCard(this);
                count = 0;
            }
            if (Time.time - t > 0.2f)
            {
                count = 0;
            }
        }
    }

    

    void Update()
    {
        HitCard();
        TakeBackALLCard();
        if (ca.isStart)
        {
            SetALLHitCard();
        }
    }

    /// <summary>
    /// 拖动选择卡牌
    /// </summary>
    void SetALLHitCard()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Vector2 mousePos2D = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            //RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);       
            //if (hit.collider != null)
            //{
            //    if (!setCards.Contains(hit.collider.gameObject))
            //    {
            //        setCards.Add(hit.collider.gameObject);
            //    }               
            //}
            GetMouseRecPos(out mousePos2D);
        }
        if (Input.GetMouseButton(0))
        {
            GetMouseRecPos(out nowMousePos2D);   
            for (int i = 0; i < myCards.Count; i++)
            {
                ca.SetMaBlackOrNull(this,myCards[i]);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            for (int i = 0; i < myCards.Count; i++)
            {
                if (ca.isMaBlack(myCards[i]))
                {
                    ca.SetHitOutCard(this,myCards[i]);
                    ca.SetMaterialNull(myCards[i]);
                }
            }
        }
    }
    /// <summary>
    /// 获取鼠标在画布坐标的位置
    /// </summary>
    /// <param name="mousPos"></param>
    void GetMouseRecPos(out Vector2 mousPos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvas.worldCamera, out mousPos);
    }
    /// <summary>
    /// 判断牌是否在滑动区域内
    /// </summary>
    /// <param name="cardPos"></param>
    /// <returns></returns>
   public bool isSelect(Vector3 cardPos)
    {
        if (mousePos2D.x <= cardPos.x && cardPos.x <= nowMousePos2D.x || nowMousePos2D.x <= cardPos.x && cardPos.x <= mousePos2D.x)
        {
            if (nowMousePos2D.y <= cardPos.y + heigth && cardPos.y - heigth <= nowMousePos2D.y)
            {
                return true;
            }
        }
        return false;
    }   
}
