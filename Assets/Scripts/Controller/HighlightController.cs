using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    List<Material> materials = new List<Material>();
    [SerializeField]
    bool isHighlighted = false;
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

    public void HandleMouseHover(Component component, object data)
    {
        if (data == null)
        {
            Highlight(false);
            return;
        }

        RaycastHit hitInfo = (RaycastHit)data;
        if (hitInfo.collider.gameObject.GetInstanceID() == this.gameObject.GetInstanceID())
        {
            Highlight(true);
            return;
        }

        Highlight(false);
    }

        public void Highlight(bool on)
    {
        if (isHighlighted && on) return ;
        if (!isHighlighted && !on) return ;

        if (on)
        {
            materials.ForEach(mat => mat.SetFloat("_isHighlighted", 1f));
            isHighlighted = true;
        } else
        {
            materials.ForEach(mat => mat.SetFloat("_isHighlighted", 0f));
            isHighlighted = false;
        }
    }


}
