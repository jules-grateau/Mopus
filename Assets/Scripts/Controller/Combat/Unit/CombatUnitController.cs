using Assets.Scripts.Controller.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts.Controller
{
    using CustomEvents;

    public class CombatUnitController : MonoBehaviour
    {

        public UnitInfo UnitInfo { get { return _unitInfo; } }
        [SerializeField]
        private UnitInfo _unitInfo;

        bool _isMoving = false;
        int _damageTaken = 0;
        GameObject _floatingTextPrefabs;

        private void Awake()
        {
            _floatingTextPrefabs = (GameObject) Resources.Load("Prefabs/UI/FloatingText");
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
                CustomEvents.UnitMovementPreviewEvent.Invoke(path);
            }
            _isMoving = false;
            CustomEvents.UnitMovementStatusEvent.Invoke(gameObject.GetInstanceID(),false);
        }

        void OnUnitTakeDamage(int instanceId, int damage)
        {
            if (instanceId != gameObject.GetInstanceID()) return;
            
            _damageTaken += damage;
            GameObject floatingDamageText = Instantiate(_floatingTextPrefabs,transform);
            floatingDamageText.GetComponent<FloatingCombatTextAnimation>().Init(damage);
        }
    }
}