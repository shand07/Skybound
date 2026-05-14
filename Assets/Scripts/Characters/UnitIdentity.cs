using UnityEngine;

namespace Skybound.Characters
{
    public enum UnitFaction
    {
        PlayerParty,
        AllyGuest,
        Neutral,
        Enemy
    }

    public class UnitIdentity : MonoBehaviour
    {
        [SerializeField] private UnitFaction faction;
        [SerializeField] private bool isPlayerControllable;

        public UnitFaction Faction => faction;
        public bool IsPlayerControllable => isPlayerControllable;

        public bool CanBeSelectedByPlayer()
        {
            return faction == UnitFaction.PlayerParty && isPlayerControllable;
        }

        public bool IsHostileToPlayer()
        {
            return faction == UnitFaction.Enemy;
        }

        public void TurnHostile()
        {
            faction = UnitFaction.Enemy;
            isPlayerControllable = false;
        }
    }
}