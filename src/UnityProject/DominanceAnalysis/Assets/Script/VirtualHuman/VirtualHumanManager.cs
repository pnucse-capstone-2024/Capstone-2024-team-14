using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Action 델리게이트 사용을 위해 System 네임스페이스 추가
using System;

public class VirtualHumanManager : MonoBehaviour
{
    public List<GameObject> characterPrefabs;

    [SerializeField]
    private int currentCharacterIndex = -1;
    private GameObject currentCharacter;

    public CinemachinePath pathAtoB;
    public CinemachinePath pathBtoC;

    public event Action OnCharacterSwitchComplete;

    private bool isSwitching = false;

    public CameraFade cameraFade;

    // Start is called before the first frame update
    void Start()
    {
        cameraFade = GameObject.Find("Main Camera").GetComponent<CameraFade>();
    }

    public void SwitchToPreviousCharacter()
    {
        if (isSwitching) return;
        StartCoroutine(SwitchToPreviousCharacterCoroutine());
    }

    public void SwitchToNextCharacter()
    {
        if (isSwitching) return;
        StartCoroutine(SwitchToNextCharacterCoroutine());
    }

    /*private IEnumerator SwitchToPreviousCharacterCoroutine()
    {
        isSwitching = true;

        // If there is a current character, send it off first
        if (currentCharacter != null)
        {
            // Farewell and move current character to point A
            VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
            yield return StartCoroutine(vhController.PlayFarewellAudioCoroutine());
            vhController.MoveToPointC();

            // Wait until character reaches destination
            yield return new WaitUntil(() => vhController.HasReachedDestination());
            Destroy(currentCharacter);
            currentCharacter = null;

            isSwitching = false;
            OnCharacterSwitchComplete?.Invoke();
            yield break;
        }

        // Decrease character index
        int newCharacterIndex = currentCharacterIndex - 1;

        if (newCharacterIndex >= 0 && newCharacterIndex < characterPrefabs.Count)
        {
            currentCharacterIndex = newCharacterIndex;
            // Spawn new character at point A and move to B
            SpawnCharacterAtA(currentCharacterIndex);
            VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
            vhController.MoveToPointB();

            // Wait until character reaches destination
            yield return new WaitUntil(() => vhController.HasReachedDestination());

            // Play greeting audio
            yield return StartCoroutine(vhController.PlayGreetingAudioCoroutine());
        }
        else
        {
            // No more characters to switch to
            currentCharacterIndex = -1; // Reset to -1 when no character is present
    
  
        }

        isSwitching = false;
        // Notify that character switch is complete
        OnCharacterSwitchComplete?.Invoke();
    }

    private IEnumerator SwitchToNextCharacterCoroutine()
    {
        isSwitching = true;

        // If there is a current character, send it off first
        if (currentCharacter != null)
        {
            // Farewell and move current character to point C
            VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
            yield return StartCoroutine(vhController.PlayFarewellAudioCoroutine());
            vhController.MoveToPointA();

            // Wait until character reaches destination
            yield return new WaitUntil(() => vhController.HasReachedDestination());
            Destroy(currentCharacter);
            currentCharacter = null;

            isSwitching = false;
            OnCharacterSwitchComplete?.Invoke();
            yield break;
        }

        // Increase character index
        int newCharacterIndex = currentCharacterIndex + 1;

        if (newCharacterIndex >= 0 && newCharacterIndex < characterPrefabs.Count)
        {
            currentCharacterIndex = newCharacterIndex;
            // Spawn new character at point C and move to B
            SpawnCharacterAtC(currentCharacterIndex);
            VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
            vhController.MoveToPointB();

            // Wait until character reaches destination
            yield return new WaitUntil(() => vhController.HasReachedDestination());

            // Play greeting audio
            yield return StartCoroutine(vhController.PlayGreetingAudioCoroutine());
        }
        else
        {
            // No more characters to switch to
            currentCharacterIndex = characterPrefabs.Count - 1; // Reset to last valid index
        }

        isSwitching = false;
        // Notify that character switch is complete
        OnCharacterSwitchComplete?.Invoke();
    }*/

