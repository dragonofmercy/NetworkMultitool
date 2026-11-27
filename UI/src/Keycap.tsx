import mouseLeft from "images/mouse-left.svg";
import mouseRight from "images/mouse-right.svg";
import mouseDrag from "images/mouse-drag.svg";
import styles from "./panel.module.scss";

const mouse: Record<string, { icon: string; label?: string; wide?: boolean }> = {
    "@click": { icon: mouseLeft },
    "@dblclick": { icon: mouseLeft, label: "x2" },
    "@rdblclick": { icon: mouseRight, label: "x2" },
    "@drag": { icon: mouseDrag, wide: true },
};

// Short names so the keycaps fit next to long translations.
const keyNames: Record<string, string> = { PageUp: "PgUp", PageDown: "PgDn", Escape: "Esc", Delete: "Del", Insert: "Ins" };

/** One key, or a mouse icon for the @click, @dblclick, @rdblclick and @drag tokens. */
export const Keycap = ({ name }: { name: string }) => {
    const m = mouse[name];
    return (
        <span className={styles.keycap}>
            {m ? <img src={m.icon} className={m.wide ? styles.mouseDrag : styles.mouseIcon} /> : keyNames[name] ?? name}
            {m?.label && <span className={styles.keycapLabel}>{m.label}</span>}
        </span>
    );
};

/** A "+" combination such as "Ctrl+Alt+S" as adjacent keycaps. */
export const Keys = ({ combo }: { combo: string }) => (
    <>
        {combo.split("+").filter((k) => k.length > 0).map((k, i) => <Keycap key={i} name={k} />)}
    </>
);
