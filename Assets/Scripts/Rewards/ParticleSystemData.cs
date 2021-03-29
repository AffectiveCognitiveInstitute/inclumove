using Aci.Unity.Data;

namespace Aci.Unity.Gamification.Rewards
{
    [System.Serializable]
    public struct ParticleSystemData
    {
        public RewardData reward;
        public BadgeSubemitterPrefabs amountPrefabs;
        public BadgeSubemitterPrefabs speedPrefabs;
        public BadgeSubemitterPrefabs streakPrefabs;
    }
}
