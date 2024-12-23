using System;
using GameStates;
using GridSystem.GridItems;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BonusItem : MonoBehaviour
{
    [SerializeField] private BonusItemType type = BonusItemType.Cherry;
    private GridItem parent;

    public void Initialise(GridItem p)
    {
        parent = p;
    }

    private void SetGraphic()
    {
        GetComponent<Image>().sprite = ScoreManager.Instance.GetBonusPickUpGraphic(type);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        BonusItemHit();
    }

    private void OnTriggerStay(Collider other)
    {
        BonusItemHit();
    }

    private void BonusItemHit()
    {
        ScoreManager.Instance.BonusItemHit(type);
        Destroy(gameObject);
    }
    
    #if UNITY_EDITOR
    [ContextMenu(nameof(ChangeToCherry))]
    private void ChangeToCherry()
    {
        type = BonusItemType.Cherry;
    }
    
    [ContextMenu(nameof(ChangeToStrawberry))]
    private void ChangeToStrawberry()
    {
        type = BonusItemType.Strawberry;
    }
    
    [ContextMenu(nameof(ChangeToOrange))]
    private void ChangeToOrange()
    {
        type = BonusItemType.Orange;
    }
    
    [ContextMenu(nameof(ChangeToApple))]
    private void ChangeToApple()
    {
        type = BonusItemType.Apple;
    }
    
    [ContextMenu(nameof(ChangeToMelon))]
    private void ChangeToMelon()
    {
        type = BonusItemType.Melon;
    }
    
    [ContextMenu(nameof(ChangeToGalaxian))]
    private void ChangeToGalaxian()
    {
        type = BonusItemType.Galaxian;
    }
    
    [ContextMenu(nameof(ChangeToBell))]
    private void ChangeToBell()
    {
        type = BonusItemType.Bell;
    }
    
    [ContextMenu(nameof(ChangeToKey))]
    private void ChangeToKey()
    {
        type = BonusItemType.Key;
    }
    #endif

    public enum BonusItemType
    {
        Cherry, Strawberry, Orange, Apple, Melon, Galaxian, Bell, Key
    }
}
