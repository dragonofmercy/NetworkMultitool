import { bindValue, trigger } from "cs2/api";

const GROUP = "NetworkMultitool";

export const active$ = bindValue<boolean>(GROUP, "active", false);
export const mode$ = bindValue<number>(GROUP, "mode", 0);
export const info$ = bindValue<string>(GROUP, "info", "");

export const toggle = () => trigger(GROUP, "toggle");
export const setMode = (id: number) => trigger(GROUP, "setMode", id);
export const apply = () => trigger(GROUP, "apply");
