import addNode from "images/add-node.svg";
import removeNode from "images/remove-node.svg";
import splitNode from "images/split-node.svg";
import slope from "images/slope.svg";
import arrangeLine from "images/arrange-line.svg";
import arrangeCircle from "images/arrange-circle.svg";
import loop from "images/loop.svg";
import connection from "images/connection.svg";
import curve from "images/curve.svg";

export interface ModeEntry {
    id: number;
    label: string;
    icon: string;
}

// Ids match the C# ToolModeId enum.
export const modes: ModeEntry[] = [
    { id: 1, label: "Add node", icon: addNode },
    { id: 2, label: "Remove node", icon: removeNode },
    { id: 3, label: "Split node", icon: splitNode },
    { id: 4, label: "Slope", icon: slope },
    { id: 5, label: "Arrange line", icon: arrangeLine },
    { id: 6, label: "Arrange circle", icon: arrangeCircle },
    { id: 7, label: "Loop", icon: loop },
    { id: 8, label: "Connection", icon: connection },
    { id: 9, label: "Curve", icon: curve },
];
