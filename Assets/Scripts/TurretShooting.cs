using MobFarm;
using System.Collections;
using UnityEngine;

public class TurretShooting : MonoBehaviour, ITurretEvents
{
    [SerializeField] private Transform source;
    [SerializeField] private Rigidbody bulletPrefab;
    [SerializeField] private float maxDistance = 500f;
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float bulletVelocity = 50f;
    [SerializeField] private int bulletCount = 10;
    [SerializeField] private float reloadDuration = 5f;

    [Range(0.001f, 0.1f)]
    [SerializeField] private float spread = 0.001f;

    private MF_EasyTurret turret;
    private bool canFire;
    private bool isFire;

    private void Awake()
    {
        canFire = true;
        turret = GetComponent<MF_EasyTurret>();
        turret.AddEventTarget(this);
    }

    private void Update()
    {
        if (isFire && canFire)
        {
            StartCoroutine(Shoot());
        }
    }

    public void TurretEvent(TurretEventType eventType)
    {
        if (eventType == TurretEventType.GainedTarget)
            isFire = true;
        else
            isFire = false;
    }

    private IEnumerator Shoot()
    {
        canFire = false;
        for (int i = 0; i < bulletCount; i++)
        {
            if (isFire == false)
                break;

            var direction = source.transform.forward + spread * UnityEngine.Random.insideUnitSphere;

            var bullet = Instantiate(bulletPrefab);
            bullet.transform.position = source.position + source.forward;
            bullet.velocity = bulletVelocity * direction.normalized;

            yield return new WaitForSeconds(1f / fireRate);
        }
        yield return new WaitForSeconds(reloadDuration);
        canFire = true;
    }
}
