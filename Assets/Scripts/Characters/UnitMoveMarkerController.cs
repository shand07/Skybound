using Skybound.UI;
using UnityEngine;

namespace Skybound.Characters
{
    public class UnitMoveMarkerController : MonoBehaviour
    {
        private MoveMarker currentMarker;

        public void SetMarker(MoveMarker newMarker)
        {
            ClearMarker();
            currentMarker = newMarker;
        }

        public void ClearMarker()
        {
            if (currentMarker != null)
            {
                Destroy(currentMarker.gameObject);
                currentMarker = null;
            }
        }
    }
}