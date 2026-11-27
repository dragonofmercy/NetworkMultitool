using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> Ru = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "Общие",
            ["Group.Keybindings"] = "Горячие клавиши",
            ["Option.SegmentLength"] = "Длина сегмента",
            ["OptionDesc.SegmentLength"] = "Желаемая длина (в метрах) сегментов дорог, создаваемых инструментом.",
            ["Option.SlopeInDegrees"] = "Уклон в градусах",
            ["OptionDesc.SlopeInDegrees"] = "Показывать уклон в градусах, а не в процентах.",
            ["Option.FollowTerrain"] = "Следовать рельефу",
            ["OptionDesc.FollowTerrain"] = "Создаваемые дороги следуют высоте рельефа, а не остаются ровными.",
            ["Option.HideGamePreview"] = "Скрыть предпросмотр игры",
            ["OptionDesc.HideGamePreview"] = "Скрывает игровой предпросмотр дороги в режимах создания, оставляя только наложение мода. Стоимость и проверка по-прежнему действуют.",
            ["Binding.Toggle"] = "Вкл/выкл инструмент",
            ["Binding.ModeAddNode"] = "Режим: добавить узел",
            ["Binding.ModeRemoveNode"] = "Режим: удалить узел",
            ["Binding.ModeSlope"] = "Режим: уклон",
            ["Binding.ModeArrangeLine"] = "Режим: выравнивание",
            ["Binding.ModeLoop"] = "Режим: петля",
            ["Binding.ModeConnection"] = "Режим: соединение",
            ["Binding.ModeCurve"] = "Режим: кривая",
            ["Binding.ModeParallel"] = "Режим: параллель",
            ["Binding.Apply"] = "Применить",
            ["Binding.RadiusUp"] = "Увеличить радиус",
            ["Binding.RadiusDown"] = "Уменьшить радиус",
            ["Binding.OffsetUp"] = "Увеличить смещение",
            ["Binding.OffsetDown"] = "Уменьшить смещение",
            ["Binding.HeightUp"] = "Поднять",
            ["Binding.HeightDown"] = "Опустить",
            ["Binding.Side"] = "Прямо/петля",
            ["Binding.Invert"] = "Развернуть одностороннее движение",
            ["Binding.SwitchOffset"] = "Сменить круг или сторону",
            ["Binding.ShiftLeft"] = "Сдвинуть конец влево",
            ["Binding.ShiftRight"] = "Сдвинуть конец вправо",

            // UI panel and button
            ["UI.Tooltip"] = "Правка дорог и путей: добавление и удаление узлов, настройка уклона, выравнивание узлов и соединение сегментов кривыми.",
            ["UI.GroupEditNodes"] = "Правка узлов",
            ["UI.GroupConnect"] = "Соединение сегментов",
            ["UI.ModeAddNode"] = "Добавить узел",
            ["UI.ModeRemoveNode"] = "Удалить узел",
            ["UI.ModeSlope"] = "Уклон",
            ["UI.ModeArrange"] = "Выравнивание",
            ["UI.ModeConnection"] = "Соединение",
            ["UI.ModeLoop"] = "Петля",
            ["UI.ModeCurve"] = "Кривая",
            ["UI.ModeParallel"] = "Параллель",
            ["UI.FollowTerrain"] = "Следовать рельефу",
            ["UI.HidePreview"] = "Скрыть предпросмотр игры",
            ["UI.Apply"] = "Применить",

            // Shared mode texts
            ["SelectMode"] = "Выберите режим",
            ["PressToCreate"] = "Нажмите {0}, чтобы создать",
            ["PressToApply"] = "Нажмите {0}, чтобы применить",
            ["Height"] = "Высота",
            ["Offset"] = "Смещение",
            ["HintHeight"] = "{0}/{1}: высота",
            ["HintOffset"] = "{0}/{1}: смещение",
            ["HintShift"] = "{0}/{1}: сдвинуть активный конец вбок",
            ["HintShiftEnd"] = "{0}: сменить сдвигаемый конец",
            ["HintRadius"] = "{0}/{1}: радиус",
            ["HintInvert"] = "{0}: развернуть (одностороннее)",

            // Add node / remove node
            ["AddNodeSelect"] = "Выберите сегмент, чтобы добавить узел",
            ["AddNodeClick"] = "Щелкните, чтобы добавить узел",
            ["AddNodeTooClose"] = "Слишком близко к существующему узлу",
            ["AddNodeOwned"] = "Этот сегмент нельзя разделить",
            ["RemoveNodeSelect"] = "Выберите узел для удаления",
            ["RemoveNodeClick"] = "Щелкните, чтобы удалить узел",
            ["RemoveNodeWrong"] = "Этот узел нельзя удалить",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "Щелкните, чтобы добавить узлы",
            ["ChainUnselect"] = "Щелкните, чтобы снять выбор",
            ["ChainNotConnected"] = "Не соединены",
            ["ChainSelect"] = "Выберите узлы по порядку",
            ["SlopeSegment"] = "Сегмент {0}",
            ["ArrangeGuide"] = "Щелкните, чтобы выбрать направляющую",

            // Creation modes
            ["SelectFirstSegment"] = "Выберите первый сегмент",
            ["SelectSecondSegment"] = "Выберите второй сегмент",
            ["ClickNodeDirection"] = "Щелкните узел, чтобы сменить направление",
            ["WrongShape"] = "Недопустимая форма",
            ["NoIntersection"] = "Нет пересечения",
            ["LoopHintDrag"] = "Перетаскивание: центр или край круга",
            ["LoopHintSide"] = "{0}: прямо/петля",
            ["ConnectionHintDragEdge"] = "Тянуть край: радиус",
            ["ConnectionHintDragCenter"] = "Тянуть центр: сдвиг/смещение",
            ["ConnectionHintFlip"] = "Двойной щелчок по центру: направление",
            ["ConnectionHintAdd"] = "Двойной щелчок по прямой: добавить круг",
            ["ConnectionHintRemove"] = "Двойной правый щелчок по центру: удалить круг",
            ["ConnectionHintRadius"] = "{0}/{1}: радиус (Shift: все)",
            ["ConnectionHintSwitch"] = "{0}: сменить круг",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+щелчок по сегменту: взять его тип",
            ["ParallelType"] = "Тип",
            ["ParallelHintPick"] = "Ctrl+щелчок по сегменту: взять тип",
            ["ParallelHintDrag"] = "Перетащите предпросмотр: смещение и сторона",
            ["ParallelHintSide"] = "{0}: сторона",
        };
    }
}
