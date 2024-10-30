using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Cinemachine;
using System.Threading.Tasks; // Task를 사용하기 위한 네임스페이스 추가


public class VirtualHumanController : MonoBehaviour
{
    // 가상인간 관련 데이터
    [Header("Virtual Human Data")]
    public VirtualHumanData virtualHumanData;  // 가상인간의 대화 및 행동 데이터
    public Personality personality;            // 가상인간의 성격 정보
    public ModeManager modeManager;            // 모드 관리
    public int id;                             // 가상인간의 ID

    // 가상인간의 애니메이션 및 행동 관련
    [Header("Animation and Behavior")]
    public Animator animator;                  // 가상인간 애니메이터
    public Eyes eyes;                          // 가상인간의 시선 처리

    // 대화 및 음성 관련
    [Header("Dialogue and Audio")]
    public TTS tts;                            // 텍스트를 음성으로 변환하는 시스템
    public STT stt;                            // 음성을 텍스트로 변환하는 시스템
    private AudioSource audioSource;            // 오디오 출력 소스
    public bool isBackChannelOn = false;      // 백채널 재생 여부

    // 대화 흐름 관리
    [Header("Conversation Flow")]
    public ConversationState conversationState; // 현재 대화 상태
    private int currentPairIndex = 0;           // 현재 선택된 질문-답변 쌍의 인덱스
    private bool isVirtualHumanSpeaking = false; // 가상인간이 말하는 동안 입력을 막기 위한 플래그

    // 플레이어와의 상호작용 관련
    [Header("Player Interaction")]
    private bool isPlayerInRange = false;       // 플레이어가 가상인간과 상호작용 범위 내에 있는지 여부
    public int dominance;                    // 가상인간의 우세 성향 (외부로 전달된 데이터)
    public SocketConn socketConn;               // 소켓 통신 연결
    private int Monologue_or_QA = 0;              // 0이면 Monologue, 1이면 QA

    // 이동 관련
    [Header("Movement")]
    public Transform lookTargetObject;           // 이동 경로를 복사할 때 사용하는 트랜스폼
    public Transform lookForwardTargetObject;    // 이동 중일 때 앞을 바라보도록 하는 타겟
    public CinemachinePath pathAtoB;            // 경로 A에서 B로 이동하는 경로
    public CinemachinePath pathBtoC;            // 경로 B에서 C로 이동하는 경로
    private bool isMoving = false;              // 가상인간이 이동 중인지 여부
    private CinemachinePath currentPath;        // 현재 이동 중인 경로
    private float pathPosition;                 // 현재 경로에서의 위치
    private Action<VirtualHumanController> onReachDestination; // 목적지에 도착했을 때 실행할 행동

    public enum ConversationState
    {
        UserQuestion,           // 사용자 질문 단계
        VirtualHumanAnswer,     // 가상인간 답변 단계
        VirtualHumanQuestion,   // 가상인간이 사용자에게 질문하는 단계
        UserAnswer              // 사용자가 가상인간의 질문에 답변하는 단계
    }

    private class VHJsonData
    {
        public bool setup { get; set; }
        public float pitch { get; set; }
        public float volume { get; set; }
        public int id { get; set; }
        public string query { get; set; }
        public int dominance_level { get; set; }
        public string character_description { get; set; }
        public string speaker { get; set; }

        public VHJsonData(bool setup, float pitch, float volume, int id, string transcript, int dominance, string personalityDesc, string avatar_name)
        {
            this.setup = setup;
            this.pitch = pitch;
            this.volume = volume;
            this.id = id;
            this.query = transcript;
            this.dominance_level = dominance;
            this.character_description = personalityDesc;
            this.speaker = avatar_name;
        }

        public VHJsonData(bool setup, int dominance, string personalityDesc)
        {
            this.setup = setup;
            this.dominance_level = dominance;
            this.character_description = personalityDesc;
            this.query = "empty for setting";
        }
    }

    private void OnEnable()
    {
        Debug.Log("VirtualHumanController OnEnable");
    }

    private void OnDisable()
    {
        stt.EventDisable();
        tts.EventDisable();
        stt.OnTranscriptReceived -= HandleTranscriptReceived;
    }

