# Diagnostics/HtmlComparisonLogService

객체 키와 항목 목록 두 집합을 비교해서 HTML 로그 파일로 저장하는 범용 비교 모듈이다.

## 복사 대상

`src` 폴더 아래의 모든 파일을 복사한다.

## 의존성

- 추가 NuGet 패키지가 필요하지 않다.
- `System.IO`, `System.Net`, `System.Text` 같은 BCL만 사용한다.

## 구조

- `ComparisonReportBuilder`: 비교 규칙과 결과 모델 생성을 담당한다.
- `ComparisonHtmlRenderer`: HTML 문자열 렌더링을 담당한다.
- `ComparisonReportPathBuilder`: 출력 경로와 파일명을 결정한다.
- `ComparisonFileWriter`: 파일 저장을 담당한다.
- `ComparisonHtmlLogService`: 호출부에서 사용할 fail-safe 진입점이다.

## 지원 비교 방식

- `ComparisonMode.Equal`: 양쪽이 같은지 비교한다.
- `ComparisonMode.SourceInTarget`: source 항목이 target 안에 모두 있는지 비교한다.
- `ComparisonMode.TargetInSource`: target 항목이 source 안에 모두 있는지 비교한다.

## 입력 모델

- `ComparisonDataset`은 `Objects` 딕셔너리 하나로 구성된다.
- 키는 비교할 객체 식별자다.
- 값은 해당 객체의 문자열 항목 목록이다.
- `TableName`, `ObjectName` 같은 도메인 전용 필드는 포함하지 않는다.

## 동작 특징

- 비교 전에 키와 항목을 정규화한다.
- 항목 목록은 공백 제거, 중복 제거, 정렬 후 비교한다.
- HTML에는 요약 표, 비교 요약 표, 상세 펼침 영역이 포함된다.
- 상세의 `보기` 링크를 누르면 해당 `details` 영역이 자동으로 열린다.
- `ComparisonMode.Equal`의 상세 차이 항목은 방향을 알 수 있게 `<TargetLabel> 전용`, `<SourceLabel> 전용` 형식으로 표시한다.
- HTML 텍스트는 escape 처리한다.
- 파일명은 안전한 문자로 정리해서 저장한다.
- 파일은 UTF-8 BOM 없이 저장한다.
- `OutputDirectory`를 지정하지 않으면 `LocalApplicationData\HtmlComparisonLogs` 아래에 저장하고, 경로를 구할 수 없으면 임시 폴더를 사용한다.
- `ComparisonHtmlLogService`는 내부 예외를 잡고 `false`를 반환한다.

## 사용 예시

```csharp
using System.Collections.Generic;
using Peace.Codebank.Diagnostics.HtmlComparisonLogging;

var source = new ComparisonDataset();
source.Objects["Customer"] = new List<string> { "Id", "Name", "CreatedAt" };

var target = new ComparisonDataset();
target.Objects["Customer"] = new List<string> { "Id", "Name", "UpdatedAt" };

var options = new ComparisonHtmlLogOptions
{
    OutputDirectory = @"C:\Logs\Comparison",
    Title = "고객 스키마 비교",
    SourceLabel = "DB",
    TargetLabel = "Config",
    Mode = ComparisonMode.Equal,
};

var service = new ComparisonHtmlLogService();
var exported = service.TryWriteHtmlReport(source, target, options, out var reportPath);
```

## 검증

- `dotnet test .\Peace.Codebank.sln`를 실행하면 모듈 테스트가 함께 검증된다.
- `HtmlComparisonLogServiceTests`는 수동 확인용 HTML 예제를 `TestResults\HtmlComparisonLogService\<framework>` 아래에 남긴다.

## 프로젝트별 확장 지점

- 출력 경로 규칙을 바꾸고 싶으면 `IComparisonReportPathBuilder`를 구현한다.
- 저장 방식을 바꾸고 싶으면 `IComparisonFileWriter`를 구현한다.
- HTML 모양을 바꾸고 싶으면 `IComparisonHtmlRenderer`를 구현한다.
- 실패 시 콘솔 대신 다른 로깅으로 보내고 싶으면 `IComparisonFailureSink`를 구현한다.
- 비교 입력을 프로젝트 도메인에서 `ComparisonDataset`으로 바꾸는 어댑터는 각 프로젝트에서 별도로 둔다.

## 원본 프로젝트에서 제거한 의존성

- `SimulationSchema` 같은 도메인 모델 의존성을 제거했다.
- `SchemaValidationComparisonFactory` 같은 입력 조립 전용 코드는 포함하지 않았다.
- `GlobalDataService` 같은 서비스 호출부 연결은 포함하지 않았다.
- DB 이름, 테이블 이름, 특정 화면 용어에 묶인 제목 생성 규칙을 포함하지 않았다.
