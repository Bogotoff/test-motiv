using UnityEngine;
using System;

/**
 * Класс для отображения элементов главного меню игры.
 */
public class MainMenu: MonoBehaviour
{
    /** Фоновая текустура. */
    public Texture2D backgroundTexture;

    /** Громкость музыки. */
    private float _musicVolume = 0.8f;
    
    /** Громкость эффектов. */
    private float _effectsVolume = 0.8f;
    
    /** Громкость музыки до применения. */
    private float _oldOptionsVolume;
    
    /** Индекс разрешения экрана. */
    private int _resIndex = 0;

    /** Чувствительность мыши. */
    private float _mouseSensitivity = 0.8f;

    /** Меню настроек. */
    private GameObject _optionsPanel;

    /** Окно выбора профиля. */
    private GameObject _selectProfileWindow;

    /**
     * Инициализация.
     */
    void Start()
    {
        _resIndex = PlayerPrefs.GetInt("resolution", -1);

        if (_resIndex == -1 || _resIndex >= Screen.resolutions.Length) {
            Resolution[] res = Screen.resolutions;
            _resIndex = 0;

            for (int i = 0; i < res.Length; i++) {
                if (Screen.currentResolution.Equals(res[i])) {
                    _resIndex = i;
                    break;
                }
            }

            PlayerPrefs.SetInt("resolution", _resIndex);
        } else {
            Screen.SetResolution(Screen.resolutions[_resIndex].width, Screen.resolutions[_resIndex].height, true);
        }

        _musicVolume      = PlayerPrefs.GetFloat("music_volume", 0.8f);
        _effectsVolume    = PlayerPrefs.GetFloat("effects_volume", 0.8f);
        _mouseSensitivity = PlayerPrefs.GetFloat("mouse_sensitivity", 0.8f);

        GameObject.Find("startButton").GetComponent<ButtonEvents>().addClickListener(onStartButtonClick);
        GameObject.Find("optionsButton").GetComponent<ButtonEvents>().addClickListener(onOptionsButtonClick);
        GameObject.Find("exitButton").GetComponent<ButtonEvents>().addClickListener(onExitButtonClick);

        _optionsPanel = GameObject.Find("optionsPanel");
        GetComponent<AudioSource>().volume = _musicVolume;

        _selectProfileWindow = GameObject.Find("selectProfile");
        
        initOptionsPanel();
        initProfileWindow();
    }

    /**
     * Инициализация окна настроек.
     */
    private void initOptionsPanel()
    {
        GameObject.Find("optionsCancelButton").GetComponent<ButtonEvents>().addClickListener(onOptionsCancelClick);
        GameObject.Find("optionsOkButton").GetComponent<ButtonEvents>().addClickListener(onOptionsOkClick);

        // громкость музыки
        GameObject.Find("volumeBar").GetComponent<UIScrollBar>().scrollValue = _musicVolume;
        GameObject.Find("volumeBar").GetComponent<UIScrollBar>().onChange    = onVolumeChange;

        // громкость эффектов
        GameObject.Find("effectsBar").GetComponent<UIScrollBar>().scrollValue = _effectsVolume;
        GameObject.Find("effectsBar").GetComponent<UIScrollBar>().onChange    = onEffectsVolumeChange;


        // чувствительность мыши
        GameObject.Find("sensitivityBar").GetComponent<UIScrollBar>().scrollValue = _mouseSensitivity;
        GameObject.Find("sensitivityBar").GetComponent<UIScrollBar>().onChange    = onSensitivityChange;

        // разрешение экрана
        GameObject.Find("resolutionLabel").GetComponent<UILabel>().text =
            "Разрешение экрана (" + Screen.resolutions[_resIndex].width + "x" + Screen.resolutions[_resIndex].height + ")";

        GameObject.Find("resolutionBar").GetComponent<UIScrollBar>().scrollValue =
            (Screen.resolutions.Length == 1) ? 0 : (float)_resIndex / (Screen.resolutions.Length - 1);

        GameObject.Find("resolutionBar").GetComponent<UIScrollBar>().onChange = onResolutionChange;

        _optionsPanel.SetActive(false);
    }

