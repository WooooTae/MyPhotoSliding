using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePiece : MonoBehaviour, IPointerClickHandler
{
    public int Index { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        int currentBoardIndex = transform.GetSiblingIndex();

        GameManager.Instance.TryMove(currentBoardIndex);
    }
}
