using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    UIGridManager gridManager;

    RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gridManager = GetComponentInParent<UIGridManager>();
        
        //transform.position = gridManager.GetClosestCell(transform).position;
        rect.anchoredPosition = gridManager.GetClosestCell(GetComponent<RectTransform>());

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetTile()
    {

    }
}
