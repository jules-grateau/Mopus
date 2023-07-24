using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.UI
{
    public class FollowMouse : MonoBehaviour
    {
        [SerializeField]
        Vector3 _offset;

        // Update is called once per frame
        void Update()
        {
            transform.position = Input.mousePosition + _offset;
        }
    }
}