    /**
     * Нажатие по кнопке Отмена в окне настроек.
     * 
     * @param target Объект инициализировавший это событие
     */
    private void onOptionsCancelClick(GameObject target)
    {
        UIScrollBar sb = GameObject.Find("volumeBar").GetComponent<UIScrollBar>();
        sb.scrollValue = PlayerPrefs.GetFloat("music_volume", 0.8f);
        onVolumeChange(sb);

        sb = GameObject.Find("effectsBar").GetComponent<UIScrollBar>();
        sb.scrollValue = PlayerPrefs.GetFloat("effects_volume", 0.8f);
        onEffectsVolumeChange(sb);

        sb = GameObject.Find("sensitivityBar").GetComponent<UIScrollBar>();
        sb.scrollValue = PlayerPrefs.GetFloat("mouse_sensitivity", 0.8f);
        onSensitivityChange(sb);

        sb = GameObject.Find("resolutionBar").GetComponent<UIScrollBar>();
        sb.scrollValue = PlayerPrefs.GetInt("resolution", 0);
        onResolutionChange(sb);

        _optionsPanel.SetActive(false);
    }

    /**
     * Нажатие по кнопке OK в окне настроек.
     * 
     * @param target Объект инициализировавший это событие
     */
    private void onOptionsOkClick(GameObject target)
    {
        PlayerPrefs.SetFloat("music_volume", _musicVolume);
        PlayerPrefs.SetFloat("effects_volume", _effectsVolume);
        PlayerPrefs.SetFloat("mouse_sensitivity", _mouseSensitivity);

        _optionsPanel.SetActive(false);
        Screen.SetResolution(Screen.resolutions[_resIndex].width, Screen.resolutions[_resIndex].height, true);
    }

    /**
     * Событие при изменении громкости музыки.
     * 
     * @param scrollBar Объект ползунка(скроллбара)
     */
    private void onVolumeChange(UIScrollBar scrollBar)
    {
        if (scrollBar != null) {
            _musicVolume = scrollBar.scrollValue;
        }

        GetComponent<AudioSource>().volume = _musicVolume;
    }

    /**
     * Событие при изменении громкости музыки.
     * 
     * @param scrollBar Объект ползунка(скроллбара)
     */
    private void onEffectsVolumeChange(UIScrollBar scrollBar)
    {
        if (scrollBar != null) {
            _effectsVolume = scrollBar.scrollValue;

            //GetComponent<AudioSource>().volume = _effectsVolume;
        }
    }

    /**
     * Событие при изменении чувствительности мыши.
     * 
     * @param scrollBar Объект ползунка(скроллбара)
     */
    private void onSensitivityChange(UIScrollBar scrollBar)
    {
        if (scrollBar != null) {
            _mouseSensitivity = scrollBar.scrollValue;
        }
    }

    /**
     * Событие при перемещении ползунка разрешения экрана.
     * 
     * @param scrollBar Объект ползунка(скроллбара)
     */
    private void onResolutionChange(UIScrollBar scrollBar)
    {
        Resolution[] resolutions = Screen.resolutions;
        _resIndex = (int)(scrollBar.scrollValue * (resolutions.Length - 1));

        GameObject.Find("resolutionLabel").GetComponent<UILabel>().text =
            "Разрешение экрана (" + resolutions[_resIndex].width + "x" + resolutions[_resIndex].height + ")";
    }

    /** Инициализация окна выбора профиля. */
    private void initProfileWindow()
    {
        _selectProfileWindow.GetComponent<ProfileSelectWindow>().setOkClick(onProfileSelect);
        _selectProfileWindow.SetActive(false);
    }

    /**
     * Выбор профиля и запуск игры.
     * 
     * @param user данные выбранного игрока
     */
    private void onProfileSelect(User user)
    {
        try {
            int res = GameData.initialize(user);

            if (res == 1) {
                //TODO Для выбранного игрока нет уровней.
                Debug.LogError("Для выбранного игрока нет уровней.");
            } else {
                if (GameData.hasNextGame()) {
                    GameInfo info = GameData.getNextGame();

                    Application.LoadLevel(info.sceneName);
                }
            }
        } catch (System.Exception e) {
            Debug.LogError(e.Message);
        }
    }

    /**
     * Событие отрисовки GUI элементов.
     * 
     * Отрисовывает элементы меню.
     */
    void OnGUI()
    {
        if (backgroundTexture != null) {
            GUI.DrawTexture(Camera.main.pixelRect, backgroundTexture, ScaleMode.ScaleAndCrop);
        }
    }

    /**
     * Нажатие по кнопке Старт.
     * 
     * @param target Объек инициализировавший событие
     */
    private void onStartButtonClick(GameObject target)
    {
        _selectProfileWindow.SetActive(true);
    }

