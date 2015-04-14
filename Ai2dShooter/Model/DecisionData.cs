namespace Ai2dShooter.Model
{
    public struct DecisionData
    {
        #region Public Fields

        public bool IsHealthy { get; private set; }

        public bool HasAmmo { get; private set; }

        public bool IsEnemyInRange { get; private set; }

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
