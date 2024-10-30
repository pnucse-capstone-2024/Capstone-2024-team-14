from dotenv import load_dotenv
from operator import itemgetter
from langchain.memory import ConversationBufferMemory
from langchain_core.prompts import ChatPromptTemplate, MessagesPlaceholder
from langchain_core.runnables import RunnableLambda, RunnablePassthrough, Runnable
from langchain_openai import ChatOpenAI
from langchain_core.output_parsers import JsonOutputParser
from pydantic import BaseModel, Field
from tts import get_mp3_length
import json


class GestureItem(BaseModel):
    word_or_phrase: str = Field(description="제스처가 강조하는 대사(한국어)의 일부")
    gesture: str = Field(description="구체적으로 수행된 제스처")


class Topic(BaseModel):
    reason: str = Field(description="해당 순간에 이 제스처를 선택한 이유를 단계별로 설명하십시오. 대사와 인물의 성격을 고려해 설명합니다.")
    speech: str = Field(description="인물이 자연스럽게 말하는 대사(한국어).")
    gesture: list[GestureItem] = Field(description="제스처 항목들의 리스트")


load_dotenv()

llm = ChatOpenAI(temperature=0.0, model_name="gpt-4o")

prompt = ChatPromptTemplate.from_messages(
    [
        ("system", """다음 작업에서, 제공된 설명에 맞춰 제공된 speech 중 수행될 제스처를 생성하십시오. 제스처는 제공된 인물의 설명을 반영하여야 합니다.
                        제공된 인물 : {character_description}. 나는 {personality}한 성격을 가진 인물이다.

                        제스처는 말과 일치하고 인물의 성격을 나타내야 합니다. 사용할 수 있는 제스처 목록은 다음과 같습니다: [{gesture_list_str}].

                        다음은 각 제스처에 대한 설명입니다.
                        [
                            Gesture1: 약간 아래를 보며 고개를 좌우로 꺾음. 여러 가능성을 제시하거나, 망설이며 결정을 미루거나, 불확실성을 표현하는 상황 등에 사용
                            Gesture2: 고개를 상하로 끄덕임
                            Gesture3: 고개를 좌우로 저음
                            Gesture4: 고민하거나 의아하다는 듯 고개를 옆으로 꺾음
                            Gesture5: 빈정대며/거만하게 고개를 상하로 끄덕임
                            Gesture6: 짜증내며 고개를 좌우로 저음
                            Gesture7: 고민하다 고개를 좌우로 저음
                            Gesture8: 장황하거나 지루한듯 고개를 상하로 끄덕임
                            Gesture9: 고민하다 고개를 상하로 끄덕임
                        ]
                        
						예제의 word_or_phrase는 제스처가 강조하는 대사(한국어)의 일부를 나타냅니다.
                        예제
                            1: {ex1}
                            2: {ex2}
                            3: {ex3}
"""),
        ("human", "#Format: {format_instructions}\n\n#Speech: {query}"),
    ]
)

output_parser = JsonOutputParser(pydantic_object=Topic)

prompt = prompt.partial(format_instructions=output_parser.get_format_instructions())

# memory = ConversationBufferMemory(return_messages=True, memory_key="chat_history")
chain = prompt | llm | output_parser

examples = [
    ["""{
	"speech": "I don't know if you... I listened to the first two albums, so I'm not sure which is... Did he release anything else after?",
	"gesture": [
		{
			"word_or_phrase": "listened to the first two albums",
			"type": "Gesture4",
			"start": "1.70"
		}
	]
}
    """,
     """{
	"speech": "What good movies have I watched recently? what's your favorite movie?",
	"gesture": [
		{
			"word_or_phrase": "What good movies have I watched recently?",
			"type": "Gesture4",
			"start": "0.0"
		}
	]
}
     """,
     """{
	"speech": "I was also listening to that, because I was much more rock oriented. Now I'm more like pop rock.",
	"gesture": [
		{
			"word_or_phrase": "I was also listening to that",
			"type": "Gesture8",
			"start": "0.0"
		},
		{
			"word_or_phrase": "Now I'm more like",
			"type": "Gesture9",
			"start": "8.10"
		}
	]
}"""],
    ["""{
	"speech": "Yeah we chat with her almost every night every evening because that's like the morning time for them",
	"gesture": [
		{
			"word_or_phrase": "almost every night",
			"type": "Gesture9",
			"start": "3.68"
		},
		{
			"word_or_phrase": "morning time for them",
			"type": "Gesture1",
			"start": "8.10"
		}
	]
}
    """,
     """{
	"speech": "I like country. I like pop. I like... I just don't like screaming metal.",
	"gesture": [
		{
			"word_or_phrase": "country",
			"type": "Gesture4",
			"start": "0.40"
		},
		{
			"word_or_phrase": "I just don't like",
			"type": "Gesture2",
			"start": "4.06"
		},
		{
			"word_or_phrase": "screaming metal",
			"type": "Gesture8",
			"start": "5.42"
		}
	]
}
     """,
     """{
	"speech": "So that part, maybe didn't work out so well, but everything else is great.",
	"gesture": [
		{
			"word_or_phrase": "is great",
			"type": "Gesture9",
			"start": "2.34"
		}
	]
}
     """],
    ["""{
	"speech": "I liked watching that. I like Game of Thrones, but that's not on there. I watched Vampire Diaries. Oh, my god. I loved Vampire Diaries.",
	"gesture": [
		{
			"word_or_phrase": "like Game of Thrones",
			"type": "Gesture7",
			"start": "1.30"
		},
		{
			"word_or_phrase": "not on there",
			"type": "Gesture2",
			"start": "2.10"
		},
		{
			"word_or_phrase": "god",
			"type": "Gesture8",
			"start": "6.87"
		}
	]
}
    """,
     """{
	"speech": "she will look not only at the camera lens, and she will actually making, if she was originally just playing with no facial expressions, if you pose a phone toward her, she will turn her face and smile toward the lens.",
	"gesture": [
		{
			"word_or_phrase": "look",
			"type": "Gesture1",
			"start": "0.57"
		},
		{
			"word_or_phrase": "if she",
			"type": "Gesture8",
			"start": "4.90"
		},
		{
			"word_or_phrase": "her face",
			"type": "Gesture4",
			"start": "12.50"
		}
	]
}
     """,
     """{
	"speech": "I've just heard I've never been there but I've heard their fireworks show every night because they have one every single night. It's good",
	"gesture": [
		{
			"word_or_phrase": "never been there",
			"type": "Gesture3",
			"start": "0.76"
		},
		{
			"word_or_phrase": "because they have one every single night",
			"type": "Gesture1",
			"start": "3.75"
		},
		{
			"word_or_phrase": "It's good"
			"type": "Gesture4", 
			"start": "5.29"
		}
	]
}"""]
]