    /**
     * Нажатие по кнопке Настройки.
     * 
     * @param target Объек инициализировавший событие
     */
    private void onOptionsButtonClick(GameObject target)
    {
        _optionsPanel.SetActive(true);
    }

    /**
     * Нажатие по кнопке Выход.
     * 
     * @param target Объек инициализировавший событие
     */
    private void onExitButtonClick(GameObject target)
    {
        Application.Quit();
    }

    /**
     * Отображает элементы окна настроек.
     * 
     * @param windowId Идентификатор окна.
     */
    /*private void drawOptionsWindow(int windowId)
    {
        float border       = _optionsMenuRect.height * 0.1f;
        float groupHeight  = (_optionsMenuRect.height - _baseButtonHeight - 2f * border) * 0.24f;
        float textHeight   = 25f;
        float sliderHeight = 30f;
        
        // Громкость музыки
        GUI.Label(new Rect(_optionsMenuRect.width  * 0.05f,
                           border,
                           _optionsMenuRect.width  * 0.9f,
                           textHeight), 
                  "Громкость музыки");
        
        _musicVolume = GUI.HorizontalSlider(new Rect(_optionsMenuRect.width  * 0.05f,
                                                     border + textHeight,
                                                     _optionsMenuRect.width  * 0.9f,
                                                     sliderHeight), 
                                            _musicVolume, 0, 100);
        
        // Громкость эффектов
        GUI.Label(new Rect(_optionsMenuRect.width  * 0.05f,
                           border + groupHeight,
                           _optionsMenuRect.width  * 0.9f,
                           textHeight), 
                  "Громкость эффектов");
        
        _effectsVolume = GUI.HorizontalSlider(new Rect(_optionsMenuRect.width  * 0.05f,
                                                       border + groupHeight + textHeight,
                                                       _optionsMenuRect.width  * 0.9f,
                                                       sliderHeight), 
                                              _effectsVolume, 0, 100);
        
        Resolution[] resolutions = Screen.resolutions;
        
        // Разрешение экрана
        GUI.Label(new Rect(_optionsMenuRect.width  * 0.05f,
                           border + 2 * groupHeight,
                           _optionsMenuRect.width  * 0.9f,
                           textHeight), 
                  "Разрешение экрана (" + resolutions[_resIndex].width + "x" + resolutions[_resIndex].height + ")");
        
        _resIndex = (int)GUI.HorizontalSlider(new Rect(_optionsMenuRect.width  * 0.05f,
                                                       border + 2 * groupHeight + textHeight,
                                                       _optionsMenuRect.width  * 0.9f,
                                                       sliderHeight),
                                              _resIndex, 0, resolutions.Length - 1);
        
        // Интенсивность мыши
        GUI.Label(new Rect(_optionsMenuRect.width  * 0.05f,
                           border + 3 * groupHeight,
                           _optionsMenuRect.width  * 0.9f,
                           textHeight), 
                  "Интенсивность мыши");
        
        _mouseIntensivity = GUI.HorizontalSlider(new Rect(_optionsMenuRect.width  * 0.05f,
                                                          border + 3 * groupHeight + textHeight,
                                                          _optionsMenuRect.width  * 0.9f,
                                                          sliderHeight), 
                                                 _mouseIntensivity, 0, 100);
        
        AudioSource audioSource = (AudioSource)GetComponent("AudioSource");
        audioSource.volume      = _musicVolume / 100.0f;
        
        float btnWidth = _optionsMenuRect.width * 0.4f;
        
        if (GUI.Button(new Rect(_optionsMenuRect.width * 0.05f,
                                _optionsMenuRect.height * 0.95f - _baseButtonHeight,
                                btnWidth, _baseButtonHeight), 
                       "ОК")
        ) {
            if (_oldResIndex != _resIndex) {
                Screen.SetResolution(resolutions[_resIndex].width, resolutions[_resIndex].height, true);
            }
            
            _setCurrentWindow(CM_MAIN);
        }
        
        if (GUI.Button(new Rect(_optionsMenuRect.width * 0.55f,
                                _optionsMenuRect.height * 0.95f - _baseButtonHeight,
                                btnWidth, _baseButtonHeight), 
                       "Отмена")
        ) {
            _resIndex          = _oldResIndex;
            _musicVolume       = _oldOptionsVolume;
            audioSource.volume = _musicVolume * 0.01f;
            
            _setCurrentWindow(CM_MAIN);
        }
    }
    */
}