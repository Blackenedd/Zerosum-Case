using Dreamteck.Splines;
using UnityEngine;

public class SplineFollow : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private float distance = 0;
    [SerializeField] private float height = 0;
    [SerializeField] [Range(-1, 1)] private float x = 0;

    [SerializeField] private SplineComputer spline;

    private float offsetX = 0;

    private void Awake()
    {
        Construct();
    }
    private void Update()
    {
        Movement();
    }
    private void Construct()
    {
        SplineSample ss = spline.Evaluate(spline.Travel(0, distance));
        transform.position = ss.position + ss.right * offsetX;
    }
    private void Movement()
    {
        distance += speed * Time.deltaTime;

        SplineSample ss = spline.Evaluate(spline.Travel(0, distance));

        offsetX += x* Time.deltaTime * 35f;

        offsetX = Mathf.Clamp(offsetX, -1.65f, 1.65f);

        transform.position = ss.position + ss.right * offsetX + ss.up * height;
    }
}
