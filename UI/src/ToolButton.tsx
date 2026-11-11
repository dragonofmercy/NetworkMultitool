import { useValue } from "cs2/api";
import { FloatingButton } from "cs2/ui";
import toolIcon from "images/tool.svg";
import { active$, toggle } from "bindings";

export const ToolButton = () => {
    const active = useValue(active$);
    return <FloatingButton src={toolIcon} selected={active} onSelect={toggle} tooltipLabel="Network Multitool (Ctrl+T)" />;
};
