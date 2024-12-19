from llm_module import get_answer
from tts_final import get_mp3, get_mp3_length
from faster_whisper_module import get_gesture_start_time
import json
import os

dominance_levels = [0, 1, 2]

character_description = [
    "나는 조용하고 겸손한 성격을 가진 사람이다. 다른 사람의 의견을 우선시하며, 주로 뒤에서 지원하는 역할을 좋아한다. 내성적이지만 다른 사람을 돕고 배려하려는 마음이 크다. 눈에 띄지 않게 행동하며, 상황에 맞춰 조심스럽게 대처하는 편이다.",
    "나는 적당한 자신감과 결단력을 가진 사람이다. 때로는 의견을 강하게 피력하기도 하지만, 대화에서 상대의 의견을 충분히 존중하려고 한다. 주도적인 역할을 하기도 하지만, 상황에 따라 물러날 줄도 아는 균형 잡힌 성격을 지녔다.",
    "나는 자신감 있고 강한 성격을 지닌 사람이다. 대화에서 주도권을 쥐고 이끌어가는 것을 좋아하며, 자신의 의견을 당당하게 말한다. 타인의 의견을 경청하기보다는 내 생각을 앞세우고, 결정적인 순간에 결단력을 발휘하는 편이다."
]

questions = [
    "내일 비가 온다는데 실내에서 할 만한 활동 추천해줘",
    "친구 생일 선물로 무엇을 사면 좋을지 추천해줘",
    "집에서 할 수 있는 간단한 운동 추천해줘",
    "스트레스 해소하는 방법 추천해줘",
    "건강에 좋은 음식이나 식단 추천해줘",
    "새로운 취미를 시작하려는데 추천해줘",
    "내일 점심 중식과 일식 중에 뭘 먹을 지 추천해줘",
    "이번 주 주말에 볼 만한 영화 추천해줘",
    "부산에서 여행할 만한 곳 추천해줘"
]

folder_path = "data/result_qna"

if not os.path.exists(folder_path):
    os.makedirs(folder_path)
    for dominance_level in dominance_levels:
        os.makedirs(folder_path + "/" + str(dominance_level))
        os.makedirs(folder_path + "/" + str(dominance_level) + "/json")
        os.makedirs(folder_path + "/" + str(dominance_level) + "/mp3")


for dominance_level in dominance_levels:
    for q_idx, question in enumerate(questions):
        json_object = {
            "id": dominance_level,
            "character_description": character_description[dominance_level],
            "dominance_level": dominance_level,
            "query": question
        }
        json_path = "data/result_qna/" + str(dominance_level) + "/json/1-" + str(q_idx + 1) + ".json"
        mp3_path = "data/result_qna/" + str(dominance_level) + "/mp3/1-" + str(q_idx + 1) + ".mp3"

        answer = get_answer(json.dumps(json_object), ensure_ascii=False)

        get_mp3(
            speech=answer['speech'],
            speaker="nihyun",
            dominance_level=dominance_level,
            mp3_path=mp3_path
        )

        answer = get_gesture_start_time(answer, mp3_path)

        answer['speech_time'] = get_mp3_length(mp3_path)

        with open(json_path, 'w', encoding='utf-8') as json_file:
            json.dump(answer, json_file, ensure_ascii=False, indent=4)