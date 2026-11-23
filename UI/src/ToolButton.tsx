import { useValue } from "cs2/api";
import { FloatingButton, Tooltip } from "cs2/ui";
import toolIcon from "images/tool.svg";
import { active$, toggle, toggleKeys$ } from "bindings";
import { useText } from "l10n";
import styles from "./panel.module.scss";

export const ToolButton = () => {
    const active = useValue(active$);
    const keys = useValue(toggleKeys$);
    const t = useText();
    const tooltip = (
        <div className={styles.tooltip}>
            <div className={styles.tooltipTitle}>Network Multitool</div>
            <div className={styles.tooltipText}>{t("UI.Tooltip", "Edit roads and rails: add or remove nodes, set slopes, arrange nodes and connect segments with curves.")}</div>
            {keys && (
                <div className={styles.tooltipKeys}>
                    {keys.split("+").map((key) => <span key={key} className={styles.tooltipKey}>{key}</span>)}
                </div>
            )}
        </div>
    );
    // The tooltip is anchored on a plain div: the floating button's own tooltipLabel shows nothing in the top-left strip.
    return (
        <Tooltip tooltip={tooltip} direction="down" delayTime={250}>
            <div>
                <FloatingButton src={toolIcon} selected={active} onSelect={toggle} />
            </div>
        </Tooltip>
    );
};
