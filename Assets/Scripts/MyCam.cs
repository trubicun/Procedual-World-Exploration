using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCam : MonoBehaviour
{
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _lookOffset;
    [SerializeField] private float _sensetivityX;
    [SerializeField] private float _sensetivityY;

    private Vector3 _look;
    private float mouseX = 0;
    private float mouseY = 0;

    private void Update()
    {
        _look = _lookTarget.position + _lookOffset;

        transform.localPosition = _lookTarget.transform.position + _positionOffset;



        mouseX += Input.GetAxis("Mouse X") * _sensetivityX;
        mouseY += Input.GetAxis("Mouse Y") * _sensetivityY;

        transform.LookAt(_look);

        transform.RotateAround(_lookTarget.position, Vector3.up, mouseX);
        transform.RotateAround(_lookTarget.position, Vector3.left, Mathf.Clamp(mouseY,0,40));
    }
}
