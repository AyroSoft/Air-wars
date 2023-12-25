using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissileTargeting : MonoBehaviour {

    [SerializeField] private ScreenButton fireBtn;

    [SerializeField]
    private Missile[] missiles;
    [SerializeField]
    private Color noTargetColor, foundTargetColor;
    [SerializeField]
    private new Camera camera;
    [SerializeField]
    private Image targetReticle;
    [SerializeField]
    private Text missilesLeft;
    [SerializeField]
    private float targetingTime = 3f;

    private Transform oldTarget;
    private Transform currentTarget;
    private int currentMissile;
    private float targetTimer;

    private void Start()
    {
        missilesLeft.text = string.Format("{0}/{1}", missiles.Length - currentMissile, missiles.Length);
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    void Update () {
        //Find a field of targets
        GameObject[] targets = GameObject.FindGameObjectsWithTag("MissileTarget");
        //Select the closest one
        currentTarget = null;
        float minDist = float.PositiveInfinity;
        Vector3 targetCenter = new Vector3(0.5f, 0.5f, 0f);
        foreach (GameObject target in targets)
        {
            Vector3 targetPos = WorldToScreenPosition(target.transform.position);
            float dist = Vector3.Distance(targetCenter, targetPos);
            if (targetPos.z > 0 && dist < minDist)
            {
                currentTarget = target.transform;
                minDist = dist;
            }
        }

        //When a target changes
        if(oldTarget != currentTarget)
        {
            oldTarget = currentTarget;
            targetTimer = targetingTime;
        }
        
        if(currentTarget != null)
        {
            targetTimer -= Time.deltaTime;
            if (targetTimer <= 0)
            {
                if (fireBtn != null)
                {
                    if (fireBtn.IsPressed && currentMissile < missiles.Length)
                    {
                        while (missiles[currentMissile] == null)
                        {
                            currentMissile++;
                        }
                        missiles[currentMissile].Launch(currentTarget);
                        currentMissile++;
                        missilesLeft.text = string.Format("{0}/{1}", missiles.Length - currentMissile, missiles.Length);
                    }
                }
                else if (Input.GetButtonDown("Fire2") && currentMissile < missiles.Length)
                {
                    while (missiles[currentMissile] == null)
                    {
                        currentMissile++;
                    }
                    missiles[currentMissile].Launch(currentTarget);
                    currentMissile++;
                    missilesLeft.text = string.Format("{0}/{1}", missiles.Length - currentMissile, missiles.Length);

                }
            }
            
        }
    }

    //If the graphics are not also dealt with in FixedUpdate, it causes choppiness
    private void FixedUpdate()
    {
        UpdateReticle();
    }
    void UpdateReticle()
    {
        if (currentTarget != null)
        {
            targetReticle.enabled = true;
            if (targetTimer > 0)
            {
                targetReticle.color = noTargetColor;
                targetReticle.rectTransform.anchoredPosition = WorldToScreenPosition(currentTarget.position);
                targetReticle.rectTransform.anchoredPosition += new Vector2(Mathf.PerlinNoise(5f, Time.time) - .5f, Mathf.PerlinNoise(30f, Time.time) - .5f) * 100f * Mathf.Clamp(targetTimer, 0f, 1f);
            }
            else
            {
                targetReticle.color = foundTargetColor;
                targetReticle.rectTransform.anchoredPosition = WorldToScreenPosition(currentTarget.position);
            }
        }
        else
        {
            targetReticle.enabled = false;
        }
    }
    Vector3 WorldToScreenPosition(Vector3 worldPos)
    {

        //first you need the RectTransform component of your canvas
        RectTransform CanvasRect = targetReticle.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        Vector3 ViewportPosition = camera.WorldToViewportPoint(worldPos);
        Vector3 WorldObject_ScreenPosition = new Vector3(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)),
        ViewportPosition.z);

        //now you can set the position of the ui element
        return WorldObject_ScreenPosition;
    }
}
