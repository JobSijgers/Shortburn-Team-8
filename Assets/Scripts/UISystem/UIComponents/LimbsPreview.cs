using HandScripts.Core;
using LimbsPickup;
using UnityEngine;
using UnityEngine.UI;
namespace UISystem.UIComponents
{
    public class LimbsPreview : MonoBehaviour
    {
        private Image _image;
        private HandSystem _handSystem;
        private LegSystem.LegSystem _legSystem;
        private LimbsController _limbsController;
        [SerializeField]private Sprite _noLimbsSprite;
        [SerializeField]private Sprite _noLegSprite;
        [SerializeField]private Sprite _noHandSprite;
        [SerializeField]private Sprite _bothLimbsSprite;
        
        private void Start()
        {
            _image = GetComponent<Image>();
            _handSystem = HandSystem.Instance;
            _legSystem = LegSystem.LegSystem.Instance;
            _limbsController = LimbsController.Instance;
        }

        private void Update()
        {
            bool hasHand = !_limbsController.HandState ? false : !_handSystem._handInUse;
            bool hasLeg = !_limbsController.LegState ? false : _legSystem.Leg == null;
            Sprite nextSprite = hasHand && hasLeg ? _bothLimbsSprite : hasHand ? _noLegSprite : hasLeg ? _noHandSprite : _noLimbsSprite;
            _image.sprite = nextSprite;
        }
    }
}
