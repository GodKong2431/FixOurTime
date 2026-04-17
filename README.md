# FixOurTime

FixOurTime은 무너진 시간을 되돌리기 위해 시계 바늘을 되찾아 나가는 2D 액션 플랫폼 프로젝트입니다. 현재 프로젝트는 2D 플랫폼 액션, 스테이지별 보스전과 함정 기믹, CSV 기반 데이터 파이프라인, JSON 기반 체크포인트 저장, Cinemachine 기반 카메라 연출, VideoPlayer 기반 컷신 흐름을 중심으로 구성되어 있습니다.

이 문서는 프로젝트에서 실제로 사용 중인 의미 있는 기술과 구조를 가능한 한 빠짐없이 정리한 README입니다. Unity 기본 모듈 전체를 기계적으로 나열하기보다, 코드베이스 안에서 역할이 드러나는 기술과 패턴 위주로 정리했습니다.

## 기술 스택

- Unity 6000.2.10f1
- URP 17.2.0
- Input System 1.14.2
- Cinemachine 3.1.5
- 2D Tilemap / 2D Tilemap Extras 5.0.1
- VideoPlayer / JsonUtility / ScriptableObject / Resources
- UnityEvent / Coroutine / Async Scene Loading
- UnityEngine.Pool / EditorWindow / AssetDatabase / Reflection

## 사용 기술 전체 정리

### 1. Unity 2D 액션 플랫폼 구조

- `Player.cs`를 중심으로 이동, 점프, 차지 점프, 공중 점프, 대시 공격, 피격, 사망, 부활을 구성한 2D 액션 플랫폼 구조입니다.
- `Rigidbody2D`, `Collider2D`, `PhysicsMaterial2D`, `LayerMask`, `Physics2D.Raycast`, `Physics2D.CircleCast` 등을 활용해 지형 판정과 충돌, 이동 물리를 처리합니다.
- 플레이어, 트랩, 보스, 투사체 모두 2D 물리 환경을 기준으로 구현되어 있습니다.

### 2. Input System 기반 입력 처리

- `Player.cs`의 `OnMove`, `OnJump`, `OnAttack`, `OnPause`, `OnSpeedBoost`가 `InputAction.CallbackContext`를 통해 입력을 처리합니다.
- 입력 이벤트를 상태 로직과 직접 연결해, 현재 플레이어 상태에 따라 허용되는 행동과 막아야 하는 행동을 분기합니다.
- 게임패드나 키보드 입력을 동일한 액션 경로로 처리할 수 있는 구조입니다.

### 3. Singleton 기반 전역 매니저 구조

- `SingleTon<T>`를 공통 베이스로 사용해 `GameManager`, `SceneChanger`, `SoundManager`, `CSVDataManager`, `CinemachinCamManager`를 전역 접근 가능한 구조로 운영합니다.
- 각 매니저는 `DontDestroyOnLoad`를 통해 씬 전환 후에도 유지되며, 시스템 간 공통 진입점 역할을 맡습니다.
- 사운드, 데이터, 카메라, 씬 전환처럼 프로젝트 전반에서 반복 접근이 필요한 기능을 매니저 단위로 분리했습니다.

### 4. CSV -> C# 클래스 -> ScriptableObject 자동 생성 파이프라인

- `CSVTableAutoBuilderWindow.cs`가 에디터 창에서 CSV와 출력 폴더를 받아 자동 빌드를 시작합니다.
- `CSVRowCodeGenerator.cs`가 CSV 헤더와 타입 정보를 읽어 데이터 클래스를 생성합니다.
- `CSVTableSOCodeGenerator.cs`가 해당 데이터 클래스를 담는 테이블 ScriptableObject 클래스를 생성합니다.
- `CSVAutoBuildPipeline.cs`는 스크립트 리로드 직후 자동으로 SO 에셋을 만들고, `CSVGenericTableEditor.cs`를 통해 CSV 내용을 실제 SO 데이터로 채워 넣습니다.
- 이 과정에는 `EditorWindow`, `AssetDatabase`, `EditorPrefs`, `DidReloadScripts`, `Reflection`이 활용됩니다.

