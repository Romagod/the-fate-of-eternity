using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    public float speedMove;
    public float speedRotate;
    public bool rotating = false;
    public bool moving = false;
    public bool reverse = false;
    private Transform _rotator;

    // Start is called before the first frame update
    void Start()
    {
        _rotator = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rotating)
        {
            _Rotate();
        }
        if (moving)
        {
            _Move();
        }
        if (!rotating && !moving && reverse)
        {
            reverse = false;
        }
    }

    public void StartAnimation()
    {
        rotating = true;
        moving = true;
    }

    public void StartReverseAnimation()
    {
        rotating = true;
        moving = true;
        reverse = true;
    }

    private void _Rotate()
    {
        bool condition = _rotator.rotation.x >= 0.21f;
        
        if (reverse)
        {
            condition = _rotator.rotation.x <= 0f;
        }

        if (condition)
        {
            rotating = false;
        }
        else
        {
            if (reverse)
            {
                _rotator.Rotate(-(speedRotate * Time.deltaTime), 0, 0);
            }
            else
            {
                _rotator.Rotate((speedRotate * Time.deltaTime), 0, 0);
            }
        }
    }
     
    private void _Move()
    {
        bool condition = _rotator.position.y <= 1.1f && _rotator.position.z >= 1.4f;

        if (reverse)
        {
            condition = (_rotator.position.y >= 2f && _rotator.position.z <= -2f);
        }

        if (condition)
        {
            moving = false;
        } 
        else
        {
            if (reverse)
            {
                _rotator.position = new Vector3(_rotator.position.x, _rotator.position.y + (speedMove * Time.deltaTime), _rotator.position.z - (speedMove * Time.deltaTime));
            }
            else
            {
                _rotator.position = new Vector3(_rotator.position.x, _rotator.position.y + (-speedMove * Time.deltaTime), _rotator.position.z + (speedMove * Time.deltaTime));
            }
            
        }
    }
}
