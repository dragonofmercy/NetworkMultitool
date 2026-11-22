import { useValue } from "cs2/api";
import { Button, Panel } from "cs2/ui";
import { active$, apply, info$, mode$, setMode } from "bindings";
import { groups, ModeEntry } from "modes";
import styles from "./panel.module.scss";

// "Label: value" lines become two columns; other lines span the row.
const Rows = ({ text, className }: { text: string; className: string }) => (
    <div className={className}>
        {text.split("\n").filter((line) => line.length > 0).map((line, i) => {
            const cut = line.indexOf(": ");
            return (
                <div key={i} className={styles.row}>
                    <span className={styles.label}>{cut < 0 ? line : line.slice(0, cut)}</span>
                    {cut >= 0 && <span className={styles.value}>{line.slice(cut + 2)}</span>}
                </div>
            );
        })}
    </div>
);

export const ModesPanel = () => {
    const active = useValue(active$);
    const mode = useValue(mode$);
    const info = useValue(info$);
    if (!active)
        return null;

    // The C# side sends the status line, the values and the key hints as blocks separated by a blank line.
    const [status, values, hints] = info.split("\n\n");
    let current: ModeEntry | undefined;
    for (const group of groups)
        current = current ?? group.modes.find((m) => m.id === mode);

    return (
        <Panel className={styles.panel} contentClassName={styles.content} header="Network Multitool">
            {groups.map((group) => (
                <div key={group.label} className={styles.group}>
                    <div className={styles.groupLabel}>{group.label}</div>
                    <div className={styles.modes}>
                        {group.modes.map((m) => (
                            <Button key={m.id} variant="flat" className={styles.mode} selected={mode === m.id} tooltipLabel={m.label} onSelect={() => setMode(m.id)}>
                                <img src={m.icon} className={styles.icon} />
                            </Button>
                        ))}
                    </div>
                </div>
            ))}
            <div className={styles.separator} />
            {current && <div className={styles.title}>{current.label}</div>}
            {status && <div className={styles.status}>{status}</div>}
            {values && <Rows text={values} className={styles.values} />}
            {current?.apply && <Button variant="flat" className={styles.apply} onSelect={apply}>Apply</Button>}
            {hints && <Rows text={hints} className={styles.hints} />}
        </Panel>
    );
};
