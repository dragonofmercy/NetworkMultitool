# Network Multitool (Cities: Skylines II)

[![Donate](https://img.shields.io/badge/Donate-Ko--fi-FF5E5B?logo=kofi&logoColor=white)](https://ko-fi.com/dragonofmercy)

A tool panel with 8 modes for editing road and rail networks in Cities: Skylines II.

This mod is a port of [Network Multitool](https://steamcommunity.com/sharedfiles/filedetails/?id=2560782729) for Cities: Skylines 1, by [MacSergey](https://github.com/MacSergey). The modes, their behavior and the geometry come from his mod; this version rewrites them for the Cities: Skylines II API.

## Credits

All credit for the original design and geometry goes to MacSergey, author of Network Multitool and of the `ModsCommon` library it is built on. This port is published with his permission.

## Modes

Left click selects/applies, right click goes back/clears. Slope, Arrange and Parallel select an ordered run of connected nodes (click to add, click an end node to unselect), then press `Enter` (3 nodes or more for Slope and Arrange, 2 or more for Parallel).

| Mode | Behavior |
| --- | --- |
| Add node | Click a segment to split it with a new node. |
| Remove node | Click a node to remove it and merge its two segments. |
| Slope | Select a chain, `Enter` gives it a constant slope between its end nodes. |
| Arrange | Select a chain, `Enter` spreads its nodes evenly along one smooth curve between its two ends. Click a segment outside the chain at an end to use it as the direction guide. |
| Loop | Select two segments, build a loop or direct arc between them: drag the center or the circle edge (or Up/Down) for the radius, `Ctrl+Alt+S` direct/loop. |
| Connection | Select two segments, connect them with two arcs: drag a circle edge for its radius and its center for its offset. Keys for fine steps: Up/Down radius (Shift: both), Left/Right offsets, `Tab` switches circle, `Ctrl+Alt+S` switches the side of the selected circle. |
| Curve | Select two segments, connect them with a smooth curve. |
| Parallel | Select a chain, build a new network alongside it. Left/Right offset, PageUp/PageDown height, `Ctrl+Alt+S` side (the left side runs the other way), Ctrl+click any segment to use its type, `Enter` to create. |

Loop, Connection and Curve also use PageUp/PageDown (height), `Ctrl+Alt+I` (invert), click a node to change direction, and `Enter` to apply.

## Default keybindings

All rebindable in the mod options.

| Action | Key |
| --- | --- |
| Toggle tool | Ctrl+Alt+T |
| Add node / Remove node | Ctrl+Alt+1 / Ctrl+Alt+2 |
| Slope / Arrange / Connection / Loop / Curve / Parallel | Ctrl+Alt+3 / Ctrl+Alt+4 / Ctrl+Alt+5 / Ctrl+Alt+6 / Ctrl+Alt+7 / Ctrl+Alt+8 |
| Apply | Enter |
| Radius up / down | Up / Down |
| Offset up / down | Right / Left |
| Height up / down | PageUp / PageDown |
| Invert / Side | Ctrl+Alt+I / Ctrl+Alt+S |
| Switch offset | Tab |

## Build

Run from `build/`. Build the UI first: the C# build copies `UI/dist` into the deployed mod (it warns when `UI/dist` is empty).

```
cd UI && npm install && npm run build && cd ..
dotnet build NetworkMultitool.csproj -c Release
dotnet test Tests/NetworkMultitool.Tests.csproj
```

Requirements: the CS2 modding toolchain (environment variables `CSII_TOOLPATH`, `CSII_USERDATAPATH`, `CSII_LOCALMODSPATH`), Node >= 18 and npm. Bun is not used.

## Known limitations

- No undo.
- Node moves bypass vanilla validation.

## AI disclaimer

This mod is developed with the help of [Claude Code](https://claude.com/claude-code),
an AI coding assistant. I decide what gets built and review the changes, and every
feature is tested by hand by me before it is released.

## Support

If this project helps to increase your productivity, you can give me a cup of coffee :)

<a href="https://ko-fi.com/dragonofmercy"><img src="https://cdn.ko-fi.com/cdn/kofi2.png?v=3" alt="Donate" width="160"></a>

## License

[MIT](LICENSE).