### 5. Resources + ScriptableObject 기반 런타임 데이터 로딩

- `CSVDataManager.cs`가 `Resources/CSV/CSVSO` 경로의 모든 `SOBase` 테이블을 로드해 런타임 딕셔너리로 관리합니다.
- 필요한 시스템은 `Get<T>()` 형태로 테이블을 가져오며, 인덱스가 없으면 `BuildIndex()`를 통해 접근용 캐시를 생성합니다.
- `SoundManager.cs`는 `SoundTable` 데이터를 읽고 `Resources.Load<AudioClip>()`으로 실제 오디오 리소스를 불러옵니다.

### 6. JSON 기반 로컬 저장과 체크포인트 시스템

- `GameData.cs`는 현재 체력, 최대 체력, 플레이어 위치, 현재 씬, 카메라 위치, 수집 아이템 상태, 활성화된 세이브 포인트 목록을 저장합니다.
- `GameDataManager.cs`는 `Application.persistentDataPath`에 JSON 파일을 생성하고 `JsonUtility`로 저장과 로드를 처리합니다.
- `SavePoint.cs`는 고유 ID를 가진 체크포인트를 관리하며, 한 번 저장된 포인트는 재접속 시 다시 비활성화됩니다.
- `Player.cs`는 세이브 데이터 작성, 로드, 체크포인트 갱신, 부활 시 복구를 직접 담당합니다.

### 7. FSM(State Pattern) 기반 플레이어와 기믹 제어

- `IState<T>` 인터페이스를 기반으로 플레이어 상태를 `Idle`, `Move`, `Charge`, `Jump`, `Fall`, `Attack`, `DashAttack`, `Hit`, `Stun`, `Dead`로 분리했습니다.
- `BookCase` 기믹도 동일한 `IState<T>` 패턴을 사용해 `Idle`, `Push`, `Stay`, `Return`, `CoolDown` 흐름을 나눠 구현합니다.
- 플레이어 본체에 모든 행동을 몰아넣지 않고 상태별 로직을 나누는 방식으로, 입력 충돌과 행동 전환 복잡도를 줄였습니다.

### 8. Coroutine 기반 전투와 연출 흐름 제어

- `SceneChanger.cs`는 영상 준비, 영상 재생, 스킵 감지, 비동기 씬 로드, 로드 완료 후 복구, 페이드 인까지를 코루틴으로 연결합니다.
- `Stage1Boss.cs` 같은 보스 스크립트는 페이즈 전환, 패턴 사이 대기, 등장 연출, 약점 노출 흐름을 코루틴으로 구성합니다.
- `PaperCraneSpawner.cs`, `CinemachinCamManager.cs`, `SoundManager.cs` 등도 시간 기반 반복 동작을 코루틴으로 처리합니다.

### 9. 보스 프레임워크와 페이즈/패턴 분리

- `BossBase.cs`는 공통 체력, 피격, 사망, 상태 전환, 리셋 기능을 제공하는 보스 베이스 클래스입니다.
- `BossState.cs`는 보스 패턴을 코루틴 기반 상태 객체로 분리하기 위한 공통 인터페이스 역할을 합니다.
- `BossZone.cs`는 플레이어가 일정 시간 보스 존에 머무를 때만 보스를 활성화하는 트리거 역할을 합니다.
- `Stage1Boss.cs`, `Stage2Boss.cs`, `Stage3AngelBoss.cs`, `Stage3DevilBoss.cs` 등은 스테이지별 보스 패턴을 독립적으로 구현합니다.
- `BossUIManager.cs`는 보스전 목표 UI를 담당하며, 스테이지 콘텐츠와 UI를 느슨하게 연결합니다.

