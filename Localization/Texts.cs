using System.Collections.Generic;
using Game.SceneFlow;

namespace NetworkMultitool.Localization
{
    /// <summary>Every user-visible text, per game locale. Mode and UI texts are registered under <see cref="Id"/>; the options screen keys are mapped by <see cref="Locale"/>.
    /// English and French are here, the other languages in Texts.<Language>.cs. Adding a language: one more dictionary in <see cref="ByLocale"/>; missing keys fall back to English.</summary>
    public static partial class Texts
    {
        public const string Prefix = "NetworkMultitool.Text.";

        public static string Id(string key) => Prefix + key;

        /// <summary>The text in the game's current language, English when the key is missing there.</summary>
        public static string T(string key)
        {
            var dictionary = GameManager.instance?.localizationManager?.activeDictionary;
            if (dictionary != null && dictionary.TryGetValue(Id(key), out var value))
                return value;
            return En.TryGetValue(key, out value) ? value : key;
        }

        public static string T(string key, params object[] args) => string.Format(T(key), args);

        public static Dictionary<string, Dictionary<string, string>> ByLocale => new Dictionary<string, Dictionary<string, string>>
        {
            ["en-US"] = En,
            ["fr-FR"] = Fr,
            ["de-DE"] = De,
            ["es-ES"] = Es,
            ["it-IT"] = It,
            ["pl-PL"] = Pl,
            ["pt-BR"] = PtBr,
            ["ru-RU"] = Ru,
            ["ja-JP"] = Ja,
            ["ko-KR"] = Ko,
            ["zh-HANS"] = ZhHans,
            ["zh-HANT"] = ZhHant,
        };

        // Panel lines are "Label: value" and hints "Keys: action": the panel splits them on the first ": ", so translations must keep it.
        // In hint keys "/" separates alternatives, "+" combines keys, and @click, @dblclick, @rdblclick, @drag are mouse icons: never translate them.
        private static readonly Dictionary<string, string> En = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "General",
            ["Group.Keybindings"] = "Keybindings",
            ["Option.SegmentLength"] = "Segment length",
            ["OptionDesc.SegmentLength"] = "Target length (in meters) of the road segments created by the tool.",
            ["Option.SlopeInDegrees"] = "Show slope in degrees",
            ["OptionDesc.SlopeInDegrees"] = "Display slopes in degrees instead of percent.",
            ["Option.FollowTerrain"] = "Follow terrain",
            ["OptionDesc.FollowTerrain"] = "Let created roads follow the terrain height instead of staying flat.",
            ["Option.HideGamePreview"] = "Hide game preview",
            ["OptionDesc.HideGamePreview"] = "Hide the game's road preview in the creation modes and keep only the mod's overlay. Cost and validation still apply.",
            ["Binding.Toggle"] = "Toggle tool",
            ["Binding.ModeAddNode"] = "Mode: add node",
            ["Binding.ModeRemoveNode"] = "Mode: remove node",
            ["Binding.ModeSlope"] = "Mode: slope",
            ["Binding.ModeArrangeLine"] = "Mode: arrange",
            ["Binding.ModeLoop"] = "Mode: loop",
            ["Binding.ModeConnection"] = "Mode: connection",
            ["Binding.ModeCurve"] = "Mode: curve",
            ["Binding.ModeParallel"] = "Mode: parallel",
            ["Binding.Apply"] = "Apply",
            ["Binding.RadiusUp"] = "Increase radius",
            ["Binding.RadiusDown"] = "Decrease radius",
            ["Binding.OffsetUp"] = "Increase offset",
            ["Binding.OffsetDown"] = "Decrease offset",
            ["Binding.HeightUp"] = "Raise height",
            ["Binding.HeightDown"] = "Lower height",
            ["Binding.Side"] = "Direct/loop",
            ["Binding.Invert"] = "Invert one-way direction",
            ["Binding.SwitchOffset"] = "Switch circle or side",
            ["Binding.ShiftLeft"] = "Shift end left",
            ["Binding.ShiftRight"] = "Shift end right",

            // UI panel and button
            ["UI.Tooltip"] = "Edit roads and rails: add or remove nodes, set slopes, arrange nodes and connect segments with curves.",
            ["UI.GroupEditNodes"] = "Edit nodes",
            ["UI.GroupConnect"] = "Connect segments",
            ["UI.ModeAddNode"] = "Add node",
            ["UI.ModeRemoveNode"] = "Remove node",
            ["UI.ModeSlope"] = "Slope",
            ["UI.ModeArrange"] = "Arrange",
            ["UI.ModeConnection"] = "Connection",
            ["UI.ModeLoop"] = "Loop",
            ["UI.ModeCurve"] = "Curve",
            ["UI.ModeParallel"] = "Parallel",
            ["UI.FollowTerrain"] = "Follow terrain",
            ["UI.HidePreview"] = "Hide game preview",
            ["UI.Apply"] = "Apply",

