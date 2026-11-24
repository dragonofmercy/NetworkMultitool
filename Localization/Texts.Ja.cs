using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> Ja = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "一般",
            ["Group.Keybindings"] = "キー割り当て",
            ["Option.SegmentLength"] = "セグメントの長さ",
            ["OptionDesc.SegmentLength"] = "ツールで作成する道路セグメントの目標の長さ(メートル)。",
            ["Option.SlopeInDegrees"] = "勾配を度で表示",
            ["OptionDesc.SlopeInDegrees"] = "勾配をパーセントではなく度で表示します。",
            ["Option.FollowTerrain"] = "地形に沿う",
            ["OptionDesc.FollowTerrain"] = "作成した道路を平らに保たず、地形の高さに沿わせます。",
            ["Option.HideGamePreview"] = "ゲームのプレビューを隠す",
            ["OptionDesc.HideGamePreview"] = "作成モードでゲームの道路プレビューを隠し、MODのオーバーレイのみを表示します。費用と検証は引き続き適用されます。",
            ["Binding.Toggle"] = "ツールの切り替え",
            ["Binding.ModeAddNode"] = "モード: ノード追加",
            ["Binding.ModeRemoveNode"] = "モード: ノード削除",
            ["Binding.ModeSlope"] = "モード: 勾配",
            ["Binding.ModeArrangeLine"] = "モード: 整列",
            ["Binding.ModeLoop"] = "モード: ループ",
            ["Binding.ModeConnection"] = "モード: 接続",
            ["Binding.ModeCurve"] = "モード: カーブ",
            ["Binding.ModeParallel"] = "モード: 平行",
            ["Binding.Apply"] = "適用",
            ["Binding.RadiusUp"] = "半径を大きく",
            ["Binding.RadiusDown"] = "半径を小さく",
            ["Binding.OffsetUp"] = "オフセットを増やす",
            ["Binding.OffsetDown"] = "オフセットを減らす",
            ["Binding.HeightUp"] = "高さを上げる",
            ["Binding.HeightDown"] = "高さを下げる",
            ["Binding.Side"] = "直接/ループ",
            ["Binding.Invert"] = "一方通行の向きを反転",
            ["Binding.SwitchOffset"] = "円または側を切り替え",

            // UI panel and button
            ["UI.Tooltip"] = "道路と線路を編集: ノードの追加・削除、勾配の設定、ノードの整列、カーブによるセグメントの接続。",
            ["UI.GroupEditNodes"] = "ノード編集",
            ["UI.GroupConnect"] = "セグメント接続",
            ["UI.ModeAddNode"] = "ノード追加",
            ["UI.ModeRemoveNode"] = "ノード削除",
            ["UI.ModeSlope"] = "勾配",
            ["UI.ModeArrange"] = "整列",
            ["UI.ModeConnection"] = "接続",
            ["UI.ModeLoop"] = "ループ",
            ["UI.ModeCurve"] = "カーブ",
            ["UI.ModeParallel"] = "平行",
            ["UI.FollowTerrain"] = "地形に沿う",
            ["UI.HidePreview"] = "ゲームのプレビューを隠す",
            ["UI.Apply"] = "適用",

            // Shared mode texts
            ["SelectMode"] = "モードを選択",
            ["PressToCreate"] = "{0}で作成",
            ["PressToApply"] = "{0}で適用",
            ["Height"] = "高さ",
            ["Radius"] = "半径",
            ["Offset"] = "オフセット",
            ["HintHeight"] = "{0}/{1}: 高さ",
            ["HintOffset"] = "{0}/{1}: オフセット",
            ["HintRadius"] = "{0}/{1}: 半径",
            ["HintInvert"] = "{0}: 向きを反転 (一方通行)",

            // Add node / remove node
            ["AddNodeSelect"] = "ノードを追加するセグメントを選択",
            ["AddNodeClick"] = "クリックでノードを追加",
            ["AddNodeTooClose"] = "既存のノードに近すぎます",
            ["AddNodeOwned"] = "このセグメントは分割できません",
            ["RemoveNodeSelect"] = "削除するノードを選択",
            ["RemoveNodeClick"] = "クリックでこのノードを削除",
            ["RemoveNodeWrong"] = "このノードは削除できません",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "クリックでノードを追加",
            ["ChainUnselect"] = "クリックで選択解除",
            ["ChainNotConnected"] = "接続されていません",
            ["ChainSelect"] = "ノードを順番に選択",
            ["SlopeSegment"] = "セグメント {0}",
            ["ArrangeGuide"] = "クリックでガイドとして選択",

            // Creation modes
            ["SelectFirstSegment"] = "1つ目のセグメントを選択",
            ["SelectSecondSegment"] = "2つ目のセグメントを選択",
            ["ClickNodeDirection"] = "ノードをクリックで向きを変更",
            ["WrongShape"] = "作成できない形状",
            ["NoIntersection"] = "交点がありません",
            ["Angle"] = "角度",
            ["LoopHintDrag"] = "ドラッグ: 中心または円の縁",
            ["LoopHintSide"] = "{0}: 直接/ループ",
            ["ConnectionRadius"] = "半径 {0}",
            ["ConnectionFirstOffset"] = "始点オフセット",
            ["ConnectionLastOffset"] = "終点オフセット",
            ["ConnectionHintDragEdge"] = "縁をドラッグ: 半径",
            ["ConnectionHintDragCenter"] = "中心をドラッグ: 移動/オフセット",
            ["ConnectionHintFlip"] = "中心をダブルクリック: 向き",
            ["ConnectionHintAdd"] = "直線をダブルクリック: 円を追加",
            ["ConnectionHintRemove"] = "中心を右ダブルクリック: 円を削除",
            ["ConnectionHintRadius"] = "{0}/{1}: 半径 (Shift: すべて)",
            ["ConnectionHintSwitch"] = "{0}: 円を切り替え",

            // Parallel
            ["ParallelPickHover"] = "セグメントをCtrl+クリック: その種類を取得",
            ["ParallelType"] = "種類",
            ["ParallelHintPick"] = "セグメントをCtrl+クリック: 種類を取得",
            ["ParallelHintSide"] = "{0}: 側",
        };
    }
}