### 10. 버프/디버프와 능력 해금 시스템

- `IStatusEffect<T>` 인터페이스를 기준으로 버프와 디버프를 동일한 방식으로 다룰 수 있게 설계했습니다.
- `SecondHandBuff`, `MinuteHandBuff`, `HourHandBuff`는 이동 속도, 차지 속도, 무한 점프 같은 능력 변화를 부여합니다.
- `ColdDebuff`, `HotDebuff`, `ExhaustDebuff`, `NightZone`, `DayZone`, `PositionSwap` 등은 환경 디버프나 상태 이상 효과를 구현합니다.
- `ItemObject.cs`가 수집 아이템과 상태 효과 적용, 영구 수집 플래그 갱신을 담당합니다.

### 11. Cinemachine 기반 카메라 제어

- `CinemachinCamManager.cs`는 플레이어를 추적하고, 화면 밖으로 나가면 화면 단위로 수직 이동하는 카메라 흐름을 처리합니다.
- 같은 매니저에서 카메라 흔들림, 특정 카메라 흔들림, 줌 인/아웃, 씬 재진입 후 추적 대상 재연결을 담당합니다.
- `ChnageBossCamera.cs`는 일반 진행 카메라와 보스 카메라의 우선순위를 바꿔 전투 연출을 전환합니다.
- 스테이지 진행과 보스전의 화면 문법을 분리하는 데 Cinemachine을 적극 사용하고 있습니다.

### 12. 비동기 씬 전환과 VideoPlayer 기반 컷신

- `SceneChanger.cs`는 `SceneManager.LoadSceneAsync()`를 사용한 비동기 씬 로드와 `CanvasGroup` 기반 페이드를 함께 관리합니다.
- 새 게임 시작 시 영상 재생, 스킵, 로드, 카메라 재연결, 세이브 위치 복구가 하나의 흐름으로 묶여 있습니다.
- `SceneInitializer.cs`, `IntroSceneManager.cs`, `StartSceneManager.cs`, `EndingSceneManager.cs`는 인트로 영상, 타이틀 진입, 새 게임 연출, 엔딩 크레딧을 담당합니다.
- `VideoPlayer`를 사용해 단순 씬 전환이 아니라 컷신 포함 진행 흐름을 구성했습니다.

### 13. 데이터 기반 오디오 시스템

- `SoundManager.cs`는 `SoundTable` CSV 데이터를 기준으로 BGM/SFX를 런타임에 로드합니다.
- BGM 페이드 인/아웃, SFX 중복 재생 방지, 화면 안에서만 재생되는 위치 기반 SFX 판단을 한곳에서 처리합니다.
- `Stage1SceneController.cs`부터 `Stage4SceneController.cs`까지 각 씬 컨트롤러가 스테이지별 BGM 시작과 보스전 전환을 담당합니다.
- `VolumeSlider.cs`, `ButtonHoverSound.cs`는 UI와 사운드 시스템을 직접 연결합니다.

### 14. 이벤트 기반 UI 갱신과 Presenter 패턴

- `Player.cs`는 체력 변경 시 `OnHpChanged` 이벤트를 발행합니다.
- `PlayerHpPresenter.cs`는 플레이어 이벤트를 받아 `PlayerHpUI.cs`를 갱신하는 중간 계층 역할을 합니다.
- `PlayerUiConnector.cs`는 런타임 연결과 해제를 담당해 프레젠터와 뷰를 묶어 줍니다.
- `UnityEvent`는 플레이어 사망/부활, 보스 사망, 보스 존 활성화 같은 이벤트 연결에도 사용됩니다.

### 15. UnityEngine.Pool 기반 오브젝트 풀링

