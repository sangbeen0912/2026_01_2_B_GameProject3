using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
public class CardManager : MonoBehaviour
{
    public List<CardData> deckCards = new List<CardData>();
    public List<CardData> handCards = new List<CardData>();
    public List<CardData> discardCards = new List<CardData>();

    public GameObject CardPrefabs;
    public Transform deckPosition;
    public Transform handPosition;
    public Transform discardPosition;

    public List<GameObject> cardObjects = new List<GameObject>();

    public CharacterStats playerStats;

    private static CardManager instance;

    public static CardManager Instance
    {
        get
        {
            if(instance == null) instance = new CardManager();
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShuffleDeck();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawCard();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ReturnDiscardsToDeck();

        }
        ArrangeHand();
    }

    public void ShuffleDeck()
    {
        List<CardData> tempDeck = new List<CardData>(deckCards);
        deckCards.Clear();

        while (tempDeck.Count > 0)
        {
            int randindex = Random.Range(0, tempDeck.Count);
            deckCards.Add(tempDeck[randindex]);
            tempDeck.RemoveAt(randindex);
        }
        Debug.Log("덱을 섞었습니다");
    }

    public void DrawCard()
    {
        if (handCards.Count >= 6)
        {
            Debug.Log("손패가 가득 찼습니다");
            return;
        }

        if (deckCards.Count == 0)
        {
            Debug.Log("덱에 카드가 없습니다");
            return;
        }

        CardData cardData = deckCards[0];
        deckCards.RemoveAt(0);

        handCards.Add(cardData);

        GameObject cardObj = Instantiate(CardPrefabs, deckPosition.position, Quaternion.identity);

        CardDisplay cardDisplay = cardObj.GetComponent<CardDisplay>();

        if (cardDisplay != null)
        {
            cardDisplay.SetupCard(cardData);
            cardDisplay.cardIndex = handCards.Count - 1;
            cardObjects.Add(cardObj);

        }

        ArrangeHand();

        Debug.Log("카드를 드로우 했습니다.: " + cardData.cardName + "(손패:" + handCards.Count + "/6");
    }

       public void ArrangeHand()
        {
            if (handCards.Count == 0) return;

            float cardWidth = 1.2f;
            float spacing = cardWidth / 2f;
            float totalWidth = (handCards.Count - 1) * spacing;
            float startX = -totalWidth / 2f;

            for (int i = 0; i < handCards.Count; i++)
            {
                if (cardObjects[i] != null)
                {
                    CardDisplay display = cardObjects[i] .GetComponent<CardDisplay>();

                    if (display != null && display.isDragging)
                        continue;

                    Vector3 targetPosition = handPosition.position + new Vector3(startX + (i * spacing), 0, 0);

                    cardObjects[i].transform.position = Vector3.Lerp(cardObjects[i].transform.position,targetPosition,Time.deltaTime * 10f);

                }
            }
        }

   public void DiscardCard(int handIndex)
   {
           if(handIndex < 0 && handIndex >= handCards.Count)
           {
             Debug.Log("유효하지 않은 카드 인덱스 입니다");
             return;
           }
           CardData cardData = handCards[handIndex];
           handCards.RemoveAt(handIndex);
           
           discardCards.Add(cardData);
        
        if (handIndex < cardObjects.Count)
        {
            Destroy(cardObjects[handIndex]);
            cardObjects.RemoveAt(handIndex);

        }
        for (int i = 0; i < cardObjects.Count; i++)
        {
            CardDisplay display = cardObjects [i] .GetComponent<CardDisplay>();
            if (display != null) display.cardIndex = i;
        }

        ArrangeHand();
        Debug.Log("카드를 버렸습니다." + cardData.cardName);
    
   }

    public void ReturnDiscardsToDeck()
    {
        if (discardCards.Count == 0)
        {
            Debug.Log("버린 카드 더미가 비어 있습니다.");
                return;

        }
        deckCards.AddRange(discardCards);
        discardCards.Clear();
        ShuffleDeck();


    }
}