    private IEnumerator SwitchToPreviousCharacterCoroutine()
    {
        isSwitching = true;

        // If there is a current character, send it off first
        if (currentCharacter != null)
        {
            // Farewell and move current character to point A
            VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
            yield return StartCoroutine(vhController.PlayFarewellAudioCoroutine());

            // Wait until fade out is done
            cameraFade.StartFadeOut();
            yield return new WaitForSeconds(cameraFade.fadeDuration);

            Destroy(currentCharacter);
            currentCharacter = null;
            currentCharacterIndex -= 1;

            isSwitching = false;
            OnCharacterSwitchComplete?.Invoke();
            yield break;
        }

        if (currentCharacterIndex >= 0 && currentCharacterIndex < characterPrefabs.Count)
        {
            // Spawn new character at point B
            SpawnCharacterAtB(currentCharacterIndex);
            VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();

            // Wait until fade in is done
            cameraFade.StartFadeIn();
            yield return new WaitForSeconds(cameraFade.fadeDuration);

            // Play greeting audio
            yield return StartCoroutine(vhController.PlayGreetingAudioCoroutine());
        }
        else
        {
            // No more characters to switch to
            currentCharacterIndex = -1; // Reset to -1 when no character is present
        }

        isSwitching = false;
        // Notify that character switch is complete
        OnCharacterSwitchComplete?.Invoke();
    }

    private IEnumerator SwitchToNextCharacterCoroutine()
    {
        isSwitching = true;

        // If there is a current character, send it off first
        if (currentCharacter != null)
        {
            // Farewell and move current character to point C
            VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
            yield return StartCoroutine(vhController.PlayFarewellAudioCoroutine());

            // Wait until fade out is done
            cameraFade.StartFadeOut();
            yield return new WaitForSeconds(cameraFade.fadeDuration);

            Destroy(currentCharacter);
            currentCharacter = null;

            isSwitching = false;
            OnCharacterSwitchComplete?.Invoke();
            yield break;
        }

        // Increase character index
        int newCharacterIndex = currentCharacterIndex + 1;

        if (newCharacterIndex >= 0 && newCharacterIndex < characterPrefabs.Count)
        {
            currentCharacterIndex = newCharacterIndex;
            // Spawn new character at point B
            SpawnCharacterAtB(currentCharacterIndex);
            VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();

            // Wait until fade in is done
            cameraFade.StartFadeIn();
            yield return new WaitForSeconds(cameraFade.fadeDuration);

            // Play greeting audio
            yield return StartCoroutine(vhController.PlayGreetingAudioCoroutine());
        }
        else
        {
            // No more characters to switch to
            currentCharacterIndex = characterPrefabs.Count - 1; // Reset to last valid index
        }

        isSwitching = false;
        // Notify that character switch is complete
        OnCharacterSwitchComplete?.Invoke();
    }

    private void SpawnCharacterAtA(int index)
    {
        currentCharacter = Instantiate(characterPrefabs[index]);
        VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
        vhController.pathAtoB = pathAtoB;
        vhController.pathBtoC = pathBtoC;
        vhController.SetPositionAtA();
        vhController.id = index;
    }

    private void SpawnCharacterAtC(int index)
    {
        currentCharacter = Instantiate(characterPrefabs[index]);
        VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
        vhController.pathAtoB = pathAtoB;
        vhController.pathBtoC = pathBtoC;
        vhController.SetPositionAtC();
        vhController.id = index;
    }

    private void SpawnCharacterAtB(int index)
    {
        currentCharacter = Instantiate(characterPrefabs[index]);
        VirtualHumanController vhController = currentCharacter.GetComponent<VirtualHumanController>();
        vhController.pathAtoB = pathAtoB;
        vhController.pathBtoC = pathBtoC;
        vhController.SetPositionAtB(); // B 지점에서 시작
        vhController.id = index;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
