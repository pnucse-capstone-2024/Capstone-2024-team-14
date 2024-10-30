from get_speech import get_speech
from llm_module_agent_to_human import get_answer
from tts_final import get_mp3, get_mp3_length
from faster_whisper_module import get_gesture_start_time
import os
import json

# get_speech()

dominance_levels = [0, 1, 2]

character_description = [
    "나는 조용하고 겸손한 성격을 가진 사람이다. 다른 사람의 의견을 우선시하며, 주로 뒤에서 지원하는 역할을 좋아한다. 내성적이지만 다른 사람을 돕고 배려하려는 마음이 크다. 눈에 띄지 않게 행동하며, 상황에 맞춰 조심스럽게 대처하는 편이다.",
    "나는 적당한 자신감과 결단력을 가진 사람이다. 때로는 의견을 강하게 피력하기도 하지만, 대화에서 상대의 의견을 충분히 존중하려고 한다. 주도적인 역할을 하기도 하지만, 상황에 따라 물러날 줄도 아는 균형 잡힌 성격을 지녔다.",
    "나는 자신감 있고 강한 성격을 지닌 사람이다. 대화에서 주도권을 쥐고 이끌어가는 것을 좋아하며, 자신의 의견을 당당하게 말한다. 타인의 의견을 경청하기보다는 내 생각을 앞세우고, 결정적인 순간에 결단력을 발휘하는 편이다."
]

gesture_mapping = {
    "Gesture1": "Head_Gesture",
    "Gesture2": "Head_Nod",
    "Gesture3": "Head_Shake",
    "Gesture4": "Head_Tilt",
    "Gesture5": "Sarcastic_Head_Nod",
    "Gesture6": "Annoyed_Head_Shake",
    "Gesture7": "Thoughtful_Head_Shake",
    "Gesture8": "Lengthy_Head_Nod",
    "Gesture9": "Thoughtful_Head_Nod"
}

# 폴더 경로 설정
folder_path = 'speech'

# speech 값을 저장할 리스트
speech_list = []

# 폴더 내의 json 파일들을 순회
for filename in os.listdir(folder_path):
    if filename.endswith('.json'):
        file_path = os.path.join(folder_path, filename)

        # json 파일 열기
        with open(file_path, 'r', encoding='utf-8') as file:
            data = json.load(file)

            # speech 필드의 값을 추출하여 리스트에 추가
            speech_list.append(data['speech'])

for dominance_level in dominance_levels:
    for idx in range(0, 10):
        # 로그 출력
        print(f"Processing {dominance_level} {idx}")
        json_object = {
            "id": idx,
            "character_description": character_description[dominance_level],
            "dominance_level": dominance_level,
            "query": speech_list[idx]
        }
        json_path = "speech/result_1minute/" + str(dominance_level) + "/json/1minute_" + str(idx) + ".json"
        mp3_path = "speech/result_1minute/" + str(dominance_level) + "/mp3/1minute_" + str(idx) + ".mp3"

        answer = get_answer(json.dumps(json_object), ensure_ascii=False)

        # get_mp3(
        #     speech=answer['speech'],
        #     speaker="nihyun",
        #     dominance_level=dominance_level,
        #     mp3_path=mp3_path
        # )

        # answer = get_gesture_start_time(answer, mp3_path)

        # answer['speech_time'] = get_mp3_length(mp3_path)

        for g in answer['gesture']:
            if g["gesture"] in gesture_mapping:
                g["gesture"] = gesture_mapping[g["gesture"]]

        with open(json_path, 'w', encoding='utf-8') as json_file:
            json.dump(answer, json_file, ensure_ascii=False, indent=4)