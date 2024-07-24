using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialStatusTotal : MonoBehaviour
{
    private static TutorialStatusTotal _instance = null;
    public  static TutorialStatusTotal instance
    {
        get
        {
            if ( _instance == null )
            {
                _instance = GameObject.Find("TutorialManager").
                        GetComponent<TutorialStatusTotal>();
            }
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

	public List<TS> tutorialStatus;
}


/*public enum    TutorialStatus : int
{
	TUTORIAL_RESULT = 0,

};
*/

public enum E_UIList
{
    NONE,
    MainMenuUI, 
    CharacterPopup,
    BuddyPopup,
    UpgradePopup,
    MissionPopup,

    GameMenuUI, 
    PausePopup
}

[System.Serializable]
public class TS
{
	public E_UIList 	  name; 
	public string 		  path;
}