using UnityEngine;
using System.Collections;

public class Valun: MonoBehaviour
{
    public Vector2 xSpeed = new Vector2(20f, 20f);
    public Vector2 ySpeed = new Vector2(0f, 0f);
    public float zSpeed;
    public Vector2 xBound = new Vector2(-100, 100);
    public Vector2 yBound = new Vector2(0f, 0f);

    private bool _xMoveLeft = false;
    private bool _yMoveDown = false;

    private float _xSpeed = 0f;
    private float _ySpeed = 0f;
    private float _zSpeed = 0f;

    private Transform _transform = null;

    void Start()
    {
        _transform = gameObject.transform;

        _xSpeed = Random.Range(xSpeed[0], xSpeed[1]);
        _ySpeed = Random.Range(ySpeed[0], ySpeed[1]);

        if (Mathf.Abs(zSpeed) > 0.01) {
            _zSpeed = zSpeed;
        } else {
            _zSpeed = 0;
        }
    }

    void FixedUpdate()
    {
        Vector3 pos = gameObject.transform.position;

        if (_xSpeed > 0) {
            if (_xMoveLeft) {
                pos.x -= _xSpeed * Time.deltaTime;

                if (pos.x <= xBound[0]) {
                    pos.x = xBound[0];
                    _xMoveLeft = false;
                }

                gameObject.transform.Rotate(Vector3.forward, Time.deltaTime * _xSpeed * 2);
            } else {
                pos.x += _xSpeed * Time.deltaTime;

                if (pos.x >= xBound[1]) {
                    pos.x = xBound[1];
                    _xMoveLeft = true;
                }
                
                gameObject.transform.Rotate(Vector3.back, Time.deltaTime * _xSpeed * 2);
            }
        }

        if (_ySpeed > 0) {
            if (_yMoveDown) {
                pos.y -= _ySpeed * Time.deltaTime;
                
                if (pos.y <= yBound[0]) {
                    pos.y = yBound[0];
                    _yMoveDown = false;
                }
            } else {
                pos.y += _ySpeed * Time.deltaTime;
                
                if (pos.y >= yBound[1]) {
                    pos.y = yBound[1];
                    _yMoveDown = true;
                }
            }
        }

        if (_zSpeed != 0) {
            pos.z += _zSpeed * Time.deltaTime;
            gameObject.transform.Rotate(Vector3.right, Time.deltaTime * _zSpeed * 2);
        }

        transform.position = new Vector3(pos.x, pos.y, pos.z);
    }
}
