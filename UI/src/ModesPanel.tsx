import { useValue } from "cs2/api";
import { Button, Icon, Panel } from "cs2/ui";
import { active$, apply, info$, mode$, setMode } from "bindings";
import { modes } from "modes";
import styles from "./panel.module.scss";

export const ModesPanel = () => {
    const active = useValue(active$);
    const mode = useValue(mode$);
    const info = useValue(info$);
    if (!active)
        return null;

    return (
        <Panel className={styles.panel} header="Network Multitool">
            <div className={styles.grid}>
                {modes.map((m) => (
                    <Button key={m.id} variant="flat" className={styles.mode} selected={mode === m.id} onSelect={() => setMode(m.id)}>
                        <Icon src={m.icon} className={styles.icon} />
                        <span>{m.label}</span>
                    </Button>
                ))}
            </div>
            <Button variant="flat" className={styles.apply} onSelect={apply}>Apply</Button>
            {info && <div className={styles.info}>{info}</div>}
        </Panel>
    );
};
