using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 牌的类型
/// </summary>
public enum eType
{
    Spade = 0,
    Heart,
    Diamond,
    Club,
    Joker,
}

public class Card
{
    public eType type;//0黑桃1红桃2梅花3方块4王
    public int num;
    public UnityEngine.Object card;
    /// <summary>
    /// 每一个牌的属性
    /// </summary>
    /// <param name="type"></param>
    /// <param name="num"></param>
    /// <param name="card"></param>
    public Card(eType type, int num, UnityEngine.Object card)
    {
        this.type = type;
        this.num = num;
        this.card = card;
    }
}

public class CardManager : MonoBehaviour
{
    Player[] players = new Player[1];//玩家数组
    Canvas canvas;

    UnityEngine.Object[] sprites;
    List<Card> cards = new List<Card>();
    Material ma;//选中的阴影的材质

    public Vector3 midPos;//牌组中心点
    Transform panle;//出牌前遮罩
    public bool isStart;//出牌前判定是否可选牌

    float y;//初始牌的Y轴    
    Transform btn;

    GameObject card;//牌的父节点
    float offset = 8;//卡牌间距

    /// <summary>
    /// 初始化卡牌类
    /// </summary>
    void Awake()
    {
        sprites = Resources.LoadAll("cardv");
        ma = Resources.Load("shadowM") as Material;

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        panle = canvas.transform.Find("Panle");
        btn = canvas.transform.Find("Button");

        card = canvas.transform.Find("Card").gameObject;

        InitCard();
        players[0] = GetComponent<Player>();
        GiveCard(players);
    }
    /// <summary>
    /// 为整租牌添加一张牌
    /// </summary>
    public void AddCard(Card card)
    {
        cards.Add(card);
    }
    /// <summary>
    /// 清空当前牌组
    /// </summary>
    public void ClearCard()
    {
        cards.Clear();
    }
    /// <summary>
    /// 为某个玩家添加一张牌
    /// </summary>
    public void AddPlayerCard(List<Card> cards, Card card)
    {
        cards.Add(card);
    }
    /// <summary>
    /// 为某个玩家减少
    /// </summary>
    public void RemovePlayerCard(List<Card> cards, Card card)
    {
        cards.Remove(card);
    }

    /// <summary>
    /// 添加出手牌库
    /// </summary>
    /// <param name="go"></param>
    public void AddHitOutCard(Player player,GameObject go)
    {
       player.hitOutCards.Add(go);
    }

    /// <summary>
    /// 删除出手牌库
    /// </summary>
    /// <param name="go"></param>
    public void RemoveHitOutCard(Player player, GameObject go)
    {
        player.hitOutCards.Remove(go);
    }
    /// <summary>
    /// 清空手牌库
    /// </summary>
    public void ClearOutHitCard(Player player)
    {
        player.hitOutCards.Clear();
    }

    /// <summary>
    /// 添加手牌
    /// </summary>
    /// <param name="go"></param>
    public void AddMyCards(Player player, GameObject go)
    {
       player.myCards.Add(go);
    }

    /// <summary>
    /// 移除手牌
    /// </summary>
    /// <param name="go"></param>
    public void RemoveMyCards(Player player, GameObject go)
    {
       player.myCards.Remove(go);
    }
    /// <summary>
    /// 初始化牌组把牌组赋值
    /// </summary>   
    void InitCard()
    {
        for (int i = 1; i < sprites.Length; i++)
        {
            if (i <= 13)
            {
                AddCard(new Card(eType.Spade, i, sprites[i]));
            }
            else if (i <= 15)
            {
                AddCard(new Card(eType.Joker, i, sprites[i]));
            }
            else if (i <= 28)
            {
                AddCard(new Card(eType.Heart, i - 15, sprites[i]));
            }
            else if (i <= 41)
            {
                AddCard(new Card(eType.Diamond, i - 28, sprites[i]));
            }
            else
            {
                AddCard(new Card(eType.Club, i - 41, sprites[i]));
            }
        }
    }

