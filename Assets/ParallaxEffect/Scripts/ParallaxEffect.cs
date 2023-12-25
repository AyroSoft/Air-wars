using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField]
    private float Border = 50f;                     //The border of an gameObject which it can't cross.
    [SerializeField]
    public int ParallaxLerpPower = 20;              //The speed of lineary interpolation between GameObject local position and default postition.
    [SerializeField]
    public int ParallaxSpeed = 140;                 //The speed of change of the position of the vector.
    [SerializeField]
    public int DirectionLerpPower = 5;              //The speed of lineary interpolation between previous and current direction vector.

    private Vector3 _previousDir;
    private Vector3 _startPosition;
    private RectTransform _imageTransfrom;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        ParallaxEffectAction();
    }

    /// <summary>
    /// Set image borders and get start parallax position from device accelerometer.
    /// </summary>
    private void Initialize()
    {
        _imageTransfrom = GetComponent<RectTransform>();
        _imageTransfrom.offsetMin = new Vector2(-Border, -Border);
        _imageTransfrom.offsetMax = new Vector2(Border, Border);
        _startPosition = -Input.acceleration;
    }

    /// <summary>
    /// Method call every frame in Update. It change position of image depending on acceleration value and return to start position.
    /// </summary>
    private void ParallaxEffectAction()
    {
        //Deduct current acceleration from start
        Vector3 direction = -Input.acceleration - _startPosition;
        direction.z = 0;

        //Normilize direction vector
        if (direction.sqrMagnitude > 1)
            direction.Normalize();

        //Add smoothing between previous and current direction vector
        direction = Vector3.Lerp(_previousDir, direction, Time.deltaTime * DirectionLerpPower);

        //Lerp GameObject local position and default postition
        gameObject.transform.localPosition = Vector3.Lerp(
                                                        gameObject.transform.localPosition,
                                                        new Vector3(direction.x * ParallaxSpeed, direction.y * ParallaxSpeed, 0),
                                                        Time.deltaTime * ParallaxLerpPower);

        //Add borders to gameObject transform
        gameObject.transform.localPosition = new Vector3(
                                                        Mathf.Clamp(gameObject.transform.localPosition.x, -Border, Border),
                                                        Mathf.Clamp(gameObject.transform.localPosition.y, -Border, Border),
                                                        0);

        _previousDir = direction;
    }
}
