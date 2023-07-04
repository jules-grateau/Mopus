using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    List<Material> materials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        materials = new List<Material>();
        foreach (Material material in meshRenderer.materials )
        {
            materials.Add(material);
        }
    }

    private void OnMouseEnter()
    {
        Highlight(true);
    }

    private void OnMouseExit()
    {
        Highlight(false);
    }

    public void Highlight(bool on)
    {
        if (on)
        {
            materials.ForEach(mat => mat.SetFloat("_isHighlighted", 1f));
        } else
        {
            materials.ForEach(mat => mat.SetFloat("_isHighlighted", 0f));
        }
    }


}
