using Assets.Scripts.Controller.Combat.UI;
using Assets.Scripts.Controller.Types;
using Assets.Scripts.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Controller
{
    public class CombatUnitController : MonoBehaviour
    {

        public UnitInfo UnitInfo { get { return _unitInfo; } }
        [SerializeField]
        private UnitInfo _unitInfo;

        bool _isMoving = false;
        int _currHp = 0; 
        //TODO : Externalize these kind of UI visualisation
        GameObject _floatingTextPrefabs;
        GameObject _unitInfoUI;

        GameObject _unitInfoUIInstance;

        Canvas _canvas;

        private void Awake()
        {
            _canvas = FindObjectOfType<Canvas>();
            _floatingTextPrefabs = Resources.Load<GameObject>("Prefabs/UI/FloatingText");
            _unitInfoUI = Resources.Load<GameObject>("Prefabs/UI/UnitInfo");
            _currHp = _unitInfo.Stats.Health;
        }


        private void OnEnable()
        {
            CustomEvents.UnitMovementEvent.AddListener(MoveTo);
            CustomEvents.DamageUnitEvent.AddListener(OnUnitTakeDamage);
        }

        private void OnDisable()
        {
            CustomEvents.UnitMovementEvent.RemoveListener(MoveTo);
            CustomEvents.DamageUnitEvent.RemoveListener(OnUnitTakeDamage);
        }

        private void MoveTo(int instanceId, List<Vector3> path)
        {
            if (gameObject.GetInstanceID() != instanceId) return;
            if (_isMoving) return;
            StartCoroutine(MoveThrought(path));
        }

        IEnumerator MoveThrought(List<Vector3> path)
        {
            _isMoving = true;
            CustomEvents.UnitMovementStatusEvent.Invoke(gameObject.GetInstanceID(),true);
            while (path.Count > 0)
            {
                Vector3 tile = path[0];
                transform.position = new Vector3(tile.x, transform.position.y, tile.z);
                path.Remove(tile);
                yield return new WaitForSeconds(0.1f);
            }
            _isMoving = false;
            CustomEvents.UnitMovementStatusEvent.Invoke(gameObject.GetInstanceID(),false);
        }

        void OnUnitTakeDamage(int instanceId, int damage)
        {
            if (instanceId != gameObject.GetInstanceID()) return;

            _currHp -= damage;
            GameObject floatingDamageText = Instantiate(_floatingTextPrefabs,transform);
            floatingDamageText.GetComponent<FloatingCombatTextAnimation>().Init(damage);

            _unitInfoUIInstance.GetComponent<UnitInfoUIController>().UpdateCurrHp(_currHp);
        }

        private void OnMouseEnter()
        {
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(transform.position + transform.localScale);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.GetComponent<RectTransform>(), screenPoint, null, out canvasPos);

            if (!_unitInfoUIInstance) {
                _unitInfoUIInstance = Instantiate(_unitInfoUI, _canvas.transform);
                _unitInfoUIInstance.GetComponent<UnitInfoUIController>().Init(UnitInfo.Stats.Health, _currHp, UnitInfo.Name);
            } else
            {
                _unitInfoUIInstance.SetActive(true);
                _unitInfoUIInstance.GetComponent<UnitInfoUIController>().UpdateCurrHp(_currHp);
            }

            _unitInfoUIInstance.transform.localPosition = canvasPos;
            
        }

        private void OnMouseExit()
        {
            _unitInfoUIInstance.SetActive(false);
        }
    }
}