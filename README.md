## 1. 📄 프로젝트 소개

### 1.1. 🪶 개요
LLM을 디지털 휴먼에 적용함으로써, 디지털 휴먼은 더 자연스럽고 의미 있는 대화를 생성할 수 있으며, 성격적 특성을 반영하여 상호작용할 수 있습니다. 디지털 휴먼은 언어적 응답 외에도 다양한 비언어적 요소(목소리 톤, 제스처, 시선)를 통해 사용자와 상호작용할 수 있습니다. 본 프로젝트는 대인관계 원형모델에 따라 디지털 휴먼이 지배적 또는 순종적 성향을 나타낼 수 있음을 연구하고, 그에 따른 사용자 인지 변화를 분석하는 것을 목표로 합니다.

### 1.2. 🎯 목표
LLM을 이용해 지배적 행동과 순종적 행동을 표현하는 디지털 휴먼을 통해 사용자가 이러한 성격적 특성을 어떻게 인식하는지에 대한 이해를 높이고자 합니다.

## 2. 👥 팀 소개

|이름|이메일|역할|
|---|---|---|
|이영진(팀장)|wnsgnswls1@pusan.ac.kr|- 유니티 내부 구현<br>- 랭체인 파트 보조<br>- 팀 계획/일정 수립|
|조성환|rnrn213@pusan.ac.kr|- 랭체인 내부 구현<br>- LLM Few-shot Prompting에 사용할 데이터 전처리<br>- LLM으로부터 예비조사용 데이터 생성|
|조주은|cje6407@pusan.ac.kr|- GENEA 데이터 영상화<br>- 예비조사 결과 데이터 분석|

## 3. 🛠️ 시스템 구성도

