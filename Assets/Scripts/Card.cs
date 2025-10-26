using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Animator _cardAnimator;
    [SerializeField]
    private Image frontImage;

    private const string ISON = "IsOn";

    public void Show() => _cardAnimator.SetBool(ISON, true);
    public void Hide() => _cardAnimator.SetBool(ISON, false);
    public void SetSprite(Sprite sprite) => frontImage.sprite = sprite;
}
