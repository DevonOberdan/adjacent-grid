using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(GetComponent<RectTransform>().position);
        print(GetComponent<RectTransform>().localPosition);
        print(GetComponent<RectTransform>().anchoredPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
