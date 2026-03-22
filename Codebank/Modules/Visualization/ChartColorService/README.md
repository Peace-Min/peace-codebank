# Visualization/ChartColorService

차트 항목에 서로 다른 색상을 순차적으로 할당하기 위한 모듈이다.

## 복사 대상

`src` 폴더 아래의 모든 파일을 복사한다.

## 의존성

- `System.Drawing.Color`를 사용한다.
- 저장소 기준으로는 추가 NuGet 패키지가 필요하지 않다.

## 참고 사항

- 첫 등록 색상은 `#FFF21818`이다.
- 이미 사용 중인 색상 목록을 `GenerateUniqueColor`에 전달한다.
- 같은 인스턴스를 계속 사용하면 색상 순서가 일정하게 이어진다.
- `IColoredItem`은 `Color` 읽기 속성만 필요하다.
