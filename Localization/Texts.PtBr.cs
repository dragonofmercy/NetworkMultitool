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
            ["Binding.ShiftLeft"] = "Deslocar extremidade para a esquerda",
            ["Binding.ShiftRight"] = "Deslocar extremidade para a direita",

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
            ["Offset"] = "Deslocamento",
            ["HintHeight"] = "altura",
            ["HintSnap"] = "arredondar ao metro",
            ["HintOffset"] = "deslocamento",
            ["HintShift"] = "deslocar a extremidade ativa para o lado",
            ["HintShiftEnd"] = "trocar a extremidade deslocada",
            ["HintRadius"] = "raio",
            ["HintInvert"] = "inverter sentido (mão única)",

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
            ["LoopHintDrag"] = "mover o centro ou a borda do círculo",
            ["LoopHintSide"] = "direto/laço",
            ["ConnectionHintDragEdge"] = "raio (borda do círculo)",
            ["ConnectionHintDragCenter"] = "mover/deslocar (centro do círculo)",
            ["ConnectionHintFlip"] = "direção (centro do círculo)",
            ["ConnectionHintAdd"] = "adicionar um círculo (em uma reta)",
            ["ConnectionHintRemove"] = "remover o círculo (centro)",
            ["ConnectionHintRadius"] = "raio (Shift: todos)",
            ["ConnectionHintSwitch"] = "trocar círculo",

            // Parallel
            ["ParallelPickHover"] = "Ctrl+clique em um segmento: copiar o tipo",
            ["ParallelType"] = "Tipo",
            ["ParallelHintPick"] = "escolher o tipo de via (em um segmento)",
            ["ParallelHintDrag"] = "deslocamento e lado (na pré-visualização)",
            ["ParallelHintSide"] = "lado",
        };
    }
}
