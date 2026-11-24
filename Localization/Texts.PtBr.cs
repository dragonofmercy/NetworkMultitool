using System.Collections.Generic;

namespace NetworkMultitool.Localization
{
    public static partial class Texts
    {
        private static readonly Dictionary<string, string> PtBr = new Dictionary<string, string>
        {
            // Options screen
            ["Settings.Title"] = "Network Multitool",
            ["Group.General"] = "Geral",
            ["Group.Keybindings"] = "Atalhos",
            ["Option.SegmentLength"] = "Comprimento do segmento",
            ["OptionDesc.SegmentLength"] = "Comprimento desejado (em metros) dos segmentos de via criados pela ferramenta.",
            ["Option.SlopeInDegrees"] = "Inclinação em graus",
            ["OptionDesc.SlopeInDegrees"] = "Exibir as inclinações em graus em vez de porcentagem.",
            ["Option.FollowTerrain"] = "Seguir o terreno",
            ["OptionDesc.FollowTerrain"] = "As vias criadas seguem a altura do terreno em vez de ficarem planas.",
            ["Option.HideGamePreview"] = "Ocultar prévia do jogo",
            ["OptionDesc.HideGamePreview"] = "Oculta a prévia de via do jogo nos modos de criação e mantém só a sobreposição do mod. O custo e a validação continuam valendo.",
            ["Binding.Toggle"] = "Ativar ferramenta",
            ["Binding.ModeAddNode"] = "Modo: adicionar nó",
            ["Binding.ModeRemoveNode"] = "Modo: remover nó",
            ["Binding.ModeSlope"] = "Modo: inclinação",
            ["Binding.ModeArrangeLine"] = "Modo: alinhar",
            ["Binding.ModeLoop"] = "Modo: laço",
            ["Binding.ModeConnection"] = "Modo: conexão",
            ["Binding.ModeCurve"] = "Modo: curva",
            ["Binding.ModeParallel"] = "Modo: paralela",
            ["Binding.Apply"] = "Aplicar",
            ["Binding.RadiusUp"] = "Aumentar raio",
            ["Binding.RadiusDown"] = "Diminuir raio",
            ["Binding.OffsetUp"] = "Aumentar deslocamento",
            ["Binding.OffsetDown"] = "Diminuir deslocamento",
            ["Binding.HeightUp"] = "Subir altura",
            ["Binding.HeightDown"] = "Baixar altura",
            ["Binding.Side"] = "Direto/laço",
            ["Binding.Invert"] = "Inverter sentido único",
            ["Binding.SwitchOffset"] = "Trocar círculo ou lado",

            // UI panel and button
            ["UI.Tooltip"] = "Edite vias e trilhos: adicione ou remova nós, ajuste inclinações, alinhe nós e conecte segmentos com curvas.",
            ["UI.GroupEditNodes"] = "Editar nós",
            ["UI.GroupConnect"] = "Conectar segmentos",
            ["UI.ModeAddNode"] = "Adicionar nó",
            ["UI.ModeRemoveNode"] = "Remover nó",
            ["UI.ModeSlope"] = "Inclinação",
            ["UI.ModeArrange"] = "Alinhar",
            ["UI.ModeConnection"] = "Conexão",
            ["UI.ModeLoop"] = "Laço",
            ["UI.ModeCurve"] = "Curva",
            ["UI.ModeParallel"] = "Paralela",
            ["UI.FollowTerrain"] = "Seguir o terreno",
            ["UI.HidePreview"] = "Ocultar prévia do jogo",
            ["UI.Apply"] = "Aplicar",

            // Shared mode texts
            ["SelectMode"] = "Selecione um modo",
            ["PressToCreate"] = "Pressione {0} para criar",
            ["PressToApply"] = "Pressione {0} para aplicar",
            ["Height"] = "Altura",
            ["Radius"] = "Raio",
            ["Offset"] = "Deslocamento",
            ["HintHeight"] = "{0}/{1}: altura",
            ["HintOffset"] = "{0}/{1}: deslocamento",
            ["HintRadius"] = "{0}/{1}: raio",
            ["HintInvert"] = "{0}: inverter sentido (mão única)",

            // Add node / remove node
            ["AddNodeSelect"] = "Selecione um segmento para adicionar um nó",
            ["AddNodeClick"] = "Clique para adicionar um nó",
            ["AddNodeTooClose"] = "Muito perto de um nó existente",
            ["AddNodeOwned"] = "Este segmento não pode ser dividido",
            ["RemoveNodeSelect"] = "Selecione um nó para remover",
            ["RemoveNodeClick"] = "Clique para remover este nó",
            ["RemoveNodeWrong"] = "Este nó não pode ser removido",

            // Node chains (slope, arrange, parallel)
            ["ChainAdd"] = "Clique para adicionar nós",
            ["ChainUnselect"] = "Clique para desmarcar",
            ["ChainNotConnected"] = "Não conectado",
            ["ChainSelect"] = "Selecione os nós em ordem",
            ["SlopeSegment"] = "Segmento {0}",
            ["ArrangeGuide"] = "Clique para usar como guia",

            // Creation modes
            ["SelectFirstSegment"] = "Selecione o primeiro segmento",
            ["SelectSecondSegment"] = "Selecione o segundo segmento",
            ["ClickNodeDirection"] = "Clique em um nó para mudar o sentido",
            ["WrongShape"] = "Forma inválida",
            ["NoIntersection"] = "Sem interseção",
            ["Angle"] = "Ângulo",
            ["LoopHintDrag"] = "Arrastar: centro ou borda do círculo",
            ["LoopHintSide"] = "{0}: direto/laço",
            ["ConnectionRadius"] = "Raio {0}",
            ["ConnectionFirstOffset"] = "Deslocamento inicial",
            ["ConnectionLastOffset"] = "Deslocamento final",
            ["ConnectionHintDragEdge"] = "Arrastar borda: raio",
            ["ConnectionHintDragCenter"] = "Arrastar centro: mover/deslocar",
            ["ConnectionHintFlip"] = "Clique duplo no centro: sentido",
            ["ConnectionHintAdd"] = "Clique duplo na reta: adicionar círculo",
            ["ConnectionHintRemove"] = "Clique duplo direito no centro: remover círculo",
            ["ConnectionHintRadius"] = "{0}/{1}: raio (Shift: todos)",
            ["ConnectionHintSwitch"] = "{0}: trocar círculo",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+clique em um segmento: copiar o tipo",
            ["ParallelType"] = "Tipo",
            ["ParallelHintPick"] = "Ctrl+clique no segmento: copiar tipo",
            ["ParallelHintSide"] = "{0}: lado",
        };
    }
}