- `PaperCraneSpawner.cs`는 종이학 적 오브젝트를 `ObjectPool<GameObject>`로 재사용합니다.
- `DestroyArea.cs`는 타일 파괴 후 생성되는 조각 오브젝트를 풀링해 반복 생성/파괴 비용을 줄입니다.
- 전투 중 자주 생성되는 오브젝트를 풀 단위로 돌려 쓰는 방식이 실제 코드에 반영되어 있습니다.

### 16. Tilemap 기반 환경 파괴와 모듈형 함정 시스템

- `DestroyArea.cs`는 `Tilemap`에서 충돌 범위 안의 셀을 제거하고, 평균 색을 따서 파편 조각을 생성하는 방식으로 환경 붕괴를 구현합니다.
- `DamageableTrapBase.cs`는 여러 함정이 공통으로 사용하는 데미지/넉백 베이스를 제공합니다.
- `Laser`, `Steam`, `Ghost`, `WindZone`, `Gear`, `BookCase`, `Paper Crane`, `DestroyTower`, `MagicBook`, `Fountain` 계열 스크립트가 개별 기믹을 모듈처럼 분리해 구현합니다.
- 각 스테이지가 서로 다른 함정 세트를 조합해 쓰는 구조라, 콘텐츠 확장성이 높습니다.

### 17. 스테이지별 씬 컨트롤러와 연출 분리

- `Stage1SceneController.cs`, `Stage2SceneController.cs`, `Stage3SceneController.cs`, `Stage4SceneController.cs`가 스테이지별 시작 연출과 BGM을 분리해서 관리합니다.
- 각 씬이 공통 매니저를 재사용하면서도, 씬 단위로 필요한 연출만 별도 제어할 수 있도록 구성되어 있습니다.
- 타이틀, 인트로, 엔딩도 전용 매니저를 사용해 흐름을 분리합니다.

## 기술 결정 기록

아래 항목은 위 기술들 중 특히 프로젝트 구조를 결정한 핵심 선택을 정리한 기록입니다.

### ADR-001. CSV 자동 생성 + ScriptableObject 런타임 로딩 구조

- 배경 : 테이블 데이터를 사람이 편하게 수정하면서도, 런타임에서는 타입 안전하게 참조할 필요가 있었습니다.
- 결정 : CSV를 입력으로 받아 데이터 클래스와 테이블 SO를 생성하고, 런타임에서는 `CSVDataManager`가 이를 로드하는 구조를 채택했습니다.
- 채택 이유 : 기획 데이터 수정과 런타임 사용 방식을 분리할 수 있고, 반복적인 테이블 정의 작업을 줄일 수 있습니다.

### ADR-002. 로컬 JSON 기반 체크포인트 저장 구조

- 배경 : 서버 저장 없이도 이어하기, 체크포인트 복귀, 아이템 수집 상태 유지가 필요했습니다.
- 결정 : `GameData`와 `GameDataManager`를 기반으로 한 로컬 JSON 저장 구조를 채택했습니다.
- 채택 이유 : 구현 복잡도가 낮고 디버깅이 쉬우며, 현재 프로젝트 규모에서 필요한 저장 요구사항을 충분히 만족합니다.

### ADR-003. 상태 패턴 기반 액션 제어

- 배경 : 이동, 점프, 공격, 피격, 사망이 한 클래스 안에 뒤섞이면 입력 충돌과 상태 분기 관리가 빠르게 복잡해질 수 있었습니다.
- 결정 : `IState<T>` 기반 FSM 구조를 플레이어와 일부 기믹에 도입했습니다.
- 채택 이유 : 행동 단위 책임 분리가 가능하고, 새로운 액션을 추가하거나 기존 액션을 수정할 때 영향 범위를 줄일 수 있습니다.

### ADR-004. 씬 단위 스테이지 분리와 보스 흐름 독립화

