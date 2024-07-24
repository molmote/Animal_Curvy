using UnityEngine;
using System.Collections;

/*
public class InputController

    private static InputCollider _instance = new InputController();
    public static InputCollider instance 
    {
        get
        {
            return _instance;
        }
    }

    private List<InputCollider> inputList;

    public void Block()
    {
        foreach ( InputCollider input in inputList )
        {
            input.Block();
        }
    }
*/

public class InputCollider : MonoBehaviour
{
    void Start()
    {
        dragTimer = 0;
    }

    private             bool        isTouched = false;
    private             float       dragTimer;
    private             float       inputBlockTimer;

    public void Block()
    {
        this.inputBlockTimer = 0;
    }

    void Update()
    {
        if ( GameInfo.instance.IsGamePaused() )
            return;

        if ( Input.GetKeyUp(KeyCode.Space) )
        {
            this.OnPress(false);        
        }

        #if UNITY_EDITOR
            if ( Input.GetKeyDown(KeyCode.Space) )
            {
                this.OnPress(true);        
            }
            if ( Input.GetKeyDown(KeyCode.Q))
            {
                // GamePlayerEntity.instance.TriggerDash(false);
            }
        #endif
    }   

    void OnPress(bool pressed)
    {
        if ( UserInfo.instance.gameplayTutorialInitiated == false )
        {
            if (pressed)
                this.OnMouseDown();
            else
                this.OnMouseUp();
        }
    }

    void OnClick (){}
    
    void OnDrop (GameObject gameObject){}

    void OnMouseUp()
    {             
    }

    void OnMouseDown()
    {        
        GamePlayerEntity.instance.ChangeDirection();
    }
}