using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    // ─── Base ───────────────────────────────────────────────
    [Header("Base Variables")]
    [SerializeField] GameObject[] projectilePrefabs;
    [SerializeField] GameObject aiProjectilePrefab;
    [SerializeField] float projectileSpeed = 10f;
    [SerializeField] float projectileLifetime = 5f;
    [SerializeField] float baseFireRate = 0.2f;
    [SerializeField] float shootDirection = 1f; // 1 = up (player), -1 = down (enemy)

    [Header("Switch Weapon")]
    [SerializeField] KeyCode switchProjectileKey = KeyCode.Q;

    // ─── AI ─────────────────────────────────────────────────
    [Header("AI Variables")]
    [SerializeField] float minimumFireRate = 0.2f;
    [SerializeField] float fireRateVariance = 0f;
    [SerializeField] bool useAI;

    [Header("AI Burst Settings")]
    [SerializeField] int minBurstCount = 3;       // min bullets per burst
    [SerializeField] int maxBurstCount = 8;       // max bullets per burst
    [SerializeField] float minBurstPause = 0.5f;  // pause between bursts
    [SerializeField] float maxBurstPause = 2f;

    [Header("Pattern Settings")]
    [SerializeField] int spreadBulletCount = 5;         // bullets in fan spread
    [SerializeField] float spreadAngle = 90f;           // total fan angle
    [SerializeField] int ringBulletCount = 12;          // bullets in full ring
    [SerializeField] int spiralBulletCount = 20;        // total spiral bullets
    [SerializeField] float spiralAngleStep = 20f;       // degrees per spiral bullet
    [SerializeField] float waveAmplitude = 45f;         // wave side-to-side angle
    [SerializeField] float waveFrequency = 3f;          // wave oscillations per burst
    [SerializeField] int scatterBulletCount = 10;       // random scatter count
    [SerializeField] float scatterAngleRange = 180f;    // scatter cone width

    [HideInInspector] public bool isFiring;

    // ─── Private ─────────────────────────────────────────────
    private int currentProjectileIndex = 0;
    private Coroutine fireCoroutine;
    private AudioManager audioManager;
    private Transform playerTransform;

    enum FirePattern { AimedAtPlayer, Ring, Wave, RandomScatter, Spread, Spiral }

    // ─────────────────────────────────────────────────────────

    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (useAI) isFiring = true;
    }

    void Update()
    {
        if (!useAI && Keyboard.current.qKey.wasPressedThisFrame)
            SwitchProjectile();

        Fire();
    }

    // ─── Fire control ─────────────────────────────────────────

    void Fire()
    {
        if (isFiring && fireCoroutine == null)
            fireCoroutine = StartCoroutine(useAI ? AIFireLoop() : PlayerFireLoop());
        else if (!isFiring && fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
            fireCoroutine = null;
        }
    }

    // ─── Player: unchanged single-bullet stream ───────────────

    IEnumerator PlayerFireLoop()
    {
        while (true)
        {
            int index = Mathf.Clamp(currentProjectileIndex, 0, projectilePrefabs.Length - 1);
            SpawnBullet(projectilePrefabs[index], transform.up * projectileSpeed);
            audioManager?.PlayShootingSFX();

            float wait = Mathf.Clamp(
                Random.Range(baseFireRate - fireRateVariance, baseFireRate + fireRateVariance),
                minimumFireRate, float.MaxValue);
            yield return new WaitForSeconds(wait);
        }
    }

    // ─── AI: random pattern each burst ───────────────────────

    IEnumerator AIFireLoop()
    {
        while (true)
        {
            // Pick a random pattern and fire mode each burst
            FirePattern pattern = (FirePattern)Random.Range(0, System.Enum.GetValues(typeof(FirePattern)).Length);
            bool doBurst = Random.value > 0.5f; // 50/50 continuous vs burst

            yield return StartCoroutine(doBurst
                ? FireBurst(pattern)
                : FireContinuousSegment(pattern));

            // Pause between bursts
            float pause = Random.Range(minBurstPause, maxBurstPause);
            yield return new WaitForSeconds(pause);
        }
    }

    // Fires a fixed count of bullets then stops
    IEnumerator FireBurst(FirePattern pattern)
    {
        int count = Random.Range(minBurstCount, maxBurstCount + 1);
        yield return StartCoroutine(ExecutePattern(pattern, count));
    }

    // Fires continuously for a random duration
    IEnumerator FireContinuousSegment(FirePattern pattern)
    {
        float duration = Random.Range(1f, 3f);
        float elapsed = 0f;
        int bulletsFired = 0;

        while (elapsed < duration)
        {
            yield return StartCoroutine(ExecutePattern(pattern, 1, bulletsFired));
            bulletsFired++;

            float wait = Mathf.Clamp(
                Random.Range(baseFireRate - fireRateVariance, baseFireRate + fireRateVariance),
                minimumFireRate, float.MaxValue);
            elapsed += wait;
            yield return new WaitForSeconds(wait);
        }
    }

    // ─── Pattern dispatcher ───────────────────────────────────

    IEnumerator ExecutePattern(FirePattern pattern, int bulletCount, int bulletIndex = 0)
    {
        switch (pattern)
        {
            case FirePattern.AimedAtPlayer:   yield return StartCoroutine(PatternAimed(bulletCount));        break;
            case FirePattern.Ring:            yield return StartCoroutine(PatternRing());                    break;
            case FirePattern.Wave:            yield return StartCoroutine(PatternWave(bulletCount, bulletIndex)); break;
            case FirePattern.RandomScatter:   yield return StartCoroutine(PatternScatter(bulletCount));      break;
            case FirePattern.Spread:          yield return StartCoroutine(PatternSpread());                  break;
            case FirePattern.Spiral:          yield return StartCoroutine(PatternSpiral(bulletCount));       break;
        }
    }

    // ─── Patterns ─────────────────────────────────────────────

    // Fires straight at the player's current position
    IEnumerator PatternAimed(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = GetDirectionToPlayer();
            SpawnBullet(aiProjectilePrefab, dir * projectileSpeed);
            audioManager?.PlayShootingSFX();

            float wait = Mathf.Clamp(
                Random.Range(baseFireRate - fireRateVariance, baseFireRate + fireRateVariance),
                minimumFireRate, float.MaxValue);
            yield return new WaitForSeconds(wait);
        }
    }

    // Fires bullets evenly in a full 360° ring
    IEnumerator PatternRing()
    {
        float angleStep = 360f / ringBulletCount;
        for (int i = 0; i < ringBulletCount; i++)
        {
            float angle = i * angleStep;
            Vector2 dir = AngleToDirection(angle);
            SpawnBullet(aiProjectilePrefab, dir * projectileSpeed);
        }
        audioManager?.PlayShootingSFX();
        yield return null;
    }

    // Oscillates left/right like a sine wave aimed at the player
    IEnumerator PatternWave(int count, int startIndex)
    {
        for (int i = 0; i < count; i++)
        {
            float t = (startIndex + i) / (float)Mathf.Max(count, 1);
            float sineOffset = Mathf.Sin(t * waveFrequency * Mathf.PI * 2f) * waveAmplitude;

            Vector2 baseDir = GetDirectionToPlayer();
            float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
            Vector2 dir = AngleToDirection(baseAngle + sineOffset);

            SpawnBullet(aiProjectilePrefab, dir * projectileSpeed);
            audioManager?.PlayShootingSFX();

            float wait = Mathf.Clamp(
                Random.Range(baseFireRate - fireRateVariance, baseFireRate + fireRateVariance),
                minimumFireRate, float.MaxValue);
            yield return new WaitForSeconds(wait);
        }
    }

    // Fires bullets randomly within a cone facing the player
    IEnumerator PatternScatter(int count)
    {
        Vector2 baseDir = GetDirectionToPlayer();
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        for (int i = 0; i < count; i++)
        {
            float randomAngle = baseAngle + Random.Range(-scatterAngleRange / 2f, scatterAngleRange / 2f);
            Vector2 dir = AngleToDirection(randomAngle);
            SpawnBullet(aiProjectilePrefab, dir * projectileSpeed);
        }
        audioManager?.PlayShootingSFX();
        yield return null;
    }

    // Fires a fan of bullets spread evenly across an arc toward the player
    IEnumerator PatternSpread()
    {
        Vector2 baseDir = GetDirectionToPlayer();
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        float halfSpread = spreadAngle / 2f;
        float step = spreadAngle / (spreadBulletCount - 1);

        for (int i = 0; i < spreadBulletCount; i++)
        {
            float angle = baseAngle - halfSpread + step * i;
            Vector2 dir = AngleToDirection(angle);
            SpawnBullet(aiProjectilePrefab, dir * projectileSpeed);
        }
        audioManager?.PlayShootingSFX();
        yield return null;
    }

    // Fires bullets in a spinning spiral pattern
    IEnumerator PatternSpiral(int count)
    {
        float currentAngle = 0f;
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = AngleToDirection(currentAngle);
            SpawnBullet(aiProjectilePrefab, dir * projectileSpeed);
            audioManager?.PlayShootingSFX();
            currentAngle += spiralAngleStep;

            float wait = Mathf.Clamp(
                Random.Range(baseFireRate - fireRateVariance, baseFireRate + fireRateVariance),
                minimumFireRate, float.MaxValue);
            yield return new WaitForSeconds(wait);
        }
    }

    // ─── Helpers ──────────────────────────────────────────────

    void SpawnBullet(GameObject prefab, Vector2 velocity)
    {
        GameObject projectile = Instantiate(prefab, transform.position, transform.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = velocity;
        Destroy(projectile, projectileLifetime);
    }

    Vector2 GetDirectionToPlayer()
    {
        if (playerTransform != null)
            return (playerTransform.position - transform.position).normalized;
        return Vector2.down * shootDirection; // fallback if no player found
    }

    // Converts a degree angle to a 2D direction vector
    Vector2 AngleToDirection(float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    void SwitchProjectile()
    {
        if (useAI || projectilePrefabs.Length == 0) return;
        currentProjectileIndex = (currentProjectileIndex + 1) % projectilePrefabs.Length;
        Debug.Log("Switched to projectile: " + currentProjectileIndex);
    }
}