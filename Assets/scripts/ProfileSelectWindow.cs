using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class ProfileSelectWindow : MonoBehaviour
{
    /** Скроллбар списка. */
    public UIScrollBar scrollBar;

    /** Скроллбар списка. */
    public GameObject draggablePanel;

    /** Контейнер для отсечение списка. */
    public GameObject clippingPanel;

    /** Префаб(шаблон) элемента профиля. */
    public GameObject profilePrefab;

    /** Кнопка ОК. */
    public UIButton okButton;

    /** Кнопка отмены. */
    public UIButton cancelButton;

    /** Размер отступа элементов. */
    public float paddingSize = 10;
    
    /** Высота элемента. */
    public float itemHeight = 60;

    /** Минимальное значение Y позиции прокручиваемого списка. */
    private float _minScrollPos = 0;

    /** Максимальное значение Y позиции прокручиваемого списка. */
    private float _maxScrollPos = 100;

    /** Список профилей. */
    private List<GameObject> _items;

    /** Выделенный профиль в списке. */
    private ProfileItem _selectedItem = null;

    private SelectProfileCallback _selectProfileCallback;

    /**
     * Инициализация класса.
     */
    void Start()
    {
        _items = new List<GameObject>();
        okButton.GetComponent<UIEventListener>().onClick = onOkClick;
        cancelButton.GetComponent<UIEventListener>().onClick = onCancelClick;
        scrollBar.onChange = onScroll;
        fillList();
    }

    private void onOkClick(GameObject button)
    {
        if (_selectedItem != null) {
            hide();

            if (_selectProfileCallback != null) {
                _selectProfileCallback(_selectedItem.itemIndex);
            }
        }
    }

    private void onCancelClick(GameObject button)
    {
        hide();
    }

    private void hide()
    {
        gameObject.SetActive(false);
    }

    public void setOkClick(SelectProfileCallback callback)
    {
        _selectProfileCallback = callback;
    }

    /**
     * Событие при прокрутке списка.
     */
    private void onScroll(UIScrollBar scrollBar)
    {
        if (_minScrollPos > _maxScrollPos) {
            return;
        }

        float posY = _minScrollPos + (_maxScrollPos - _minScrollPos) * scrollBar.scrollValue;

        Vector3 pos = new Vector3(draggablePanel.transform.localPosition.x,
                                  posY,
                                  draggablePanel.transform.localPosition.z);

        draggablePanel.transform.localPosition = pos;

        seekObjects(pos.y);
    }

    /**
     * Скрывает невидимые элементы.
     */
    private void seekObjects(float panelOffset)
    {
        UIPanel clipPanel = clippingPanel.GetComponent<UIPanel>();

        float topY    = clipPanel.clipRange.y + clipPanel.clipRange.w * 0.5f - draggablePanel.transform.localPosition.y + itemHeight * 0.5f;
        float bottomY = topY - clipPanel.clipRange.w - itemHeight;

        for (int i = 0; i < _items.Count; i++) {
            if (_items[i].transform.localPosition.y < bottomY || _items[i].transform.localPosition.y > topY) {
                _items[i].layer = LayerMask.NameToLayer("Ignore Raycast");
                //_items[i].SetActive(false);
            } else {
                _items[i].layer = LayerMask.NameToLayer("Default");
                //_items[i].SetActive(true);
            }
        }
    }

    /**
     * Заполняет список.
     */
    public void fillList()
    {
        QueryResult res = DataBase.getInstance().query(
            "SELECT userId, age, name, surname, famillar " +
            "FROM mv_users " +
            "ORDER BY userId ASC"
        );

        UIPanel clipPanel = clippingPanel.GetComponent<UIPanel>();

        float startY = clipPanel.clipRange.y + (clipPanel.clipRange.w - itemHeight) * 0.5f - paddingSize;

        _items.Clear();

        GameObject item;
        ProfileItem profile;
        int num = res.numRows();

        for (int i = 0; i < num; i++) {
            item = (GameObject)Instantiate(profilePrefab);
            item.transform.parent = draggablePanel.transform;
            item.transform.localPosition = new Vector3(0, startY - i * itemHeight, 0);
            item.transform.localScale = Vector3.one;
            profile = item.GetComponent<ProfileItem>();
            profile.nameLabel.text  = res[i].asString("name") + " " + res[i].asString("surname");
            profile.scoreLabel.text = res[i].asString("age");
            profile.itemIndex = i;

            item.GetComponent<UIEventListener>().onClick = onItemClick;
            _items.Add(item);
        }

        _maxScrollPos = clipPanel.clipRange.y - clipPanel.clipRange.w * 0.5f - startY + (num - 1) * itemHeight + itemHeight * 0.5f + paddingSize;

        onScroll(scrollBar);
        
        if (_minScrollPos > _maxScrollPos) {
            scrollBar.gameObject.SetActive(false);
        }
    }

    /** Клик по кнопке. */
    private void onItemClick(GameObject button)
    {
        if (_selectedItem != null) {
            _selectedItem.selected = false;
        }

        _selectedItem = button.GetComponent<ProfileItem>();
        _selectedItem.selected = true;
    }


}
