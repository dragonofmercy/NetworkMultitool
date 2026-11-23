import { useValue } from "cs2/api";
import { labels$ } from "bindings";
import styles from "./labels.module.scss";

// The C# side sends one label per line: viewport x, viewport y from the top, then the text lines, tab-separated.
export const WorldLabels = () => {
    const labels = useValue(labels$);
    if (!labels)
        return null;
    return (
        <>
            {labels.split("\n").filter((line) => line.length > 0).map((line, i) => {
                const [x, y, ...text] = line.split("\t");
                return (
                    <div key={i} className={styles.label} style={{ left: `${parseFloat(x) * 100}%`, top: `${parseFloat(y) * 100}%` }}>
                        {text.map((t, j) => <div key={j}>{t}</div>)}
                    </div>
                );
            })}
        </>
    );
};
