import addNode from "images/add-node.svg";
import removeNode from "images/remove-node.svg";
import slope from "images/slope.svg";
import arrange from "images/arrange.svg";
import loop from "images/loop.svg";
import connection from "images/connection.svg";
import curve from "images/curve.svg";

export interface ModeEntry {
    id: number;
    label: string;
    icon: string;
    /** The mode is validated with the Apply key, so the panel shows an Apply button. */
    apply?: boolean;
    /** The mode builds roads, so the panel shows the Follow terrain toggle. */
    terrain?: boolean;
}

export interface ModeGroup {
    label: string;
    modes: ModeEntry[];
}

// Ids match the C# ToolModeId enum.
export const groups: ModeGroup[] = [
    {
        label: "Edit nodes",
        modes: [
            { id: 1, label: "Add node", icon: addNode },
            { id: 2, label: "Remove node", icon: removeNode },
            { id: 3, label: "Slope", icon: slope, apply: true },
            { id: 7, label: "Arrange", icon: arrange, apply: true },
        ],
    },
    {
        label: "Connect segments",
        modes: [
            { id: 5, label: "Connection", icon: connection, apply: true, terrain: true },
            { id: 4, label: "Loop", icon: loop, apply: true, terrain: true },
            { id: 6, label: "Curve", icon: curve, apply: true, terrain: true },
        ],
    },
];
