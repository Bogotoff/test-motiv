using UnityEngine;
using System;

/**
 * Класс для отображения элементов главного меню игры.
 */
public class MainMenu: MonoBehaviour
{
    /**
     * @name Идентификаторы окон.
     * @{
     */
    private const int CM_MAIN    = 0;     /**< Главное меню. */
    private const int CM_OPTIONS = 1;     /**< Меню настроек. */
    private const int CM_SELECT_USER = 2; /**< Меню выбора пользователя. */
    /** @} */
    
    /** Фоновая текстура. */
    public Texture backgroundTexture;
    
    /** Ширина кнопки в процентах [0..1]. */
    public float buttonWidth;

    /** Область экрана. */
    private Rect _clientRect;

    /** Индекс текущего окна. */
    private int _currentWindow = CM_MAIN;
    
    /** Область окна настройки. */
    private Rect _optionsMenuRect;
    
    /** Громкость музыки. */
    private float _musicVolume = 80f;
    
    /** Громкость эффектов. */
    private float _effectsVolume = 80f;
    
    /** Громкость музыки до применения. */
    private float _oldOptionsVolume;
    
    /** Индекс разрешения экрана. */
    private int _resIndex = 0;
    
    /** Индекс разрешения экрана, до применения настроек. */
    private int _oldResIndex;
    
    /** Интенсивность мыши. */
    private float _mouseIntensivity = 80f;
    
    /** Ширина кнопки в пикселях (по-умолчанию) */
    private float _baseButtonWidth;
    
    /** Высота кнопки в пикселях (по-умолчанию) */
    private float _baseButtonHeight;
    
    /**
     * Инициализация.
     */
    void Start()
    {
        if (backgroundTexture == null) {
            Debug.LogError("backgroundTexture == null");
        }
        
        updateClientRect();
        
        Resolution[] res = Screen.resolutions;
        _resIndex        = 0;
        
        for (int i = 0; i < res.Length; i++) {
            if (Screen.currentResolution.Equals(res[i])) {
                _resIndex = i;
                break;
            }
        }
    }
    
    /**
     * Событие отрисовки GUI элементов.
     * 
     * Отрисовывает элементы меню.
     */
    void OnGUI()
    {
        updateClientRect();
        
        GUI.DrawTexture(_clientRect, backgroundTexture, ScaleMode.ScaleAndCrop);
        
        if (_currentWindow == CM_MAIN) {
            drawMainMenu();
        } else if (_currentWindow == CM_OPTIONS) {
            drawOptionsMenu();
        } else if (_currentWindow == CM_SELECT_USER) {
            drawSelectUserMenu();
        }
    }
    
    /**
     * Обновляет размер экрана и размеры кнопок.
     */
    private void updateClientRect()
    {
        _clientRect = new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight);
        
        _baseButtonWidth   = _clientRect.width * buttonWidth;
        
        if (_baseButtonWidth < 100) {
            _baseButtonWidth = 100;
        }
        
        _baseButtonHeight  = _baseButtonWidth * 0.2f;
    }
    
    /**
     * Отображает главное меню.
     */
    private void drawMainMenu()
    {
        if (GUI.Button(new Rect((_clientRect.width - _baseButtonWidth)* 0.5f,
                                (_clientRect.height - _baseButtonHeight)* 0.5f - _baseButtonHeight * 1.4f,
                                _baseButtonWidth, _baseButtonHeight), "Старт")
        ) {
            Debug.Log("Start");
        }
        
        if (GUI.Button(new Rect((_clientRect.width - _baseButtonWidth)* 0.5f,
                                (_clientRect.height - _baseButtonHeight)* 0.5f,
                                _baseButtonWidth, _baseButtonHeight), "Опции")
        ) {
            // Запоминаем настройки для возможности отмены
            _oldOptionsVolume = _musicVolume;
            _oldResIndex      = _resIndex;
            
            _setCurrentWindow(CM_OPTIONS);
        }
        
        if (GUI.Button(new Rect((_clientRect.width - _baseButtonWidth)* 0.5f,
                                (_clientRect.height - _baseButtonHeight)* 0.5f + _baseButtonHeight * 1.4f,
                                _baseButtonWidth, _baseButtonHeight), "Выход")
        ) {
            Application.Quit();
        }
    }
    
    /**
     * Отображает меню настроек.
     */
    private void drawOptionsMenu()
    {
        float windowHeight = _clientRect.height * 0.55f;
        float windowWidth  = windowHeight * 0.7f;
        
        if (windowWidth < 300) {
            windowWidth = 300;
        }
        
        if (windowHeight < 300) {
            windowHeight = 300;
        }
        
        _optionsMenuRect = GUI.ModalWindow(0, 
                                           new Rect(_clientRect.width * 0.5f - windowWidth * 0.5f,
                                                    _clientRect.height * 0.5f - windowHeight * 0.5f,
                                                    windowWidth, windowHeight), 
                                           drawOptionsWindow, "Опции");
    }
    
    /**
     * Отображает элементы окна настроек.
     * 
     * @param windowId Идентификатор окна.
     */
    private void drawOptionsWindow(int windowId)
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

    /**
     * TODO удалить или дописать.
     */
    private void drawSelectUserMenu()
    {
        /*
        float windowHeight = _clientRect.height * 0.55f;
        float windowWidth  = windowHeight * 0.7f;
        
        if (windowWidth < 300) {
            windowWidth = 300;
        }
        
        if (windowHeight < 300) {
            windowHeight = 300;
        }
        
        spMenuRect = GUI.ModalWindow(0, new Rect(_clientRect.width * 0.5f - windowWidth * 0.5f,
                                     _clientRect.height * 0.5f - windowHeight * 0.5f,
                                     windowWidth, windowHeight), drawOptionsWindow, "Опции");
        */
    }
    
    /**
     * Задает текущее окно.
     * 
     * @param windowId Идентификатор окна.
     */
    private void _setCurrentWindow(int windowId)
    {
        _currentWindow = windowId;
    }
}