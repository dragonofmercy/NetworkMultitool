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
            ["Radius"] = "Radius",
            ["Offset"] = "Offset",
            ["HintHeight"] = "{0}/{1}: height",
            ["HintOffset"] = "{0}/{1}: offset",
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
            ["Angle"] = "Angle",
            ["LoopHintDrag"] = "Drag: center or circle edge",
            ["LoopHintSide"] = "{0}: direct/loop",
            ["ConnectionRadius"] = "Radius {0}",
            ["ConnectionFirstOffset"] = "First offset",
            ["ConnectionLastOffset"] = "Last offset",
            ["ConnectionHintDragEdge"] = "Drag edge: radius",
            ["ConnectionHintDragCenter"] = "Drag center: move/offset",
            ["ConnectionHintFlip"] = "Double-click center: direction",
            ["ConnectionHintAdd"] = "Double-click straight: add circle",
            ["ConnectionHintRemove"] = "Right double-click center: remove circle",
            ["ConnectionHintRadius"] = "{0}/{1}: radius (Shift: all)",
            ["ConnectionHintSwitch"] = "{0}: switch circle",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+click a segment: pick its type",
            ["ParallelType"] = "Type",
            ["ParallelHintPick"] = "Ctrl+click segment: pick type",
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
            ["Radius"] = "Rayon",
            ["Offset"] = "Décalage",
            ["HintHeight"] = "{0}/{1}: hauteur",
            ["HintOffset"] = "{0}/{1}: décalage",
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
            ["Angle"] = "Angle",
            ["LoopHintDrag"] = "Glisser: centre ou bord du cercle",
            ["LoopHintSide"] = "{0}: direct/boucle",
            ["ConnectionRadius"] = "Rayon {0}",
            ["ConnectionFirstOffset"] = "Décalage début",
            ["ConnectionLastOffset"] = "Décalage fin",
            ["ConnectionHintDragEdge"] = "Glisser le bord: rayon",
            ["ConnectionHintDragCenter"] = "Glisser le centre: déplacer/décaler",
            ["ConnectionHintFlip"] = "Double-clic au centre: sens",
            ["ConnectionHintAdd"] = "Double-clic sur une droite: ajouter un cercle",
            ["ConnectionHintRemove"] = "Double-clic droit au centre: retirer le cercle",
            ["ConnectionHintRadius"] = "{0}/{1}: rayon (Maj: tous)",
            ["ConnectionHintSwitch"] = "{0}: changer de cercle",

            ["ParallelPickHover"] = "Ctrl+clic sur un segment: prendre son type",
            ["ParallelType"] = "Type",
            ["ParallelHintPick"] = "Ctrl+clic sur un segment: choisir le type",
            ["ParallelHintSide"] = "{0}: côté",
        };
    }
}
