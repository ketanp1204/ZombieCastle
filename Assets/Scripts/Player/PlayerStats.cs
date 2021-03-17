
public static class PlayerStats
{
    public enum PlayerState
    {
        Idle,
        Combat,
        Attacking,
        TakingDamage
    }

    public static int initialHealth;
    public static int currentHealth;
    public static bool IsDead;
    public static bool isFirstScene;
    public static PlayerState playerState;

    public static void Initialize()
    {
        initialHealth = 100;
        IsDead = false;
        isFirstScene = true;
        playerState = PlayerState.Idle;
    }
}
