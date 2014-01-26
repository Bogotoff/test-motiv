using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Обработка событий для кнопки.
 */
public class ButtonEvents: MonoBehaviour
{
    /** Список обработчиков клика по кнопке. */
    private List<ClickCallback> _clickList = new List<ClickCallback>();

    /**
     * Обработчик события клика по объекту.
     */
	void OnClick()
    {
        for (int i = 0; i < _clickList.Count; i++) {
            _clickList[i](gameObject);
        }
    }

    /**
     * Добавляет новый обработчик нажатия по кнопке.
     * 
     * @param метод для обработки щелчка
     * 
     * @throw NullReferenceException
     */
    public void addClickListener(ClickCallback onClick)
    {
        if (onClick == null) {
            throw new System.NullReferenceException("ButtonEvents: onClick delegate can not be null!");
        }

        _clickList.Add(onClick);
    }
}
