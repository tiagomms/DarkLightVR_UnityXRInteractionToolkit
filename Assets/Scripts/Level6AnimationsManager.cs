using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level6AnimationsManager : MonoBehaviour {

	public FallAndStopChildRigidBodies trashOutsideParent;
  public GameObject Hidden_UI;
	// Use this for initialization
	void Start () {
    LightSpiritsController.instance.SetActionToAllLightSpirits((int)LightSpiritsController.LSAnimations.FLYING);
		StartCoroutine(FallTrashOutside());
		StartCoroutine(PowerEnabled_Coroutine());
    Hidden_UI.SetActive(Global.Shared_Controllers.FOUND_EASTER_EGG);
	}

    private void OnEnable()
    {
        EventManager.StartListening(Global.Shared_Events.CHANGE_SCENE, PlayerFoundEasterEgg);
    }
    private void OnDisable()
    {
        EventManager.StopListening(Global.Shared_Events.CHANGE_SCENE, PlayerFoundEasterEgg);
    }

    private void PlayerFoundEasterEgg()
    {
		Global.Shared_Controllers.FOUND_EASTER_EGG = true;
    }

    private IEnumerator PowerEnabled_Coroutine()
    {
		if (!Global.Shared_Controllers.FOUND_EASTER_EGG) {
			yield return new WaitForSeconds(5f);
			ControllerHintsManager.instance.ShowSpecificTextHint(Global.Shared_Hints.TUT_SELECTIONRAY, "Congratulations\nYou are able to heal\nmore clearly now");
			yield return new WaitForSeconds(15f);
			ControllerHintsManager.instance.HideSpecificTextHint(Global.Shared_Hints.TUT_SELECTIONRAY);
		}
    }

    private IEnumerator FallTrashOutside()
    {
        yield return new WaitForSeconds(0.5f);
		if (trashOutsideParent != null) {
			trashOutsideParent.FallAndStopChildRigidbodiesAnimation();
		}
    }
}
