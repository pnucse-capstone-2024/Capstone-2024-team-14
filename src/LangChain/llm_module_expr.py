from dotenv import load_dotenv
from langchain_core.prompts import ChatPromptTemplate
from langchain_openai import ChatOpenAI
from langchain_core.output_parsers import JsonOutputParser
from pydantic import BaseModel, Field
import json


class Topic(BaseModel):
    speech: str = Field(description="인물이 자연스럽게 말하는 대사(한국어).")

load_dotenv()

llm = ChatOpenAI(temperature=0.0, model_name="gpt-4o")

prompt = ChatPromptTemplate.from_messages(
    [
        ("system", """다음 작업에서, 제공된 설명에 맞춰 제공된 speech의 말투와 단어 선택을 재구성하세요. 응답은 제공된 인물의 설명을 반영하여야 합니다. 응답에는 제공된 인물의 설명에 대한 내용이 포함되면 안됩니다.
                        제공된 인물 : {character_description}. 나는 {personality}한 성격을 가진 인물이다.

                        대사는 자연스럽고 표현력 있게 작성되어야 합니다.
"""),
        ("human", "#Format: {format_instructions}\n\n#Speech: {query}"),
    ]
)

output_parser = JsonOutputParser(pydantic_object=Topic)

prompt = prompt.partial(format_instructions=output_parser.get_format_instructions())

chain = prompt | llm | output_parser

character_id = -1

character_description = [
    "나는 조용하고 겸손한 성격을 가진 사람이다. 다른 사람의 의견을 우선시하며, 주로 뒤에서 지원하는 역할을 좋아한다. 내성적이지만 다른 사람을 돕고 배려하려는 마음이 크다. 눈에 띄지 않게 행동하며, 상황에 맞춰 조심스럽게 대처하는 편이다.",
    "나는 적당한 자신감과 결단력을 가진 사람이다. 때로는 의견을 강하게 피력하기도 하지만, 대화에서 상대의 의견을 충분히 존중하려고 한다. 주도적인 역할을 하기도 하지만, 상황에 따라 물러날 줄도 아는 균형 잡힌 성격을 지녔다.",
    "나는 자신감 있고 강한 성격을 지닌 사람이다. 대화에서 주도권을 쥐고 이끌어가는 것을 좋아하며, 자신의 의견을 당당하게 말한다. 타인의 의견을 경청하기보다는 내 생각을 앞세우고, 결정적인 순간에 결단력을 발휘하는 편이다."
]

personality = ["submissive", "neither submissive nor dominant", "dominant"]


def get_answer(jsonData):
    global chain
    global character_id
    data = json.loads(jsonData)

    dominance_level = data['dominance_level']

    answer = chain.invoke({
        "character_description": data['character_description'],
        "personality": personality[dominance_level],
        "query": data['query']
    })
    return answer
