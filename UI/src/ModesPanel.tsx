import { useValue } from "cs2/api";
import { Button, Panel } from "cs2/ui";
import { active$, apply, followTerrain$, hidePreview$, info$, mode$, setMode, toggleFollowTerrain, toggleHidePreview } from "bindings";
import { groups, ModeEntry } from "modes";
import { useText } from "l10n";
import styles from "./panel.module.scss";
import mouseLeft from "images/mouse-left.svg";
import mouseRight from "images/mouse-right.svg";
import mouseDrag from "images/mouse-drag.svg";

// "Label: value" lines become two columns; other lines span the row.
const Rows = ({ text, className }: { text: string; className: string }) => (
    <div className={className}>
        {text.split("\n").filter((line) => line.length > 0).map((line, i) => {
            const cut = line.indexOf(": ");
            return (
                <div key={i} className={styles.row}>
                    <span className={styles.label}>{cut < 0 ? line : line.slice(0, cut)}</span>
                    {cut >= 0 && <span>{line.slice(cut + 2)}</span>}
                </div>
            );
        })}
    </div>
);

const mouse: Record<string, { icon: string; label?: string }> = {
    "@click": { icon: mouseLeft },
    "@dblclick": { icon: mouseLeft, label: "x2" },
    "@rdblclick": { icon: mouseRight, label: "x2" },
    "@drag": { icon: mouseDrag },
};

// Short names so the keycaps fit next to long translations.
const keyNames: Record<string, string> = { PageUp: "PgUp", PageDown: "PgDn", Escape: "Esc", Delete: "Del", Insert: "Ins" };

const Keycap = ({ name }: { name: string }) => {
    const m = mouse[name];
    return (
        <span className={styles.keycap}>
            {m ? <img src={m.icon} className={m.icon === mouseDrag ? styles.mouseDrag : styles.mouseIcon} /> : keyNames[name] ?? name}
            {m?.label && <span className={styles.keycapLabel}>{m.label}</span>}
        </span>
    );
};

// Hint lines are "Keys: action": "/" separates alternatives and "+" combines keys (see Texts.cs).
const Hints = ({ text }: { text: string }) => (
    <div className={styles.hints}>
        {text.split("\n").filter((line) => line.length > 0).map((line, i) => {
            const cut = line.indexOf(": ");
            const keys = cut < 0 ? [] : line.slice(0, cut).split("/").filter((alt) => alt.length > 0);
            return (
                <div key={i} className={styles.hint}>
                    <span className={styles.hintAction}>{cut < 0 ? line : line.slice(cut + 2)}</span>
                    <span className={styles.hintKeys}>
                        {keys.map((alt, j) => (
                            <span key={j} className={styles.hintAlt}>
                                {j > 0 && <span className={styles.hintSep}>/</span>}
                                {alt.split("+").filter((k) => k.length > 0).map((k, n) => <Keycap key={n} name={k} />)}
                            </span>
                        ))}
                    </span>
                </div>
            );
        })}
    </div>
);

const Toggle =({ on, onClick, label }: { on: boolean; onClick: () => void; label: string }) => (
    <div className={styles.toggle} onClick={onClick}>
        <div className={on ? `${styles.switch} ${styles.on}` : styles.switch}>
            <div className={styles.knob} />
        </div>
        {label}
    </div>
);

export const ModesPanel = () => {
    const active = useValue(active$);
    const mode = useValue(mode$);
    const info = useValue(info$);
    const followTerrain = useValue(followTerrain$);
    const hidePreview = useValue(hidePreview$);
    const t = useText();
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
                    <div className={styles.groupLabel}>{t(group.textKey, group.label)}</div>
                    <div className={styles.modes}>
                        {group.modes.map((m) => (
                            <Button key={m.id} variant="flat" className={mode === m.id ? `${styles.mode} ${styles.selected}` : styles.mode} selected={mode === m.id} tooltipLabel={t(m.textKey, m.label)} onSelect={() => setMode(m.id)}>
                                <img src={m.icon} className={styles.icon} />
                            </Button>
                        ))}
                    </div>
                </div>
            ))}
            <div className={styles.separator} />
            {current && <div className={styles.title}>{t(current.textKey, current.label)}</div>}
            {status && <div className={styles.status}>{status}</div>}
            {values && <Rows text={values} className={styles.values} />}
            {current?.terrain && (
                <>
                    <Toggle on={followTerrain} onClick={toggleFollowTerrain} label={t("UI.FollowTerrain", "Follow terrain")} />
                    <Toggle on={hidePreview} onClick={toggleHidePreview} label={t("UI.HidePreview", "Hide game preview")} />
                </>
            )}
            {current?.apply && <Button variant="flat" className={styles.apply} onSelect={apply}>{t("UI.Apply", "Apply")}</Button>}
            {hints && <Hints text={hints} />}
        </Panel>
    );
};
