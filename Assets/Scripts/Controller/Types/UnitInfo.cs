using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Controller.Types
{
    [System.Serializable]
    public class UnitInfo 
    {
        public string Name { get { return _name; } }

        [SerializeField]
        private string _name;
        public CombatStatistics Stats { get { return _stats; } }

        [SerializeField]
        private CombatStatistics _stats;

        public bool IsPlayerControlled { get { return _isPlayerControlled; } }

        [SerializeField]
        private bool _isPlayerControlled;

        public int TeamId { get { return _teamId; } }

        [SerializeField]
        private int _teamId;
    }
}