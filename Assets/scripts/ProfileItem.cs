using UnityEngine;
using System.Collections;

public class ProfileItem: MonoBehaviour
{
    public UILabel nameLabel;
    public UILabel scoreLabel;
    public int itemIndex;
    public Color selectedColor;
    private Color _defaultColor;

    private UIButton _button;
    private bool _selected;

    void Awake()
    {
        _button = GetComponent<UIButton>();
    }

    void Start()
    {
        _defaultColor = _button.defaultColor;
        selected = false;
    }

    public bool selected
    {
        get
        {
            return _selected;
        }

        set
        {
            setColor(value ? selectedColor : _defaultColor);
        }
    }

    /**
     * Меняет цвет кнопки.
     * 
     * @param value цвет
     */
    private void setColor(Color value)
    {
        _button.defaultColor = value;
        _button.enabled = false;
        _button.enabled = true;
    }
}
