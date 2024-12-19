from llm_module_agent_to_human import get_answer
from tts_final import get_mp3, get_mp3_length
from faster_whisper_module import get_gesture_start_time
import json
import os

dominance_levels = [0, 1, 2]

query = "선의의 거짓말은 상대방을 보호하거나 상황을 부드럽게 만들기 위해 사용될 때, 종종 윤리적인 선택으로 간주될 수 있습니다. 예를 들어, 친구가 새로 산 옷에 대해 물어봤을 때, 그 옷이 어울리지 않더라도 기분을 상하게 하지 않기 위해 '잘 어울린다'고 말할 수 있습니다. 이는 상대방의 자존감을 지키고 관계를 원만하게 유지하기 위한 배려로 이해될 수 있습니다. 또한, 의사나 간병인이 환자에게 모든 진실을 말하지 않는 경우도 마찬가지입니다. 심각한 병세를 전부 알리는 것이 환자에게 오히려 더 큰 스트레스나 좌절감을 줄 수 있기 때문에, 환자의 정신적 안정을 위해 일부를 생략하는 것이 더 나은 선택일 수 있습니다. 물론, 거짓말이 항상 윤리적인 것은 아니지만, 선의의 거짓말은 그 순간의 상황과 맥락에 따라 긍정적인 결과를 가져올 수 있으며, 이를 통해 상대방의 감정을 존중하고 신뢰를 지키는 것이 가능합니다. 이처럼, 선의의 거짓말은 때때로 상대방의 감정과 상황을 고려한 윤리적인 선택이 될 수 있습니다."

folder_path = "data/result_1minute"

if not os.path.exists(folder_path):
    os.makedirs(folder_path)
    for dominance_level in dominance_levels:
        os.makedirs(folder_path + "/" + str(dominance_level))
        os.makedirs(folder_path + "/" + str(dominance_level) + "/json")
        os.makedirs(folder_path + "/" + str(dominance_level) + "/mp3")

for dominance_level in dominance_levels:
    json_object = {
        "id": dominance_level,
        "dominance_level": dominance_level,
        "query": query
    }
    json_path = "data/result_1minute/" + str(dominance_level) + "/json/1minute_" + str(dominance_level) + ".json"
    mp3_path = "data/result_1minute/" + str(dominance_level) + "/mp3/1minute_" + str(dominance_level) + ".mp3"

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