            // Shared mode texts
            ["SelectMode"] = "Select a mode",
            ["PressToCreate"] = "Press {0} to create",
            ["PressToApply"] = "Press {0} to apply",
            ["Height"] = "Height",
            ["Offset"] = "Offset",
            ["HintHeight"] = "{0}/{1}: height",
            ["HintSnap"] = "Shift+@drag: snap to whole meters",
            ["HintOffset"] = "{0}/{1}: offset",
            ["HintShift"] = "{0}/{1}: shift the active end sideways",
            ["HintShiftEnd"] = "{0}: switch the shifted end",
            ["HintRadius"] = "{0}/{1}: radius",
            ["HintInvert"] = "{0}: invert direction (one-way)",

            // Add node / remove node
            ["AddNodeSelect"] = "Select a segment to add a node",
            ["AddNodeClick"] = "Click to add a node",
            ["AddNodeTooClose"] = "Too close to an existing node",
            ["AddNodeOwned"] = "This segment can't be split",
            ["RemoveNodeSelect"] = "Select a node to remove",
            ["RemoveNodeClick"] = "Click to remove this node",
            ["RemoveNodeWrong"] = "This node can't be removed",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "Click to add nodes",
            ["ChainUnselect"] = "Click to unselect",
            ["ChainNotConnected"] = "Not connected",
            ["ChainSelect"] = "Select nodes in order",
            ["SlopeSegment"] = "Segment {0}",
            ["ArrangeGuide"] = "Click to select as guide",

            // Creation modes
            ["SelectFirstSegment"] = "Select first segment",
            ["SelectSecondSegment"] = "Select second segment",
            ["ClickNodeDirection"] = "Click a node to change direction",
            ["WrongShape"] = "Wrong shape",
            ["NoIntersection"] = "No intersection",
            ["LoopHintDrag"] = "@drag: move the center or the circle edge",
            ["LoopHintSide"] = "{0}: direct/loop",
            ["ConnectionHintDragEdge"] = "@drag: radius (circle edge)",
            ["ConnectionHintDragCenter"] = "@drag: move/offset (circle center)",
            ["ConnectionHintFlip"] = "@dblclick: direction (circle center)",
            ["ConnectionHintAdd"] = "@dblclick: add a circle (on a straight)",
            ["ConnectionHintRemove"] = "@rdblclick: remove the circle (center)",
            ["ConnectionHintRadius"] = "{0}/{1}: radius (Shift: all)",
            ["ConnectionHintSwitch"] = "{0}: switch circle",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+click a segment: pick its type",
            ["ParallelType"] = "Type",
            ["ParallelHintPick"] = "Ctrl+@click: pick the road type (on a segment)",
            ["ParallelHintDrag"] = "@drag: offset and side (on the preview)",
            ["ParallelHintSide"] = "{0}: side",
        };

