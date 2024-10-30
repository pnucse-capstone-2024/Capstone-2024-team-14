from dotenv import load_dotenv
from langchain_openai import ChatOpenAI
from langchain_core.prompts import PromptTemplate
from langchain_core.output_parsers import StrOutputParser
import json

load_dotenv()

llm = ChatOpenAI()

template = """
You are a person with the following personality traits, scored from 0 to 1:
- PA (Assured-Dominant): {pa_score}
- BC (Arrogant-Calculating): {bc_score}
- DE (Cold-Hearted): {de_score}
- FG (Aloof-Introverted): {fg_score}
- HI (Unassured-Submissive): {hi_score}
- JK (Unassuming-Ingenuous): {jk_score}
- LM (Warm-Agreeable): {lm_score}
- NO (Gregarious-Extraverted): {no_score}

Respond in Korean to the following statement: "{question}"
Only provide the response in Korean without any additional explanations.
"""

def generate_persona(personality):
    return {
        "pa_score": personality["paScore"] / 4.0,
        "bc_score": personality["bcScore"] / 4.0,
        "de_score": personality["deScore"] / 4.0,
        "fg_score": personality["fgScore"] / 4.0,
        "hi_score": personality["hiScore"] / 4.0,
        "jk_score": personality["jkScore"] / 4.0,
        "lm_score": personality["lmScore"] / 4.0,
        "no_score": personality["noScore"] / 4.0
    }

prompt = PromptTemplate(
    template=template,
    input_variables=[
        "question",
        "pa_score",
        "bc_score",
        "de_score",
        "fg_score",
        "hi_score",
        "jk_score",
        "lm_score",
        "no_score"
    ],
)

output_parser = StrOutputParser() # llm의 output 중에 llm의 대답만 string으로 출력해주는 parser

chain = prompt | llm | output_parser # llm에 prompt와 output_parser를 연결한다.

def get_answer(jsonData):
    data = json.loads(jsonData)
    transcript = data['transcript']
    persona = generate_persona(data['personality'])

    return chain.invoke({
        "question": transcript,
        "pa_score": persona["pa_score"],
        "bc_score": persona["bc_score"],
        "de_score": persona["de_score"],
        "fg_score": persona["fg_score"],
        "hi_score": persona["hi_score"],
        "jk_score": persona["jk_score"],
        "lm_score": persona["lm_score"],
        "no_score": persona["no_score"]
    })

if __name__ == "__main__":
    # For testing the module directly
    test_personality = {
        "paScore": 4,
        "bcScore": 2,
        "deScore": 1,
        "fgScore": 0,
        "hiScore": 0,
        "jkScore": 1,
        "lmScore": 2,
        "noScore": 3
    }

    test_json = json.dumps({
        "transcript": "당신의 업무 접근 방식은 어때요?",
        "personality": test_personality
    })

    answer = get_answer(test_json)
    print(answer)
