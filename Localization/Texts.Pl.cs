using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> Pl = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "Ogólne",
            ["Group.Keybindings"] = "Skróty klawiszowe",
            ["Option.SegmentLength"] = "Długość segmentu",
            ["OptionDesc.SegmentLength"] = "Docelowa długość (w metrach) segmentów drogi tworzonych przez narzędzie.",
            ["Option.SlopeInDegrees"] = "Nachylenie w stopniach",
            ["OptionDesc.SlopeInDegrees"] = "Wyświetlaj nachylenie w stopniach zamiast w procentach.",
            ["Option.FollowTerrain"] = "Podążaj za terenem",
            ["OptionDesc.FollowTerrain"] = "Tworzone drogi podążają za wysokością terenu zamiast pozostawać płaskie.",
            ["Option.HideGamePreview"] = "Ukryj podgląd gry",
            ["OptionDesc.HideGamePreview"] = "Ukrywa podgląd drogi z gry w trybach tworzenia i pozostawia tylko nakładkę moda. Koszt i weryfikacja nadal obowiązują.",
            ["Binding.Toggle"] = "Włącz/wyłącz narzędzie",
            ["Binding.ModeAddNode"] = "Tryb: dodaj węzeł",
            ["Binding.ModeRemoveNode"] = "Tryb: usuń węzeł",
            ["Binding.ModeSlope"] = "Tryb: nachylenie",
            ["Binding.ModeArrangeLine"] = "Tryb: wyrównaj",
            ["Binding.ModeLoop"] = "Tryb: pętla",
            ["Binding.ModeConnection"] = "Tryb: połączenie",
            ["Binding.ModeCurve"] = "Tryb: łuk",
            ["Binding.ModeParallel"] = "Tryb: równoległa",
            ["Binding.Apply"] = "Zastosuj",
            ["Binding.RadiusUp"] = "Zwiększ promień",
            ["Binding.RadiusDown"] = "Zmniejsz promień",
            ["Binding.OffsetUp"] = "Zwiększ przesunięcie",
            ["Binding.OffsetDown"] = "Zmniejsz przesunięcie",
            ["Binding.HeightUp"] = "Podnieś",
            ["Binding.HeightDown"] = "Obniż",
            ["Binding.Side"] = "Bezpośrednio/pętla",
            ["Binding.Invert"] = "Odwróć kierunek jednokierunkowej",
            ["Binding.SwitchOffset"] = "Zmień okrąg lub stronę",
            ["Binding.ShiftLeft"] = "Przesuń koniec w lewo",
            ["Binding.ShiftRight"] = "Przesuń koniec w prawo",

            // UI panel and button
            ["UI.Tooltip"] = "Edytuj drogi i tory: dodawaj lub usuwaj węzły, ustawiaj nachylenie, wyrównuj węzły i łącz segmenty łukami.",
            ["UI.GroupEditNodes"] = "Edycja węzłów",
            ["UI.GroupConnect"] = "Łączenie segmentów",
            ["UI.ModeAddNode"] = "Dodaj węzeł",
            ["UI.ModeRemoveNode"] = "Usuń węzeł",
            ["UI.ModeSlope"] = "Nachylenie",
            ["UI.ModeArrange"] = "Wyrównaj",
            ["UI.ModeConnection"] = "Połączenie",
            ["UI.ModeLoop"] = "Pętla",
            ["UI.ModeCurve"] = "Łuk",
            ["UI.ModeParallel"] = "Równoległa",
            ["UI.FollowTerrain"] = "Podążaj za terenem",
            ["UI.HidePreview"] = "Ukryj podgląd gry",
            ["UI.Apply"] = "Zastosuj",

            // Shared mode texts
            ["SelectMode"] = "Wybierz tryb",
            ["PressToCreate"] = "Naciśnij {0}, aby utworzyć",
            ["PressToApply"] = "Naciśnij {0}, aby zastosować",
            ["Height"] = "Wysokość",
            ["Offset"] = "Przesunięcie",
            ["HintHeight"] = "wysokość",
            ["HintSnap"] = "zaokrąglij do pełnych metrów",
            ["HintOffset"] = "przesunięcie",
            ["HintShift"] = "przesuń aktywny koniec w bok",
            ["HintShiftEnd"] = "zmień przesuwany koniec",
            ["HintRadius"] = "promień",
            ["HintInvert"] = "odwróć kierunek (jednokierunkowa)",

            // Add node / remove node
            ["AddNodeSelect"] = "Wybierz segment, aby dodać węzeł",
            ["AddNodeClick"] = "Kliknij, aby dodać węzeł",
            ["AddNodeTooClose"] = "Zbyt blisko istniejącego węzła",
            ["AddNodeOwned"] = "Tego segmentu nie można podzielić",
            ["RemoveNodeSelect"] = "Wybierz węzeł do usunięcia",
            ["RemoveNodeClick"] = "Kliknij, aby usunąć ten węzeł",
            ["RemoveNodeWrong"] = "Tego węzła nie można usunąć",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "Kliknij, aby dodać węzły",
            ["ChainUnselect"] = "Kliknij, aby odznaczyć",
            ["ChainNotConnected"] = "Niepołączone",
            ["ChainSelect"] = "Wybierz węzły po kolei",
            ["SlopeSegment"] = "Segment {0}",
            ["ArrangeGuide"] = "Kliknij, aby użyć jako prowadnicy",

            // Creation modes
            ["SelectFirstSegment"] = "Wybierz pierwszy segment",
            ["SelectSecondSegment"] = "Wybierz drugi segment",
            ["ClickNodeDirection"] = "Kliknij węzeł, aby zmienić kierunek",
            ["WrongShape"] = "Nieprawidłowy kształt",
            ["NoIntersection"] = "Brak przecięcia",
            ["LoopHintDrag"] = "przesuń środek lub krawędź okręgu",
            ["LoopHintSide"] = "bezpośrednio/pętla",
            ["ConnectionHintDragEdge"] = "promień (krawędź okręgu)",
            ["ConnectionHintDragCenter"] = "przesuń/odsuń (środek okręgu)",
            ["ConnectionHintFlip"] = "kierunek (środek okręgu)",
            ["ConnectionHintAdd"] = "dodaj okrąg (na prostej)",
            ["ConnectionHintRemove"] = "usuń okrąg (środek)",
            ["ConnectionHintRadius"] = "promień (Shift: wszystkie)",
            ["ConnectionHintSwitch"] = "zmień okrąg",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+klik na segmencie: pobierz jego typ",
            ["ParallelType"] = "Typ",
            ["ParallelHintPick"] = "wybierz typ drogi (na segmencie)",
            ["ParallelHintDrag"] = "przesunięcie i strona (na podglądzie)",
            ["ParallelHintSide"] = "strona",
        };
    }
}