character_id = -1

character_description = [
    "나는 조용하고 겸손한 성격을 가진 사람이다. 다른 사람의 의견을 우선시하며, 주로 뒤에서 지원하는 역할을 좋아한다. 내성적이지만 다른 사람을 돕고 배려하려는 마음이 크다. 눈에 띄지 않게 행동하며, 상황에 맞춰 조심스럽게 대처하는 편이다.",
    "나는 적당한 자신감과 결단력을 가진 사람이다. 때로는 의견을 강하게 피력하기도 하지만, 대화에서 상대의 의견을 충분히 존중하려고 한다. 주도적인 역할을 하기도 하지만, 상황에 따라 물러날 줄도 아는 균형 잡힌 성격을 지녔다.",
    "나는 자신감 있고 강한 성격을 지닌 사람이다. 대화에서 주도권을 쥐고 이끌어가는 것을 좋아하며, 자신의 의견을 당당하게 말한다. 타인의 의견을 경청하기보다는 내 생각을 앞세우고, 결정적인 순간에 결단력을 발휘하는 편이다."
]

personality = ["submissive", "neither submissive nor dominant", "dominant"]

gesture_list = ["Gesture1" "Gesture2", "Gesture3", "Gesture4", "Gesture5",
                "Gesture6", "Gesture7", "Gesture8", "Gesture9"]
gesture_list_str = ', '.join(gesture_list)


def get_answer(jsonData):
    global memory
    global chain
    global character_id
    global gesture_list_str
    data = json.loads(jsonData)

    # if character_id != data['id']:
    #     character_id = data['id']
    #     memory = ConversationBufferMemory(return_messages=True, memory_key="chat_history")
    #     chain = (
    #             RunnablePassthrough.assign(
    #                 chat_history=RunnableLambda(memory.load_memory_variables)
    #                              | itemgetter(memory.memory_key)
    #             )
    #             | prompt
    #             | llm
    #             | output_parser
    #     )

    chain = prompt | llm | output_parser

    dominance_level = data['dominance_level']

    answer = chain.invoke({
        "character_description": character_description[dominance_level],
        "personality": personality[dominance_level],
        "gesture_list_str": gesture_list_str,
        "ex1": examples[dominance_level][0],
        "ex2": examples[dominance_level][1],
        "ex3": examples[dominance_level][2],
        "query": data['query']
    })
    # memory.save_context(inputs={"human": data['query']}, outputs={"ai": str(answer)})
    return answer


# json_object = {
#     "id": 1,
#     "dominance_level": 2,
#     "query": "선의의 거짓말은 상대방을 보호하거나 상황을 부드럽게 만들기 위해 사용될 때, 종종 윤리적인 선택으로 간주될 수 있습니다. 예를 들어, 친구가 새로 산 옷에 대해 물어봤을 때, 그 옷이 어울리지 않더라도 기분을 상하게 하지 않기 위해 '잘 어울린다'고 말할 수 있습니다. 이는 상대방의 자존감을 지키고 관계를 원만하게 유지하기 위한 배려로 이해될 수 있습니다. 또한, 의사나 간병인이 환자에게 모든 진실을 말하지 않는 경우도 마찬가지입니다. 심각한 병세를 전부 알리는 것이 환자에게 오히려 더 큰 스트레스나 좌절감을 줄 수 있기 때문에, 환자의 정신적 안정을 위해 일부를 생략하는 것이 더 나은 선택일 수 있습니다. 물론, 거짓말이 항상 윤리적인 것은 아니지만, 선의의 거짓말은 그 순간의 상황과 맥락에 따라 긍정적인 결과를 가져올 수 있으며, 이를 통해 상대방의 감정을 존중하고 신뢰를 지키는 것이 가능합니다. 이처럼, 선의의 거짓말은 때때로 상대방의 감정과 상황을 고려한 윤리적인 선택이 될 수 있습니다."
# }

# json_string = json.dumps(json_object)

# ans = get_answer(json_string)

# speech_time = get_mp3_length(ans['speech'], 'result/0/mp3/2-1.mp3')

# ans['speech_time'] = speech_time

# with open('result/0/json/2-1.json', 'w', encoding='utf-8') as json_file:
#     json.dump(ans, json_file, ensure_ascii=False, indent=4)


