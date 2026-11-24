using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> ZhHant = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "一般",
            ["Group.Keybindings"] = "按鍵設定",
            ["Option.SegmentLength"] = "路段長度",
            ["OptionDesc.SegmentLength"] = "工具所建立道路路段的目標長度（公尺）。",
            ["Option.SlopeInDegrees"] = "以角度顯示坡度",
            ["OptionDesc.SlopeInDegrees"] = "以角度而非百分比顯示坡度。",
            ["Option.FollowTerrain"] = "貼合地形",
            ["OptionDesc.FollowTerrain"] = "讓建立的道路貼合地形高度，而不是保持水平。",
            ["Option.HideGamePreview"] = "隱藏遊戲預覽",
            ["OptionDesc.HideGamePreview"] = "在建立模式中隱藏遊戲內建的道路預覽，只保留模組的疊加顯示。費用與驗證仍然有效。",
            ["Binding.Toggle"] = "開關工具",
            ["Binding.ModeAddNode"] = "模式: 新增節點",
            ["Binding.ModeRemoveNode"] = "模式: 刪除節點",
            ["Binding.ModeSlope"] = "模式: 坡度",
            ["Binding.ModeArrangeLine"] = "模式: 排列",
            ["Binding.ModeLoop"] = "模式: 環形",
            ["Binding.ModeConnection"] = "模式: 連接",
            ["Binding.ModeCurve"] = "模式: 曲線",
            ["Binding.ModeParallel"] = "模式: 平行",
            ["Binding.Apply"] = "套用",
            ["Binding.RadiusUp"] = "增加半徑",
            ["Binding.RadiusDown"] = "減少半徑",
            ["Binding.OffsetUp"] = "增加偏移",
            ["Binding.OffsetDown"] = "減少偏移",
            ["Binding.HeightUp"] = "升高",
            ["Binding.HeightDown"] = "降低",
            ["Binding.Side"] = "直接/環形",
            ["Binding.Invert"] = "反轉單行方向",
            ["Binding.SwitchOffset"] = "切換圓或側邊",

            // UI panel and button
            ["UI.Tooltip"] = "編輯道路與鐵路: 新增或刪除節點、設定坡度、排列節點，並以曲線連接路段。",
            ["UI.GroupEditNodes"] = "編輯節點",
            ["UI.GroupConnect"] = "連接路段",
            ["UI.ModeAddNode"] = "新增節點",
            ["UI.ModeRemoveNode"] = "刪除節點",
            ["UI.ModeSlope"] = "坡度",
            ["UI.ModeArrange"] = "排列",
            ["UI.ModeConnection"] = "連接",
            ["UI.ModeLoop"] = "環形",
            ["UI.ModeCurve"] = "曲線",
            ["UI.ModeParallel"] = "平行",
            ["UI.FollowTerrain"] = "貼合地形",
            ["UI.HidePreview"] = "隱藏遊戲預覽",
            ["UI.Apply"] = "套用",

            // Shared mode texts
            ["SelectMode"] = "選擇一個模式",
            ["PressToCreate"] = "按 {0} 建立",
            ["PressToApply"] = "按 {0} 套用",
            ["Height"] = "高度",
            ["Radius"] = "半徑",
            ["Offset"] = "偏移",
            ["HintHeight"] = "{0}/{1}: 高度",
            ["HintOffset"] = "{0}/{1}: 偏移",
            ["HintRadius"] = "{0}/{1}: 半徑",
            ["HintInvert"] = "{0}: 反轉方向（單行）",

            // Add node / remove node
            ["AddNodeSelect"] = "選擇一個路段以新增節點",
            ["AddNodeClick"] = "點擊以新增節點",
            ["AddNodeTooClose"] = "距離現有節點太近",
            ["AddNodeOwned"] = "此路段無法分割",
            ["RemoveNodeSelect"] = "選擇要刪除的節點",
            ["RemoveNodeClick"] = "點擊以刪除此節點",
            ["RemoveNodeWrong"] = "此節點無法刪除",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "點擊以新增節點",
            ["ChainUnselect"] = "點擊以取消選取",
            ["ChainNotConnected"] = "未連接",
            ["ChainSelect"] = "依序選擇節點",
            ["SlopeSegment"] = "路段 {0}",
            ["ArrangeGuide"] = "點擊設為參考",

            // Creation modes
            ["SelectFirstSegment"] = "選擇第一個路段",
            ["SelectSecondSegment"] = "選擇第二個路段",
            ["ClickNodeDirection"] = "點擊節點以變更方向",
            ["WrongShape"] = "形狀無效",
            ["NoIntersection"] = "無交點",
            ["Angle"] = "角度",
            ["LoopHintDrag"] = "拖曳: 圓心或圓邊",
            ["LoopHintSide"] = "{0}: 直接/環形",
            ["ConnectionRadius"] = "半徑 {0}",
            ["ConnectionFirstOffset"] = "起點偏移",
            ["ConnectionLastOffset"] = "終點偏移",
            ["ConnectionHintDragEdge"] = "拖曳圓邊: 半徑",
            ["ConnectionHintDragCenter"] = "拖曳圓心: 移動/偏移",
            ["ConnectionHintFlip"] = "雙擊圓心: 方向",
            ["ConnectionHintAdd"] = "雙擊直線段: 新增圓",
            ["ConnectionHintRemove"] = "右鍵雙擊圓心: 移除圓",
            ["ConnectionHintRadius"] = "{0}/{1}: 半徑（Shift 全部）",
            ["ConnectionHintSwitch"] = "{0}: 切換圓",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+點擊路段: 選取其類型",
            ["ParallelType"] = "類型",
            ["ParallelHintPick"] = "Ctrl+點擊路段: 選取類型",
            ["ParallelHintSide"] = "{0}: 側邊",
        };
    }
}
