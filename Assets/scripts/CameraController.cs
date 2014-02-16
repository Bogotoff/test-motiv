using UnityEngine;
using System.Collections;

/**
 * Контроллер камеры.
 */
public class CameraController: MonoBehaviour
{
    /** Позиция, в которую движется камера. */
    public Transform rootTransform;

    /** Позиция, куда смотрит камера. */
    public Transform targetTransform;

    /** Скорость передвижения. */
    public float moveSpeed;

    /** Кэшированная компонента Transform. */
    private Transform _cashedTransform;

    /**
     * Инициализация скрипта.
     */
    void Awake()
    {
        _cashedTransform = transform;
        _cashedTransform.LookAt(targetTransform);

        LateUpdate();
    }

    /**
     * Вызывается после вызова Update.
     * 
     * Изменение положения и поворота камеры необходимо производить здесь.
     */
    void LateUpdate()
    {
        if (rootTransform == null || targetTransform == null) {
            return;
        }

        _cashedTransform.position = Vector3.Lerp(_cashedTransform.position, rootTransform.position, moveSpeed * Time.deltaTime);
        _cashedTransform.LookAt(targetTransform);
    }
}
