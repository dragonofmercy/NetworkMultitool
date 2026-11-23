import { useLocalization } from "cs2/l10n";

// Same prefix as Texts.Id on the C# side, which registers these keys for every language.
const PREFIX = "NetworkMultitool.Text.";

/** Returns a translator: the text for the key in the game's language, or the English fallback. */
export const useText = () => {
    const { translate } = useLocalization();
    return (key: string, fallback: string) => translate(PREFIX + key, fallback) ?? fallback;
};
