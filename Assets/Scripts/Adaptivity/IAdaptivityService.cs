namespace Aci.Unity.Adaptivity
{
    public struct AdaptivityLevelChangedEventArgs
    {
        public byte previousLevel;
        public byte newLevel;
    }

    public delegate void AdaptivityLevelChangedDelegate(AdaptivityLevelChangedEventArgs args);

    public interface IAdaptivityService
    {
        /// <summary>
        /// The maximum adaptivity level that can be reached.
        /// </summary>
        byte maxLevel { get; }

        /// <summary>
        /// The minimum adaptivity level that can be reached.
        /// </summary>
        byte minLevel { get; }

        /// <summary>
        /// The current adaptivity level.
        /// </summary>
        byte level { get; set; }

        /// <summary>
        /// Called when the adaptivity level changes.
        /// </summary>
        event AdaptivityLevelChangedDelegate adaptivityLevelChanged;
    }
}
