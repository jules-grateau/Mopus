using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputController : MonoBehaviour
{
    [SerializeField]
    GameEvent mouseClickGameEvent;
    [SerializeField]
    GameEvent mouseHoverGameEvent;

    private void Update()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(cameraRay, out hitInfo, 1000))
        {
            mouseHoverGameEvent.Raise(this, hitInfo);
            if (Input.GetMouseButton(0))
            {
                mouseClickGameEvent.Raise(this, hitInfo);
            }
        } else
        {
            mouseHoverGameEvent.Raise(this, null);
        }
        
    }
}
