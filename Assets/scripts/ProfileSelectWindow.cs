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

    /** Список игроков. */
    private List<User> _users;

    /** Выделенный профиль в списке. */
    private ProfileItem _selectedItem = null;

    private SelectProfileCallback _selectProfileCallback;

    /**
     * Инициализация класса.
     */
    void Start()
    {
        _items = new List<GameObject>();
        _users = new List<User>();
        okButton.GetComponent<UIEventListener>().onClick     = onOkClick;
        cancelButton.GetComponent<UIEventListener>().onClick = onCancelClick;
        scrollBar.onChange = onScroll;
        fillList();
    }

    private void onOkClick(GameObject button)
    {
        if (_selectedItem != null) {
            hide();

            if (_selectProfileCallback != null) {
                _selectProfileCallback(_users[_selectedItem.itemIndex]);
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
            "SELECT userId, age, name, surname, famillar, gameCount " +
            "FROM mv_users " +
            "ORDER BY userId ASC"
        );

        UIPanel clipPanel = clippingPanel.GetComponent<UIPanel>();

        float startY = clipPanel.clipRange.y + (clipPanel.clipRange.w - itemHeight) * 0.5f - paddingSize;
        int i;

        for (i = 0; i < _items.Count; i++) {
            _users[i] = null;
        }

        _users.Clear();
        _items.Clear();

        GameObject item;
        ProfileItem profile;

        // демо-игрок
        _users.Add(new User(-1, -1, "игрок", "Демо-", "", 0));

        item = (GameObject)Instantiate(profilePrefab);
        item.transform.parent = draggablePanel.transform;
        item.transform.localPosition = new Vector3(0, startY - 0, 0);
        item.transform.localScale = Vector3.one;
        profile = item.GetComponent<ProfileItem>();
        profile.scoreLabel.text = "-";
        profile.nameLabel.text  = "Демо-игрок";
        profile.itemIndex       = 0;

        item.GetComponent<UIEventListener>().onClick = onItemClick;
        _items.Add(item);

        int num = res.numRows();

        for (i = 0; i < num; i++) {
            _users.Add(new User(
                res[i].asInt("userId"),
                res[i].asInt("age"),
                res[i].asString("name"),
                res[i].asString("surname"),
                res[i].asString("famillar"),
                res[i].asInt("gameCount")
            ));

            item = (GameObject)Instantiate(profilePrefab);
            item.transform.parent = draggablePanel.transform;
            item.transform.localPosition = new Vector3(0, startY - (i + 1) * itemHeight, 0);
            item.transform.localScale = Vector3.one;
            profile = item.GetComponent<ProfileItem>();
            profile.scoreLabel.text = res[i].asString("age");
            profile.nameLabel.text  = res[i].asString("name") + " " 
                                    + res[i].asString("surname") + " " 
                                    + res[i].asString("famillar")[0] + ".";
            profile.itemIndex = i + 1;

            item.GetComponent<UIEventListener>().onClick = onItemClick;
            _items.Add(item);
        }

        _maxScrollPos = clipPanel.clipRange.y - clipPanel.clipRange.w * 0.5f 
                      - startY + (num - 1) * itemHeight + itemHeight * 0.5f + paddingSize;

        onScroll(scrollBar);
        
        if (_minScrollPos > _maxScrollPos) {
            scrollBar.gameObject.SetActive(false);
        }
    }

    /**
     * Клик по кнопке.
     * 
     * @param button кнопка
     */
    private void onItemClick(GameObject button)
    {
        if (_selectedItem != null) {
            _selectedItem.selected = false;
        }

        _selectedItem = button.GetComponent<ProfileItem>();
        _selectedItem.selected = true;
    }


}
