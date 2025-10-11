using Nanodogs.UniversalScripts;
using System.Collections.Generic;
using UnityEngine;

namespace Nanodogs.API.NanoMissions
{
    public class MissionManager : MonoBehaviour
    {
        public Transform player;
        public NanoMission firstMission;
        public List<NanoMission> activeMissions = new List<NanoMission>();

        public void StartMission(NanoMission mission)
        {
            mission.StartMission();

            // Assign player to GoTo objectives (optional but handy)
            foreach (var obj in mission.objectives)
            {
                if (obj is GoToPointObjective goTo)
                    goTo.SetPlayer(player);
            }

            activeMissions.Add(mission);
        }

        private void Update()
        {
            if (player == null && Camera.main != null)
                player = Camera.main.transform;

            if (player != null)
            {
                StartMission(firstMission);
            }

            foreach (var mission in activeMissions)
                mission.CheckObjectives();
        }
    }
}
