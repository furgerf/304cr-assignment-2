namespace Ai2dShooter.Model
{
    /// <summary>
    /// Represents all data necessary for a DtPlayer to make a decision.
    /// </summary>
    public struct DecisionData
    {
        #region Public Fields

        // input data
        public bool IsHealthy { get; private set; }

        public bool HasAmmo { get; private set; }

        public bool IsEnemyInRange { get; private set; }

        // output data
        public DtPlayer.DecisionType Decision { get; private set; }

        #endregion

        #region Constructor

        public DecisionData(bool isHealthy, bool hasAmmo, bool isEnemyInRange, DtPlayer.DecisionType decision) : this()
        {
            Decision = decision;
            IsEnemyInRange = isEnemyInRange;
            HasAmmo = hasAmmo;
            IsHealthy = isHealthy;
        }

        #endregion
    }
}
