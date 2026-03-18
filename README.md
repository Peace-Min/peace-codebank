# Peace.Codebank

`Peace.Codebank`는 개인용 C# 재사용 코드 저장소다.

이 저장소의 목적은 공용 프레임워크나 DLL을 만드는 것이 아니다. 여러 프로젝트에서 다시 쓸 만한 로직을 모아 두고, `net472`와 `net8.0`에서 검증한 뒤 필요한 파일만 복사해서 사용하는 것을 목표로 한다.

## 구조

- `AGENTS.md`: 저장소 개요, 방향성, 글로벌 규칙 문서.
- `Codebank/Peace.Codebank.csproj`: 솔루션 탐색기에서 모듈을 바로 볼 수 있게 하는 프로젝트.
- `Codebank/Modules/<category>/<module-name>/src`: 실제로 복사해서 가져갈 소스 파일.
- `Codebank/Modules/<category>/<module-name>/tests`: 해당 모듈의 테스트 코드.
- `Codebank/Modules/<category>/<module-name>/README.md`: 모듈 설명, 의존성, 복사 기준.
- `TestApp/Peace.Codebank.Tests`: 전체 모듈을 검증하는 테스트 프로젝트.
- `docs/MODULE_GUIDE.md`: 모듈 추가와 관리 기준 문서.
- `docs/COMMIT_CONVENTION.md`: 커밋 메시지 규칙과 Git hook 기준 문서.

## 동작 방식

`Peace.Codebank` 프로젝트는 모듈 소스를 정리해서 보여 주는 역할을 한다.
`Peace.Codebank.Tests` 프로젝트는 모듈 테스트를 실행하는 검증용 프로젝트다.
실제 재사용은 각 모듈의 `src` 폴더에서 필요한 파일만 복사해서 가져가는 방식으로 진행한다.

## 현재 포함된 모듈

- `Visualization/ChartColorService`: 차트 항목에 서로 다른 색상을 할당하기 위한 로직.

## 일반 작업 순서

1. `Codebank/Modules` 아래에 새 모듈 폴더를 만든다.
2. 재사용할 소스를 `src`에 넣는다.
3. 검증 코드를 `tests`에 넣는다.
4. 사용법과 제약을 모듈 `README.md`에 정리한다.
5. `dotnet test .\Peace.Codebank.sln`로 검증한다.

## 명령어

```powershell
dotnet build .\Peace.Codebank.sln
dotnet test .\Peace.Codebank.sln
```
