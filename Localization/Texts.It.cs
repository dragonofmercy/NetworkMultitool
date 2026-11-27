using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> It = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "Generale",
            ["Group.Keybindings"] = "Scorciatoie",
            ["Option.SegmentLength"] = "Lunghezza segmenti",
            ["OptionDesc.SegmentLength"] = "Lunghezza desiderata (in metri) dei segmenti di strada creati dallo strumento.",
            ["Option.SlopeInDegrees"] = "Pendenza in gradi",
            ["OptionDesc.SlopeInDegrees"] = "Mostra le pendenze in gradi anziché in percentuale.",
            ["Option.FollowTerrain"] = "Segui il terreno",
            ["OptionDesc.FollowTerrain"] = "Le strade create seguono l'altezza del terreno invece di restare piatte.",
            ["Option.HideGamePreview"] = "Nascondi anteprima del gioco",
            ["OptionDesc.HideGamePreview"] = "Nasconde l'anteprima stradale del gioco nelle modalità di creazione e mostra solo quella della mod. Costi e validazione restano attivi.",
            ["Binding.Toggle"] = "Attiva/disattiva strumento",
            ["Binding.ModeAddNode"] = "Modalità: aggiungi nodo",
            ["Binding.ModeRemoveNode"] = "Modalità: rimuovi nodo",
            ["Binding.ModeSlope"] = "Modalità: pendenza",
            ["Binding.ModeArrangeLine"] = "Modalità: allinea",
            ["Binding.ModeLoop"] = "Modalità: anello",
            ["Binding.ModeConnection"] = "Modalità: raccordo",
            ["Binding.ModeCurve"] = "Modalità: curva",
            ["Binding.ModeParallel"] = "Modalità: parallela",
            ["Binding.Apply"] = "Applica",
            ["Binding.RadiusUp"] = "Aumenta raggio",
            ["Binding.RadiusDown"] = "Riduci raggio",
            ["Binding.OffsetUp"] = "Aumenta scostamento",
            ["Binding.OffsetDown"] = "Riduci scostamento",
            ["Binding.HeightUp"] = "Alza",
            ["Binding.HeightDown"] = "Abbassa",
            ["Binding.Side"] = "Diretto/anello",
            ["Binding.Invert"] = "Inverti senso unico",
            ["Binding.SwitchOffset"] = "Cambia cerchio o lato",
            ["Binding.ShiftLeft"] = "Sposta estremità a sinistra",
            ["Binding.ShiftRight"] = "Sposta estremità a destra",

            // UI panel and button
            ["UI.Tooltip"] = "Modifica strade e ferrovie: aggiungi o rimuovi nodi, imposta pendenze, allinea nodi e collega segmenti con curve.",
            ["UI.GroupEditNodes"] = "Modifica nodi",
            ["UI.GroupConnect"] = "Collega segmenti",
            ["UI.ModeAddNode"] = "Aggiungi nodo",
            ["UI.ModeRemoveNode"] = "Rimuovi nodo",
            ["UI.ModeSlope"] = "Pendenza",
            ["UI.ModeArrange"] = "Allinea",
            ["UI.ModeConnection"] = "Raccordo",
            ["UI.ModeLoop"] = "Anello",
            ["UI.ModeCurve"] = "Curva",
            ["UI.ModeParallel"] = "Parallela",
            ["UI.FollowTerrain"] = "Segui il terreno",
            ["UI.HidePreview"] = "Nascondi anteprima del gioco",
            ["UI.Apply"] = "Applica",

            // Shared mode texts
            ["SelectMode"] = "Scegli una modalità",
            ["PressToCreate"] = "Premi {0} per creare",
            ["PressToApply"] = "Premi {0} per applicare",
            ["Height"] = "Altezza",
            ["Offset"] = "Scostamento",
            ["HintHeight"] = "altezza",
            ["HintSnap"] = "arrotonda al metro",
            ["HintOffset"] = "scostamento",
            ["HintShift"] = "sposta lateralmente l'estremità attiva",
            ["HintShiftEnd"] = "cambia l'estremità spostata",
            ["HintRadius"] = "raggio",
            ["HintInvert"] = "inverti direzione (senso unico)",

            // Add node / remove node
            ["AddNodeSelect"] = "Seleziona un segmento per aggiungere un nodo",
            ["AddNodeClick"] = "Clicca per aggiungere un nodo",
            ["AddNodeTooClose"] = "Troppo vicino a un nodo esistente",
            ["AddNodeOwned"] = "Questo segmento non può essere diviso",
            ["RemoveNodeSelect"] = "Seleziona un nodo da rimuovere",
            ["RemoveNodeClick"] = "Clicca per rimuovere questo nodo",
            ["RemoveNodeWrong"] = "Questo nodo non può essere rimosso",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "Clicca per aggiungere nodi",
            ["ChainUnselect"] = "Clicca per deselezionare",
            ["ChainNotConnected"] = "Non collegato",
            ["ChainSelect"] = "Seleziona i nodi in ordine",
            ["SlopeSegment"] = "Segmento {0}",
            ["ArrangeGuide"] = "Clicca per usare come guida",

            // Creation modes
            ["SelectFirstSegment"] = "Seleziona il primo segmento",
            ["SelectSecondSegment"] = "Seleziona il secondo segmento",
            ["ClickNodeDirection"] = "Clicca un nodo per cambiare direzione",
            ["WrongShape"] = "Forma non valida",
            ["NoIntersection"] = "Nessuna intersezione",
            ["LoopHintDrag"] = "sposta il centro o il bordo del cerchio",
            ["LoopHintSide"] = "diretto/anello",
            ["ConnectionHintDragEdge"] = "raggio (bordo del cerchio)",
            ["ConnectionHintDragCenter"] = "sposta/scosta (centro del cerchio)",
            ["ConnectionHintFlip"] = "direzione (centro del cerchio)",
            ["ConnectionHintAdd"] = "aggiungi un cerchio (su un rettilineo)",
            ["ConnectionHintRemove"] = "rimuovi il cerchio (centro)",
            ["ConnectionHintRadius"] = "raggio (Maiusc: tutti)",
            ["ConnectionHintSwitch"] = "cambia cerchio",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+clic su un segmento: usa il suo tipo",
            ["ParallelType"] = "Tipo",
            ["ParallelHintPick"] = "scegli il tipo di strada (su un segmento)",
            ["ParallelHintDrag"] = "scostamento e lato (sull'anteprima)",
            ["ParallelHintSide"] = "lato",
        };
    }
}
