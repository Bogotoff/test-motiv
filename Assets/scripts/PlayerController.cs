using UnityEngine;
using System.Collections;

/**
 * Контроллер для управления персонажем.
 */
public class PlayerController: MonoBehaviour
{
    /**
     * Отображаемый объект.
     */
    public GameObject graphics;

    public GameObject playerLook;
    public GameObject playerLookUp;

    public float moveCoef = 1f;
    public float sideCoef = 1f;

    /** Ускорение вдоль трассы. */
    public float accelaration;

    /** Боковое ускорение. */
    public float sideAccelaration;

    /** Максимальная скорость. */
    public float maxSpeed;

    /** Максимальная боковая скорость. */
    public float maxSpeedSide;

    /** UI элемент, для отображения скорости. */
    public UISlider speedSlider;

    /** Количество разбиений для UI элемента. */
    public int sliderSteps = 10;

    /** Высота прыжка(в метрах). */
    public float jumpHeight = 1;

    /** Ускорение свободного падения. */
    public float gravity = -9.81f;

    /** Скорость игрока. */
    public Vector3 speed = new Vector3(0, 0, 0);

    /** Кэшированная компонента Transform. */
    private Transform _cachedTransform;

    /** Центр экрана. */
    private Vector3 _screenCenter;

    /** Контроллер для управления физическими свойствами. */
    private CharacterController _controller;

    private bool isForceStop = false;

    private float _angle = 0;
    private float _mouseSensitivity = 1f;

    /**
     * Запуск скрипта.
     */
    void Start()
    {
        //accelaration = 6;
        //maxSpeed = 150;
        _cachedTransform = transform;
        _screenCenter    = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        _controller = GetComponent<CharacterController>();

        _mouseSensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", 0.4f);
        _mouseSensitivity = 0.3f + _mouseSensitivity * (2-0.3f); //[0.3..2]
    }

    /**
     * Обновление кадра.
     */
    void Update()
    {
        if (_controller == null) {
            _controller = GetComponent<CharacterController>();
        }
        
        if (_controller == null) {
            return;
        }
        float value;

        float speedForward = speed.z;
        float speedSide    = speed.x;

        float dx = 0;

        Vector3 deltaPos = (Input.mousePosition - _screenCenter) / (float)Screen.width * _mouseSensitivity;

        if (isForceStop) {
            deltaPos.x = 0;
            deltaPos.y = -1;
        }

        if (_controller.isGrounded) {
            speedForward += deltaPos.y * accelaration;
            speedSide    += deltaPos.x * sideAccelaration;

            dx = deltaPos.x * sideAccelaration;
        } else {
            speedSide    += deltaPos.x * sideAccelaration * 0.4f;
            dx = deltaPos.x * sideAccelaration * 0.4f;
        }

        if (speedForward > maxSpeed) {
            speedForward = maxSpeed;
        }
        
        if (speedForward < 0) {
            speedForward = 0;
        }

        float localMaxSpeedSide = (speedForward < maxSpeedSide) ? speedForward : maxSpeedSide;
        
        if (speedSide > localMaxSpeedSide) {
            speedSide = localMaxSpeedSide;
        } else
        if (speedSide < -localMaxSpeedSide) {
            speedSide = -localMaxSpeedSide;
        }
        
        float verticalSpeed = speed.y + gravity * Time.deltaTime;

        if (_controller.isGrounded) {
            if (!isForceStop && Input.GetMouseButtonDown(0)) {
                verticalSpeed = calculateJumpVerticalSpeed(jumpHeight);
            } else {
                //verticalSpeed = gravity * Time.deltaTime;
            }
        }

        if (speedSlider != null) {
            value = speedForward / maxSpeed;

            if (sliderSteps > 0) {
                value = Mathf.Ceil(value * sliderSteps) / (float)sliderSteps;
            }

            speedSlider.sliderValue = value;
        }

        speed = new Vector3(speedSide, verticalSpeed, speedForward);

        deltaPos = speed * 1;
        deltaPos.y = 0;

        deltaPos = deltaPos + Vector3.forward * moveCoef;

        value = dx * sideCoef * Mathf.Sqrt(speedForward);

        if (value > 0.5f) {
            value = 0.5f;
        } else
        if (value < -0.5f) {
            value = -0.5f;
        }

        _angle += (value - _angle) * 0.2f;

        graphics.transform.LookAt(graphics.transform.position + deltaPos,
                                  new Vector3(_angle, 1, 0).normalized);

        //deltaPos = speed * Time.deltaTime;
        _controller.Move(speed * Time.deltaTime);
    }

    /**
     * Вычисляет скорость прыжка для достижения заданной высоты с учетом ускорения свободного падения.
     * 
     * @param targetHumpHeight необходимая высота
     * 
     * @return скорость прыжка
     */
    private float calculateJumpVerticalSpeed(float targetJumpHeight)
    {
        return Mathf.Sqrt(-2 * targetJumpHeight * gravity);
    }

    public void forceStop()
    {
        isForceStop = true;
    }
}
