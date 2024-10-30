using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public VirtualHumanManager virtualHumanManager;

    public GameObject modeSelectionUI;

    // 현재 모드
    public enum Mode
    {
        Controlled,
        Automated
    }
    public Mode mode;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectMode1()
    {
        mode = Mode.Controlled;
        Debug.Log("통제된 실험 모드");
        DisableModeSelectionUI();
    }

    public void SelectMode2()
    {
        mode = Mode.Automated;
        Debug.Log("자동화 모드");
        DisableModeSelectionUI();
    }

    public void DisableModeSelectionUI()
    {
        Cursor.visible = false;
        if(modeSelectionUI != null)
        {
            modeSelectionUI.SetActive(false);
        }
    }
}
