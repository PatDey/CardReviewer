using SFB;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using WebP;

public class CardReviewerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Button selectCardbackButton;
    [SerializeField]
    private Button selectBackgroundButton;
    [SerializeField]
    private Button selectCardsButton;
    [SerializeField]
    private Button startReviewButton;
    [SerializeField]
    private Button closeAppButton;
    [SerializeField]
    private Image cardbackPreviewImage;
    [SerializeField]
    private Image backgroundPreviewImage;
    [SerializeField]
    private Image backgroundImage;
    [SerializeField]
    private RectTransform scrollContentTransform;
    [SerializeField]
    private GameObject reviewScreenGameObject;
    [SerializeField]
    private Card cardOne;
    [SerializeField]
    private Card cardTwo;
    [SerializeField]
    private Image cardPreviewPrefab;

    private List<Sprite> _cardSprites = new List<Sprite>();

    private List<Image> _loadedPrefabs = new List<Image>();
    private bool _isReviewRunning;
    private int _nextCardSpriteIndex;
    private bool _currentCard;

    private ExtensionFilter[] _filters = new ExtensionFilter[] {new ExtensionFilter("Image Files", "jpg", "png", "webp")};

    void Start()
    {
        selectCardbackButton.onClick.AddListener(SelectCardbackButton_OnClick);
        selectBackgroundButton.onClick.AddListener(SelectBackgroundButton_OnClick);
        selectCardsButton.onClick.AddListener(SelectCardsButton_OnClick);
        startReviewButton.onClick.AddListener(StartReviewButton_OnClick);
        closeAppButton.onClick.AddListener(CloseAppButton_OnClick);
        reviewScreenGameObject.SetActive(false);
    }

    private void CloseAppButton_OnClick() => Application.Quit();

    private void Update()
    {
        if(Keyboard.current.fKey.wasPressedThisFrame)
            Screen.fullScreen = !Screen.fullScreen;
            

        if(!_isReviewRunning)
            return;

        if(Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            reviewScreenGameObject.SetActive(false);
            _isReviewRunning = false;
        }

        if(Keyboard.current.rightArrowKey.wasPressedThisFrame)
            SetNextCard();
    }

    private void SelectBackgroundButton_OnClick() =>  SelectImage("Select Background", backgroundPreviewImage, (Sprite sprite) => backgroundImage.sprite = sprite);

    private void SelectCardbackButton_OnClick() => SelectImage("Select Cardback", cardbackPreviewImage, SetCardbackImages);

    private void SetCardbackImages(Sprite sprite)
    {
        cardOne.SetBackSprite(sprite);
        cardTwo.SetBackSprite(sprite);
    }

    private void SelectImage(string title, Image previewImage, Action<Sprite> setActualImages)
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Background", "", _filters, false);

        if(paths.Length > 0)
        {
            Texture2D tex = CreateTextureFromPath(paths[0]);
            Sprite sprite =  Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
            previewImage.sprite = sprite;
            setActualImages?.Invoke(sprite);

        }
    }
    private void SelectCardsButton_OnClick()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select Cards", "", _filters, true);

        if(paths.Length > 0)
        {
            foreach (var image in _loadedPrefabs)
            {
                Destroy(image.gameObject);
            }

            _loadedPrefabs.Clear();
            _cardSprites.Clear();

            foreach(string path in paths)
            { 
                Texture2D tex = CreateTextureFromPath(path);

                Image cardImage = Instantiate(cardPreviewPrefab, scrollContentTransform);
                Sprite cardSprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), Vector2.zero);
                cardImage.sprite = cardSprite;
                _loadedPrefabs.Add(cardImage);
                _cardSprites.Add(cardSprite);
            }

           scrollContentTransform.sizeDelta = new Vector2(scrollContentTransform.sizeDelta.x, Mathf.Ceil(paths.Length / 3f) * 360f);
        }
    }
    private void StartReviewButton_OnClick()
    {
        _nextCardSpriteIndex = 0;
        cardOne.Hide();
        cardTwo.Hide(); 
        reviewScreenGameObject.SetActive(true);
        _isReviewRunning = true;
    }

    private void SetNextCard()
    {
        if(_nextCardSpriteIndex < _cardSprites.Count)
        { 
            Card nextCard = _currentCard ? cardTwo : cardOne;
            Card activeCard = _currentCard ? cardOne : cardTwo;

            nextCard.SetFrontSprite(_cardSprites[_nextCardSpriteIndex]);
            _nextCardSpriteIndex++;

            activeCard.Hide();
            nextCard.Show();
            _currentCard = !_currentCard;
        }
    }

    private Texture2D CreateTextureFromPath(string path)
    {
        byte[] imageBytes = File.ReadAllBytes(path);
        Texture2D tex = new Texture2D(0,0);
        if(Path.GetExtension(path) == ".webp")
            tex = Texture2DExt.CreateTexture2DFromWebP(imageBytes, lMipmaps: false, lLinear: false, lError: out Error lError);
        else 
            tex.LoadImage(imageBytes);

        return tex;
    }
}
