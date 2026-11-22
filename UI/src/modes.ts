import addNode from "images/add-node.png";
import removeNode from "images/remove-node.png";
import slope from "images/slope.png";
import loop from "images/loop.png";
import connection from "images/connection.png";
import curve from "images/curve.png";

export interface ModeEntry {
    id: number;
    label: string;
    icon: string;
    /** The mode is validated with the Apply key, so the panel shows an Apply button. */
    apply?: boolean;
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
        ],
    },
    {
        label: "Connect segments",
        modes: [
            { id: 5, label: "Connection", icon: connection, apply: true },
            { id: 4, label: "Loop", icon: loop, apply: true },
            { id: 6, label: "Curve", icon: curve, apply: true },
        ],
    },
];