- 배경 : 각 스테이지는 서로 다른 보스, 함정, 배경음악, 연출 흐름을 가지고 있어 하나의 단일 구조로 묶기 어려웠습니다.
- 결정 : `stage1`부터 `stage4`까지 씬 단위로 분리하고, 스테이지 컨트롤러와 보스 스크립트가 각 흐름을 맡는 구조를 채택했습니다.
- 채택 이유 : 스테이지별 디버깅과 반복 개선이 쉬워지고, 콘텐츠 확장이 독립적으로 가능합니다.

### ADR-005. Cinemachine + 비동기 씬 전환 + VideoPlayer 연출 조합

- 배경 : 단순 씬 이동만으로는 인트로, 새 게임 시작, 보스전, 엔딩의 연출 밀도를 만들기 어려웠습니다.
- 결정 : Cinemachine 카메라 제어, 비동기 씬 로드, 페이드, 비디오 재생을 하나의 전환 프레임워크로 묶었습니다.
- 채택 이유 : 플레이 흐름과 연출 흐름을 끊지 않으면서도, 씬 전환의 안정성과 시각적 완성도를 함께 확보할 수 있습니다.

### ADR-006. 공통 베이스 + 모듈형 콘텐츠 확장 구조

- 배경 : 보스와 함정 종류가 늘어날수록 공통 로직과 개별 패턴 로직을 분리하지 않으면 중복이 빠르게 증가할 수 있었습니다.
- 결정 : `BossBase`, `BossState`, `DamageableTrapBase`, `IStatusEffect<T>` 같은 공통 베이스를 중심으로 확장하는 구조를 채택했습니다.
- 채택 이유 : 공통 규칙을 재사용하면서도, 스테이지별 콘텐츠는 독립적으로 구현할 수 있어 유지보수성과 확장성이 좋아집니다.

### ADR-007. 반복 생성 오브젝트에 대한 풀링 적용

- 배경 : 파편, 종이학 같은 반복 생성 오브젝트는 전투와 연출 중 생성 빈도가 높아질 수 있었습니다.
- 결정 : `UnityEngine.Pool` 기반 오브젝트 풀링을 선택했습니다.
- 채택 이유 : `Instantiate`와 `Destroy` 호출을 줄여 런타임 부하를 완화하고, 효과성 오브젝트의 재사용 경로를 명확히 만들 수 있습니다.

## 핵심 흐름

- 데이터 제작 : CSV 파일을 기반으로 에디터 도구가 데이터 클래스와 테이블 SO를 생성하고, CSV 내용을 SO에 채워 넣습니다.
- 데이터 로드 : `CSVDataManager.cs`가 런타임에 SO 테이블을 로드하고, `SoundManager.cs` 같은 시스템이 필요한 데이터를 참조합니다.
- 게임 시작 : `SceneInitializer.cs`와 `IntroSceneManager.cs`가 인트로 영상을 처리하고, `SceneChanger.cs`가 새 게임 또는 이어하기 흐름을 이어받습니다.
- 플레이 진행 : `Player.cs`의 입력 처리와 상태 머신이 액션을 제어하고, 트랩과 보스, 카메라, 사운드, UI가 그 흐름을 따라 반응합니다.
- 보스전 전개 : `BossZone.cs`가 진입을 감지하면 해당 보스를 활성화하고, 보스는 `BossBase`와 `BossState` 구조 위에서 패턴을 실행합니다.
- 수집과 강화 : `ItemObject.cs`가 시계 바늘 수집을 처리하고, 버프 시스템이 이동 속도 증가, 차지 속도 향상, 무한 점프 같은 변화를 적용합니다.
- 저장과 복원 : `SavePoint.cs`와 `Player.CheckPoint()`가 진행 상태를 `GameData`에 기록하고, `GameDataManager.cs`가 이를 JSON으로 저장합니다.
- 장면 연출 : `CinemachinCamManager.cs`, `ChnageBossCamera.cs`, `SoundManager.cs`, UI 스크립트가 플레이 상황에 맞는 카메라, 사운드, 화면 반응을 연결합니다.
