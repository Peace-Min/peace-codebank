# 커밋 컨벤션

## 목적

커밋 메시지를 일관되게 유지해서 변경 목적을 빠르게 파악할 수 있게 한다.

## 기본 형식

```text
<type>(<scope>): <subject>
```

scope가 필요 없으면 아래 형식도 허용한다.

```text
<type>: <subject>
```

## type 목록

- `feat`: 기능 추가.
- `fix`: 버그 수정.
- `refactor`: 동작 변경 없는 구조 개선.
- `docs`: 문서 변경.
- `test`: 테스트 추가 또는 수정.
- `build`: 빌드, 패키지, SDK, 의존성 설정 변경.
- `ci`: CI, 스크립트, 자동화 설정 변경.
- `style`: 동작에 영향 없는 코드 스타일 정리.
- `chore`: 기타 유지보수 작업.
- `perf`: 성능 개선.

## 작성 규칙

- 첫 줄은 `<type>(<scope>): <subject>` 형식을 따른다.
- `type`과 `scope`는 영문 소문자를 사용한다.
- `subject`는 한국어로 작성해도 된다.
- `subject` 끝에는 마침표를 붙이지 않는다.
- 첫 줄은 72자를 넘기지 않는다.
- scope는 `codebank`, `testapp`, `chart-color-service`, `docs`, `hooks`처럼 변경 범위를 짧게 표현한다.

## 예시

```text
feat(chart-color-service): 색상 생성 모듈 추가
docs(codebank): 저장소 규칙 문서 정리
build(testapp): net472 테스트 설정 보완
chore(hooks): 커밋 메시지 검증 hook 추가
```

## Git hook 적용 기준

- `.githooks/commit-msg`는 첫 줄이 위 형식을 따르는지 검사한다.
- `.githooks/pre-commit`는 `dotnet build`와 `dotnet test --no-build`를 실행해 커밋 전에 저장소 상태를 검증한다.
- 로컬 저장소에서는 `git config core.hooksPath .githooks`로 hook 경로를 고정한다.
