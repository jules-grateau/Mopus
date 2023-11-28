using Assets.Scripts.ScriptableObjets.Parameters;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    List<Material> _materials = new List<Material>();
    ColorsParams _colorsParams;
    Color _currColor;
    

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        _materials = new List<Material>();
        foreach (Material material in meshRenderer.materials )
        {
            _materials.Add(material);
            _materials.ForEach(mat => {
                //We need to enable the EMISSION
                mat.EnableKeyword("_EMISSION");
            });
        }

        _colorsParams = Resources.Load<ColorsParams>("ScriptableObjects/Parameters/ColorsParams");
    }

    private void OnMouseEnter()
    {
        Highlight(true, _colorsParams.HoverPreviewColor);
    }

    private void OnMouseExit()
    {
        Highlight(false, _colorsParams.HoverPreviewColor);
    }

    public void Highlight(bool on, Color color )
    {
        var newColor = _currColor;

        if (on)
        {
            newColor += color;
        }
        else
        {
            newColor -= color;
        }

        _materials.ForEach(mat => {
            mat.SetColor("_EmissionColor", newColor);
        });

        _currColor = newColor;
    }
}
