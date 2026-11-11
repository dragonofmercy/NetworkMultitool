import { ModRegistrar } from "cs2/modding";
import { ModesPanel } from "ModesPanel";
import { ToolButton } from "ToolButton";

const register: ModRegistrar = (moduleRegistry) => {
    moduleRegistry.append("GameTopLeft", ToolButton);
    moduleRegistry.append("Game", ModesPanel);
};

export default register;