    // Start is called before the first frame update
    void Start()
    {
        lookForwardTargetObject = transform.Find("LookForwardTarget");
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        modeManager = GameObject.Find("ModeManager").GetComponent<ModeManager>();
        socketConn = GameObject.Find("SocketManager").GetComponent<SocketConn>();
        personality = GetComponent<Personality>();
        dominance = personality.CalculateDominance();

        tts = GetComponent<TTS>();
        stt = GetComponent<STT>();

        tts.SetVolumePitch(GetVolumePitch());

        if (modeManager.mode == ModeManager.Mode.Automated)
        {
            StartCoroutine(SetBackChannelFrequency());
        }

        animator.SetInteger("DominanceLevel", dominance);

        conversationState = ConversationState.UserQuestion;

    }

    IEnumerator SetBackChannelFrequency()
    {
        if (socketConn.client != null && socketConn.client.Connected)
        {
            string jsonData = Convert2Json("", true);
            Debug.Log($"JSON Data: {jsonData}");

            Task<string> task = socketConn.SendAndReceiveDataAsync(jsonData);
            yield return new WaitUntil(() => task.IsCompleted);

            string response = task.Result;
            Debug.Log($"Response: {response}");

            VHResponseData responseData = JsonConvert.DeserializeObject<VHResponseData>(response);
            if (responseData != null)
            {
                personality.backChannelFrequency = responseData.backchannel;
            }
        }
        else
        {
            Debug.LogError("Client is not connected.");
        }
    }

