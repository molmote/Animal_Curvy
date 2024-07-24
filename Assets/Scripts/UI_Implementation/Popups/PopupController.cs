using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PopupController 
{
    public delegate void PopupCallback(string response, PopupParam popupParam);

    private Dictionary<string, BasePopupEntity> registeredPopupDict     = new Dictionary<string, BasePopupEntity>();
    private Stack<BasePopupEntity>              appearedPopupStack      = new Stack<BasePopupEntity>();

    private static PopupController instance_                            = new PopupController();
    public static PopupController instance 
    { 
        get { return instance_; } 
    }

    //
    // Private Methods
    private void ApplyLayerToCamera(int layerIndex)
    {
        int uiLayerNumber               = Defines.POPUP_UI_FIRST_LAYER + layerIndex;
        int cameraCullingMask           = LayerMask.NameToLayer("TransparentFX") << uiLayerNumber;

        GameObject  cameraObj           = GameObject.FindWithTag("MainCamera");
        UICamera    uiCamera            = cameraObj.GetComponent<UICamera>();
        Camera      camera              = cameraObj.GetComponent<Camera>();
        
        // uiCamera.eventReceiverMask    = cameraCullingMask;
        // camera.cullingMask           |= cameraCullingMask;
    }

    public void Initialize()
    {

    }

    //
    // Public Methods
    //
    public void Clear()
    {
        this.registeredPopupDict.Clear();
        this.appearedPopupStack.Clear();     
    }


    public void Register(string popupName, BasePopupEntity popup)
    {
        if (this.registeredPopupDict.ContainsKey(popupName) == false)
        {
            this.registeredPopupDict[popupName] = popup;
        }
    }
    
    public void InvokeFromActive(string methodName)
    {
        this.appearedPopupStack.Peek().Invoke(methodName, 0f);
    }
    
    public void Open(string popupName,                     
                     PopupParam popupParam  = null, 
                     PopupCallback Callback = null)
    {
        if(this.registeredPopupDict.ContainsKey(popupName))
        {
            BasePopupEntity popup = this.registeredPopupDict[popupName];

            if ( popup.GetComponent<Animation>().isPlaying || popup.isClosing )
            {
                MyLogger.Red("POPUP", string.Format("[OPEN] Popup is being closed [{0}]", popupName));
                return;
            }

            if(popup.enabled == false )
            {
                this.appearedPopupStack.Push(popup);
                int layerIndex = this.appearedPopupStack.Count;
                popup.Activate(layerIndex, popupParam, Callback);
                popup.enabled  = true;
                this.ApplyLayerToCamera(layerIndex);

                this.CheckAndEnableController();
            }
        }
        else
        {
            MyLogger.Red("POPUP", string.Format("[OPEN] Popup NOT found [{0}]", popupName));
        }
    }

    public BasePopupEntity GetPopupEntity(string popupName)
    {
        return this.registeredPopupDict[popupName];
    }

    public void OpenConfirmWithMessage(string message)
    {
        PopupParam popupParam   = new PopupParam();
        popupParam.message      = message;
        this.Open("popup_common_confirm", popupParam);
    }

    public void OpenPacketError(string error)
    {
        PopupParam popupParam   = new PopupParam();
        popupParam.message      = error;
        this.Open("popup_common_confirm", popupParam);
    }

    public bool IsAlive(string popupName)
    {
        foreach (BasePopupEntity popupWindow in this.appearedPopupStack)
        {
            if (popupWindow.CompareByName(popupName))
            {
                return true;
            }
        }
        return false;
    }

    // public void CloseByForce()

    public void Close(string popupName, string aniName ="", float invoke = -1f)
    {
        if(this.registeredPopupDict.ContainsKey(popupName))     
        {
            Stack<BasePopupEntity> comparedPopupStack   = new Stack<BasePopupEntity>();

            while (this.appearedPopupStack.Count > 0)
            {
                BasePopupEntity popup = this.appearedPopupStack.Pop();

                // MyLogger.Log("DEBUG","POPUP " + popup.name + " " + popupName);

                if (popup.CompareByName(popupName))
                {
                    if(popup.enabled)
                    {
                        // at once case is normal, other cases are abnormal
                        // the popup will be closed are locateed at top

                        int layerIndex = this.appearedPopupStack.Count;
                        popup.DeactivateWithCallback(aniName, invoke);
                        popup.enabled  = false;
                        this.ApplyLayerToCamera(layerIndex);
                    }
                    else
                    {
                        MyLogger.LogError("Error", "popup error");
                    }

                    break;
                }
                else
                {
                    comparedPopupStack.Push(popup);
                }
            }   

            while (comparedPopupStack.Count > 0)
            {
                BasePopupEntity popup = comparedPopupStack.Pop();
                this.appearedPopupStack.Push(popup);
                // int layerIndex = this.appearedPopupStack.Count;
                // popup.SetLayer(layerIndex);
            }
        }
        else
        {
            MyLogger.Red("POPUP", string.Format("[CLOSE] Popup NOT found [{0}]", popupName));
        }
    }

    public void CheckAndEnableController()
    {
        if(this.appearedPopupStack.Count <= 0 )
        {
            GameUIManager.instance.SetControllerEnable(true);
            //GameUIManager.instance.SetEffectCameraStatus(true);
        }
        else
        {
            GameUIManager.instance.SetControllerEnable(false);
            //GameUIManager.instance.SetEffectCameraStatus(false);
        }
    }

    public bool IsExistPopup()
    {
        return this.appearedPopupStack.Count > 0;
    }

    /*public void CloseByAndroidBack()
    {
        BasePopupEntity popup = this.appearedPopupStack.Peek();
        MyLogger.Blue("CloseByAndroidBack", popup.GetPop );
        popup.CloseByAndroidBack();
    }*/

    public bool IsPopupAvailableAtPeek(string popupName)
    {        
        BasePopupEntity popup = this.appearedPopupStack.Peek();
        return popup.CompareByName(popupName);
    }
}
