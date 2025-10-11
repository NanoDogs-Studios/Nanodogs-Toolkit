using System;
using UnityEngine;

namespace Nanodogs.API.NanoMissions
{
    [CreateAssetMenu(fileName = "NewNanoMission", menuName = "Nanodogs/Nano Mission", order = 1)]
    public class NanoMission : ScriptableObject
    {
        [Header("Basic Info")]
        public string missionName;
        [TextArea] public string description;

        [Header("Mission State")]
        public MissionState state = MissionState.NotStarted;

        [Header("Objectives")]
        public MissionObjective[] objectives;

        public event Action<NanoMission> OnMissionStarted;
        public event Action<NanoMission> OnMissionCompleted;
        public event Action<NanoMission> OnMissionFailed;

        public virtual void StartMission()
        {
            if (state != MissionState.NotStarted) return;
            state = MissionState.InProgress;

            Debug.Log($"Mission '{missionName}' started.");

            // Activate all objectives
            foreach (var obj in objectives)
                obj.Activate();

            OnMissionStarted?.Invoke(this);
        }

        public virtual void CompleteMission()
        {
            if (state != MissionState.InProgress) return;
            state = MissionState.Completed;

            Debug.Log($"Mission '{missionName}' completed!");
            OnMissionCompleted?.Invoke(this);
            GiveRewards();
        }

        public virtual void FailMission()
        {
            if (state != MissionState.InProgress) return;
            state = MissionState.Failed;

            Debug.Log($"Mission '{missionName}' failed.");
            OnMissionFailed?.Invoke(this);
        }

        public void CheckObjectives()
        {
            if (state != MissionState.InProgress) return;

            bool allCompleted = true;
            foreach (var obj in objectives)
            {
                if (!obj.CheckProgress())
                    allCompleted = false;
            }

            if (allCompleted)
                CompleteMission();
        }

        protected virtual void GiveRewards()
        {
            // Hook into your inventory or XP system here
        }
    }

    public enum MissionState
    {
        NotStarted,
        InProgress,
        Completed,
        Failed
    }
}
