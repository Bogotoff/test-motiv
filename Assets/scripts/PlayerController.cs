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

    /** Скрипт для обработки косаний с землей. */
    public GroundTrigger groundTrigger;

    /** Источник брызгов снега. */
    public ParticleSystem emitter;


    /** Кэшированная компонента Transform. */
    private Transform _cachedTransform;

    /** Центр экрана. */
    private Vector3 _screenCenter;

    /** Контроллер для управления физическими свойствами. */
    private CharacterController _controller;

    private bool isForceStop = false;

    private float _angle = 0;
    private float _mouseSensitivity = 1f;
    private Transform _emitterLocalTransform;

    /**
     * Запуск скрипта.
     */
    void Start()
    {
        _cachedTransform = transform;
        _screenCenter    = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0);

        _controller = GetComponent<CharacterController>();

        _mouseSensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", 0.4f);
        _mouseSensitivity = 0.3f + _mouseSensitivity * (2 - 0.3f); //[0.3..2]

        _emitterLocalTransform = emitter.gameObject.transform;
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
        float dx = 0;
        float speedForward = speed.z;
        float speedSide    = speed.x;

        Vector3 deltaPos = (Input.mousePosition - _screenCenter) / (float)Screen.width * _mouseSensitivity;

        if (isForceStop) {
            deltaPos.x = 0;
            deltaPos.y = -1;
        }

        if (groundTrigger.isGrounded) {
            dx = deltaPos.x * sideAccelaration;
            speedForward += deltaPos.y * accelaration;
            speedSide    += dx;
            emitter.enableEmission = true;
        } else {
            dx         = deltaPos.x * sideAccelaration * 0.4f;
            speedSide += dx;
            emitter.enableEmission = false;
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
        } else if (speedSide < -localMaxSpeedSide) {
            speedSide = -localMaxSpeedSide;
        }
        
        float verticalSpeed = _controller.velocity.y + gravity;// * Time.deltaTime;

        if (groundTrigger.isGrounded) {
            if (!isForceStop && Input.GetMouseButtonDown(0)) {
                verticalSpeed = jumpHeight;//calculateJumpVerticalSpeed(jumpHeight);// / Time.deltaTime;
            } else {
                verticalSpeed -= 1;//2 * gravity * Time.deltaTime;
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
        } else if (value < -0.5f) {
            value = -0.5f;
        }

        _angle += (value - _angle) * 0.2f;

        graphics.transform.LookAt(graphics.transform.position + deltaPos,
                                  new Vector3(_angle, 1, 0).normalized);

        if (Mathf.Abs(_angle) > 0.35f) {
            emitter.startSpeed = Mathf.Abs(dx) * 8;

            Vector3 rotation = _emitterLocalTransform.localEulerAngles;

            if (_angle > 0) {
                rotation.y = -60;
                _emitterLocalTransform.localPosition = new Vector3(-0.3f, 0, _emitterLocalTransform.localPosition.z);
            } else {
                rotation.y = 60;
                _emitterLocalTransform.localPosition = new Vector3(0.3f, 0, _emitterLocalTransform.localPosition.z);
            }

            _emitterLocalTransform.localEulerAngles = rotation;
        } else {
            emitter.enableEmission = false;
        }
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
