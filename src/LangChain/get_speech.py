from llm_module_expr import get_answer
import json
import os

dominance_levels = [0, 1, 2]

character_description = [
    "나는 조용하고 겸손한 성격을 가진 사람이다. 다른 사람의 의견을 우선시하며, 주로 뒤에서 지원하는 역할을 좋아한다. 내성적이지만 다른 사람을 돕고 배려하려는 마음이 크다. 눈에 띄지 않게 행동하며, 상황에 맞춰 조심스럽게 대처하는 편이다.",
    "나는 적당한 자신감과 결단력을 가진 사람이다. 때로는 의견을 강하게 피력하기도 하지만, 대화에서 상대의 의견을 충분히 존중하려고 한다. 주도적인 역할을 하기도 하지만, 상황에 따라 물러날 줄도 아는 균형 잡힌 성격을 지녔다.",
    "나는 자신감 있고 강한 성격을 지닌 사람이다. 대화에서 주도권을 쥐고 이끌어가는 것을 좋아하며, 자신의 의견을 당당하게 말한다. 타인의 의견을 경청하기보다는 내 생각을 앞세우고, 결정적인 순간에 결단력을 발휘하는 편이다."
]

query = "선의의 거짓말은 상대방을 보호하거나 상황을 부드럽게 만들기 위해 사용될 때, 종종 윤리적인 선택으로 간주될 수 있습니다. 예를 들어, 친구가 새로 산 옷에 대해 물어봤을 때, 그 옷이 어울리지 않더라도 기분을 상하게 하지 않기 위해 '잘 어울린다'고 말할 수 있습니다. 이는 상대방의 자존감을 지키고 관계를 원만하게 유지하기 위한 배려로 이해될 수 있습니다. 또한, 의사나 간병인이 환자에게 모든 진실을 말하지 않는 경우도 마찬가지입니다. 심각한 병세를 전부 알리는 것이 환자에게 오히려 더 큰 스트레스나 좌절감을 줄 수 있기 때문에, 환자의 정신적 안정을 위해 일부를 생략하는 것이 더 나은 선택일 수 있습니다. 물론, 거짓말이 항상 윤리적인 것은 아니지만, 선의의 거짓말은 그 순간의 상황과 맥락에 따라 긍정적인 결과를 가져올 수 있으며, 이를 통해 상대방의 감정을 존중하고 신뢰를 지키는 것이 가능합니다. 이처럼, 선의의 거짓말은 때때로 상대방의 감정과 상황을 고려한 윤리적인 선택이 될 수 있습니다."

folder_path = 'speech'

def get_speech():
    if not os.path.exists(folder_path):
        os.makedirs(folder_path)

    for dominance_level in dominance_levels:
        json_object = {
            "id": 1,
            "character_description": character_description[dominance_level],
            "dominance_level": 0,
            "query": query
        }

        answer = get_answer(json.dumps(json_object), ensure_ascii=False)

        json_path = folder_path + "/" + str(dominance_level) + ".json"

        with open(json_path, 'w', encoding='utf-8') as json_file:
            json.dump(answer, json_file, ensure_ascii=False, indent=4)