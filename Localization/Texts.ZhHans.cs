using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> ZhHans = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "常规",
            ["Group.Keybindings"] = "按键绑定",
            ["Option.SegmentLength"] = "路段长度",
            ["OptionDesc.SegmentLength"] = "工具所创建道路路段的目标长度（米）。",
            ["Option.SlopeInDegrees"] = "以角度显示坡度",
            ["OptionDesc.SlopeInDegrees"] = "以角度而非百分比显示坡度。",
            ["Option.FollowTerrain"] = "贴合地形",
            ["OptionDesc.FollowTerrain"] = "让创建的道路贴合地形高度，而不是保持水平。",
            ["Option.HideGamePreview"] = "隐藏游戏预览",
            ["OptionDesc.HideGamePreview"] = "在创建模式中隐藏游戏自带的道路预览，只保留模组的叠加显示。费用和验证仍然有效。",
            ["Binding.Toggle"] = "开关工具",
            ["Binding.ModeAddNode"] = "模式: 添加节点",
            ["Binding.ModeRemoveNode"] = "模式: 删除节点",
            ["Binding.ModeSlope"] = "模式: 坡度",
            ["Binding.ModeArrangeLine"] = "模式: 排列",
            ["Binding.ModeLoop"] = "模式: 环形",
            ["Binding.ModeConnection"] = "模式: 连接",
            ["Binding.ModeCurve"] = "模式: 曲线",
            ["Binding.ModeParallel"] = "模式: 平行",
            ["Binding.Apply"] = "应用",
            ["Binding.RadiusUp"] = "增大半径",
            ["Binding.RadiusDown"] = "减小半径",
            ["Binding.OffsetUp"] = "增大偏移",
            ["Binding.OffsetDown"] = "减小偏移",
            ["Binding.HeightUp"] = "升高",
            ["Binding.HeightDown"] = "降低",
            ["Binding.Side"] = "直接/环形",
            ["Binding.Invert"] = "反转单行方向",
            ["Binding.SwitchOffset"] = "切换圆或侧边",
            ["Binding.ShiftLeft"] = "向左移动端点",
            ["Binding.ShiftRight"] = "向右移动端点",

            // UI panel and button
            ["UI.Tooltip"] = "编辑道路和铁路: 添加或删除节点、设置坡度、排列节点，并用曲线连接路段。",
            ["UI.GroupEditNodes"] = "编辑节点",
            ["UI.GroupConnect"] = "连接路段",
            ["UI.ModeAddNode"] = "添加节点",
            ["UI.ModeRemoveNode"] = "删除节点",
            ["UI.ModeSlope"] = "坡度",
            ["UI.ModeArrange"] = "排列",
            ["UI.ModeConnection"] = "连接",
            ["UI.ModeLoop"] = "环形",
            ["UI.ModeCurve"] = "曲线",
            ["UI.ModeParallel"] = "平行",
            ["UI.FollowTerrain"] = "贴合地形",
            ["UI.HidePreview"] = "隐藏游戏预览",
            ["UI.Apply"] = "应用",

            // Shared mode texts
            ["SelectMode"] = "选择一个模式",
            ["PressToCreate"] = "按 {0} 创建",
            ["PressToApply"] = "按 {0} 应用",
            ["Height"] = "高度",
            ["Offset"] = "偏移",
            ["HintHeight"] = "高度",
            ["HintSnap"] = "取整到米",
            ["HintOffset"] = "偏移",
            ["HintShift"] = "将当前端点向侧边移动",
            ["HintShiftEnd"] = "切换移动的端点",
            ["HintRadius"] = "半径",
            ["HintInvert"] = "反转方向（单行）",

            // Add node / remove node
            ["AddNodeSelect"] = "选择一个路段以添加节点",
            ["AddNodeClick"] = "点击添加节点",
            ["AddNodeTooClose"] = "离现有节点太近",
            ["AddNodeOwned"] = "此路段无法分割",
            ["RemoveNodeSelect"] = "选择要删除的节点",
            ["RemoveNodeClick"] = "点击删除此节点",
            ["RemoveNodeWrong"] = "此节点无法删除",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "点击添加节点",
            ["ChainUnselect"] = "点击取消选择",
            ["ChainNotConnected"] = "未连接",
            ["ChainSelect"] = "按顺序选择节点",
            ["SlopeSegment"] = "路段 {0}",
            ["ArrangeGuide"] = "点击设为参考",

            // Creation modes
            ["SelectFirstSegment"] = "选择第一个路段",
            ["SelectSecondSegment"] = "选择第二个路段",
            ["ClickNodeDirection"] = "点击节点以改变方向",
            ["WrongShape"] = "形状无效",
            ["NoIntersection"] = "无交点",
            ["LoopHintDrag"] = "移动圆心或圆边",
            ["LoopHintSide"] = "直接/环形",
            ["ConnectionHintDragEdge"] = "半径（圆边）",
            ["ConnectionHintDragCenter"] = "移动/偏移（圆心）",
            ["ConnectionHintFlip"] = "方向（圆心）",
            ["ConnectionHintAdd"] = "添加圆（在直线段上）",
            ["ConnectionHintRemove"] = "移除圆（圆心）",
            ["ConnectionHintRadius"] = "半径（Shift 全部）",
            ["ConnectionHintSwitch"] = "切换圆",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+点击路段: 选取其类型",
            ["ParallelType"] = "类型",
            ["ParallelHintPick"] = "选取道路类型（在路段上）",
            ["ParallelHintDrag"] = "偏移与侧（在预览上）",
            ["ParallelHintSide"] = "侧边",
        };
    }
}
