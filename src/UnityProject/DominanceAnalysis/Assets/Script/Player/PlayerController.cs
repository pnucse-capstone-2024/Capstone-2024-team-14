using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform cameraTransform;
    public VirtualHumanManager virtualHumanManager;

    private bool inputEnabled = true;

    void Start()
    {
        // Subscribe to the event from VirtualHumanManager
        virtualHumanManager.OnCharacterSwitchComplete += HandleCharacterSwitchComplete;
    }

    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        virtualHumanManager.OnCharacterSwitchComplete -= HandleCharacterSwitchComplete;
    }

    void HandleCharacterSwitchComplete()
    {
        inputEnabled = true;
    }

    void Update()
    {
        // If input is disabled, do not process input
        if (!inputEnabled)
        {
            return;
        }


        // Q를 누르면 VirtualHumanManager의 SwitchToPreviousCharacter 함수 호출
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 로그 출력
            Debug.Log("Q key pressed");
            inputEnabled = false;  // Disable input
            virtualHumanManager.SwitchToPreviousCharacter();
        }

        // E를 누르면 VirtualHumanManager의 SwitchToNextCharacter 함수 호출
        if (Input.GetKeyDown(KeyCode.E))
        {
            inputEnabled = false;  // Disable input
            virtualHumanManager.SwitchToNextCharacter();
        }
    }
}
