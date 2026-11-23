import addNode from "images/add-node.svg";
import removeNode from "images/remove-node.svg";
import slope from "images/slope.svg";
import arrange from "images/arrange.svg";
import loop from "images/loop.svg";
import connection from "images/connection.svg";
import curve from "images/curve.svg";
import parallel from "images/parallel.svg";

export interface ModeEntry {
    id: number;
    /** English fallback; the translation is looked up under textKey. */
    label: string;
    textKey: string;
    icon: string;
    /** The mode is validated with the Apply key, so the panel shows an Apply button. */
    apply?: boolean;
    /** The mode builds roads, so the panel shows the Follow terrain and Hide game preview toggles. */
    terrain?: boolean;
}

export interface ModeGroup {
    label: string;
    textKey: string;
    modes: ModeEntry[];
}

// Ids match the C# ToolModeId enum.
export const groups: ModeGroup[] = [
    {
        label: "Edit nodes",
        textKey: "UI.GroupEditNodes",
        modes: [
            { id: 1, label: "Add node", textKey: "UI.ModeAddNode", icon: addNode },
            { id: 2, label: "Remove node", textKey: "UI.ModeRemoveNode", icon: removeNode },
            { id: 3, label: "Slope", textKey: "UI.ModeSlope", icon: slope, apply: true },
            { id: 7, label: "Arrange", textKey: "UI.ModeArrange", icon: arrange, apply: true },
        ],
    },
    {
        label: "Connect segments",
        textKey: "UI.GroupConnect",
        modes: [
            { id: 5, label: "Connection", textKey: "UI.ModeConnection", icon: connection, apply: true, terrain: true },
            { id: 4, label: "Loop", textKey: "UI.ModeLoop", icon: loop, apply: true, terrain: true },
            { id: 6, label: "Curve", textKey: "UI.ModeCurve", icon: curve, apply: true, terrain: true },
            { id: 8, label: "Parallel", textKey: "UI.ModeParallel", icon: parallel, apply: true, terrain: true },
        ],
    },
];
