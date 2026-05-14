using Skybound.Characters;
using UnityEngine;

namespace Skybound.Systems.FogOfWar
{
    [RequireComponent(typeof(UnitIdentity))]
    public class VisionSource : MonoBehaviour
    {
        [SerializeField] private float visionRange = 8f;

        private UnitIdentity unitIdentity;

        public float VisionRange => visionRange;

        private void Awake()
        {
            unitIdentity = GetComponent<UnitIdentity>();
        }

        public bool CanRevealFog()
        {
            return unitIdentity != null && unitIdentity.CanBeSelectedByPlayer();
        }
    }
}