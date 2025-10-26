using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Animator _cardAnimator;
    [SerializeField]
    private Image frontImage;
    [SerializeField]
    private Image backImage;

    private const string ISON = "IsOn";

    public void Show() => _cardAnimator.SetBool(ISON, true);
    public void Hide() => _cardAnimator.SetBool(ISON, false);
    public void SetFrontSprite(Sprite sprite) => frontImage.sprite = sprite;
    public void SetBackSprite(Sprite sprite) => backImage.sprite = sprite;
}
