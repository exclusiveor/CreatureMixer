using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TObject.Shared;
//EventManager
public class EventManager
{
    public static event EventController.MethodContainer OnUIInitializedEvent;
    public void CallOnUIInitializedEvent(EventData ob = null) { if (OnUIInitializedEvent != null) OnUIInitializedEvent(ob); }

    public static event EventController.MethodContainer OnChangeBoardUIEvent;
    public void CallOnChangeBoardUIEvent(EventData ob = null) { if (OnChangeBoardUIEvent != null) OnChangeBoardUIEvent(ob); }


    public static event EventController.MethodContainer OnChangeCursorSystemCustom;
    public void CallOnChangeCursorSystemCustom(EventData ob = null) { if (OnChangeCursorSystemCustom != null) OnChangeCursorSystemCustom(ob); }

    //public static event EventController.MethodContainer OnStartLevelAnimationFinishedEvent;
    //public void CallOnStartLevelAnimationFinishedEvent(EventData ob = null) { if (OnStartLevelAnimationFinishedEvent != null) OnStartLevelAnimationFinishedEvent(ob); }

    //public static event EventController.MethodContainer OnTeleportToMainMenu;
    //public void CallOnTeleportToMainMenuEvent(EventData ob = null) { if (OnTeleportToMainMenu != null) OnTeleportToMainMenu(ob); }

    public static event EventController.MethodContainer OnShowWindowEvent;
    public void CallOnShowWindowEvent(EventData ob = null) { if (OnShowWindowEvent != null) OnShowWindowEvent(ob); }

    public static event EventController.MethodContainer OnHideWindowEvent;
    public void CallOnHideWindowEvent(EventData ob = null) { if (OnHideWindowEvent != null) OnHideWindowEvent(ob); }

    public static event EventController.MethodContainer OnShowPopUpEvent;
    public void CallOnShowPopUpEvent(EventData ob = null) { if (OnShowPopUpEvent != null) OnShowPopUpEvent(ob); }

    public static event EventController.MethodContainer OnHidePopUpEvent;
    public void CallOnHidePopUpEvent(EventData ob = null) { if (OnHidePopUpEvent != null) OnHidePopUpEvent(ob); }

    //public static event EventController.MethodContainer OnResourceCollectedEvent;
    //public void CallOnResourceCollectedEvent(EventData ob = null) { if (OnResourceCollectedEvent != null) OnResourceCollectedEvent(ob); }

    public static event EventController.MethodContainer OnTutorialNeededEvent;
    public void CallOnTutorialNeededEvent(EventData ob = null) { if (OnTutorialNeededEvent != null) OnTutorialNeededEvent(ob); }

    public static event EventController.MethodContainer OnTutorialCloseNeededEvent;
    public void CallOnTutorialCloseNeededEvent(EventData ob = null) { if (OnTutorialCloseNeededEvent != null) OnTutorialCloseNeededEvent(ob); }

    public static event EventController.MethodContainer OnOpenFormNeededEvent;
    public void CallOnOpenFormNeededEvent(EventData ob = null) { if (OnOpenFormNeededEvent != null) OnOpenFormNeededEvent(ob); }

    //public static event EventController.MethodContainer OnShowCautionPopupEvent;
    //public void CallOnShowCautionPopupEvent(EventData ob = null) { if (OnShowCautionPopupEvent != null) OnShowCautionPopupEvent(ob); }

    //public static event EventController.MethodContainer OnHideCautionPopupEvent;
    //public void CallOnHideCautionPopupEvent(EventData ob = null) { if (OnHideCautionPopupEvent != null) OnHideCautionPopupEvent(ob); }

    public static event EventController.MethodContainer OnNeedSaveLevelEvent;
    public void CallOnNeedSaveLevelEvent(EventData ob = null) { if (OnNeedSaveLevelEvent != null) OnNeedSaveLevelEvent(ob); }