    public void SetPositionAtA()
    {
        // A 지점에 위치시키기 (pathAtoB 경로의 시작점)
        currentPath = pathAtoB;
        pathPosition = 0f;
        transform.position = currentPath.EvaluatePositionAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Normalized);
    }

    public void SetPositionAtB()
    {
        currentPath = pathBtoC;
        pathPosition = 0f;
        transform.position = currentPath.EvaluatePositionAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Normalized);
    }

    public void SetPositionAtC()
    {
        // C 지점에 위치시키기 (pathBtoC 경로의 끝점)
        currentPath = pathBtoC;
        pathPosition = 1f;
        transform.position = currentPath.EvaluatePositionAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Normalized);
    }

    public void MoveToPointA(Action<VirtualHumanController> onReachDestination = null)
    {
        pathPosition = 1f;
        currentPath = pathAtoB;
        this.onReachDestination = onReachDestination;
        MoveAlongPath(-1);  // A로 가려면 역방향
    }

    public void MoveToPointB(Action<VirtualHumanController> onReachDestination = null)
    {
        currentPath = (currentPath == pathBtoC) ? pathBtoC : pathAtoB;
        this.onReachDestination = onReachDestination;
        MoveAlongPath((currentPath == pathAtoB) ? 1 : -1);  // A에서 B 또는 C에서 B
    }

    public void MoveToPointC(Action<VirtualHumanController> onReachDestination = null)
    {
        pathPosition = 0f;
        currentPath = pathBtoC;
        this.onReachDestination = onReachDestination;
        MoveAlongPath(1);  // C로 가려면 정방향
    }

    private void MoveAlongPath(int direction)
    {
        if (currentPath == null) return;
        StartCoroutine(MoveAndAnimate(direction));
    }

    private IEnumerator MoveAndAnimate(int direction)
    {
        isMoving = true;
        animator.SetTrigger("Walk");

        eyes.lookTarget = lookForwardTargetObject;

        float speed = 0.2f;

        // direction에 따른 이동 범위
        float start = direction > 0 ? 0f : 1f;
        float end = direction > 0 ? 1f : 0f;

        // 로그 출력
        Debug.Log($"Move from {start} to {end}, pathPosition {pathPosition}");

        while ((direction > 0 && pathPosition < end) || (direction < 0 && pathPosition > end))
        {
            pathPosition += direction * speed * Time.deltaTime;
            pathPosition = Mathf.Clamp01(pathPosition);
            Vector3 targetPosition = currentPath.EvaluatePositionAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Normalized);
            transform.position = targetPosition;

            // 걷는 방향으로 부드럽게 회전
            if (direction > 0)
            {
                transform.LookAt(targetPosition + currentPath.EvaluateTangentAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Normalized));
            }
            else
            {
                transform.LookAt(targetPosition - currentPath.EvaluateTangentAtUnit(pathPosition, CinemachinePathBase.PositionUnits.Normalized));
            }

            yield return null;
        }

        transform.position = currentPath.EvaluatePositionAtUnit(end, CinemachinePathBase.PositionUnits.Normalized);

        animator.SetTrigger("Idle");
        isMoving = false;

        // 도착 콜백 호출
        onReachDestination?.Invoke(this);
        eyes.LookForward();
        
    }

    public bool HasReachedDestination()
    {
        return !isMoving;
    }

    public IEnumerator PlayGreetingAudioCoroutine()
    {
        if (virtualHumanData.greetingAudio != null)
        {
            audioSource.clip = virtualHumanData.greetingAudio;
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }

    public IEnumerator PlayFarewellAudioCoroutine()
    {
        if (virtualHumanData.farewellAudio != null)
        {
            audioSource.clip = virtualHumanData.farewellAudio;
            audioSource.Play();
            yield return new WaitWhile(() => audioSource.isPlaying);
        }
    }

    // 가상인간의 입장 인사 처리 (Q 키로 호출됨)
    public void Greet()
    {
        Debug.Log("가상인간 입장 인사");
        //PlayAudioAndMotion(virtualHumanData.greetingAudio, null);  // 인사 오디오와 모션 실행
        conversationState = ConversationState.UserQuestion;  // 입장 후 사용자 질문 단계로 설정
    }

    // 가상인간의 퇴장 인사 처리 (E 키로 호출됨)
    public void Farewell()
    {
        Debug.Log("가상인간 퇴장 인사");
        // PlayAudioAndMotion(virtualHumanData.farewellAudio, null);  // 퇴장 오디오와 모션 실행
        // 퇴장 후 관련 로직 추가 가능 (모드에 따른 다른 처리도 추가할 수 있음)
    }

    // 통제된 실험 모드에서 가상인간의 답변을 처리하는 함수
    IEnumerator ProcessVirtualHumanAnswerControlled()
    {
        var pair = virtualHumanData.questionAnswerPairs[currentPairIndex];

        string virtualHumanAnswer = pair.virtualHumanAnswer;
        List<GestureData> gestures = pair.virtualHumanAnswerGestures;

        Debug.Log("가상인간의 답변: " + virtualHumanAnswer);
        Debug.Log("제스처 데이터 개수: " + gestures.Count);

        // 제스처와 대화를 실행
        yield return StartCoroutine(ExecuteGesturesAndDialog(gestures, virtualHumanAnswer));
    }

    // 자동화된 실험 모드에서 가상인간의 답변을 처리하는 코루틴 함수
    IEnumerator ProcessVirtualHumanAnswerAutomated(string transcript)
    {
        Debug.Log($"ContTranscript: {transcript}");
        if (socketConn.client != null && socketConn.client.Connected)
        {
            string jsonData = Convert2Json(transcript, false);
            Debug.Log($"JSON Data: {jsonData}");
            Task<string> task = socketConn.SendAndReceiveDataAsync(jsonData);
            Debug.Log("Task Started");
            yield return new WaitUntil(() => task.IsCompleted);

            string response = task.Result;

            // response 출력
            Debug.Log($"Response: {response}");

            // 받은 JSON 데이터를 Parsing
            VHResponseData responseData = JsonConvert.DeserializeObject<VHResponseData>(response);

            if (responseData != null)
            {
                // gesture 리스트와 speech 텍스트를 분리하여 처리
                List<GestureData> gestures = responseData.gesture;
                string dialogText = responseData.speech;

                Debug.Log("가상인간의 자동화된 답변: " + dialogText);
                Debug.Log("제스처 데이터 개수: " + gestures.Count);

                // 상대 경로의 파일을 재생
                string relativePath = "../../../LangChain/mp3/answer.mp3"; // 상대 경로
                string audioPath = System.IO.Path.Combine(Application.dataPath, relativePath); // 절대 경로로 변환
                AudioClip audioClip = LoadAudioClip(audioPath);

                // 제스처와 대사를 실행
                yield return StartCoroutine(ExecuteGesturesAndAudio(gestures, audioClip));
            }
            else
            {
                Debug.LogError("Failed to parse the response from LLM.");
            }
        }
        else
        {
            Debug.LogError("Client is not connected.");
        }
    }

    // 통제된 실험 모드에서 가상인간의 질문을 처리하는 함수
    IEnumerator ProcessVirtualHumanQuestionControlled()
    {
        var pair = virtualHumanData.questionAnswerPairs[currentPairIndex];

        string virtualHumanQuestion = pair.virtualHumanQuestion;
        List<GestureData> gestures = pair.virtualHumanQuestionGestures;

        Debug.Log("가상인간의 질문: " + virtualHumanQuestion);
        Debug.Log("제스처 데이터 개수: " + gestures.Count);

        // 제스처와 대화를 실행
        yield return StartCoroutine(ExecuteGesturesAndDialog(gestures, virtualHumanQuestion));
    }

    // 자동화된 실험 모드에서 가상인간의 질문을 처리하는 함수
    IEnumerator ProcessVirtualHumanQuestionAutomated()
    {
        int randomIndex = UnityEngine.Random.Range(0, virtualHumanData.questionAnswerPairs.Length);
        var pair = virtualHumanData.questionAnswerPairs[randomIndex];

        string virtualHumanQuestion = pair.virtualHumanQuestion;
        List<GestureData> gestures = pair.virtualHumanQuestionGestures;

        Debug.Log("가상인간의 자동화된 질문: " + virtualHumanQuestion);
        Debug.Log("제스처 데이터 개수: " + gestures.Count);

        // 제스처와 대사를 실행
        yield return StartCoroutine(ExecuteGesturesAndDialog(gestures, virtualHumanQuestion));
    }

    // 다음 상태로 넘어가기 전에 잠시 대기하는 코루틴
    IEnumerator WaitBeforeProceeding(float second)
    {
        yield return new WaitForSeconds(second);
    }

    // 성공적으로 사용자의 마이크 입력을 받았을 때 호출되는 함수
    void HandleTranscriptReceived(string transcript)
    {
        // 로그 출력
        Debug.Log("Transcript: " + transcript);
        // modeManager.mode가 automated인 경우
        if (modeManager.mode == ModeManager.Mode.Automated)
        {
            StartCoroutine(HandleAutomatedTranscript(transcript));
        }
        else if(modeManager.mode == ModeManager.Mode.Controlled)
        {
            StartCoroutine(HandleControlledTranscript(transcript));
        }
    }

    // modeManager.mode가 controlled인 경우 수행하는 코루틴
    IEnumerator HandleControlledTranscript(string transcript)
    {
        switch (conversationState)
        {
            case ConversationState.UserQuestion:
                isVirtualHumanSpeaking = true;
                conversationState = ConversationState.VirtualHumanAnswer;
                yield return StartCoroutine(ProcessVirtualHumanAnswerControlled());
                yield return StartCoroutine(WaitBeforeProceeding(1.0f)); // 필요에 따라 대기 시간 조절

                conversationState = ConversationState.VirtualHumanQuestion;
                yield return StartCoroutine(ProcessVirtualHumanQuestionControlled());
                yield return StartCoroutine(WaitBeforeProceeding(1.0f)); // 필요에 따라 대기 시간 조절

                isVirtualHumanSpeaking = false;
                conversationState = ConversationState.UserAnswer;
                break;
            case ConversationState.UserAnswer:
                conversationState = ConversationState.UserQuestion;
                break;
        }
    }

    // modeManager.mode가 automated인 경우 수행하는 함수
    IEnumerator HandleAutomatedTranscript(string transcript)
    {
        switch (conversationState)
        {
            case ConversationState.UserQuestion:
                isVirtualHumanSpeaking = true;
                conversationState = ConversationState.VirtualHumanAnswer;
                yield return ProcessVirtualHumanAnswerAutomated(transcript);

                //conversationState = ConversationState.VirtualHumanQuestion;
                //yield return ProcessVirtualHumanQuestionAutomated();
                //yield return StartCoroutine(WaitBeforeProceeding(1.0f));

                isVirtualHumanSpeaking = false;
                conversationState = ConversationState.UserQuestion;
                //conversationState = ConversationState.UserAnswer;
                break;
            default:
                // 에러 발생
                Debug.LogError("Invalid conversation state");
                break;
            //case ConversationState.UserAnswer:
            //    conversationState = ConversationState.UserQuestion;
            //    break;
        }
    }

    // 스페이스바 입력을 처리할수 있는 상태인가
    public bool CanProcessUserInput()
    {
        return conversationState == ConversationState.UserQuestion || conversationState == ConversationState.UserAnswer;
    }

    // 사용자의 질문, 가상인간의 성격을 JSON 형태로 변환
    string Convert2Json(string transcript, bool isSetup)
    {
        VHJsonData vHJsonData;
        if (isSetup)
        {
            vHJsonData = new VHJsonData(isSetup, dominance, personality.personalityDesc);
        }
        else
        {
            vHJsonData = new VHJsonData(isSetup, tts.pitch, tts.volume, id, transcript, dominance, personality.personalityDesc, tts.avatar_name);
        }

        return JsonConvert.SerializeObject(vHJsonData);

    }

    // 상대경로를 통해 오디오 클립을 반환하는 함수


    // 제스처와 대화를 실행하고 대화가 완료될 때까지 대기하는 코루틴
    IEnumerator ExecuteGesturesAndDialog(List<GestureData> gestures, string dialogText)
    {
        // 대화가 끝날 때까지 대기할 플래그
        bool isDialogCompleted = false;
        Action onPlaybackCompleted = () => { isDialogCompleted = true; };

        // TTS 재생 완료 콜백 설정
        tts.OnPlaybackCompleted += onPlaybackCompleted;
        // 대화 재생 시작
        tts.PlayDialog(dialogText);

        // 제스처 실행 코루틴을 병렬로 실행
        StartCoroutine(ExecuteAllGestures(gestures));
        // 대화가 완료될 때까지 대기
        while (!isDialogCompleted)
        {
            yield return null;
        }

        tts.OnPlaybackCompleted -= onPlaybackCompleted;
    }

    // 제스처와 대화를 실행하고 대화가 완료될 때까지 대기하는 코루틴
    IEnumerator ExecuteGesturesAndAudio(List<GestureData> gestures, AudioClip audioClip)
    {
        // 대화가 끝날 때까지 대기할 플래그
        bool isDialogCompleted = false;

        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();

            // 오디오의 길이 출력
            Debug.Log($"Audio clip length: {audioClip.length}");

            // 오디오 재생 완료 후 isDialogCompleted를 true로 설정
            StartCoroutine(WaitForAudioToComplete(audioClip.length, () => { isDialogCompleted = true; }));
        }
        else
        {
            Debug.LogError($"Failed to load audio clip");
            isDialogCompleted = true; // 오류 발생 시에도 바로 완료 처리
        }

        // 제스처 실행 코루틴을 병렬로 실행
        StartCoroutine(ExecuteAllGestures(gestures));

        // 대화가 완료될 때까지 대기
        while (!isDialogCompleted)
        {
            yield return null;
        }
    }

    // 제스처 리스트를 병렬로 실행하는 코루틴
    IEnumerator ExecuteAllGestures(List<GestureData> gestures)
    {
        foreach (GestureData gesture in gestures)
        {
            // 개별 제스처를 병렬로 실행
            StartCoroutine(ExecuteGesture(gesture));
        }

        yield return null;
    }

    // 오디오 재생이 완료될 때까지 대기하는 코루틴
    IEnumerator WaitForAudioToComplete(float duration, Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        Debug.Log("Audio playback completed.");
        onComplete?.Invoke();  // 오디오 재생이 끝나면 isDialogCompleted를 true로 설정하는 콜백 호출
    }

    // 오디오 클립을 로드하는 함수
    AudioClip LoadAudioClip(string path)
    {
        AudioClip clip = null;
        try
        {
            // UnityWebRequest를 사용하여 로컬 파일 로드
            var www = new WWW("file://" + path);
            while (!www.isDone) { } // 비동기 완료 대기

            // 파일 패스 출력
            Debug.Log($"Loaded audio file from path: {path}");
            clip = www.GetAudioClip();
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading audio file: {e.Message}");
        }

        return clip;
    }

    // 제스처 실행을 담당하는 코루틴 (end 시간이 없으므로 start 시간에만 맞춰 실행)
    IEnumerator ExecuteGesture(GestureData gesture)
    {
        // 제스처의 시작 시간까지 대기
        if (gesture.start > 0)
        {
            yield return new WaitForSeconds(gesture.start);
        }

        // 제스처 실행
        Debug.Log($"Executing gesture: {gesture.gesture}");

        // 제스처 타입이 "Head_Common"이 아닌 경우에만 트리거 실행
        if (gesture.gesture != "Head_Common")
        {
            // 애니메이터 트리거 실행
            animator.SetTrigger(gesture.gesture);
        }

        Debug.Log($"Gesture {gesture.gesture} executed at {gesture.start} seconds.");
    }

    public void OnGestureStart()
    {
        eyes.EnableEye(false);
        eyes.EnableHead(false);
    }

    public void OnGestureEnd()
    {
        eyes.UpdateRuntimeExpressionControllers(ref eyes.heads);
        eyes.UpdateRuntimeExpressionControllers(ref eyes.eyes);

        eyes.EnableEye(true);
        eyes.EnableHead(true);
    }

    public void PlayAction(string action)
    {
        dominance = personality.CalculateDominance();
        string actionClipName = $"{action}_{dominance}";
        animator.Play(actionClipName);
    }

    // 사용자를 바라보는 시선 조정
    void AdjustLookTarget(int dominance)
    {
        // 메인 카메라 아래의 "LookTarget" 태그를 가진 오브젝트를 찾음
        GameObject lookTargetObject = GameObject.FindWithTag("LookTarget");

        if (lookTargetObject == null)
        {
            Debug.LogError("Look target object with tag 'LookTarget' not found under the main camera!");
            return;
        }

        eyes.headTargetOffset = dominance == 0 ? new Vector3(0, -0.035f, 0) :
                                dominance == 1 ? new Vector3(0, 0.015f, 0) :
                                new Vector3(0, 0.065f, 0);

        // 지정된 오브젝트의 Transform을 사용
        Transform copiedTransform = lookTargetObject.transform;

        // 카메라의 위치에 맞춰 로컬 포지션을 업데이트
        // low면 -0.2, mid면 -0.1, high면 0
        float yOffset = dominance == 0 ? -0.16f : dominance == 1 ? -0.08f : 0f;

        // 상대를 쳐다보는 빈도를 결정하는 변수
        float lookAtFrequency = dominance == 0 ? 0.3f : dominance == 1 ? 0.6f : 0.9f;

        // Transform의 localPosition 값을 업데이트
        copiedTransform.localPosition = new Vector3(0, yOffset, 0);

        // eyes.lookTarget을 찾은 오브젝트로 설정
        eyes.lookTarget = copiedTransform;
        eyes.affinityPercentage = lookAtFrequency;

    }

    // 백채널 재생 여부 결정
    public bool ShouldPlayBackChannel()
    {
        if (!isBackChannelOn) return false;
        return UnityEngine.Random.Range(0f, 1f) <= personality.backChannelFrequency;
    }

    // 백채널 재생
    public void PlayBackChannel()
    {
        bool bShouldPlay = ShouldPlayBackChannel();
        if (audioSource.isPlaying || bShouldPlay) return;

        int randomIndex = UnityEngine.Random.Range(0, virtualHumanData.backChannelAudios.Count);
        AudioClip clipToPlay = virtualHumanData.backChannelAudios[randomIndex];
        
        // 오디오 소스에 clipToPlay를 설정하고 재생
        audioSource.clip = clipToPlay;
        audioSource.Play();
    }

    // Dominance에 따른 볼륨과 피치 값 반환
    public (int, int) GetVolumePitch()
    {
        return dominance == 0 ? (-5, 0) :
               dominance == 1 ? (0, -1) :
               (5, -2);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            AdjustLookTarget(dominance);
            eyes.LookForward();
            stt.OnTranscriptReceived += HandleTranscriptReceived;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
            eyes.lookTarget = null;
            stt.OnTranscriptReceived -= HandleTranscriptReceived;
        }
    }

    void Update()
    {
        if (isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.Space) && CanProcessUserInput())
            {
                stt.StartRecord();
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                stt.StopRecord();
            }

            if(Monologue_or_QA == 0 && CanProcessUserInput())
            {
                if (Input.GetKeyDown(KeyCode.Alpha1) && conversationState == ConversationState.UserQuestion)
                {
                    StartCoroutine(ExecuteGesturesAndAudio(virtualHumanData.monologueDatas[0].monologueGestures, virtualHumanData.monologueDatas[0].monologueAudio));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && conversationState == ConversationState.UserQuestion)
                {
                    StartCoroutine(ExecuteGesturesAndAudio(virtualHumanData.monologueDatas[1].monologueGestures, virtualHumanData.monologueDatas[1].monologueAudio));
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3) && conversationState == ConversationState.UserQuestion)
                {
                    StartCoroutine(ExecuteGesturesAndAudio(virtualHumanData.monologueDatas[2].monologueGestures, virtualHumanData.monologueDatas[2].monologueAudio));
                }
            }
            else if(Monologue_or_QA == 1 && CanProcessUserInput())
            {
                // 1~9 키중 하나를 누르면 누른 숫자-1로 currentPairIndex를 설정. conversationState가 UserQuestion인 경우에만 적용
                if (Input.GetKeyDown(KeyCode.Alpha1) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 0;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 1;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 2;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 3;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 4;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 5;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha7) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 6;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha8) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 7;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha9) && conversationState == ConversationState.UserQuestion)
                {
                    currentPairIndex = 8;
                }
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Monologue_or_QA = Monologue_or_QA == 0 ? 1 : 0;
            }

        }
        else
        {
            if(isMoving)
            {
                eyes.lookTarget = lookForwardTargetObject;
                eyes.LookForward();
            }
        }
    }
}