    /// 洗牌算法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="listtemp"></param>
    void Reshuffle<T>(List<T> listtemp)
    {
        //随机交换
        System.Random ram = new System.Random();
        int currentIndex;
        T tempValue;
        for (int i = 0; i < listtemp.Count; i++)
        {
            currentIndex = ram.Next(0, listtemp.Count - i);
            tempValue = listtemp[currentIndex];
            listtemp[currentIndex] = listtemp[listtemp.Count - 1 - i];
            listtemp[listtemp.Count - 1 - i] = tempValue;
        }
    }
    /// <summary>
    /// 发牌
    /// </summary>
    public void GiveCard(Player[] players)
    {
        Reshuffle(cards);
        for (int i = 0; i < players.Length; i++)
        {
            for (int j = 0; j < cards.Count; j++)
            {
                if (players[i].playerCard.Count < 17)
                {
                    AddPlayerCard(players[i].playerCard, cards[j]);
                }
                else
                {
                    return;
                }
            }
        }
    }
    /// <summary>
    /// 生成卡牌
    /// </summary>
    public IEnumerator CreatCard(Player player)
    {
        y = btn.transform.position.y;
        midPos = new Vector3(Screen.width / 2 + Screen.width % 2, y);
        for (int i = 0; i < player.playerCard.Count; i++)
        {
            var go = GameObject.Instantiate(btn);
            go.GetComponent<Image>().overrideSprite = player.playerCard[i].card as Sprite;
            go.SetParent(card.transform);
            go.name = player.playerCard[i].type + "_" + player.playerCard[i].num;
            go.transform.localScale = Vector3.one;

            AddMyCards(player,go.gameObject);
            SetRectPos(player.myCards[i], midPos + Vector3.right * i * offset);
            player.myCards[i].transform.position = midPos + Vector3.right * i * offset;
            for (int j = 0; j < player.myCards.Count; j++)
            {
                player.myCards[j].transform.position -= Vector3.right * offset;
            }
            yield return new WaitForSeconds(0.1f);
        }
        SetAllBtn(player,player.myCards);
        SetAllCardPos(midPos, player.myCards);
        panle.gameObject.SetActive(false);
        isStart = true;
    }

    /// <summary>
    /// 基于中心刷新牌位置
    /// </summary>
    /// <param name="midPos"></param>
    public void SetAllCardPos(Vector3 midPos, List<GameObject> cards)
    {
        cards.Sort((a, b) => int.Parse(a.name.Split('_')[1]).CompareTo(int.Parse(b.name.Split('_')[1])));
        cards.Reverse();

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.position = midPos + Vector3.right * i * offset;
            for (int j = 0; j < cards.Count; j++)
            {
                cards[j].transform.position -= Vector3.right * offset;
                cards[j].transform.SetAsLastSibling();
            }
        }
    }

    /// <summary>
    /// 为当前所有牌添加点击事件
    /// </summary>
    void SetAllBtn(Player player,List<GameObject> myCards)
    {
        foreach (var item in myCards)
        {
            item.GetComponent<Button>().onClick.AddListener(() => SetHitOutCard(player,item.gameObject));
        }
    }
    /// <summary>
    /// 设置卡的位置
    /// </summary>
    /// <param name="go"></param>
    public void SetHitOutCard(Player player,GameObject go)
    {
        var pos = GetRectPos(go);
        if (pos.y == -150f)
        {
            SetRectPos(go, new Vector2(pos.x, -130));
            AddHitOutCard(player,go);
        }
        else
        {
            SetRectPos(go, new Vector2(pos.x, -150));
            RemoveHitOutCard(player,go);
        }
    }
    /// <summary>
    /// 获得Rec的Pos
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public Vector3 GetRectPos(GameObject go)
    {
        return go.GetComponent<RectTransform>().anchoredPosition3D;
    }

    /// <summary>
    /// 设置Rec的Pos
    /// </summary>
    /// <param name="go"></param>
    /// <param name="v"></param>
    public void SetRectPos(GameObject go, Vector3 v)
    {
        go.GetComponent<RectTransform>().anchoredPosition3D = v;
    }

    public void SetMaBlackOrNull(Player palyer, GameObject card)
    {
        Vector3 cardPos = GetRectPos(card);
        if (palyer.isSelect(cardPos))
        {
            SetMaterialBlack(card);
        }
        else
        {
            SetMaterialNull(card);
        }
    }
    /// <summary>
    /// 选中改变阴影材质
    /// </summary>
    /// <param name="card"></param>
    public void SetMaterialBlack(GameObject card)
    {
        card.GetComponent<Image>().material = ma;
    }

    public void SetMaterialNull(GameObject card)
    {
        card.GetComponent<Image>().material = null;
    }

    public bool isMaBlack(GameObject card)
    {
        if (card.GetComponent<Image>().material == ma)
        {
            return true;
        }
        return false;
    }
}
