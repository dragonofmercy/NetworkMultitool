using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> De = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "Allgemein",
            ["Group.Keybindings"] = "Tastenbelegung",
            ["Option.SegmentLength"] = "Segmentlänge",
            ["OptionDesc.SegmentLength"] = "Ziellänge (in Metern) der vom Werkzeug erstellten Straßensegmente.",
            ["Option.SlopeInDegrees"] = "Steigung in Grad",
            ["OptionDesc.SlopeInDegrees"] = "Steigungen in Grad statt in Prozent anzeigen.",
            ["Option.FollowTerrain"] = "Gelände folgen",
            ["OptionDesc.FollowTerrain"] = "Erstellte Straßen folgen der Geländehöhe, statt eben zu bleiben.",
            ["Option.HideGamePreview"] = "Spielvorschau ausblenden",
            ["OptionDesc.HideGamePreview"] = "Blendet die Straßenvorschau des Spiels in den Erstellungsmodi aus und zeigt nur die Überlagerung des Mods. Kosten und Prüfung gelten weiterhin.",
            ["Binding.Toggle"] = "Werkzeug umschalten",
            ["Binding.ModeAddNode"] = "Modus: Knoten hinzufügen",
            ["Binding.ModeRemoveNode"] = "Modus: Knoten entfernen",
            ["Binding.ModeSlope"] = "Modus: Steigung",
            ["Binding.ModeArrangeLine"] = "Modus: Anordnen",
            ["Binding.ModeLoop"] = "Modus: Schleife",
            ["Binding.ModeConnection"] = "Modus: Verbindung",
            ["Binding.ModeCurve"] = "Modus: Kurve",
            ["Binding.ModeParallel"] = "Modus: Parallel",
            ["Binding.Apply"] = "Anwenden",
            ["Binding.RadiusUp"] = "Radius vergrößern",
            ["Binding.RadiusDown"] = "Radius verkleinern",
            ["Binding.OffsetUp"] = "Versatz vergrößern",
            ["Binding.OffsetDown"] = "Versatz verkleinern",
            ["Binding.HeightUp"] = "Höhe anheben",
            ["Binding.HeightDown"] = "Höhe absenken",
            ["Binding.Side"] = "Direkt/Schleife",
            ["Binding.Invert"] = "Einbahnrichtung umkehren",
            ["Binding.SwitchOffset"] = "Kreis oder Seite wechseln",
            ["Binding.ShiftLeft"] = "Ende nach links verschieben",
            ["Binding.ShiftRight"] = "Ende nach rechts verschieben",

            // UI panel and button
            ["UI.Tooltip"] = "Straßen und Schienen bearbeiten: Knoten hinzufügen oder entfernen, Steigungen festlegen, Knoten anordnen und Segmente mit Kurven verbinden.",
            ["UI.GroupEditNodes"] = "Knoten bearbeiten",
            ["UI.GroupConnect"] = "Segmente verbinden",
            ["UI.ModeAddNode"] = "Knoten hinzufügen",
            ["UI.ModeRemoveNode"] = "Knoten entfernen",
            ["UI.ModeSlope"] = "Steigung",
            ["UI.ModeArrange"] = "Anordnen",
            ["UI.ModeConnection"] = "Verbindung",
            ["UI.ModeLoop"] = "Schleife",
            ["UI.ModeCurve"] = "Kurve",
            ["UI.ModeParallel"] = "Parallel",
            ["UI.FollowTerrain"] = "Gelände folgen",
            ["UI.HidePreview"] = "Spielvorschau ausblenden",
            ["UI.Apply"] = "Anwenden",

            // Shared mode texts
            ["SelectMode"] = "Modus wählen",
            ["PressToCreate"] = "{0} drücken zum Erstellen",
            ["PressToApply"] = "{0} drücken zum Anwenden",
            ["Height"] = "Höhe",
            ["Radius"] = "Radius",
            ["Offset"] = "Versatz",
            ["HintHeight"] = "{0}/{1}: Höhe",
            ["HintOffset"] = "{0}/{1}: Versatz",
            ["ShiftStart"] = "Versatz Start",
            ["ShiftEnd"] = "Versatz Ende",
            ["HintShift"] = "{0}/{1}: aktives Ende seitlich verschieben",
            ["HintShiftEnd"] = "{0}: verschobenes Ende wechseln",
            ["HintRadius"] = "{0}/{1}: Radius",
            ["HintInvert"] = "{0}: Richtung umkehren (Einbahn)",

            // Add node / remove node
            ["AddNodeSelect"] = "Segment wählen, um einen Knoten hinzuzufügen",
            ["AddNodeClick"] = "Klicken, um einen Knoten hinzuzufügen",
            ["AddNodeTooClose"] = "Zu nah an einem vorhandenen Knoten",
            ["AddNodeOwned"] = "Dieses Segment kann nicht geteilt werden",
            ["RemoveNodeSelect"] = "Zu entfernenden Knoten wählen",
            ["RemoveNodeClick"] = "Klicken, um diesen Knoten zu entfernen",
            ["RemoveNodeWrong"] = "Dieser Knoten kann nicht entfernt werden",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "Klicken, um Knoten hinzuzufügen",
            ["ChainUnselect"] = "Klicken zum Abwählen",
            ["ChainNotConnected"] = "Nicht verbunden",
            ["ChainSelect"] = "Knoten der Reihe nach wählen",
            ["SlopeSegment"] = "Segment {0}",
            ["ArrangeGuide"] = "Klicken, um als Führung zu wählen",

            // Creation modes
            ["SelectFirstSegment"] = "Erstes Segment wählen",
            ["SelectSecondSegment"] = "Zweites Segment wählen",
            ["ClickNodeDirection"] = "Knoten anklicken, um die Richtung zu ändern",
            ["WrongShape"] = "Ungültige Form",
            ["NoIntersection"] = "Kein Schnittpunkt",
            ["Angle"] = "Winkel",
            ["LoopHintDrag"] = "Ziehen: Mitte oder Kreisrand",
            ["LoopHintSide"] = "{0}: Direkt/Schleife",
            ["ConnectionRadius"] = "Radius {0}",
            ["ConnectionFirstOffset"] = "Versatz Anfang",
            ["ConnectionLastOffset"] = "Versatz Ende",
            ["ConnectionHintDragEdge"] = "Rand ziehen: Radius",
            ["ConnectionHintDragCenter"] = "Mitte ziehen: Verschieben/Versatz",
            ["ConnectionHintFlip"] = "Doppelklick Mitte: Richtung",
            ["ConnectionHintAdd"] = "Doppelklick Gerade: Kreis hinzufügen",
            ["ConnectionHintRemove"] = "Rechts-Doppelklick Mitte: Kreis entfernen",
            ["ConnectionHintRadius"] = "{0}/{1}: Radius (Umschalt: alle)",
            ["ConnectionHintSwitch"] = "{0}: Kreis wechseln",

            // Parallel
            ["ParallelPickHover"] = "Strg+Klick auf ein Segment: Typ übernehmen",
            ["ParallelType"] = "Typ",
            ["ParallelHintPick"] = "Strg+Klick Segment: Typ übernehmen",
            ["ParallelHintSide"] = "{0}: Seite",
        };
    }
}