        private static readonly Dictionary<string, string> Fr = new Dictionary<string, string>
        {
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "Général",
            ["Group.Keybindings"] = "Raccourcis",
            ["Option.SegmentLength"] = "Longueur des segments",
            ["OptionDesc.SegmentLength"] = "Longueur visée (en mètres) des segments de route créés par l'outil.",
            ["Option.SlopeInDegrees"] = "Pente en degrés",
            ["OptionDesc.SlopeInDegrees"] = "Afficher les pentes en degrés au lieu de pourcentages.",
            ["Option.FollowTerrain"] = "Suivre le terrain",
            ["OptionDesc.FollowTerrain"] = "Les routes créées suivent la hauteur du terrain au lieu de rester à plat.",
            ["Option.HideGamePreview"] = "Masquer l'aperçu du jeu",
            ["OptionDesc.HideGamePreview"] = "Masque l'aperçu de route du jeu dans les modes de création pour ne garder que celui du mod. Le coût et la validation restent actifs.",
            ["Binding.Toggle"] = "Activer l'outil",
            ["Binding.ModeAddNode"] = "Mode: ajouter un nœud",
            ["Binding.ModeRemoveNode"] = "Mode: supprimer un nœud",
            ["Binding.ModeSlope"] = "Mode: pente",
            ["Binding.ModeArrangeLine"] = "Mode: arranger",
            ["Binding.ModeLoop"] = "Mode: boucle",
            ["Binding.ModeConnection"] = "Mode: connexion",
            ["Binding.ModeCurve"] = "Mode: courbe",
            ["Binding.ModeParallel"] = "Mode: parallèle",
            ["Binding.Apply"] = "Appliquer",
            ["Binding.RadiusUp"] = "Augmenter le rayon",
            ["Binding.RadiusDown"] = "Diminuer le rayon",
            ["Binding.OffsetUp"] = "Augmenter le décalage",
            ["Binding.OffsetDown"] = "Diminuer le décalage",
            ["Binding.HeightUp"] = "Monter",
            ["Binding.HeightDown"] = "Descendre",
            ["Binding.Side"] = "Direct/boucle",
            ["Binding.Invert"] = "Inverser le sens unique",
            ["Binding.SwitchOffset"] = "Changer de cercle ou de côté",
            ["Binding.ShiftLeft"] = "Décaler l'extrémité à gauche",
            ["Binding.ShiftRight"] = "Décaler l'extrémité à droite",

            ["UI.Tooltip"] = "Modifier les routes et les voies ferrées: ajouter ou supprimer des nœuds, régler les pentes, arranger les nœuds et relier des segments par des courbes.",
            ["UI.GroupEditNodes"] = "Modifier les nœuds",
            ["UI.GroupConnect"] = "Relier des segments",
            ["UI.ModeAddNode"] = "Ajouter un nœud",
            ["UI.ModeRemoveNode"] = "Supprimer un nœud",
            ["UI.ModeSlope"] = "Pente",
            ["UI.ModeArrange"] = "Arranger",
            ["UI.ModeConnection"] = "Connexion",
            ["UI.ModeLoop"] = "Boucle",
            ["UI.ModeCurve"] = "Courbe",
            ["UI.ModeParallel"] = "Parallèle",
            ["UI.FollowTerrain"] = "Suivre le terrain",
            ["UI.HidePreview"] = "Masquer l'aperçu du jeu",
            ["UI.Apply"] = "Appliquer",

            ["SelectMode"] = "Choisir un mode",
            ["PressToCreate"] = "Appuyer sur {0} pour créer",
            ["PressToApply"] = "Appuyer sur {0} pour appliquer",
            ["Height"] = "Hauteur",
            ["Offset"] = "Décalage",
            ["HintHeight"] = "{0}/{1}: hauteur",
            ["HintSnap"] = "Shift+@drag: arrondir au mètre",
            ["HintOffset"] = "{0}/{1}: décalage",
            ["HintShift"] = "{0}/{1}: décaler l'extrémité active sur le côté",
            ["HintShiftEnd"] = "{0}: changer d'extrémité décalée",
            ["HintRadius"] = "{0}/{1}: rayon",
            ["HintInvert"] = "{0}: inverser le sens (sens unique)",

            ["AddNodeSelect"] = "Sélectionner un segment pour ajouter un nœud",
            ["AddNodeClick"] = "Cliquer pour ajouter un nœud",
            ["AddNodeTooClose"] = "Trop près d'un nœud existant",
            ["AddNodeOwned"] = "Ce segment ne peut pas être coupé",
            ["RemoveNodeSelect"] = "Sélectionner un nœud à supprimer",
            ["RemoveNodeClick"] = "Cliquer pour supprimer ce nœud",
            ["RemoveNodeWrong"] = "Ce nœud ne peut pas être supprimé",

            ["ChainAdd"] = "Cliquer pour ajouter des nœuds",
            ["ChainUnselect"] = "Cliquer pour désélectionner",
            ["ChainNotConnected"] = "Non connecté",
            ["ChainSelect"] = "Sélectionner les nœuds dans l'ordre",
            ["SlopeSegment"] = "Segment {0}",
            ["ArrangeGuide"] = "Cliquer pour choisir comme guide",

            ["SelectFirstSegment"] = "Sélectionner le premier segment",
            ["SelectSecondSegment"] = "Sélectionner le second segment",
            ["ClickNodeDirection"] = "Cliquer sur un nœud pour changer de sens",
            ["WrongShape"] = "Forme impossible",
            ["NoIntersection"] = "Pas d'intersection",
            ["LoopHintDrag"] = "@drag: déplacer le centre ou le bord du cercle",
            ["LoopHintSide"] = "{0}: direct/boucle",
            ["ConnectionHintDragEdge"] = "@drag: rayon (bord du cercle)",
            ["ConnectionHintDragCenter"] = "@drag: déplacer/décaler (centre du cercle)",
            ["ConnectionHintFlip"] = "@dblclick: sens (centre du cercle)",
            ["ConnectionHintAdd"] = "@dblclick: ajouter un cercle (sur une droite)",
            ["ConnectionHintRemove"] = "@rdblclick: retirer le cercle (centre)",
            ["ConnectionHintRadius"] = "{0}/{1}: rayon (Maj: tous)",
            ["ConnectionHintSwitch"] = "{0}: changer de cercle",

            ["ParallelPickHover"] = "Ctrl+clic sur un segment: prendre son type",
            ["ParallelType"] = "Type",
            ["ParallelHintPick"] = "Ctrl+@click: choisir le type de route (sur un segment)",
            ["ParallelHintDrag"] = "@drag: décalage et côté (sur l'aperçu)",
            ["ParallelHintSide"] = "{0}: côté",
        };
    }
}
