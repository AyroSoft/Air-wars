using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MachineGun : MonoBehaviour {

    private float nextFire;
    private Vector3 originalGunAngle;

    [SerializeField] private FixedTouchField fireButton;

    [SerializeField]
    private int ammo = 1200;
    [SerializeField]
    private float fireRate = .5f;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private Text bulletsAmountUI;
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private bool aims;
    [SerializeField]
    private float maxAimAngle;
    [SerializeField]
    private new AudioSource audio;

	// Use this for initialization
	void Start () {
        originalGunAngle = transform.localEulerAngles;
        bulletsAmountUI.text = ammo.ToString();
        audio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {

        if (fireButton != null)
        {
            if (fireButton?.IsRightPressed == true && Time.time > nextFire && ammo > 0)
            {
                if (audio != null && !audio.isPlaying)
                {
                    audio.Play();
                }
                nextFire = Time.time + fireRate;
                ammo--;
                bulletsAmountUI.text = ammo.ToString();
                Instantiate(bulletPrefab, transform.position, transform.rotation);
            }
            if (fireButton?.IsRightPressed == false)
            {
                if (audio != null && audio.isPlaying)
                {
                    audio.Stop();
                }
            }
        }
        else
        {
            if (Input.GetButton("Fire1") && Time.time > nextFire && ammo > 0)
            {
                if (audio != null && !audio.isPlaying)
                {
                    audio.Play();
                }
                nextFire = Time.time + fireRate;
                ammo--;
                bulletsAmountUI.text = ammo.ToString();
                Instantiate(bulletPrefab, transform.position, transform.rotation);
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (audio != null && audio.isPlaying)
                {
                    audio.Stop();
                }
            }
        }
        if(aims)
        {
            transform.localEulerAngles = originalGunAngle;
            Vector3 targetPosition = camera.transform.position + (camera.transform.forward * 300f);
            Quaternion lookRot = Quaternion.LookRotation(targetPosition - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRot, maxAimAngle);
        }
	}
}
