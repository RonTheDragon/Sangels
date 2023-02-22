using Cinemachine;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private AxisState _xAxis;
    [SerializeField] private AxisState _yAxis;
    [SerializeField] Transform _rotated;
    private InputHandler _inputHandler => GetComponent<InputHandler>();

    private void Start()
    {
        _xAxis.SetInputAxisProvider(0,_inputHandler);
        _yAxis.SetInputAxisProvider(1, _inputHandler);
    }

    // Update is called once per frame
    private void Update()
    {
        _xAxis.Update(Time.fixedDeltaTime);
        _yAxis.Update(Time.fixedDeltaTime);
        _rotated.eulerAngles = new Vector3(_yAxis.Value, _xAxis.Value, 0);
    }
}