![시퀀스 다이어그램](https://github.com/user-attachments/assets/0682866c-0473-4384-a24a-a74e1b3086bd)
<p align="center"><시퀀스 다이어그램></p>

### 3.1. 🧑‍💻 시스템 구성 설명
1. **유니티 환경에서의 캐릭터 관리**: VirtualHumanManager를 통해 각 캐릭터의 성격, 발화 및 제스처, 백채널 빈도 등이 관리됩니다.
2. **LLM 기반 데이터 생성**: LLM을 사용해 주어진 성격 설명에 따라 대화와 제스처 데이터를 생성합니다.
3. **실험 모드**: 실험(예비조사) 참가자는 각 캐릭터와 상호작용하며 성격적 차이를 경험합니다.
4. **자동 모드**: 미리 준비된 대화/행동 프리셋없이 랭체인과의 통신을 통해, 실시간으로 상호작용합니다.
<p align="center">
  <img src="https://github.com/user-attachments/assets/0ca3b0d1-a9a7-46cd-969d-3d7e3d82e61b" width="30%" />
  <img src="https://github.com/user-attachments/assets/47c1cb13-34c2-445a-b72d-1a295b95603f" width="30%" />
  <img src="https://github.com/user-attachments/assets/37453af7-dbdb-4181-9692-f325da5bc76d" width="30%" />
</p>

<p align="center"><각각 립싱크, 시선처리, 제스처를 구현한 사진></p>

## 4. 🧪 연구 내용 및 제스처 생성 분석

### 4.1. 📊 예비조사 설계
본 연구에서는 LLM을 활용한 디지털 휴먼과 사용자가 상호작용하는 예비조사을 통해 성격적 특성을 인식하는 과정을 평가하였습니다. 각 디지털 휴먼은 지배적(dominant), 중립적(neutral), 순종적(submissive) 성격을 나타내며, 예비조사 참가자들은 이들 캐릭터와 상호작용하며 설문에 응답하였습니다. 

예비조사는 두 가지 상태에서 진행되었습니다:
1. **음소거 상태**: 시각적 요소(제스처, 시선 등)만을 바탕으로 캐릭터의 성격을 평가.
2. **음소거 해제 상태**: 청각적 요소(목소리 톤, 높낮이 등)가 추가된 상태에서 캐릭터의 성격을 평가.

### 4.2. 📊 예비조사 절차
1. 참가자는 각 캐릭터와 1분 동안 상호작용한 후, 제시된 질문에 따라 캐릭터의 성격을 평가했습니다.
2. 설문은 PA (자기확신/주장) 및 HI (비주장/소심) 관련 10개의 항목으로 구성되었으며, 각 항목은 5점 척도로 평가되었습니다.
3. 동일한 질문 세트를 음소거 상태와 음소거 해제 상태에서 각각 응답하도록 하였습니다.

<p align="center">
  <img src="https://github.com/user-attachments/assets/0d441679-6097-42b4-ae58-726ad2f3283f" width="60%"/>
  <br>
  <img src="https://github.com/user-attachments/assets/d13357a7-823b-484f-bca9-0907ed42379a" width="60%"/>
  <br>
  <구글 설문지 일부의 사진>
</p>

### 4.3. 📊 설문 질문 예시
- 자신만만하다 (1-5점)
- 당당하다 (1-5점)
- 주장적이다 (1-5점)
- 추진력있다 (1-5점)
- 자기확신이 있다 (1-5점)
- 수동적이다 (1-5점)
- 비주장적이다 (1-5점)
- 자신없다 (1-5점)
- 소심하다 (1-5점)
- 유약하다 (1-5점)

### 4.4. 📊 제스처 분석
디지털 휴먼은 성격에 맞는 비언어적 요소(제스처, 시선, 목소리 톤 등)를 기반으로 상호작용합니다. 

- **지배적 성격**: 더 큰 제스처와 빈번한 고개 끄덕임(Head_Nod), 강한 고개 흔들기(Head_Shake)를 사용하였으며, 특히 Sarcastic_Head_Nod와 Annoyed_Head_Shake는 지배적 성격에서만 나타났습니다.
- **중립적 성격**: 상대적으로 균형 잡힌 제스처 사용.
- **순종적 성격**: 작은 동작과 낮은 목소리 톤, 시선을 자주 피하는 행동을 보였습니다.

## 5. 📈 예비조사 결과

1. **음소거 상태 결과**
음소거 상태에서의 설문 분석 결과, 시각적 비언어적 요소가 캐릭터 성격 인식에 큰 영향을 미쳤으며, Low와 Mid, Low와 High 간의 차이가 통계적으로 유의미하였습니다. 
<p align="center">
  <img src="https://github.com/user-attachments/assets/c5a79252-e0f3-4718-84ef-65a51b53d5ca" width="90%">
</p>

2. **음소거 해제 상태 결과**
음소거 해제 상태에서는 시각적 요소와 청각적 요소가 결합되었음에도 불구하고, 청각적 요소만의 효과는 상대적으로 미미했습니다. Mid와 High 그룹 간의 차이는 통계적으로 유의하지 않았습니다.
<p align="center">
  <img src="https://github.com/user-attachments/assets/45be274d-19d9-41c8-91a5-6d8f9ff34ac7" width="90%">
</p>

이는 미리 정해진 대본에 따라 모든 디지털 휴먼의 언어적 **dominance**가 동일하게 설정되었기 때문에, 음성이 포함된 영상에서는 청각적 요소의 지배성이 충분히 드러나지 않고 중화되었을 가능성이 존재합니다. 따라서 언어적 요소가 크게 작용하여 **청각적 요소의 차별성**이 나타나지 않았을 가능성이 있으며, 이는 행동만으로 보여줄 때보다 덜 유의한 결과로 이어졌다고 해석할 수 있습니다.

## 6. ✅ 결론
이러한 결과를 통해, **시각적 비언어적 요소**가 사용자 인식에 미치는 영향이 청각적 요소보다 더 뚜렷하게 나타났으며, 특히 **지배적 성격**의 캐릭터는 제스처의 빈도와 강도에서 차별화되었습니다.

더 자세한 진행/결과 내용은 **최종보고서** 파일을 통해 확인할 수 있습니다.

## 7. 📹 소개 및 시연 영상

[![14_NotHuman_Video](http://img.youtube.com/vi/r3TJZwKrCik/0.jpg)](https://youtu.be/r3TJZwKrCik)

## 8. 🛠️ 설치 및 사용법

### 1. 환경 설정

1. **API 키 설정**
  - **OpenAI API Key**: `src/LangChain/.env` 파일을 열고, `OPENAI_API_KEY` 항목에 API 키를 입력하세요. API 키는 [OpenAI API 키 생성 페이지](https://platform.openai.com/settings/organization/api-keys)에서 생성할 수 있습니다.
  - **CLOVA Voice API**: `CLIENT_ID`와 `CLIENT_SECRET` 항목을 입력하세요. CLOVA Voice API는 [Naver Cloud Guide](https://guide.ncloud-docs.com/docs/clovavoice-start)에서 신청하여 생성할 수 있습니다.

2. **Unity 프로젝트 설정**
  - Unity 2022.3.10f1 버전을 다운로드하고 `src/UnityProject/DominanceAnalysis`를 열어 프로젝트를 불러옵니다.

3. **Google Cloud API 설정**
  - [Google Cloud Console](https://console.cloud.google.com/)에 접속하여 API 키를 생성합니다.
  - **API 및 서비스 - 라이브러리**에서 **Cloud Speech-to-Text API**와 **Cloud Text-to-Speech API**를 검색하고 활성화합니다.

4. **Google Cloud API 키 적용**
  - **GCSpeechRecognition** 및 **GCTextToSpeech** 프리팹을 찾아 Inspector의 Api Key 필드에 생성한 키를 입력합니다.
<p align="center">
  <img src="https://github.com/user-attachments/assets/c5597cae-e181-4977-8bcf-89a5528d7b51" width="45%" />
  <img src="https://github.com/user-attachments/assets/faff47b7-292e-414c-9447-4b0d927f05c9" width="45%" />
</p>

5. **Avatar 에셋 임포트**
  - 'src/UnityProject/Avatar.zip' 파일을 압축 해제하여 'Avatar.unitypackage' 파일을 얻습니다.
  - 유니티 프로젝트가 켜진 상태에서, 해당 파일을 실행하여 임포트합니다.
![Avatar_asset_import](https://github.com/user-attachments/assets/f25e1b5d-c793-4026-95ba-d3f654ed1162)

### 2. 실행 및 모드 선택

1. **유니티 플레이**
  - 플레이하면 CMD 창이 켜지며 LangChain과 자동으로 연결됩니다.

2. **모드 선택**
<p>
  <img src="https://github.com/user-attachments/assets/abb1c203-82bd-4be5-b716-aebcad56bace" width="80%" />
</p>
  - **실험 모드**와 **자동 모드** 중 하나를 선택합니다.
   
  - **실험 모드**:
    - 실험 환경에서 사용하는 모드입니다. `P` 키를 통해 **질의응답 모드**와 **1분 발화 모드**를 토글할 수 있습니다.
    - `Q/E` 키를 통해 디지털 휴먼을 불러옵니다.
    - **질의응답 모드**: `0-9` 키를 사용해 원하는 대화/행동 프리셋을 불러오고, 대화 중 스페이스바를 누른 상태에서 말할 수 있습니다.
    - **1분 발화 모드**: `0-9` 키를 눌러 1분 동안 대화/제스처를 재생합니다.

  - **자동 모드**:
    - 모든 대화와 제스처가 자동으로 생성되는 모드입니다. 대화 중 스페이스바를 누른 상태에서 말할 수 있습니다.