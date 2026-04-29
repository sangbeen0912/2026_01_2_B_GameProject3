using UnityEngine;
using TMPro;
public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public int cardIndex;

    public MeshRenderer cardRenderer;
    public TextMeshPro nameText;
    public TextMeshPro costText;
    public TextMeshPro attackText;
    public TextMeshPro descriptionText;

    public bool isDragging = false;
    private Vector3 originalPosition;

    public LayerMask enemyLayer;
    public LayerMask playerLayer;

    public void Start()
    {
        playerLayer = LayerMask.GetMask("Player");
        enemyLayer = LayerMask.GetMask("Enemy");

        SetupCard(cardData);


    }

    public void SetupCard(CardData data)
    {
        cardData = data;

        if (nameText != null)
            nameText.text = cardData.cardName;
        if (costText != null)
            costText.text = cardData.manaCost.ToString();
        if (attackText != null)
            attackText.text = cardData.effectAmount.ToString();
        if (descriptionText != null)
            descriptionText.text = cardData.description;

        if (cardRenderer != null && data.artwork != null)
        {
            Material cardMaterial = cardRenderer.material;
            cardMaterial.mainTexture = data.artwork.texture;
        }
        
        if(descriptionText != null)
        {
            descriptionText.text = data.GetAdditionalEffectDescription();

        }
    }


    private void OnMouseDown()
    {
        isDragging = true;
        originalPosition = transform.position;
    }
    private void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);

        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        if(CardManager.Instance != null)
        {
            float disToDiscard = Vector3.Distance(transform.position, CardManager.Instance.discardPosition.position);
            if(disToDiscard < 2.0f)
            {
                CardManager.Instance.DiscardCard(cardIndex);
                return;
            }
        }

       if( CardManager.Instance.playerStats != null && CardManager.Instance.playerStats.currentMana <cardData.manaCost)
        {
           Debug.Log("마나가 부족합니다 ");
            transform.position = originalPosition;
            return;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool cardUsed = false;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            CharacterStats enemyStats = hit.collider.GetComponent<CharacterStats>(); // 적에게 공격 효과 적용

            if (enemyStats != null)
            {
                if (cardData.cardType == CardData.CardType.Attack)
                {
                    enemyStats.TakeDamage(cardData.effectAmount);
                    Debug.Log($"{cardData.cardName} 카드로 적에게 {cardData.effectAmount} 데미지를 입혔습니다.");
                    cardUsed = true;
                }
            }
            else
            {
                Debug.Log("이 카드는 적에게 사용 할 수 없습니다.");
            }
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayer))
        {
            if(CardManager.Instance.playerStats != null)
            {
                CardManager.Instance.playerStats.Heal(cardData.effectAmount);
                cardUsed = true;
            }
        }

        if (!cardUsed)
        {
            transform.position = originalPosition;
            if (CardManager.Instance != null)
                CardManager.Instance.ArrangeHand();
                
            return;
            
        }

       CardManager.Instance.playerStats.UseMana(cardData.manaCost);
        Debug.Log($"카드 {cardData.cardName} 사용으로 마나 {cardData.manaCost} 소모 했습니다.");
        
        if(cardData.additionalEffects != null && cardData.additionalEffects.Count > 0)
        {
            ProcessAdditionalEffectsAndDiscard();
        }
        else
        {
            if (CardManager.Instance != null)
                CardManager.Instance.DiscardCard(cardIndex);
        }



    }

    public void ProcessAdditionalEffectsAndDiscard()
    {
        CardData cardDataCopy = cardData;
        int cardIndexCopy = cardIndex;

        foreach (var effect in cardDataCopy.additionalEffects)
         {
            switch (effect.effectType)
            {
                case CardData.AdditionalEffectType.DrawCard:

                    for (int i = 0; i < effect.effectAmount; i++)
                    {
                        if (CardManager.Instance != null)
                            CardManager.Instance.DrawCard();
                    }

                    Debug.Log($"카드 {effect.effectAmount} 장 드로우했습니다");
                    break;

                case CardData.AdditionalEffectType.DiscardCard: // 카드 버리기 구현 (랜덤 버리기)
                    for (int i = 0; i < effect.effectAmount; i++)
                    {
                        if (CardManager.Instance != null && CardManager.Instance.handCards.Count > 0)
                        {
                            // 손패 크기 기준으로 랜덤 인덱스 생성
                            int randomIndex = Random.Range(0, CardManager.Instance.handCards.Count);

                            Debug.Log($"랜덤 카드 버리기 : 선택된 인덱스 {randomIndex}, 현재 손패 크기 : {CardManager.Instance.handCards.Count}");

                            if (cardIndexCopy < CardManager.Instance.handCards.Count)
                            {
                                if (randomIndex != cardIndexCopy)
                                {
                                    CardManager.Instance.DiscardCard(randomIndex);

                                    // 만약 버린 카드의 인덱스가 현재 카드의 인덱스보다 작다면 현재 카드의 인덱스를 1 감소 시켜야 함
                                    if (randomIndex < cardIndexCopy)
                                    {
                                        cardIndexCopy--;
                                    }
                                }
                                else if (CardManager.Instance.handCards.Count > 1)
                                {
                                    // 자기 자신을 버리려 할 경우 다른 카드 선택
                                    int newIndex = (randomIndex + 1) % CardManager.Instance.handCards.Count;
                                    CardManager.Instance.DiscardCard(newIndex);

                                    if (newIndex < cardIndexCopy)
                                    {
                                        cardIndexCopy--;
                                    }
                                }
                            }
                            else
                            {
                                // cardIndexCopy가 더 이상 유효하지 않은 경우, 아무 카드나 버림
                                CardManager.Instance.DiscardCard(randomIndex);
                            }
                        }
                    }
                    break;
                case CardData.AdditionalEffectType.GainMana:
                    if (CardManager.Instance.playerStats != null)
                    {
                        CardManager.Instance.playerStats.GainMana(effect.effectAmount);
                        Debug.Log($"마나를 {effect.effectAmount} 획득 했습니다.");
                    }
                    break;

                case CardData.AdditionalEffectType.ReduceEnemyMana:
                    if (CardManager.Instance.EnemyStats != null)
                    {
                        CardManager.Instance.EnemyStats.UseMana(effect.effectAmount);
                        Debug.Log($"적이 마나를 {effect.effectAmount} 잃었습니다.");
                    }
                    break;
            }

        }
        if (CardManager.Instance != null)
            {
                CardManager.Instance.DiscardCard(cardIndexCopy);
        }
    }
}
