from faster_whisper import WhisperModel
import re
from rapidfuzz import fuzz

def get_gesture_start_time(json_data, audio_file_path):
    # Whisper 모델 로드 (CPU 사용, float32)
    model = WhisperModel("small", device="cpu", compute_type="float32")
    
    # initial_prompt에 대본 추가
    initial_prompt = "This conversation will be conducted in Korean, and the script-like sentences will be output. The script is as follows:" + json_data["speech"]
    
    # 음성 파일을 단어별 타임스탬프로 변환
    segments, _ = model.transcribe(audio_file_path, language="ko", initial_prompt=initial_prompt, beam_size=5, word_timestamps=True)
    
    # 단어와 시작 시간을 리스트로 저장
    model_words = []
    for segment in segments:
        for word in segment.words:
            cleaned_word = re.sub(r'[^\w\s]', '', word.word).strip()
            if cleaned_word:
                model_words.append({
                    'word': cleaned_word,
                    'start_time': word.start
                })
    
    # 모델 단어들을 문자열로 결합
    transcribed_text = ' '.join([w['word'] for w in model_words])
    
    # 대본 전처리
    speech_text = re.sub(r'[^\w\s]', '', json_data["speech"]).strip()
    speech_length = len(speech_text)
    
    # 이전 제스처의 시작 시간을 저장하기 위한 변수
    prev_start_time = 0.0

    # 각 제스처 처리
    for idx, gesture in enumerate(json_data["gesture"]):
        phrase = gesture["word_or_phrase"]
        # 제스처 문구 전처리
        phrase_cleaned = re.sub(r'[^\w\s]', '', phrase).strip()
        phrase_words = phrase_cleaned.split()
        phrase_len = len(phrase_words)
        
        # 매칭 파라미터 설정
        if phrase_len <= 3:
            delta = 0  # 짧은 문구는 길이 변화 없음
            threshold = 90  # 높은 임계값 설정
        else:
            delta = 1  # 길이 변화 허용
            threshold = 70  # 낮은 임계값 설정
        
        # 최적의 매칭 찾기
        best_score = 0
        best_start_time = None
        for l in range(max(1, phrase_len - delta), phrase_len + delta + 1):
            for i in range(len(model_words) - l + 1):
                candidate_words = [w['word'] for w in model_words[i:i + l]]
                score = fuzz.token_sort_ratio(' '.join(phrase_words), ' '.join(candidate_words))
                if score > best_score:
                    best_score = score
                    best_start_time = model_words[i]['start_time']
                    if best_score == 100:
                        break
            if best_score == 100:
                break
        
        if best_score >= threshold and best_start_time is not None:
            # 매칭 점수가 임계값 이상인 경우 시작 시간 설정
            gesture_start_time = float(f"{best_start_time:.2f}")
        else:
            # 매칭 점수가 임계값 미만인 경우 시작 시간 예측
            # 대본에서의 위치 비율 계산
            phrase_index = speech_text.find(phrase_cleaned)
            if phrase_index == -1:
                phrase_index = 0  # 위치를 찾지 못한 경우 0으로 설정
            position_ratio = phrase_index / speech_length if speech_length > 0 else 0
            
            # 다음 제스처의 시작 시간 찾기
            next_start_time = None
            for next_idx in range(idx + 1, len(json_data["gesture"])):
                next_gesture = json_data["gesture"][next_idx]
                if 'start' in next_gesture and next_gesture['start'] is not None:
                    next_start_time = float(next_gesture['start'])
                    break
            if next_start_time is None:
                next_start_time = json_data.get("speech_time", 0.0)
            
            # 시작 시간 예측 및 소수점 둘째 자리까지 표현
            estimated_time = prev_start_time + (next_start_time - prev_start_time) * position_ratio
            estimated_time = max(prev_start_time, min(estimated_time, next_start_time))
            gesture_start_time = float(f"{estimated_time:.2f}")
        
        # 시작 시간 설정 및 이전 시작 시간 업데이트
        gesture["start"] = str(gesture_start_time)
        prev_start_time = gesture_start_time
    
    return json_data
