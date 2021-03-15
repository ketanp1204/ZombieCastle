
public static class PlayerStats
{
    public enum PlayerState
    {
        Idle,
        Combat,
        Attacking,
        TakingDamage
    }

    public static int initialHealth = 100;
    public static int currentHealth;
    public static bool IsDead;
    public static bool isFirstScene = true;
    public static PlayerState playerState = PlayerState.Idle;
}
