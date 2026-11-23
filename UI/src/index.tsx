import { ModRegistrar } from "cs2/modding";
import { ModesPanel } from "ModesPanel";
import { WorldLabels } from "WorldLabels";
import { ToolButton } from "ToolButton";

const register: ModRegistrar = (moduleRegistry) => {
    moduleRegistry.append("GameTopLeft", ToolButton);
    moduleRegistry.append("Game", ModesPanel);
    moduleRegistry.append("Game", WorldLabels);
};

export default register;
