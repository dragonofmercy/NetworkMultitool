import { useValue } from "cs2/api";
import { labels$ } from "bindings";
import styles from "./labels.module.scss";

// The C# side sends one label per line: viewport x, viewport y from the top, screen direction x, y, then the text lines, tab-separated.
export const WorldLabels = () => {
    const labels = useValue(labels$);
    if (!labels)
        return null;
    return (
        <>
            {labels.split("\n").filter((line) => line.length > 0).map((line, i) => {
                const [x, y, dx, dy, ...text] = line.split("\t");
                // Like CS1 InfoLabel: the box sits on the side given by the direction, its near edge on the point (outer shift + inner centering, both in % of the box).
                return (
                    <div key={i} className={styles.anchor} style={{ left: `${parseFloat(x) * 100}%`, top: `${parseFloat(y) * 100}%`, transform: `translate(${parseFloat(dx) * 50}%, ${parseFloat(dy) * 50}%)` }}>
                        <div className={styles.label}>{text.map((t, j) => <div key={j}>{t}</div>)}</div>
                    </div>
                );
            })}
        </>
    );
};
