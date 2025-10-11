using UnityEngine;

namespace Nanodogs.API.NanoMissions
{
    [CreateAssetMenu(fileName = "NewGoToPointObjective", menuName = "Nanodogs/Missions/Objective - Go To Point")]
    public class GoToPointObjective : MissionObjective
    {
        [Header("Target")]
        public Vector3 targetPosition;

        [Header("Settings")]
        public float radius = 3f;

        private Transform player;

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        public override bool CheckProgress()
        {
            if (state != ObjectiveState.Active || player == null) return false;

            float distance = Vector3.Distance(player.position, targetPosition);
            if (distance <= radius)
            {
                Complete();
                return true;
            }
            return state == ObjectiveState.Completed;
        }
    }
}
