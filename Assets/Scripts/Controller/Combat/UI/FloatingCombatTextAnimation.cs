using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingCombatTextAnimation : MonoBehaviour
{
    TextMeshPro _tmp;
    void Awake()
    {
        _tmp = GetComponent<TextMeshPro>();
        _tmp.text = "";
        Destroy(gameObject, 1f);   
    }

    public void Init(float num)
    {
        _tmp.text += "-"+num.ToString();
    }
}
