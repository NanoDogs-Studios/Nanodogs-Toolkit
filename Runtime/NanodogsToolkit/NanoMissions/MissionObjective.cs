using UnityEngine;

namespace Nanodogs.API.NanoMissions
{
    public enum ObjectiveState { Inactive, Active, Completed }

    public abstract class MissionObjective : ScriptableObject
    {
        public string objectiveName;
        public ObjectiveState state = ObjectiveState.Inactive;

        public virtual void Activate()
        {
            state = ObjectiveState.Active;
        }

        public virtual void Complete()
        {
            state = ObjectiveState.Completed;
        }

        public abstract bool CheckProgress();
    }
}
