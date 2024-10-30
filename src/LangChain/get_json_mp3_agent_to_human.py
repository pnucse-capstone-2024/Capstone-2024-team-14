from llm_module_agent_to_human import get_answer
from tts_final import get_mp3, get_mp3_length
from faster_whisper_module import get_gesture_start_time
import json
import os

dominance_levels = [0, 1, 2]

questions = [
    "좋아하는 계절은 언제인가요? 그 이유도 함께 말해주세요",
    "여행을 가신다면 국내와 해외 중 어디를 선호하시나요? 이유는 무엇인가요",
    "여가 시간에는 주로 무엇을 하시나요",
    "특별히 존경하는 인물이 있나요? 그 이유는 무엇인가요",
    "운동을 좋아하시나요? 좋아하신다면 어떤 운동인가요",
    "미래에 도전해보고 싶은 일이 있으신가요? 그 이유도 함께 말씀해주세요",
    "산이 좋나요 바다가 좋나요? 그 이유도 함께 말해주세요",
    "지난 주 주말에 무엇을 하고 시간을 보내셨나요?",
    "가장 좋아하는 노래 한 곡을 추천해주세요. 그 이유도 함께 말해주세요"
]

questions_list = [
    [
        "어떤 계절을 가장 좋아하시나요? 그리고 그 이유도 들려주실 수 있나요?",
        "여행을 가신다면 국내와 해외 중에서 어디를 더 선호하시나요? 왜 그렇게 생각하시는지 궁금해요.",
        "여가 시간에는 주로 어떤 활동을 하시나요?",
        "혹시 특별히 존경하는 인물이 있으신가요? 그 이유도 함께 말씀해주시면 좋겠어요.",
        "운동을 좋아하시는지 여쭤봐도 될까요? 좋아하신다면 어떤 운동을 즐기시는지 궁금해요.",
        "미래에 도전해보고 싶은 일이 있으신가요? 그 이유도 함께 말씀해주시면 좋을 것 같아요.",
        "산과 바다 중 어느 쪽을 더 좋아하시나요? 그 이유도 함께 듣고 싶어요.",
        "지난 주말에는 무엇을 하셨나요? 시간을 어떻게 보내셨는지 궁금하네요.",
        "가장 좋아하는 노래가 있으신가요? 그 노래를 추천해주실 수 있나요? 이유도 함께요."
    ],
    [
        "어떤 계절을 가장 좋아하시나요? 그리고 왜 그 계절을 좋아하시는지 말씀해주시면 좋겠어요.",
        "여행을 간다면, 국내와 해외 중 어디를 선호하시나요? 이유가 궁금하네요.",
        "여가 시간에는 보통 어떤 활동을 하시나요?",
        "존경하는 인물이 있으신가요? 그 인물을 왜 존경하시는지도 함께 말씀해주시면 좋겠어요.",
        "운동을 좋아하시나요? 좋아하신다면, 어떤 운동을 즐기시는지 알고 싶어요.",
        "미래에 도전해보고 싶은 일이 있으신가요? 그리고 그 이유는 무엇인가요?",
        "산과 바다 중 어느 쪽을 더 좋아하시나요? 그 이유가 궁금합니다.",
        "지난 주말에는 어떻게 시간을 보내셨나요?",
        "가장 좋아하는 노래가 있다면, 그 노래를 추천해주실 수 있나요? 이유도 함께 말씀해주시면 좋겠어요."
    ],
    [
        "좋아하는 계절이 뭐야? 그 계절을 왜 좋아하는지도 말해줄래?",
        "여행을 간다면, 국내랑 해외 중에서 어디를 더 선호해? 그 이유는 뭔가?",
        "여가 시간에 보통 뭘 하면서 보내?",
        "존경하는 인물 있어? 그 사람을 왜 존경하는지도 알려줄 수 있어?",
        "운동 좋아해? 그렇다면, 어떤 운동을 주로 즐겨?",
        "미래에 도전해보고 싶은 게 있다면, 그게 뭐야? 그리고 그 이유도 궁금해.",
        "산이 좋아, 바다가 좋아? 이유도 함께 말해줘.",
        "지난 주말엔 뭘 하면서 보냈어?",
        "좋아하는 노래 하나 추천해줄래? 왜 그 노래가 좋은지도 함께 말해줘."
    ]
]


folder_path = "data/result_qna"

if not os.path.exists(folder_path):
    os.makedirs(folder_path)
    for dominance_level in dominance_levels:
        os.makedirs(folder_path + "/" + str(dominance_level))
        os.makedirs(folder_path + "/" + str(dominance_level) + "/json")
        os.makedirs(folder_path + "/" + str(dominance_level) + "/mp3")


for dominance_level in dominance_levels:
    for q_idx, question in enumerate(questions_list[dominance_level]):
        json_object = {
            "id": dominance_level,
            "dominance_level": dominance_level,
            "query": question
        }
        json_path = "data/result_qna/" + str(dominance_level) + "/json/2-" + str(q_idx + 1) + ".json"
        mp3_path = "data/result_qna/" + str(dominance_level) + "/mp3/2-" + str(q_idx + 1) + ".mp3"

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