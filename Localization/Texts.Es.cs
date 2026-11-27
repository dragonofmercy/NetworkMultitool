using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> Es = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "General",
            ["Group.Keybindings"] = "Asignación de teclas",
            ["Option.SegmentLength"] = "Longitud de segmento",
            ["OptionDesc.SegmentLength"] = "Longitud objetivo (en metros) de los segmentos de carretera creados por la herramienta.",
            ["Option.SlopeInDegrees"] = "Pendiente en grados",
            ["OptionDesc.SlopeInDegrees"] = "Mostrar las pendientes en grados en lugar de porcentaje.",
            ["Option.FollowTerrain"] = "Seguir el terreno",
            ["OptionDesc.FollowTerrain"] = "Las carreteras creadas siguen la altura del terreno en lugar de quedar planas.",
            ["Option.HideGamePreview"] = "Ocultar vista previa del juego",
            ["OptionDesc.HideGamePreview"] = "Oculta la vista previa de carretera del juego en los modos de creación y deja solo la del mod. El coste y la validación siguen aplicándose.",
            ["Binding.Toggle"] = "Activar herramienta",
            ["Binding.ModeAddNode"] = "Modo: añadir nodo",
            ["Binding.ModeRemoveNode"] = "Modo: eliminar nodo",
            ["Binding.ModeSlope"] = "Modo: pendiente",
            ["Binding.ModeArrangeLine"] = "Modo: alinear",
            ["Binding.ModeLoop"] = "Modo: bucle",
            ["Binding.ModeConnection"] = "Modo: conexión",
            ["Binding.ModeCurve"] = "Modo: curva",
            ["Binding.ModeParallel"] = "Modo: paralelo",
            ["Binding.Apply"] = "Aplicar",
            ["Binding.RadiusUp"] = "Aumentar radio",
            ["Binding.RadiusDown"] = "Reducir radio",
            ["Binding.OffsetUp"] = "Aumentar desplazamiento",
            ["Binding.OffsetDown"] = "Reducir desplazamiento",
            ["Binding.HeightUp"] = "Subir altura",
            ["Binding.HeightDown"] = "Bajar altura",
            ["Binding.Side"] = "Directo/bucle",
            ["Binding.Invert"] = "Invertir sentido único",
            ["Binding.SwitchOffset"] = "Cambiar de círculo o lado",
            ["Binding.ShiftLeft"] = "Desplazar extremo a la izquierda",
            ["Binding.ShiftRight"] = "Desplazar extremo a la derecha",

            // UI panel and button
            ["UI.Tooltip"] = "Editar carreteras y vías férreas: añadir o eliminar nodos, ajustar pendientes, alinear nodos y unir segmentos con curvas.",
            ["UI.GroupEditNodes"] = "Editar nodos",
            ["UI.GroupConnect"] = "Unir segmentos",
            ["UI.ModeAddNode"] = "Añadir nodo",
            ["UI.ModeRemoveNode"] = "Eliminar nodo",
            ["UI.ModeSlope"] = "Pendiente",
            ["UI.ModeArrange"] = "Alinear",
            ["UI.ModeConnection"] = "Conexión",
            ["UI.ModeLoop"] = "Bucle",
            ["UI.ModeCurve"] = "Curva",
            ["UI.ModeParallel"] = "Paralelo",
            ["UI.FollowTerrain"] = "Seguir el terreno",
            ["UI.HidePreview"] = "Ocultar vista previa del juego",
            ["UI.Apply"] = "Aplicar",

            // Shared mode texts
            ["SelectMode"] = "Elige un modo",
            ["PressToCreate"] = "Pulsa {0} para crear",
            ["PressToApply"] = "Pulsa {0} para aplicar",
            ["Height"] = "Altura",
            ["Offset"] = "Desplazamiento",
            ["HintHeight"] = "altura",
            ["HintSnap"] = "redondear al metro",
            ["HintOffset"] = "desplazamiento",
            ["HintShift"] = "desplazar el extremo activo hacia un lado",
            ["HintShiftEnd"] = "cambiar el extremo desplazado",
            ["HintRadius"] = "radio",
            ["HintInvert"] = "invertir sentido (sentido único)",

            // Add node / remove node
            ["AddNodeSelect"] = "Elige un segmento para añadir un nodo",
            ["AddNodeClick"] = "Haz clic para añadir un nodo",
            ["AddNodeTooClose"] = "Demasiado cerca de un nodo existente",
            ["AddNodeOwned"] = "Este segmento no se puede dividir",
            ["RemoveNodeSelect"] = "Elige un nodo para eliminar",
            ["RemoveNodeClick"] = "Haz clic para eliminar este nodo",
            ["RemoveNodeWrong"] = "Este nodo no se puede eliminar",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "Haz clic para añadir nodos",
            ["ChainUnselect"] = "Haz clic para deseleccionar",
            ["ChainNotConnected"] = "No conectado",
            ["ChainSelect"] = "Elige los nodos en orden",
            ["SlopeSegment"] = "Segmento {0}",
            ["ArrangeGuide"] = "Haz clic para usar como guía",

            // Creation modes
            ["SelectFirstSegment"] = "Elige el primer segmento",
            ["SelectSecondSegment"] = "Elige el segundo segmento",
            ["ClickNodeDirection"] = "Haz clic en un nodo para cambiar el sentido",
            ["WrongShape"] = "Forma no válida",
            ["NoIntersection"] = "Sin intersección",
            ["LoopHintDrag"] = "mover el centro o el borde del círculo",
            ["LoopHintSide"] = "directo/bucle",
            ["ConnectionHintDragEdge"] = "radio (borde del círculo)",
            ["ConnectionHintDragCenter"] = "mover/desplazar (centro del círculo)",
            ["ConnectionHintFlip"] = "dirección (centro del círculo)",
            ["ConnectionHintAdd"] = "añadir un círculo (en una recta)",
            ["ConnectionHintRemove"] = "quitar el círculo (centro)",
            ["ConnectionHintRadius"] = "radio (Mayús: todos)",
            ["ConnectionHintSwitch"] = "cambiar de círculo",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+clic en un segmento: tomar su tipo",
            ["ParallelType"] = "Tipo",
            ["ParallelHintPick"] = "elegir el tipo de vía (en un segmento)",
            ["ParallelHintDrag"] = "desplazamiento y lado (en la vista previa)",
            ["ParallelHintSide"] = "lado",
        };
    }
}
