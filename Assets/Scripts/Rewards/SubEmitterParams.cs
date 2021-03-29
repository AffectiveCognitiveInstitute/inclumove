namespace Aci.Unity.Gamification.Rewards
{
    public struct SubEmitterParams
    {
        public int particlesTier1Count;
        public int particlesTier2Count;
        public int particlesTier3Count;

        public SubEmitterParams(int particlesTier1Count, int particlesTier2Count, int particlesTier3Count)
        {
            this.particlesTier1Count = particlesTier1Count;
            this.particlesTier2Count = particlesTier2Count;
            this.particlesTier3Count = particlesTier3Count;
        }
    }
}
