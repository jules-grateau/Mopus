using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Controller.Combat.UI
{
    public class UnitInfoUIManager : MonoBehaviour
    {

        Canvas _canvas;

        // Use this for initialization
        void Awake()
        {
            var _canvas = FindObjectOfType<Canvas>();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}