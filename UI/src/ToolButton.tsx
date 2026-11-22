import { useValue } from "cs2/api";
import { FloatingButton } from "cs2/ui";
import toolIcon from "images/tool.svg";
import { active$, toggle, toggleKeys$ } from "bindings";
import styles from "./panel.module.scss";

export const ToolButton = () => {
    const active = useValue(active$);
    const keys = useValue(toggleKeys$);
    const tooltip = (
        <div className={styles.tooltip}>
            <div className={styles.tooltipTitle}>Network Multitool</div>
            <div className={styles.tooltipText}>Edit roads and rails: add or remove nodes, set slopes, arrange nodes and connect segments with curves.</div>
            {keys && (
                <div className={styles.tooltipKeys}>
                    {keys.split("+").map((key) => <span key={key} className={styles.tooltipKey}>{key}</span>)}
                </div>
            )}
        </div>
    );
    return <FloatingButton src={toolIcon} selected={active} onSelect={toggle} tooltipLabel={tooltip} />;
};