    //TROPHIES
    public static event EventController.MethodContainer OnTrophyCompletedEvent;
	public void CallOnTrophyCompletedEvent(EventData ob = null) { if (OnTrophyCompletedEvent != null) OnTrophyCompletedEvent(ob); }

	// ======= Feeding =============

	public static event EventController.MethodContainer OnStartPlayPressedEvent;
    public void CallOnStartPlayPressedEvent(EventData ob = null) { if (OnStartPlayPressedEvent != null) OnStartPlayPressedEvent(ob); }

    public static event EventController.MethodContainer OnGoldCountChangedEvent;
	public void CallOnGoldCountChangedEvent(EventData ob = null) { if (OnGoldCountChangedEvent != null) OnGoldCountChangedEvent(ob); }

	public static event EventController.MethodContainer OnResourcesChangedEvent;
	public void CallOnResourcesChangedEvent(EventData ob = null) { if (OnResourcesChangedEvent != null) OnResourcesChangedEvent(ob); }

    public static event EventController.MethodContainer OnShowAddResourceEffect;
    public void CallOnShowAddResourceEffect(EventData ob = null) { if (OnShowAddResourceEffect != null) OnShowAddResourceEffect(ob); }

    public static event EventController.MethodContainer OnTurnWasMadeEvent;
	public void CallOnTurnWasMadeEvent(EventData ob = null) { if (OnTurnWasMadeEvent != null) OnTurnWasMadeEvent(ob); }

    public static event EventController.MethodContainer OnCombineWasMadeEvent;
    public void CallOnCombineWasMadeEvent(EventData ob = null) { if (OnCombineWasMadeEvent != null) OnCombineWasMadeEvent(ob); }

	public static event EventController.MethodContainer OnPowerUpUsedEvent;
	public void CallOnPowerUpUsedEvent(EventData ob = null) { if (OnPowerUpUsedEvent != null) OnPowerUpUsedEvent(ob); }

	public static event EventController.MethodContainer OnReachMaxPipeLevelEvent;
	public void CallOnReachMaxPipeLevelEvent(EventData ob = null) { if (OnReachMaxPipeLevelEvent != null) OnReachMaxPipeLevelEvent(ob); }


    public static event EventController.MethodContainer OnPowerUpsResetNeededEvent;
    public void CallOnPowerUpsResetNeededEvent(EventData ob = null) { if (OnPowerUpsResetNeededEvent != null) OnPowerUpsResetNeededEvent(ob); }

    public static event EventController.MethodContainer OnUISwitchNeededEvent;
    public void CallOnUISwitchNeededEvent(EventData ob = null) { if (OnUISwitchNeededEvent != null) OnUISwitchNeededEvent(ob); }

    public static event EventController.MethodContainer OnShowNotificationEvent;
    public void CallOnShowNotificationEvent(EventData ob = null) { if (OnShowNotificationEvent != null) OnShowNotificationEvent(ob); }

    public static event EventController.MethodContainer OnHideNotificationEvent;
    public void CallOnHideNotificationEvent(EventData ob = null) { if (OnHideNotificationEvent != null) OnHideNotificationEvent(ob); }

	public static event EventController.MethodContainer OnInterstitialClosedEvent;
	public void CallOnInterstitialClosedEvent(EventData ob = null) { if (OnInterstitialClosedEvent != null) OnInterstitialClosedEvent(ob); }

    public static event EventController.MethodContainer OnRewardedVideoClosedEvent;
    public void CallOnRewardedVideoClosedEvent(EventData ob = null) { if (OnRewardedVideoClosedEvent != null) OnRewardedVideoClosedEvent(ob); }

    public static event EventController.MethodContainer OnPurchaseCompleteEvent;
    public void CallOnPurchaseCompleteEvent(EventData ob = null) { if (OnPurchaseCompleteEvent != null) OnPurchaseCompleteEvent(ob); }


}
