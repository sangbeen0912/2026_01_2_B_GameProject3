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
       if(CardManager.Instance.playerStats == null && CardManager.Instance.playerStats.currentMana<cardData.manaCost)
        {
            Debug.Log($"마나가 부족합니다.! (필요 :{cardData.manaCost} ,guswo : {CardManager.Instance.playerStats.currentMana}");
            transform.position = originalPosition;
            return;
        }
        
        
        
        
        
        isDragging = false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool cardUsed = false;


        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            CharacterStats enemyStats = hit.collider.GetComponent<CharacterStats>();

            if (enemyStats != null)
            {
                if (cardData.cardType == CardData.CardType.Attack)
                {
                    Debug.Log($"{cardData.cardName}카드로 적에게 {cardData.effectAmount}데미지를 입혔습니다");
                    enemyStats.TakeDamage(cardData.effectAmount);
                    cardUsed = true;
                }
                else
                {
                    Debug.Log("이 카드는 적에게 사용할 수 없습니다.");
                }
            }
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, playerLayer))
        {
            CharacterStats playerStats = hit.collider.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                if (playerStats != null)
                {
                    if (cardData.cardType == CardData.CardType.Heal)
                    {
                        Debug.Log($"{cardData.cardName}카드로 플레이어의 체력을 {cardData.effectAmount}회복했습니다");
                        playerStats.Heal(cardData.effectAmount);
                        cardUsed = true;
                    }
                    else
                    {
                        Debug.Log("이 카드는 플레이어에게 사용할 수 없습니다.");
                    }
                }
                else if (CardManager.Instance != null)
                {
                    float disToDisCard = Vector3.Distance(transform.position, CardManager.Instance.transform.position); 
                    if(disToDisCard <2.0f)
                    {
                        CardManager.Instance.DiscardCard(cardIndex);
                        return;
                    }

                }
                
                
                if (!cardUsed)
                {
                    transform.position = originalPosition;
                    CardManager.Instance.ArrangeHand();
                }
                else
                {
                    if(CardManager.Instance != null)
                        CardManager.Instance.DiscardCard(cardIndex);

                    CardManager.Instance.playerStats.UseMana(cardData.manaCost);
                    Debug.Log($"마나를 {cardData.manaCost} 사용했습니다");
                }
            }
        }
    }
}
