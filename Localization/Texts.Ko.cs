using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> Ko = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "일반",
            ["Group.Keybindings"] = "키 설정",
            ["Option.SegmentLength"] = "세그먼트 길이",
            ["OptionDesc.SegmentLength"] = "도구로 생성하는 도로 세그먼트의 목표 길이(미터)입니다.",
            ["Option.SlopeInDegrees"] = "경사를 도 단위로 표시",
            ["OptionDesc.SlopeInDegrees"] = "경사를 퍼센트 대신 도 단위로 표시합니다.",
            ["Option.FollowTerrain"] = "지형 따르기",
            ["OptionDesc.FollowTerrain"] = "생성한 도로가 평평하게 유지되지 않고 지형 높이를 따르게 합니다.",
            ["Option.HideGamePreview"] = "게임 미리보기 숨기기",
            ["OptionDesc.HideGamePreview"] = "생성 모드에서 게임의 도로 미리보기를 숨기고 모드의 오버레이만 표시합니다. 비용과 검증은 그대로 적용됩니다.",
            ["Binding.Toggle"] = "도구 켜기/끄기",
            ["Binding.ModeAddNode"] = "모드: 노드 추가",
            ["Binding.ModeRemoveNode"] = "모드: 노드 제거",
            ["Binding.ModeSlope"] = "모드: 경사",
            ["Binding.ModeArrangeLine"] = "모드: 정렬",
            ["Binding.ModeLoop"] = "모드: 루프",
            ["Binding.ModeConnection"] = "모드: 연결",
            ["Binding.ModeCurve"] = "모드: 곡선",
            ["Binding.ModeParallel"] = "모드: 평행",
            ["Binding.Apply"] = "적용",
            ["Binding.RadiusUp"] = "반경 늘리기",
            ["Binding.RadiusDown"] = "반경 줄이기",
            ["Binding.OffsetUp"] = "오프셋 늘리기",
            ["Binding.OffsetDown"] = "오프셋 줄이기",
            ["Binding.HeightUp"] = "높이 올리기",
            ["Binding.HeightDown"] = "높이 내리기",
            ["Binding.Side"] = "직접/루프",
            ["Binding.Invert"] = "일방통행 방향 반전",
            ["Binding.SwitchOffset"] = "원 또는 측면 전환",
            ["Binding.ShiftLeft"] = "끝점 왼쪽으로 이동",
            ["Binding.ShiftRight"] = "끝점 오른쪽으로 이동",

            // UI panel and button
            ["UI.Tooltip"] = "도로와 철도 편집: 노드 추가/제거, 경사 설정, 노드 정렬, 곡선으로 세그먼트 연결.",
            ["UI.GroupEditNodes"] = "노드 편집",
            ["UI.GroupConnect"] = "세그먼트 연결",
            ["UI.ModeAddNode"] = "노드 추가",
            ["UI.ModeRemoveNode"] = "노드 제거",
            ["UI.ModeSlope"] = "경사",
            ["UI.ModeArrange"] = "정렬",
            ["UI.ModeConnection"] = "연결",
            ["UI.ModeLoop"] = "루프",
            ["UI.ModeCurve"] = "곡선",
            ["UI.ModeParallel"] = "평행",
            ["UI.FollowTerrain"] = "지형 따르기",
            ["UI.HidePreview"] = "게임 미리보기 숨기기",
            ["UI.Apply"] = "적용",

            // Shared mode texts
            ["SelectMode"] = "모드 선택",
            ["PressToCreate"] = "{0} 키를 눌러 생성",
            ["PressToApply"] = "{0} 키를 눌러 적용",
            ["Height"] = "높이",
            ["Offset"] = "오프셋",
            ["HintHeight"] = "높이",
            ["HintSnap"] = "1 m 단위로 반올림",
            ["HintOffset"] = "오프셋",
            ["HintShift"] = "활성 끝점을 옆으로 이동",
            ["HintShiftEnd"] = "이동할 끝점 전환",
            ["HintRadius"] = "반경",
            ["HintInvert"] = "방향 반전 (일방통행)",

            // Add node / remove node
            ["AddNodeSelect"] = "노드를 추가할 세그먼트 선택",
            ["AddNodeClick"] = "클릭하여 노드 추가",
            ["AddNodeTooClose"] = "기존 노드와 너무 가깝습니다",
            ["AddNodeOwned"] = "이 세그먼트는 분할할 수 없습니다",
            ["RemoveNodeSelect"] = "제거할 노드 선택",
            ["RemoveNodeClick"] = "클릭하여 이 노드 제거",
            ["RemoveNodeWrong"] = "이 노드는 제거할 수 없습니다",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "클릭하여 노드 추가",
            ["ChainUnselect"] = "클릭하여 선택 해제",
            ["ChainNotConnected"] = "연결되지 않음",
            ["ChainSelect"] = "노드를 순서대로 선택",
            ["SlopeSegment"] = "세그먼트 {0}",
            ["ArrangeGuide"] = "클릭하여 가이드로 선택",

            // Creation modes
            ["SelectFirstSegment"] = "첫 번째 세그먼트 선택",
            ["SelectSecondSegment"] = "두 번째 세그먼트 선택",
            ["ClickNodeDirection"] = "노드를 클릭하여 방향 변경",
            ["WrongShape"] = "불가능한 형태",
            ["NoIntersection"] = "교차점 없음",
            ["LoopHintDrag"] = "중심 또는 원의 가장자리 이동",
            ["LoopHintSide"] = "직접/루프",
            ["ConnectionHintDragEdge"] = "반경 (원의 가장자리)",
            ["ConnectionHintDragCenter"] = "이동/오프셋 (원의 중심)",
            ["ConnectionHintFlip"] = "방향 (원의 중심)",
            ["ConnectionHintAdd"] = "원 추가 (직선 위)",
            ["ConnectionHintRemove"] = "원 제거 (중심)",
            ["ConnectionHintRadius"] = "반경 (Shift: 전체)",
            ["ConnectionHintSwitch"] = "원 전환",

            // Parallel
            ["ParallelPickHover"] = "세그먼트 Ctrl+클릭: 해당 유형 선택",
            ["ParallelType"] = "유형",
            ["ParallelHintPick"] = "도로 유형 선택 (세그먼트 위)",
            ["ParallelHintDrag"] = "오프셋 및 측면 (미리보기 위)",
            ["ParallelHintSide"] = "측면",
        };
    }
}
