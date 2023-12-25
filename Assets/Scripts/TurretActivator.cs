using MobFarm;
using UnityEngine;

public class TurretActivator : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private MF_EasyTurret turret;
    [SerializeField] private float minHeight;
    [SerializeField] private float distance;

    private bool hasTarget;

    private void Update()
    {
        if (Vector3.Distance(target.position, turret.transform.position) < distance && target.position.y > minHeight)
        {
            if (hasTarget == false)
            {
                turret.SetTarget(target);
                hasTarget = true;
            }
        }
        else if (hasTarget == true)
        {
            turret.SetTarget(null);
            hasTarget = false;
        }
    }
}