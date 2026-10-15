namespace MiraculousSurfers.Gameplay.Upgrades
{
    public readonly struct UpgradePurchase
    {
        public bool Applied { get; }
        public bool Persisted { get; }
        public int NewIndex { get; }
        public float NewValue { get; }

        public UpgradePurchase(bool applied, bool persisted, int newIndex, float newValue)
        {
            Applied = applied;
            Persisted = persisted;
            NewIndex = newIndex;
            NewValue = newValue;
        }
    }
}
